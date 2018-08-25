using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XC2SaveNETThingy
{
    public class HanaCircuits : IXC2SaveObject
    {
        public const int SIZE = 0x2;

        public bool Specials0_Enabled { get; set; }
        public bool Specials1_Enabled { get; set; }
        public bool Specials2_Enabled { get; set; }
        public bool Skill0_Enabled { get; set; }
        public bool Skill1_Enabled { get; set; }
        public bool Skill2_Enabled { get; set; }

        public HanaCircuits(byte[] data)
        {
            Specials0_Enabled = (data[0] & 1) == 1;
            Specials1_Enabled = ((data[0] >> 1) & 1) == 1;
            Specials2_Enabled = ((data[0] >> 2) & 1) == 1;
            Skill0_Enabled = ((data[0] >> 3) & 1) == 1;
            Skill1_Enabled = ((data[0] >> 4) & 1) == 1;
            Skill2_Enabled = ((data[0] >> 5) & 1) == 1;
        }

        public Byte[] ToRawData()
        {
            Byte data = 0;
            data += (Byte)(Specials0_Enabled ? 1 : 0);
            data += (Byte)((Specials1_Enabled ? 1 : 0) << 1);
            data += (Byte)((Specials2_Enabled ? 1 : 0) << 2);
            data += (Byte)((Skill0_Enabled ? 1 : 0) << 3);
            data += (Byte)((Skill1_Enabled ? 1 : 0) << 4);
            data += (Byte)((Skill2_Enabled ? 1 : 0) << 5);

            return new Byte[] { data, 0x00 };
        }
    };
}
