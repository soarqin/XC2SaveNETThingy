using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XC2SaveNETThingy
{
    public class ArtsEnhance : IXC2SaveObject
    {
        public const int SIZE = 0xC;

        public UInt16 EnhanceID { get; set; }
        public UInt16 RecastRev { get; set; }
        public UInt16 ItemID { get; set; }
        public Byte[] Unk_0x06 { get; set; }
        public Byte[] Unk_0x08 { get; set; }
        public ItemHandle16 ItemHandle { get; set; }

        public ArtsEnhance(Byte[] data)
        {
            EnhanceID = BitConverter.ToUInt16(data.GetByteSubArray(0x0, 2), 0);
            RecastRev = BitConverter.ToUInt16(data.GetByteSubArray(0x2, 2), 0);
            ItemID = BitConverter.ToUInt16(data.GetByteSubArray(0x4, 2), 0);
            Unk_0x06 = data.GetByteSubArray(0x6, 2);
            Unk_0x08 = data.GetByteSubArray(0x8, 2);
            ItemHandle = new ItemHandle16(data.GetByteSubArray(0xA, ItemHandle16.SIZE));
        }

        public Byte[] ToRawData()
        {
            List<Byte> result = new List<Byte>();

            result.AddRange(BitConverter.GetBytes(EnhanceID));
            result.AddRange(BitConverter.GetBytes(RecastRev));
            result.AddRange(BitConverter.GetBytes(ItemID));
            result.AddRange(Unk_0x06);
            result.AddRange(Unk_0x08);
            result.AddRange(ItemHandle.ToRawData());

            if (result.Count != SIZE)
            {
                string message = "ArtsEnhance: SIZE ALL WRONG!!!" + Environment.NewLine +
                "Size should be " + SIZE + " bytes..." + Environment.NewLine +
                "...but Size is " + result.Count + " bytes!";

                throw new Exception(message);
            }

            return result.ToArray();
        }
    };
}
