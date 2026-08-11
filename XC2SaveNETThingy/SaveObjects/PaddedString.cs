using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XC2SaveNETThingy
{
    public abstract class PaddedString : IXC2SaveObject
    {
        private readonly Byte[] OriginalData;
        private readonly string OriginalString;
        private readonly int OriginalLength;

        public string Str { get; set; }
        public int Length { get; set; }

        public PaddedString(Byte[] data, int size)
        {
            OriginalData = (Byte[])data.Clone();
            Str = "";
            Length = data[data.Length - 4];
            for (int i = 0; i < Length; i++)
                Str += (char)data[i];

            OriginalString = Str;
            OriginalLength = Length;
        }

        public abstract Byte[] ToRawData();

        public Byte[] ToRawData(int size)
        {
            if (size == OriginalData.Length &&
                Length == OriginalLength &&
                String.Equals(Str, OriginalString, StringComparison.Ordinal))
            {
                return (Byte[])OriginalData.Clone();
            }

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
}
