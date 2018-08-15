using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XC2SaveNETThingy
{
    public abstract class ItemHandle : IXC2SaveObject
    {
        public Byte Type { get; set; }
        protected const int TYPE_BITS = 6;

        public UInt32 Serial { get; set; }

        protected ItemHandle(Byte[] data, int size)
        {
            UInt32 value;
            if (size <= 2)
                value = BitConverter.ToUInt16(data, 0);
            else
                value = BitConverter.ToUInt32(data, 0);

            int bits = size * 8;

            Serial = (value >> TYPE_BITS);
            Type = (Byte)(value & (Byte)(Math.Pow(2, TYPE_BITS) - 1));
        }

        public abstract Byte[] ToRawData();
    }

    public class ItemHandle16 : ItemHandle
    {
        public const int SIZE = 0x2;

        public ItemHandle16(Byte[] data) : base(data, SIZE) { }

        public override Byte[] ToRawData()
        {
            return BitConverter.GetBytes((UInt16)((Serial << TYPE_BITS) + Type));
        }
    }

    public class ItemHandle32 : ItemHandle
    {
        public const int SIZE = 0x4;

        public ItemHandle32(Byte[] data) : base(data, SIZE) { }

        public override Byte[] ToRawData()
        {
            return BitConverter.GetBytes(((Serial << TYPE_BITS) + Type));
        }
    }
}
