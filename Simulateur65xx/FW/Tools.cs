using System;
using System.Globalization;
using System.Windows.Forms;
using static System.Net.Mime.MediaTypeNames;

namespace Simulateur65xx.FW
{
    public class Tools
    {
        public static string Hex2Dec(string text)
        {
            int v = Hex2Int(text, -1);
            if (v < 0) return "???";
            return v.ToString().Trim();
        }

        public static string Hex2Bin(string text)
        {
            string s = "";
            foreach (char c in text)
            {
                if (s!="") s += " ";
                switch (c)
                {
                    case '0': 
                        s += "0000";
                        break;
                    case '1':
                        s += "0001";
                        break;
                    case '2':
                        s += "0010";
                        break;
                    case '3':
                        s += "0011";
                        break;
                    case '4':
                        s += "0100";
                        break;
                    case '5':
                        s += "0101";
                        break;
                    case '6':
                        s += "0110";
                        break;
                    case '7':
                        s += "0111";
                        break;
                    case '8':
                        s += "1000";
                        break;
                    case '9':
                        s += "1001";
                        break;
                    case 'A':
                        s += "1010";
                        break;
                    case 'B':
                        s += "1011";
                        break;
                    case 'C':
                        s += "1100";
                        break;
                    case 'D':
                        s += "1101";
                        break;
                    case 'E':
                        s += "1110";
                        break;
                    case 'F':
                        s += "1111";
                        break;
                    default:
                        s += "????";
                        break;
                }
            }

            return s;
        }

        public static void CopyRegisterValues(TextBox h, TextBox b, TextBox d)
        {
            if (h == null) return;
            if (b != null)
                b.Text = Tools.Hex2Bin(h.Text);
            if (d != null)
                d.Text = Tools.Hex2Dec(h.Text);
        }

        public static int Hex2Int(string text, int defaut)
        {
            try
            {
                if (!text.StartsWith("0x")) text = "0x" + text;
                int v = Convert.ToInt32(text, 16);// NumberStyles.HexNumber);
                return v;
            }
            catch
            {
                return defaut;
            }
        }
    }
}