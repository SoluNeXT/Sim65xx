using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using System.Xml.Serialization;
using Simulateur65xx.OB;
using Simulateur65xx.FW;
using Timer = System.Windows.Forms.Timer;

namespace Simulateur65xx
{
    public partial class Main : Form
    {
        public static bool isClosing = false;
        private Animation _animation;
        private Memory memory;
        private CPU cpu;

        public Main()
        {
            InitializeComponent();
        }

        private void Main_FormClosing(object sender, FormClosingEventArgs e)
        {
            isClosing = true;
            _animation.RunningAnimation = false;
            _animation.timerAnimation.Enabled = false;
            Timing.timer1.Enabled = false;
        }

        private void btnEXEC_Click(object sender, EventArgs e)
        {
            _animation.StartAnimation(Animation.ANIM.PC_2_MEMORY);
            while (_animation.RunningAnimation)
            {
                Timing.Wait(100);
            }

            memory.SetMemoryPosToPC();
            Timing.Wait(10* _animation.Speed);
            _animation.StartAnimation(Animation.ANIM.MEMORY_2_PC);

        }


        private void btnPAUSE_Click(object sender, EventArgs e)
        {
            _animation.StartAnimation(Animation.ANIM.MEMORY_2_PC);

        }

        private void Main_Load(object sender, EventArgs e)
        {
            _animation = new Animation(pbLIGHT, cbSPEED);
            memory = new Memory(tbMEMORY, tbSTACK);
            cpu = new CPU(memory);
            cpu.DefineRegistersFromScreen(tbAh, tbXh,tbYh, tbSPh, tbPSh, tbPC);
            ResetCommandAndParam();
            cpu.Reset();
            memory.SetMemoryPosToPC();
            memory.DisplayMemory();
            memory.DisplayStack();
            cpu.DisplayRegisters();
        }

        private bool leftDown = false;
        private void btnUp_MouseDown(object sender, MouseEventArgs ev)
        {
            NavigateInMemory(sender, -1);
        }

        private void NavigateInMemory(object sender, int sens)
        {
            leftDown = true;
            Button b = (Button)sender;
            int h = b.Height;
            Point btn = PointToScreen(b.Location);
            Timer t = new Timer();
            t.Interval = 100;
            t.Tick += (s, e) =>
            {
                t.Enabled = false;
                Point p = Cursor.Position;
                int Y = p.Y - btn.Y;
                int velocity;
                if (sens > 0)
                {
                    velocity = (int)(12 * (decimal)Y / (decimal)h);
                }
                else
                {
                    velocity = (int)(12 * ((decimal)h - (decimal)Y) / (decimal)h);
                }
                memory.position += sens * (int)Math.Pow(2, velocity);
                if (memory.position < 0) memory.position = 0;
                if (memory.position > 65535) memory.position = 65535;
                memory.DisplayMemory();
                if(leftDown)
                    t.Enabled= true;
            };
            t.Enabled = true;
        }

        private void btnDown_MouseDown(object sender, MouseEventArgs e)
        {
            NavigateInMemory(sender, 1);
            return;
            //while (e.Button == MouseButtons.Left)
            {
                Button b = (Button)sender;
                int h = b.Height;
                Point btn = PointToScreen(b.Location);
                Point p = Cursor.Position;
                int Y = p.Y - btn.Y;
                int velocity = (int)(12 * (decimal)Y / (decimal)h);
                memory.position += (int)Math.Pow(2, velocity);

                if (memory.position > 65535) memory.position = 65535;
                memory.DisplayMemory();
                //Wait(100);
            }
        }

        private void btnDebut_Click(object sender, EventArgs e)
        {
            memory.position = 0;
            memory.DisplayMemory();
        }

        private void btnFin_Click(object sender, EventArgs e)
        {
            memory.position = 65535;
            memory.DisplayMemory();
        }

        private void btn_MouseUp(object sender, MouseEventArgs e)
        {
            leftDown = false;
        }

        private void btn_MouseLeave(object sender, EventArgs e)
        {
            leftDown = false;
        }

        private void tbPSh_TextChanged(object sender, EventArgs e)
        {
            Tools.CopyRegisterValues(tbPSh, tbPSb, null);
        }

        private void tbSPh_TextChanged(object sender, EventArgs e)
        {
            Tools.CopyRegisterValues(tbSPh, tbSPb, tbSPd);
        }

        private void tbOPCODE_TextChanged(object sender, EventArgs e)
        {
            string opCode=tbOPCODE.Text;
            if (opCode == "")
            {
                tbCOMMANDE.Text = "";
                return;
            }

            OpCode o = OpCode.GetOpCode(opCode);
            tbCOMMANDE.Text = o.LongName;
        }

        private void tbYh_TextChanged(object sender, EventArgs e)
        {
            Tools.CopyRegisterValues(tbYh, tbYb, tbYd);
        }

        private void tbXh_TextChanged(object sender, EventArgs e)
        {
            Tools.CopyRegisterValues(tbXh, tbXb, tbXd);
        }

