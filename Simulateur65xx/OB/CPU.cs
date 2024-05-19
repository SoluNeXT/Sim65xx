using System;
using System.Windows.Forms;

namespace Simulateur65xx.OB
{
    public class CPU
    {
        public byte SP;
        public byte A;
        public byte X;
        public byte Y;
        public byte opCode;
        public ushort param;
        public ushort PC;

        //Flags PS
        public bool Carry;
        public bool Overflow;
        public bool Negative;
        public bool Zero;
        public bool Decimal;
        public bool Interrupt;
        public bool Break;

        private Memory memory;
        private TextBox TbA;
        private TextBox TbX;
        private TextBox TbY;
        private TextBox TbSP;
        private TextBox TbPS;
        private TextBox TbPC;
        
        public byte PS {
            set
            {
                Carry = (value & 1) != 0;
                Zero = (value & 2) != 0;
                Interrupt = (value & 4) != 0;
                Decimal = (value & 8) != 0;
                Break = (value & 16) != 0;
                Overflow = (value & 64) != 0;
                Negative = (value & 128) != 0;
            }
            get
            {
                return (byte)(
                    (Carry ? 1 : 0) |
                    (Zero ? 2 : 0) |
                    (Interrupt ? 4 : 0) |
                    (Decimal ? 8 : 0) |
                    (Break ? 16 : 0) |
                    (32) |
                    (Overflow ? 64 : 0) |
                    (Negative ? 128 : 0)
                );
            }
        }

        public int TotalCycles = 0;
        

        public CPU(Memory Refmemory)
        {
            memory = Refmemory;
            memory.LinkCpuSP(this);
        }

        public void DefineRegistersFromScreen(TextBox tbA, TextBox tbX, TextBox tbY, TextBox tbSP, TextBox tbPS, TextBox tbPC)
        {
            TbA = tbA;
            TbX = tbX;
            TbY = tbY;
            TbSP = tbSP;
            TbPS = tbPS;
            TbPC = tbPC;
        }

        public void Reset()
        {
            A = 0;
            X = 0;
            Y = 0;
            SP = 255;
            PS = 0;
            PC = 0xFFFC;
            DisplayRegisters();

            memory.Load();
            PC = memory.GetWordAt(0xFFFC);
            memory.DisplayMemory();
            TotalCycles = 0;

        }

        public void DisplayRegisters()
        {
            TbA.Text = A.ToString("X2");
            TbX.Text = X.ToString("X2");
            TbY.Text = Y.ToString("X2");
            TbSP.Text = SP.ToString("X2");
            TbPS.Text = PS.ToString("X2");
            TbPC.Text = PC.ToString("X4");
        }

        public void SetZ()
        {
            Zero = true;
        }
        public void SetN()
        {
            Negative = true;
        }
        public void SetI()
        {
            Interrupt = true;
        }
        public void SetC()
        {
            Carry = true;
        }
        public void SetV()
        {
            Overflow = true;
        }
        public void SetB()
        {
            Break = true;
        }
        public void SetD()
        {
            Decimal = true;
        }
        public void UnSetD()
        {
            Decimal = false;
        }
        public void UnSetZ()
        {
            Zero = false;
        }
        public void UnSetN()
        {
            Negative = false;
        }
        public void UnSetI()
        {
            Interrupt = false;
        }
        public void UnSetC()
        {
            Carry = false;
        }
        public void UnSetV()
        {
            Overflow = false;
        }
        public void UnSetB()
        {
            Break = false;
        }

    }
}