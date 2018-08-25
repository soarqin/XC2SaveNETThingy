using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XC2SaveNETThingy
{
    public class BladeSkill : IXC2SaveObject
    {
        public const int SIZE = 0x6;

        public UInt16 ID { get; set; }
        public Byte Unk_0x02 { get; set; }
        public Byte Level { get; set; }
        public Byte MaxLevel { get; set; }
        public Byte Unk_0x05 { get; set; }

        public BladeSkill(Byte[] data)
        {
            ID = BitConverter.ToUInt16(data.GetByteSubArray(0, 2), 0);
            Unk_0x02 = data[2];
            Level = data[3];
            MaxLevel = data[4];
            Unk_0x05 = data[5];
        }

        public Byte[] ToRawData()
        {
            List<Byte> result = new List<byte>();

            result.AddRange(BitConverter.GetBytes(ID));
            result.Add(Unk_0x02);
            result.Add(Level);
            result.Add(MaxLevel);
            result.Add(Unk_0x05);

            return result.ToArray();
        }
    }
}