        private void tbAh_TextChanged(object sender, EventArgs e)
        {
            Tools.CopyRegisterValues(tbAh, tbAb, tbAd);
        }

        private void tbLOGS_TextChanged(object sender, EventArgs e)
        {

        }

        private void btnREBOOT_Click(object sender, EventArgs e)
        {
            btnEXECUTE.Visible = false;
            btnSTOP.Visible = false;
            btnFETCH.Visible = true;
            cpu.Reset();
            ResetCommandAndParam();

            _animation.StartAnimation(Animation.ANIM.PC_2_MEMORY);

            cpu.PC = 0xFFFC;
            memory.SetMemoryPosTo(cpu.PC);
            Timing.Wait(10 * _animation.Speed);

            _animation.StartAnimation(Animation.ANIM.MEMORY_2_PC);

            tbPC.Text = "--" + memory.GetByteAt(cpu.PC).ToString("X2");

            _animation.StartAnimation(Animation.ANIM.PC_2_MEMORY);

            memory.SetMemoryPosTo((ushort)(cpu.PC + 1));
            Timing.Wait(10 * _animation.Speed);

            _animation.StartAnimation(Animation.ANIM.MEMORY_2_PC);

            cpu.PC = memory.GetWordAt(cpu.PC);
            cpu.DisplayRegisters();

            btnEXECUTE.Visible = false;
            btnSTOP.Visible = true;
            btnFETCH.Visible = false;

        }

        private void ResetCommandAndParam()
        {
            tbPC.Text = "----";
            tbLOGS.Text = "";
            tbOPCODE.Text = "";
            tbPARAMh.Text = "";
            PasAPasStatus = 0;
            _animation.SetStartPoint(Animation.PC_2_MEMORY);
        }

        private void tbPARAMh_TextChanged(object sender, EventArgs e)
        {
            string h = tbPARAMh.Text.Replace(" ","").Trim();
            if (h == "")
            {
                tbPARAMb.Text = "";
                tbPARAMd.Text = "";
                return;
            }

            tbPARAMb.Text = Tools.Hex2Bin(h);
            tbPARAMd.Text= Tools.Hex2Dec(h);

        }

        // 0 >> Need OpCode   => FETCH
        // 1 >> Need Param 1  => FETCH
        // 2 >> Need Param 2  => FETCH
        // 3 >> Run OpCode    => EXECUTE
        private byte PasAPasStatus = 0;

        private void btnSTEP_Click(object sender, EventArgs e)
        {
            if (PasAPasStatus == 0)
            {
                OpCode opcode = ReadOpCode();
                if (opcode.NbParameters == 1) PasAPasStatus = 2;
                if (opcode.NbParameters == 2) PasAPasStatus = 1;
                if (opcode.NbParameters == 0) PasAPasStatus = 3;
                
            }

            if (PasAPasStatus == 1)
            {
                ReadFirstParam();
                PasAPasStatus = 2;
            }

            if (PasAPasStatus == 2)
            {
                ReadSecondParam();
                PasAPasStatus = 3;
            }

            if (PasAPasStatus == 3)
            {
                ExecuteOpCode();
                PasAPasStatus = 0;
            }

        }


        private ushort ReadSecondParam()
        {
            OpCode o = OpCode.GetOpCode(tbOPCODE.Text);
            btnEXECUTE.Visible = false;
            btnSTOP.Visible = false;
            btnFETCH.Visible = true;

            _animation.StartAnimation(Animation.ANIM.PC_2_MEMORY);
            memory.DisplayMemory(cpu.PC);
            Timing.Wait(10 * _animation.Speed);

            _animation.StartAnimation(Animation.ANIM.MEMORY_2_PARAM);
            Timing.Wait(10 * _animation.Speed);

            tbPARAMh.Text = memory.GetByteAt(cpu.PC).ToString("X2");
            Timing.Wait(10 * _animation.Speed);

            _animation.StartAnimation(Animation.ANIM.PARAM_2_OPCODE);
            Timing.Wait(10 * _animation.Speed);

            btnEXECUTE.Visible = false;
            btnSTOP.Visible = true;
            btnFETCH.Visible = false;

            return (ushort)Tools.Hex2Int(tbPARAMh.Text, 0);
        }

