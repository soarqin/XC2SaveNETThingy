using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XC2SaveNETThingy
{
    public class MercGroupPreset : IXC2SaveObject
    {
        public const int SIZE = 0xC;
        public const int COUNT = 8;

        public UInt16[] Members { get; set; }

        public MercGroupPreset(Byte[] data)
        {
            Members = new UInt16[6];
            for (int i = 0; i < Members.Length; i++)
                Members[i] = BitConverter.ToUInt16(data.GetByteSubArray(i * 2, 2), 0);
        }

        public Byte[] ToRawData()
        {
            List<Byte> result = new List<byte>();

            foreach (UInt16 m in Members)
                result.AddRange(BitConverter.GetBytes(m));

            if (result.Count != SIZE)
            {
                string message = "MercenaryTeamPreset: SIZE ALL WRONG!!!" + Environment.NewLine +
                "Size should be " + SIZE + " bytes..." + Environment.NewLine +
                "...but Size is " + result.Count + " bytes!";

                throw new Exception(message);
            }

            return result.ToArray();
        }
    }
}
