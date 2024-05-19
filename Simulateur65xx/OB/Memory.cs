using System;
using System.Globalization;
using System.IO;
using System.Windows.Forms;
using Simulateur65xx.FW;

namespace Simulateur65xx.OB
{
    public class Memory
    {
        public byte[] memory = new byte[65536];
        public int position = 32768;

        private TextBox tbMemory;
        private TextBox tbStack;
        private CPU cpu;

        private string memFile = "default.mem";

        public Memory(TextBox textBoxMemory, TextBox textBoxStack)
        {
            tbMemory = textBoxMemory;
            tbStack = textBoxStack;
        }

        public void DisplayMemory()
        {
            DisplayMemory((ushort)position);
        }
        public void DisplayMemory(ushort pos)
        {
            tbMemory.Clear();
            int begin = pos - 16;
            for (int i = 0; i < 42; i++)
            {
                int p = i + begin;
                if (i > 0)
                    tbMemory.Text += "\r\n";
                if(p >= 0 && p < 65536)
                    tbMemory.Text += $"{p.ToString("X4")} {memory[p].ToString("X2")}";
            }
        }

        public void DisplayStack()
        {
            tbStack.Clear();
            int begin = cpu.SP - 6 + 256;
            for (int i = 0; i < 15; i++)
            {
                int p = i + begin;
                if (i > 0)
                    tbStack.Text += "\r\n";
                if (p >= 256 && p < 512)
                    tbStack.Text += $"{p.ToString("X4")} {memory[p].ToString("X2")}";
            }
        }

        public void LinkCpuSP(CPU refCPU)
        {
            cpu=refCPU;
        }

        public void SetMemoryPosTo(string hex)
        {
            if (Main.isClosing) return;
            SetMemoryPosTo((ushort)Tools.Hex2Int(hex,0));
        }

        public void SetMemoryPosTo(ushort memPos)
        {
            position = memPos;
            DisplayMemory();
        }


        public void SetMemoryPosToPC()
        {
            if (Main.isClosing) return;
            position = cpu.PC;
            DisplayMemory();
        }


        public void Load(string fileName = null)
        {
            if (fileName != null && File.Exists(fileName))
            {
                memFile = fileName;
            }

            string[] fileLines = File.ReadAllLines(memFile);

            int pos = 0;
            int to = -1;
            foreach (string line in fileLines)
            {
                string[] lignes = line.Split(';');
                string ligne = lignes[0].Replace("\t", " ").Replace(",", " ");
                string[] tmp = ligne.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                if (tmp.Length == 0) continue;
                if (tmp[0][0] == '*')
                {
                    string [] tmp2 = tmp[0].Split(new char[] {'-','*'}, StringSplitOptions.RemoveEmptyEntries);
                    pos = Tools.Hex2Int(tmp2[0],0);
                    if(pos<0x0000 || pos > 0xffff) continue;
                    if (tmp2.Length == 2)
                    {
                        to = Tools.Hex2Int(tmp2[1], -1);
                        int idx = 1;
                        while (pos <= to)
                        {
                            if (tmp[idx] != "??" && tmp[idx] != "--")
                                memory[pos] = (byte)Tools.Hex2Int(tmp[idx], 0);
                           
                            pos++;
                            if (pos > 0xffff) continue;
                            idx++;
                            if (idx >= tmp.Length) idx = 1;
                        }
                    }
                    else
                    {
                        for (int idx = 1; idx < tmp.Length; idx++)
                        {
                            if (tmp[idx] != "??" && tmp[idx] != "--")
                                memory[pos] = (byte)Tools.Hex2Int(tmp[idx], 0);
                            pos++;
                            if (pos > 0xffff) break;
                        }
                    }
                }
                else
                {
                    for (int idx = 0; idx < tmp.Length; idx++)
                    {
                        if (tmp[idx] != "??" && tmp[idx] != "--")
                            memory[pos] = (byte)Tools.Hex2Int(tmp[idx], 0);
                        pos++;
                        if (pos > 0xffff) break;
                    }
                }
            }

        }

        public ushort GetWordAt(int memPos)
        {
            return (ushort)(memory[memPos] | memory[memPos + 1]<<8);
        }

        public byte GetByteAt(ushort memPos)
        {
            return (byte)memory[memPos];
        }
    }
}