        private void ExecuteOpCode()
        {
            OpCode o = OpCode.GetOpCode(tbOPCODE.Text);
            cpu.TotalCycles += o.NbCycles;
            int p = Tools.Hex2Int(tbPARAMh.Text,0);
            btnEXECUTE.Visible = true;
            btnSTOP.Visible = false;
            btnFETCH.Visible = false;

            string hexa = o.Hex;
            if (o.NbParameters > 0) 
                hexa += " " + (p & 255).ToString("X2");
            else 
                hexa += "   ";
            if (o.NbParameters > 1)
                hexa += " " + (p >> 8).ToString("X2");
            else 
                hexa += "   ";

            string mem = (cpu.PC - o.NbParameters).ToString("X4");
            string param =string.Format(OpCode.PFormat[o.ParamType], (p & 255).ToString("X2"), (p >> 8).ToString("X2"));
            string cycles = " "+o.NbCycles.ToString("0")+" ";
            string totalcycles = cpu.TotalCycles.ToString();
            while (totalcycles.Length < 6) totalcycles = " " + totalcycles;

            string l = mem + " " + hexa + " " + o.ShortName+" "+ param +" "+ cycles +" "+ totalcycles;

            tbLOGS.Text = $"{l}\r\n{tbLOGS.Text}";

            switch (o.ShortName)
            {
                case "LDA":
                    ExecuteLDA(o,p);
                    break;
                case "BRK":
                    ExecuteBRK(o, p);
                    break;
                default:
                    break;
            }

            btnEXECUTE.Visible = false;
            btnSTOP.Visible = true;
            btnFETCH.Visible = false;

        }

        private void ExecuteLDA(OpCode o, int p)
        {
            if (o.Value == (byte)OpCode.OPCODE.LDA_ABSOLUTE)
            {
                Execute_LDA_ABSOLUTE(o,p);
            }
        }

        private void ExecuteBRK(OpCode o, int p)
        {
            _animation.StartAnimation(Animation.ANIM.OPCODE_2_PC);
            Timing.Wait(10 * _animation.Speed);
        }

        private void Execute_LDA_ABSOLUTE(OpCode o, int p)
        {
            _animation.StartAnimation(Animation.ANIM.OPCODE_2_A);
            Timing.Wait(10 * _animation.Speed);

            cpu.A = (byte)p;
            cpu.DisplayRegisters();
//            tbAh.Text = p.ToString("X2");
            Timing.Wait(10 * _animation.Speed);

            _animation.StartAnimation(Animation.ANIM.A_2_PS);
            Timing.Wait(10 * _animation.Speed);

            int ps = Tools.Hex2Int(tbPSh.Text, 0);
            if (p == 0)
                cpu.SetZ();
            else
                cpu.UnSetZ();
            if (p > 127)
                cpu.SetN();
            else
                cpu.UnSetN();
            cpu.DisplayRegisters();
            Timing.Wait(10 * _animation.Speed);

            _animation.StartAnimation(Animation.ANIM.PS_2_PC);
            Timing.Wait(10 * _animation.Speed);

            cpu.PC++;
            cpu.DisplayRegisters();
            Timing.Wait(10 * _animation.Speed);

        }


        private ushort ReadFirstParam()
        {
            OpCode o = OpCode.GetOpCode(tbOPCODE.Text);
            btnEXECUTE.Visible = false;
            btnSTOP.Visible = false;
            btnFETCH.Visible = true;

            _animation.StartAnimation(Animation.ANIM.PC_2_MEMORY);
            memory.DisplayMemory(cpu.PC);
            Timing.Wait(10 * _animation.Speed);

            _animation.StartAnimation(Animation.ANIM.MEMORY_2_PARAM);
            Timing.Wait(10 * _animation.Speed);

            tbPARAMh.Text = memory.GetByteAt(cpu.PC).ToString("X2")+tbPARAMh.Text;
            Timing.Wait(10 * _animation.Speed);

            _animation.StartAnimation(Animation.ANIM.PARAM_2_PC);
            Timing.Wait(10 * _animation.Speed);

            cpu.PC++;
            cpu.DisplayRegisters();
            Timing.Wait(10 * _animation.Speed);

            btnEXECUTE.Visible = false;
            btnSTOP.Visible = true;
            btnFETCH.Visible = false;

            return (ushort)Tools.Hex2Int(tbPARAMh.Text,0);
        }

        private OpCode ReadOpCode()
        {
            btnEXECUTE.Visible = false;
            btnSTOP.Visible = false;
            btnFETCH.Visible = true;
            
            _animation.StartAnimation(Animation.ANIM.PC_2_MEMORY);
            memory.DisplayMemory(cpu.PC);
            Timing.Wait(10 * _animation.Speed);

            _animation.StartAnimation(Animation.ANIM.MEMORY_2_OPCODE);
            Timing.Wait(10 * _animation.Speed);

            tbOPCODE.Text = memory.GetByteAt(cpu.PC).ToString("X2");
            tbPARAMh.Text = "";
            Timing.Wait(10 * _animation.Speed);

            OpCode opcode = OpCode.GetOpCode(tbOPCODE.Text);

            if (opcode.NbParameters > 0)
            {
                _animation.StartAnimation(Animation.ANIM.OPCODE_2_PC);
                Timing.Wait(10 * _animation.Speed);
            }

            cpu.PC++;
            cpu.DisplayRegisters();
            Timing.Wait(10 * _animation.Speed);

            btnEXECUTE.Visible = false;
            btnSTOP.Visible = true;
            btnFETCH.Visible = false;

            return opcode;
        }
    }
}
