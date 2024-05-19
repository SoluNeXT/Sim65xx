using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using Simulateur65xx.FW;

namespace Simulateur65xx.OB
{
    public class OpCode
    {
        public byte Value;
        public string Hex;
        public string ShortName;
        public string LongName;
        public PType ParamType;
        public byte NbParameters;
        public byte NbCycles;
        public bool OverCycles;

        public OpCode(string hex, string shortName, string longName, PType pType, byte nbParameters,
            byte nbCycles, bool overCycles=false)
        {
            Value = (byte)Tools.Hex2Int(hex, 0);
            Hex = hex;
            ShortName = shortName;
            LongName = longName;
            NbParameters = nbParameters;
            NbCycles = nbCycles;
            OverCycles = overCycles;
            ParamType = pType;
        }

        public enum PType {
            NoParam=0,
            Immediate,
            Absolute,
            AbsoluteIndexedX,
            AbsoluteIndexedY,
            Indirect,
            IndirectIndexedY,
            IndexedXIndirect,
        }

        public static Dictionary<PType, string> PFormat = new Dictionary<PType, string>
        {
            { PType.NoParam, "            " },
            { PType.Immediate, "#${0}        " },
        };

        public enum OPCODE
        {
            LDA_ABSOLUTE = 0xA9,
            BRK = 0x00,
        }
        
        public static List<OpCode> OpCodes = new List<OpCode>
        {
            new OpCode("A9","LDA", "LDA immediate", PType.Immediate,1,2),
            new OpCode("00","BRK", "BREAK", PType.NoParam,0,7),
        };

        public static OpCode GetOpCode(string hex)
        {
            return GetOpCode((byte)Tools.Hex2Int(hex,0));
        }

        public static OpCode GetOpCode(byte opCode)
        {
            foreach (OpCode o in OpCodes)
            {
                if(o.Value==opCode)
                    return o;
            }

            return null;
        }
    }
}