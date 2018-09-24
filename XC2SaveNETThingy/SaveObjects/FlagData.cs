using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XC2SaveNETThingy
{
    public class FlagData : IXC2SaveObject
    {
        public class Flag_1
        {
            public bool Value { get; set; }

            public Flag_1(bool value)
            {
                Value = value;
            }
        }

        public class Flag_2_4_8
        {
            public Byte Value { get; set; }

            public Flag_2_4_8(Byte value)
            {
                Value = value;
            }
        }

        public class Flag_16
        {
            public UInt16 Value { get; set; }

            public Flag_16(UInt16 value)
            {
                Value = value;
            }
        }

        public class Flag_32
        {
            public UInt32 Value { get; set; }

            public Flag_32(UInt32 value)
            {
                Value = value;
            }
        }

        public Flag_1[] Flags_1Bit { get; set; }
        public Flag_2_4_8[] Flags_2Bit { get; set; }
        public Flag_2_4_8[] Flags_4Bit { get; set; }
        public Flag_2_4_8[] Flags_8Bit { get; set; }
        public Flag_16[] Flags_16Bit { get; set; }
        public Flag_32[] Flags_32Bit { get; set; }

        public const int FLAGS_1BIT_COUNT = 65536;
        public const int FLAGS_2BIT_COUNT = 65536;
        public const int FLAGS_4BIT_COUNT = 8192;
        public const int FLAGS_8BIT_COUNT = 8192;
        public const int FLAGS_16BIT_COUNT = 3072;
        public const int FLAGS_32BIT_COUNT = 3336;

        public const int FLAGS_1BIT_BYTES = 0x2000;
        public const int FLAGS_2BIT_BYTES = 0x4000;
        public const int FLAGS_4BIT_BYTES = 0x1000;
        public const int FLAGS_8BIT_BYTES = 0x2000;
        public const int FLAGS_16BIT_BYTES = 0x1800;
        public const int FLAGS_32BIT_BYTES = 0x3420;

        private static readonly Dictionary<string, int> LOC = new Dictionary<string, int>()
        {
            { "Flags_1Bit", 0x0 },
            { "Flags_2Bit", 0x2000 },
            { "Flags_4Bit", 0x6000 },
            { "Flags_8Bit", 0x7000 },
            { "Flags_16Bit", 0x9000 },
            { "Flags_32Bit", 0xA800 },
        };

        public FlagData(Byte[] data)
        {
            Flags_1Bit = new Flag_1[FLAGS_1BIT_COUNT];
            BitArray derp = new BitArray(data.GetByteSubArray(LOC["Flags_1Bit"], FLAGS_1BIT_BYTES));
            for (int i = 0; i < Flags_1Bit.Length; i++)
                Flags_1Bit[i] = new Flag_1(derp[i]);

            Flags_2Bit = new Flag_2_4_8[FLAGS_2BIT_COUNT];
            BitArray wat = new BitArray(data.GetByteSubArray(LOC["Flags_2Bit"], FLAGS_2BIT_BYTES));
            for (int i = 0; i < Flags_2Bit.Length; i++)
                Flags_2Bit[i] = new Flag_2_4_8((Byte)(
                    ((wat[(i * 2) + 1] ? 1 : 0) << 1) +
                    (wat[(i * 2)] ? 1 : 0)));

            Flags_4Bit = new Flag_2_4_8[FLAGS_4BIT_COUNT];
            wat = new BitArray(data.GetByteSubArray(LOC["Flags_4Bit"], FLAGS_4BIT_BYTES));
            for (int i = 0; i < Flags_4Bit.Length; i++)
                Flags_4Bit[i] = new Flag_2_4_8((Byte)(
                    ((wat[(i * 4) + 3] ? 1 : 0) << 3) +
                    ((wat[(i * 4) + 2] ? 1 : 0) << 2) +
                    ((wat[(i * 4) + 1] ? 1 : 0) << 1) +
                    (wat[(i * 4)] ? 1 : 0)
                    ));

            Flags_8Bit = new Flag_2_4_8[FLAGS_8BIT_COUNT];
            for (int i = 0; i < Flags_8Bit.Length; i++)
                Flags_8Bit[i] = new Flag_2_4_8(data[LOC["Flags_8Bit"] + i]);

            Flags_16Bit = new Flag_16[FLAGS_16BIT_COUNT];
            for (int i = 0; i < Flags_16Bit.Length; i++)
                Flags_16Bit[i] = new Flag_16(BitConverter.ToUInt16(data.GetByteSubArray(LOC["Flags_16Bit"] + (i * 2), 2), 0));

            Flags_32Bit = new Flag_32[FLAGS_32BIT_COUNT];
            for (int i = 0; i < Flags_32Bit.Length; i++)
                Flags_32Bit[i] = new Flag_32(BitConverter.ToUInt32(data.GetByteSubArray(LOC["Flags_32Bit"] + (i * 4), 4), 0));
        }

        public Byte[] ToRawData()
        {
            List<Byte> result = new List<Byte>();

            for (int i = 0; i < FLAGS_1BIT_BYTES; i++)
                result.Add((Byte)(
                        ((Flags_1Bit[(i * 8) + 7].Value ? 1 : 0) << 7) +
                        ((Flags_1Bit[(i * 8) + 6].Value ? 1 : 0) << 6) +
                        ((Flags_1Bit[(i * 8) + 5].Value ? 1 : 0) << 5) +
                        ((Flags_1Bit[(i * 8) + 4].Value ? 1 : 0) << 4) +
                        ((Flags_1Bit[(i * 8) + 3].Value ? 1 : 0) << 3) +
                        ((Flags_1Bit[(i * 8) + 2].Value ? 1 : 0) << 2) +
                        ((Flags_1Bit[(i * 8) + 1].Value ? 1 : 0) << 1) +
                        (Flags_1Bit[i * 8].Value ? 1 : 0)
                    ));

            for (int i = 0; i < FLAGS_2BIT_BYTES; i++)
                result.Add((Byte)
                    (
                        (Flags_2Bit[(i * 4) + 3].Value << 6) +
                        (Flags_2Bit[(i * 4) + 2].Value << 4) +
                        (Flags_2Bit[(i * 4) + 1].Value << 2) +
                        (Flags_2Bit[i * 4].Value)
                    ));

            for (int i = 0; i < FLAGS_4BIT_BYTES; i++)
                result.Add((Byte)
                    (
                        (Flags_4Bit[(i * 2) + 1].Value << 4) +
                        (Flags_4Bit[i * 2].Value)
                    ));

            foreach (Flag_2_4_8 f in Flags_8Bit)
                result.Add(f.Value);

            foreach (Flag_16 f in Flags_16Bit)
                result.AddRange(BitConverter.GetBytes(f.Value));

            foreach (Flag_32 f in Flags_32Bit)
                result.AddRange(BitConverter.GetBytes(f.Value));

            return result.ToArray();
        }

    }
}
