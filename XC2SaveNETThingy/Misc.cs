using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XC2SaveNETThingy
{
    public class gf_ElapseTime : IXC2SaveObject
    {
        public const int SIZE = 0x4;
        //UInt32 Seconds : 6;
        //UInt32 Minutes : 6;
        //UInt32 Hours : 20;

        Byte Seconds;
        private const int SECONDS_BITS = 6;
        Byte Minutes;
        private const int MINUTES_BITS = 6;
        UInt32 Hours;
        private const int HOURS_BITS = 20;

        public gf_ElapseTime(Byte[] data)
        {
            UInt32 value = BitConverter.ToUInt32(data, 0);

            Hours = (value >> ((SIZE * 8) - HOURS_BITS));
            Minutes = (Byte)((value >> ((SIZE * 8) - HOURS_BITS - MINUTES_BITS)) & ((2 ^ MINUTES_BITS) - 1));
            Seconds = (Byte)(value & ((2 ^ SECONDS_BITS) - 1));
        }

        public gf_ElapseTime()
        {
            Hours = 0;
            Minutes = 0;
            Seconds = 0;
        }

        public Byte[] ToRawData()
        {
            return BitConverter.GetBytes((UInt32)((Hours << ((SIZE * 8) - HOURS_BITS)) + (Minutes << SECONDS_BITS) + Seconds));
        }
    }

    public abstract class PaddedString : IXC2SaveObject
    {
        public string Str = "";
        public int Length;

        public PaddedString(Byte[] data, int size)
        {
            Length = data[data.Length - 4];
            for (int i = 0; i < Length; i++)
            {
                Str += (char)data[i];
            }
        }

        public abstract Byte[] ToRawData();

        public Byte[] ToRawData(int size)
        {
            Length = Str.Length;

            Byte[] bytes = new Byte[size + 4];

            for (int i = 0; i < bytes.Length - 4; i++)
            {
                if (i < Length)
                {
                    bytes[i] = (Byte)Str[i];
                }
                else
                {
                    bytes[i] = 0x00;
                }
            }

            bytes[bytes.Length - 4] = (Byte)Length;
            bytes[bytes.Length - 3] = 0x00;
            bytes[bytes.Length - 2] = 0x00;
            bytes[bytes.Length - 1] = 0x00;

            return bytes;
        }

        public override string ToString()
        {
            return Str;
        }
    }

    public class PaddedString16 : PaddedString
    {
        public const int SIZE = 0x14;

        public PaddedString16(Byte[] data) : base(data, SIZE) { }

        public override byte[] ToRawData()
        {
            return ToRawData(SIZE);
        }
    }

    public class PaddedString32 : PaddedString
    {
        public const int SIZE = 0x24;

        public PaddedString32(Byte[] data) : base(data, SIZE) { }

        public override byte[] ToRawData()
        {
            return ToRawData(SIZE);
        }
    }

    public abstract class ItemHandle : IXC2SaveObject
    {
        public Byte Type;
        private const int TYPE_BITS = 6;

        public UInt32 Serial;

        protected ItemHandle(Byte[] data, int size)
        {
            UInt16 value = BitConverter.ToUInt16(data, 0);
            int bits = size * 8;

            Serial = (UInt16)(value >> bits - (bits - TYPE_BITS));
            Type = (Byte)(value & ((2 ^ (TYPE_BITS)) - 1));
        }

        public abstract Byte[] ToRawData();

        protected Byte[] ToRawData(int size)
        {
            int bits = size * 8;
            return BitConverter.GetBytes((UInt32)((Serial << (bits - (bits - TYPE_BITS)) + Type)));
        }
    }

    public class ItemHandle16 : ItemHandle
    {
        public const int SIZE = 0x2;

        public ItemHandle16(Byte[] data) : base(data, SIZE) { }

        public override Byte[] ToRawData()
        {
            return ToRawData(SIZE);
        }
    }

    public class ItemHandle32 : ItemHandle
    {
        public const int SIZE = 0x4;

        public ItemHandle32(Byte[] data) : base(data, SIZE) { }

        public override Byte[] ToRawData()
        {
            return ToRawData(SIZE);
        }
    }
}
