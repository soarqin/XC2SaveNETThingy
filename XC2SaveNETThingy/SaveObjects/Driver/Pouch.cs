using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XC2SaveNETThingy
{
    public class Pouch : IXC2SaveObject
    {
        public const int SIZE = 0x8;
        public const int COUNT = 3;

        public float Time { get; set; }
        public UInt16 ItemId { get; set; }
        public bool IsEnabled { get; set; }

        public Pouch(Byte[] data)
        {
            Time = BitConverter.ToSingle(data.GetByteSubArray(0, 4), 0);
            ItemId = BitConverter.ToUInt16(data.GetByteSubArray(4, 2), 0);
            IsEnabled = BitConverter.ToUInt16(data.GetByteSubArray(6, 2), 0) == 1;
        }

        public Byte[] ToRawData()
        {
            List<Byte> result = new List<Byte>();

            result.AddRange(BitConverter.GetBytes(Time));
            result.AddRange(BitConverter.GetBytes(ItemId));
            result.AddRange(BitConverter.GetBytes((UInt16)(IsEnabled ? 1 : 0)));

            if (result.Count != SIZE)
            {
                string message = "Pouch: SIZE ALL WRONG!!!" + Environment.NewLine +
                "Size should be " + SIZE + " bytes..." + Environment.NewLine +
                "...but Size is " + result.Count + " bytes!";

                throw new Exception(message);
            }

            return result.ToArray();
        }
    }
}
