using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XC2SaveNETThingy
{
    public class ElapseTime : IXC2SaveObject
    {
        public const int SIZE = 0x4;

        public Byte Seconds { get; set; }
        private const int SECONDS_BITS = 6;
        public Byte Minutes { get; set; }
        private const int MINUTES_BITS = 6;
        public UInt32 Hours { get; set; }
        private const int HOURS_BITS = 20;

        public ElapseTime(Byte[] data)
        {
            UInt32 value = BitConverter.ToUInt32(data, 0);

            Hours = (value >> ((SIZE * 8) - HOURS_BITS));
            Minutes = (Byte)((value >> ((SIZE * 8) - HOURS_BITS - MINUTES_BITS)) & (Byte)(Math.Pow(2, MINUTES_BITS) - 1));
            Seconds = (Byte)(value & (Byte)(Math.Pow(2, SECONDS_BITS) - 1));
        }

        public ElapseTime()
        {
            Hours = 0;
            Minutes = 0;
            Seconds = 0;
        }

        public Byte[] ToRawData()
        {
            return BitConverter.GetBytes((UInt32)((Hours << ((SIZE * 8) - HOURS_BITS)) + (Minutes << SECONDS_BITS) + Seconds));
        }

        public override string ToString()
        {
            return Hours + ":" + Minutes.ToString("D2") + ":" + Seconds.ToString("D2");
        }
    }

    public abstract class PaddedString : IXC2SaveObject
    {
        public string Str { get; set; }
        public int Length { get; set; }

        public PaddedString(Byte[] data, int size)
        {
            Str = "";
            Length = data[data.Length - 4];
            for (int i = 0; i < Length; i++)
                Str += (char)data[i];
        }

        public abstract Byte[] ToRawData();

        public Byte[] ToRawData(int size)
        {
            Length = Str.Length;

            Byte[] bytes = new Byte[size];

            for (int i = 0; i < bytes.Length - 4; i++)
            {
                if (i < Length)
                    bytes[i] = (Byte)Str[i];
                else
                    bytes[i] = 0x00;
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
        protected const int TYPE_BITS = 6;

        public UInt32 Serial;

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
