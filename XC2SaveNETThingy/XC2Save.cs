using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XC2SaveNETThingy
{
    public class Vec3 : IXC2SaveObject
    {
        public const int SIZE = 0xC;

        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }

        public Vec3(float X = 0, float Y = 0, float Z = 0)
        {
            this.X = X;
            this.Y = Y;
            this.Z = Z;
        }

        public Vec3(Byte[] data)
        {
            X = BitConverter.ToSingle(data.GetByteSubArray(0, 4), 0);
            Y = BitConverter.ToSingle(data.GetByteSubArray(4, 4), 0);
            Z = BitConverter.ToSingle(data.GetByteSubArray(8, 4), 0);
        }

        public virtual Byte[] ToRawData()
        {
            List<Byte> result = new List<Byte>();

            result.AddRange(BitConverter.GetBytes(X));
            result.AddRange(BitConverter.GetBytes(Y));
            result.AddRange(BitConverter.GetBytes(Z));

            if (result.Count != SIZE)
            {
                string message = "Vec3: SIZE ALL WRONG!!!" + Environment.NewLine +
                "Size should be " + SIZE + " bytes..." + Environment.NewLine +
                "...but Size is " + result.Count + " bytes!";

                throw new Exception(message);
            }

            return result.ToArray();
        }
    }

    public class Vec3Padded : Vec3, IXC2SaveObject
    {
        public new const int SIZE = 0x10;

        public float Padding { get; set; }

        public Vec3Padded(float X = 0, float Y = 0, float Z = 0, float Padding = 0) : base(X, Y, Z)
        {
            this.Padding = Padding;
        }

        public Vec3Padded(Byte[] data) : base(data)
        {
            Padding = BitConverter.ToSingle(data.GetByteSubArray(0xC, 4), 0);
        }

        public override Byte[] ToRawData()
        {
            List<Byte> result = new List<Byte>();

            result.AddRange(base.ToRawData());
            result.AddRange(BitConverter.GetBytes(Padding));

            if (result.Count != SIZE)
            {
                string message = "Vec3Padded: SIZE ALL WRONG!!!" + Environment.NewLine +
                "Size should be " + SIZE + " bytes..." + Environment.NewLine +
                "...but Size is " + result.Count + " bytes!";

                throw new Exception(message);
            }

            return result.ToArray();
        }
    }

    public class XC2Save : IXC2SaveObject
    {
        public class GameTime : IXC2SaveObject
        {
            public const int SIZE = 0x4;

            public Byte Seconds { get; set; }
            private const int SECONDS_BITS = 6;

            public Byte Minutes { get; set; }
            private const int MINUTES_BITS = 6;

            public Byte Hours { get; set; }
            private const int HOURS_BITS = 5;

            public UInt16 Days { get; set; }
            private const int DAYS_BITS = 15;

            public GameTime(Byte[] data)
            {
                UInt32 value = BitConverter.ToUInt32(data, 0);

                Days = (UInt16)(value >> (32 - DAYS_BITS));
                Hours = (Byte)((value >> (32 - DAYS_BITS - HOURS_BITS)) & (Byte)(Math.Pow(2, HOURS_BITS) - 1));
                Minutes = (Byte)((value >> (32 - DAYS_BITS - HOURS_BITS - MINUTES_BITS)) & (Byte)(Math.Pow(2, MINUTES_BITS) - 1));
                Seconds = (Byte)(value & (Byte)(Math.Pow(2, SECONDS_BITS) - 1));
            }

            public Byte[] ToRawData()
            {
                UInt32 result = (UInt32)((Days << (32 - DAYS_BITS)) + (Hours << (32 - DAYS_BITS - HOURS_BITS)) + (Minutes << SECONDS_BITS) + Seconds);

                return BitConverter.GetBytes(result);
            }
        };
        public class RealTime : IXC2SaveObject
        {
            public const int SIZE = 0x8;
            // 13 bits
            public UInt16 Year { get; set; }
            private const int YEAR_BITS = 13;

            // 4 bits
            public Byte Month { get; set; }
            private const int MONTH_BITS = 4;

            // 9 bits
            public UInt16 UnkB { get; set; }
            private const int UNKB_BITS = 9;

            // 5 bits
            public Byte Day { get; set; }
            private const int DAY_BITS = 5;

            // 5 bits
            public Byte Hour { get; set; }
            private const int HOUR_BITS = 5;

            // 6 bits
            public Byte Minute { get; set; }
            private const int MINUTE_BITS = 6;

            // 6 bits 
            public Byte Second { get; set; }
            private const int SECOND_BITS = 6;

            // 10 bits
            public UInt16 MSecond { get; set; }
            private const int MSECOND_BITS = 10;

            // 6 bits
            public Byte UnkA { get; set; }
            private const int UNKA_BITS = 6;

            public RealTime(Byte[] data)
            {
                UInt64 value = BitConverter.ToUInt64(data, 0);

                Year = (UInt16)(value >> (64 - YEAR_BITS));
                Month = (Byte)((value << YEAR_BITS) >> (64 - MONTH_BITS));
                UnkB = (UInt16)((value << (YEAR_BITS + MONTH_BITS)) >> (64 - UNKB_BITS));
                Day = (Byte)((value << (YEAR_BITS + MONTH_BITS + UNKB_BITS)) >> (64 - DAY_BITS));
                Hour = (Byte)((value << (YEAR_BITS + MONTH_BITS + UNKB_BITS + DAY_BITS)) >> (64 - HOUR_BITS));
                Minute = (Byte)((value << (YEAR_BITS + MONTH_BITS + UNKB_BITS + DAY_BITS + HOUR_BITS)) >> (64 - MINUTE_BITS));
                Second = (Byte)((value << (YEAR_BITS + MONTH_BITS + UNKB_BITS + DAY_BITS + HOUR_BITS + MINUTE_BITS)) >> (64 - SECOND_BITS));
                MSecond = (UInt16)((value << (YEAR_BITS + MONTH_BITS + UNKB_BITS + DAY_BITS + HOUR_BITS + MINUTE_BITS + SECOND_BITS)) >> (64 - MSECOND_BITS));
                UnkA = (Byte)((value << (YEAR_BITS + MONTH_BITS + UNKB_BITS + DAY_BITS + HOUR_BITS + MINUTE_BITS + SECOND_BITS + MSECOND_BITS)) >> (64 - UNKA_BITS));
            }

            public Byte[] ToRawData()
            {
                UInt64 derp = 0;

                derp += (UInt64)Year << (64 - YEAR_BITS);
                derp += (UInt64)Month << (64 - YEAR_BITS - MONTH_BITS);
                derp += (UInt64)UnkB << (64 - YEAR_BITS - MONTH_BITS - UNKB_BITS);
                derp += (UInt64)Day << (64 - YEAR_BITS - MONTH_BITS - UNKB_BITS - DAY_BITS);
                derp += (UInt64)Hour << (64 - YEAR_BITS - MONTH_BITS - UNKB_BITS - DAY_BITS - HOUR_BITS);
                derp += (UInt64)Minute << (64 - YEAR_BITS - MONTH_BITS - UNKB_BITS - DAY_BITS - HOUR_BITS - MINUTE_BITS);
                derp += (UInt64)Second << (64 - YEAR_BITS - MONTH_BITS - UNKB_BITS - DAY_BITS - HOUR_BITS - MINUTE_BITS - SECOND_BITS);
                derp += (UInt64)MSecond << (64 - YEAR_BITS - MONTH_BITS - UNKB_BITS - DAY_BITS - HOUR_BITS - MINUTE_BITS - SECOND_BITS - MSECOND_BITS);
                derp += (UInt64)UnkA;

                return BitConverter.GetBytes(derp);
            }

            public override string ToString()
            {
                return String.Format("{0:0000}-{1:00}-{2:00} {3:00}:{4:00}:{5:00}.{6:000}", Year, Month, Day, Hour, Minute, Second, MSecond);
            }
        }
        public class PartyMember : IXC2SaveObject
        {
            public const int SIZE = 0x6;

            public UInt16 DriverId { get; set; }
            public Byte[] Unk_0x02 { get; set; }

            public PartyMember(Byte[] data)
            {
                DriverId = BitConverter.ToUInt16(data.GetByteSubArray(0, 2), 0);
                Unk_0x02 = data.GetByteSubArray(2, 4);
            }

            public Byte[] ToRawData()
            {
                List<Byte> result = new List<byte>();

                result.AddRange(BitConverter.GetBytes(DriverId));
                result.AddRange(Unk_0x02);

                if (result.Count != SIZE)
                {
                    string message = "PartyMember: SIZE ALL WRONG!!!" + Environment.NewLine +
                    "Size should be " + SIZE + " bytes..." + Environment.NewLine +
                    "...but Size is " + result.Count + " bytes!";

                    throw new Exception(message);
                }

                return result.ToArray();
            }

            public override string ToString()
            {
                return (string)XC2Data.Drivers().Select("Name WHERE ID = " + DriverId)[0][0];
            }
        }
        public class XC2Party : IXC2SaveObject
        {
            public const int SIZE = 0x54;
            public const int MEMBERS_COUNT = 10;

            private static readonly Dictionary<string, int> LOC = new Dictionary<string, int>()
            {
                { "Members", 0x0 },
                { "Unk_0x3C", 0x3C },
                { "PartyLeader", 0x40 },
                { "Unk_0x44", 0x44 },
                { "PartyGauge", 0x4C },
                { "Unk_0x4E", 0x4E },
            };

            public PartyMember[] Members { get; set; }
            public Byte[] Unk_0x3C { get; set; }
            public UInt32 Leader { get; set; }
            public Byte[] Unk_0x44 { get; set; }
            public UInt16 PartyGauge { get; set; }
            public Byte[] Unk_0x4E { get; set; }

            public XC2Party(Byte[] data)
            {
                Members = new PartyMember[10];
                for (int i = 0; i < Members.Length; i++)
                    Members[i] = new PartyMember(data.GetByteSubArray(LOC["Members"] + (i * PartyMember.SIZE), PartyMember.SIZE));

                Unk_0x3C = data.GetByteSubArray(LOC["Unk_0x3C"], 4);
                Leader = BitConverter.ToUInt32(data.GetByteSubArray(LOC["PartyLeader"], 4), 0);
                Unk_0x44 = data.GetByteSubArray(LOC["Unk_0x44"], 8);
                PartyGauge = BitConverter.ToUInt16(data.GetByteSubArray(LOC["PartyGauge"], 2), 0);
                Unk_0x4E = data.GetByteSubArray(LOC["Unk_0x4E"], 6);
            }

            public Byte[] ToRawData()
            {
                List<Byte> result = new List<Byte>();

                foreach (PartyMember m in Members)
                    result.AddRange(m.ToRawData());

                result.AddRange(Unk_0x3C);
                result.AddRange(BitConverter.GetBytes(Leader));
                result.AddRange(Unk_0x44);
                result.AddRange(BitConverter.GetBytes(PartyGauge));
                result.AddRange(Unk_0x4E);

                if (result.Count != SIZE)
                {
                    string message = "XC2Party: SIZE ALL WRONG!!!" + Environment.NewLine +
                    "Size should be " + SIZE + " bytes..." + Environment.NewLine +
                    "...but Size is " + result.Count + " bytes!";

                    throw new Exception(message);
                }

                return result.ToArray();
            }
        }

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

            public Flag_1[] Flags_1Bit {get; set;}
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

            public static readonly Dictionary<int, string> NAMES_32BIT = new Dictionary<int, string>()
            {
                { 0, "Flag 1" },
                { 263, "Poppi QT deal critical hits" },
                { 267, "Poppi QTpi Driver Combp Finishers" },
                { 269, "Poppi QTpi Blade Combos Finished" },
                { 276, "Pyra Blade Swaps" },
                { 294, "Total No. of Quests Completed" },
                { 300, "Total No. of Treasure Troves Opened" },
                { 308, "Total No. of Dishes Cooked by Pyra"}
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
        public class MapData : IXC2SaveObject
        {
            public const int SIZE = 0xC4;

            public Vec3Padded[] DriverPositions { get; set; }
            public Vec3Padded[] BladePositions { get; set; }
            public Vec3Padded[] DriverRotations { get; set; }
            public Vec3Padded[] BladeRotations { get; set; }
            public UInt32 MapJumpID { get; set; }

            public MapData(Byte[] data)
            {
                DriverPositions = new Vec3Padded[3];
                for (int i = 0; i < DriverPositions.Length; i++)
                    DriverPositions[i] = new Vec3Padded(data.GetByteSubArray(i * Vec3Padded.SIZE, Vec3Padded.SIZE));

                BladePositions = new Vec3Padded[3];
                for (int i = 0; i < BladePositions.Length; i++)
                    BladePositions[i] = new Vec3Padded(data.GetByteSubArray(0x30 + (i * Vec3Padded.SIZE), Vec3Padded.SIZE));

                DriverRotations = new Vec3Padded[3];
                for (int i = 0; i < DriverRotations.Length; i++)
                    DriverRotations[i] = new Vec3Padded(data.GetByteSubArray(0x60 + (i * Vec3Padded.SIZE), Vec3Padded.SIZE));

                BladeRotations = new Vec3Padded[3];
                for (int i = 0; i < BladeRotations.Length; i++)
                    BladeRotations[i] = new Vec3Padded(data.GetByteSubArray(0x90 + (i * Vec3Padded.SIZE), Vec3Padded.SIZE));

                MapJumpID = BitConverter.ToUInt32(data.GetByteSubArray(0xC0, 4), 0);
            }

            public Byte[] ToRawData()
            {
                List<Byte> result = new List<Byte>();

                foreach (Vec3Padded vp in DriverPositions)
                    result.AddRange(vp.ToRawData());

                foreach (Vec3Padded vp in BladePositions)
                    result.AddRange(vp.ToRawData());

                foreach (Vec3Padded vp in DriverRotations)
                    result.AddRange(vp.ToRawData());

                foreach (Vec3Padded vp in BladeRotations)
                    result.AddRange(vp.ToRawData());

                result.AddRange(BitConverter.GetBytes(MapJumpID));

                if (result.Count != SIZE)
                {
                    string message = "SDataMap: SIZE ALL WRONG!!!" + Environment.NewLine +
                    "Size should be " + SIZE + " bytes..." + Environment.NewLine +
                    "...but Size is " + result.Count + " bytes!";

                    throw new Exception(message);
                }

                return result.ToArray();
            }
        }
        public class MercGroup : IXC2SaveObject
        {
            public const int SIZE = 0x28;

            public UInt16[] TeamMemberIDs { get; set; }
            public Byte[] Unk_0x0C { get; set; }
            public UInt32 TeamID { get; set; }
            public UInt32 MissionID { get; set; }
            public Byte[] Unk_0x18 { get; set; }
            public UInt32 MissionTime { get; set; }
            public UInt32 MissionTimeOriginal { get; set; }
            public Byte[] Unk_0x24 { get; set; }

            private static readonly Dictionary<string, int> LOC = new Dictionary<string, int>()
            {
                { "TeamMemberIds", 0x0 },
                { "field_C", 0xC },
                { "TeamId", 0x10 },
                { "MissionId", 0x14 },
                { "field_18", 0x18 },
                { "MissionTime", 0x1C },
                { "MissionTimeOriginal", 0x20 },
                { "field_24", 0x24 }
            };

            public MercGroup(Byte[] data)
            {
                TeamMemberIDs = new UInt16[6];
                for (int i = 0; i < TeamMemberIDs.Length; i++)
                    TeamMemberIDs[i] = BitConverter.ToUInt16(data.GetByteSubArray(LOC["TeamMemberIds"] + (i * 2), 2), 0);

                Unk_0x0C = data.GetByteSubArray(LOC["field_C"], 4);
                TeamID = BitConverter.ToUInt32(data.GetByteSubArray(LOC["TeamId"], 4), 0);
                MissionID = BitConverter.ToUInt32(data.GetByteSubArray(LOC["MissionId"], 4), 0);
                Unk_0x18 = data.GetByteSubArray(LOC["field_18"], 4);
                MissionTime = BitConverter.ToUInt32(data.GetByteSubArray(LOC["MissionTime"], 4), 0);
                MissionTimeOriginal = BitConverter.ToUInt32(data.GetByteSubArray(LOC["MissionTimeOriginal"], 4), 0);
                Unk_0x24 = data.GetByteSubArray(LOC["field_24"], 4);
            }

            public Byte[] ToRawData()
            {
                List<Byte> result = new List<Byte>();

                foreach (UInt16 tm in TeamMemberIDs)
                    result.AddRange(BitConverter.GetBytes(tm));

                result.AddRange(Unk_0x0C);
                result.AddRange(BitConverter.GetBytes(TeamID));
                result.AddRange(BitConverter.GetBytes(MissionID));
                result.AddRange(Unk_0x18);
                result.AddRange(BitConverter.GetBytes(MissionTime));
                result.AddRange(BitConverter.GetBytes(MissionTimeOriginal));
                result.AddRange(Unk_0x24);

                if (result.Count != SIZE)
                {
                    string message = "MercenaryTeam: SIZE ALL WRONG!!!" + Environment.NewLine +
                    "Size should be " + SIZE + " bytes..." + Environment.NewLine +
                    "...but Size is " + result.Count + " bytes!";

                    throw new Exception(message);
                }

                return result.ToArray();
            }
        }
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
        public class TaskAchieve : IXC2SaveObject
        {
            public const int SIZE = 0xC;
            public const int COUNT = 128;

            //public UInt16 Unk_0x0 { get; set; }
            //public UInt16 Unk_0x2 { get; set; }
            //public UInt16 Unk_0x4 { get; set; }
            //public UInt16 Unk_0x6 { get; set; }
            //public UInt32 Unk_0x8 { get; set; }
            public Byte[] Unk { get; set; }

            public TaskAchieve(Byte[] data)
            {
                //Unk_0x0 = BitConverter.ToUInt16(data.GetByteSubArray(0, 2), 0);
                //Unk_0x2 = BitConverter.ToUInt16(data.GetByteSubArray(2, 2), 0);
                //Unk_0x4 = BitConverter.ToUInt16(data.GetByteSubArray(4, 2), 0);
                //Unk_0x6 = BitConverter.ToUInt16(data.GetByteSubArray(6, 2), 0);
                //Unk_0x8 = BitConverter.ToUInt32(data.GetByteSubArray(8, 4), 0);
                Unk = data;
            }

            public Byte[] ToRawData()
            {
                List<Byte> result = new List<byte>();

                //result.AddRange(BitConverter.GetBytes(Unk_0x0));
                //result.AddRange(BitConverter.GetBytes(Unk_0x2));
                //result.AddRange(BitConverter.GetBytes(Unk_0x4));
                //result.AddRange(BitConverter.GetBytes(Unk_0x6));
                //result.AddRange(BitConverter.GetBytes(Unk_0x8));
                result.AddRange(Unk);

                if (result.Count != SIZE)
                {
                    string message = "TaskAchieve: SIZE ALL WRONG!!!" + Environment.NewLine +
                    "Size should be " + SIZE + " bytes..." + Environment.NewLine +
                    "...but Size is " + result.Count + " bytes!";

                    throw new Exception(message);
                }

                return result.ToArray();
            }
        }
        public class WeatherInfo : IXC2SaveObject
        {
            public const int SIZE = 0x20;
            public const int COUNT = 64;

            public string Name { get; set; }
            public Byte[] Unk_0x04 { get; set; }
            //public UInt32 Unk_0x04 { get; set; }
            //public UInt32 Unk_0x08 { get; set; }
            //public UInt32 Unk_0x0C { get; set; }
            //public float Unk_0x10 { get; set; }
            //public float Unk_0x14 { get; set; }
            //public UInt32 Unk_0x18 { get; set; }
            //public UInt32 Unk_0x1C { get; set; }

            public WeatherInfo(Byte[] data)
            {
                Name = Encoding.ASCII.GetString(data.GetByteSubArray(0x00, 4));
                Unk_0x04 = data.GetByteSubArray(0x04, 0x1C);
                //Unk_0x04 = BitConverter.ToUInt32(data.GetByteSubArray(0x4, 4), 0);
                //Unk_0x08 = BitConverter.ToUInt32(data.GetByteSubArray(0x8, 4), 0);
                //Unk_0x0C = BitConverter.ToUInt32(data.GetByteSubArray(0xC, 4), 0);
                //Unk_0x10 = BitConverter.ToSingle(data.GetByteSubArray(0x10, 4), 0);
                //Unk_0x14 = BitConverter.ToSingle(data.GetByteSubArray(0x14, 4), 0);
                //Unk_0x18 = BitConverter.ToUInt32(data.GetByteSubArray(0x18, 4), 0);
                //Unk_0x1C = BitConverter.ToUInt32(data.GetByteSubArray(0x1C, 4), 0);

            }

            public Byte[] ToRawData()
            {
                List<Byte> result = new List<byte>();

                result.AddRange(Encoding.ASCII.GetBytes(Name));
                result.AddRange(Unk_0x04);
                //result.AddRange(BitConverter.GetBytes(Unk_0x04));
                //result.AddRange(BitConverter.GetBytes(Unk_0x08));
                //result.AddRange(BitConverter.GetBytes(Unk_0x0C));
                //result.AddRange(BitConverter.GetBytes(Unk_0x10));
                //result.AddRange(BitConverter.GetBytes(Unk_0x14));
                //result.AddRange(BitConverter.GetBytes(Unk_0x18));
                //result.AddRange(BitConverter.GetBytes(Unk_0x1C));

                if (result.Count != SIZE)
                {
                    string message = "Weather: SIZE ALL WRONG!!!" + Environment.NewLine +
                    "Size should be " + SIZE + " bytes..." + Environment.NewLine +
                    "...but Size is " + result.Count + " bytes!";

                    throw new Exception(message);
                }

                return result.ToArray();
            }
        }
        public class Event : IXC2SaveObject
        {
            public const int SIZE = 0x64;
            public const int COUNT = 500;

            private static readonly Dictionary<string, int> LOC = new Dictionary<string, int>()
            {
                { "EventID", 0x0 },
                { "Creator", 0x2 },
                { "PlayBladeID", 0x4 },
                { "VoiceID", 0x6 },
                { "Attribute", 0x8 },
                { "ExtraParts", 0xA },
                { "Unk_0x0C", 0xC },
                { "Weapons", 0xE },
                { "Unk_0x22", 0x22 },
                { "BladeIDs", 0x2E },
                { "Unk_0x42", 0x42 },
                { "Time", 0x4C },
                { "CurrentMapWeatherID", 0x50 },
                { "CurrentWeatherType", 0x52 },
                { "Unk_0x54", 0x54 }
            };

            public UInt16 EventID { get; set; }
            public UInt16 Creator { get; set; }
            public UInt16 PlayBladeID { get; set; }
            public UInt16 VoiceID { get; set; }
            public UInt16 Attribute { get; set; }
            public UInt16 ExtraParts { get; set; }
            public Byte[] Unk_0x0C { get; set; }
            public UInt16[] Weapons { get; set; }
            public Byte[] Unk_0x22 { get; set; }
            public UInt16[] BladeIDs { get; set; }
            public Byte[] Unk_0x42 { get; set; }
            public GameTime Time { get; set; }
            public UInt16 CurrentMapWeatherID { get; set; }
            public UInt16 CurrentWeatherType { get; set; }
            public Byte[] Unk_0x54 { get; set; }

            public Event(Byte[] data)
            {
                EventID = BitConverter.ToUInt16(data.GetByteSubArray(LOC["EventID"], 2), 0);
                Creator = BitConverter.ToUInt16(data.GetByteSubArray(LOC["Creator"], 2), 0);
                PlayBladeID = BitConverter.ToUInt16(data.GetByteSubArray(LOC["PlayBladeID"], 2), 0);
                VoiceID = BitConverter.ToUInt16(data.GetByteSubArray(LOC["VoiceID"], 2), 0);
                Attribute = BitConverter.ToUInt16(data.GetByteSubArray(LOC["Attribute"], 2), 0);
                ExtraParts = BitConverter.ToUInt16(data.GetByteSubArray(LOC["ExtraParts"], 2), 0);
                Unk_0x0C = data.GetByteSubArray(LOC["Unk_0x0C"], 2);

                Weapons = new UInt16[10];
                for (int i = 0; i < Weapons.Length; i++)
                    Weapons[i] = BitConverter.ToUInt16(data.GetByteSubArray(LOC["Weapons"] + (i * 2), 2), 0);

                //Unk_0x22 = new UInt16[6];
                //for (int i = 0; i < Unk_0x22.Length; i++)
                //    Unk_0x22[i] = BitConverter.ToUInt16(data.GetByteSubArray(LOC["Unk_0x22"] + (i * 2), 2), 0);
                Unk_0x22 = data.GetByteSubArray(LOC["Unk_0x22"], 0x0C);

                BladeIDs = new UInt16[10];
                for (int i = 0; i < BladeIDs.Length; i++)
                    BladeIDs[i] = BitConverter.ToUInt16(data.GetByteSubArray(LOC["BladeIDs"] + (i * 2), 2), 0);

                //Unk_0x42 = new UInt16[5];
                //for (int i = 0; i < Unk_0x42.Length; i++)
                //    Unk_0x42[i] = BitConverter.ToUInt16(data.GetByteSubArray(LOC["Unk_0x42"] + (i * 2), 2), 0);
                Unk_0x42 = data.GetByteSubArray(LOC["Unk_0x42"], 0x0A);

                Time = new GameTime(data.GetByteSubArray(LOC["Time"], 4));
                CurrentMapWeatherID = BitConverter.ToUInt16(data.GetByteSubArray(LOC["CurrentMapWeatherID"], 2), 0);
                CurrentWeatherType = BitConverter.ToUInt16(data.GetByteSubArray(LOC["CurrentWeatherType"], 2), 0);

                //Unk_0x54 = new UInt16[8];
                //for (int i = 0; i < Unk_0x54.Length; i++)
                //    Unk_0x54[i] = BitConverter.ToUInt16(data.GetByteSubArray(LOC["Unk_0x54"], 2), 0);
                Unk_0x54 = data.GetByteSubArray(LOC["Unk_0x54"], 0x10);
            }

            public Byte[] ToRawData()
            {
                List<Byte> result = new List<Byte>();

                result.AddRange(BitConverter.GetBytes(EventID));
                result.AddRange(BitConverter.GetBytes(Creator));
                result.AddRange(BitConverter.GetBytes(PlayBladeID));
                result.AddRange(BitConverter.GetBytes(VoiceID));
                result.AddRange(BitConverter.GetBytes(Attribute));
                result.AddRange(BitConverter.GetBytes(ExtraParts));
                result.AddRange(Unk_0x0C);

                foreach (UInt16 w in Weapons)
                    result.AddRange(BitConverter.GetBytes(w));

                //foreach (UInt16 u in Unk_0x22)
                //    result.AddRange(BitConverter.GetBytes(u));
                result.AddRange(Unk_0x22);

                foreach (UInt16 u in BladeIDs)
                    result.AddRange(BitConverter.GetBytes(u));

                //foreach (UInt16 u in Unk_0x42)
                //    result.AddRange(BitConverter.GetBytes(u));
                result.AddRange(Unk_0x42);
                result.AddRange(Time.ToRawData());
                result.AddRange(BitConverter.GetBytes(CurrentMapWeatherID));
                result.AddRange(BitConverter.GetBytes(CurrentWeatherType));

                //foreach (UInt16 u in Unk_0x54)
                //    result.AddRange(BitConverter.GetBytes(u));
                result.AddRange(Unk_0x54);

                if (result.Count != SIZE)
                {
                    string message = "Event: SIZE ALL WRONG!!!" + Environment.NewLine +
                    "Size should be " + SIZE + " bytes..." + Environment.NewLine +
                    "...but Size is " + result.Count + " bytes!";

                    throw new Exception(message);
                }

                return result.ToArray();
            }
        }
        public class TigerTigerData : IXC2SaveObject
        {
            public const int SIZE = 0x100;

            public Byte[] Unk_0x00 { get; set; }
            public UInt32 Stage1HighScore { get; set; }
            public bool Stage1ScoreIsEasyMode { get; set; }
            public string Stage1HighScoreInitials { get; set; }
            public UInt32 Stage2HighScore { get; set; }
            public bool Stage2ScoreIsEasyMode { get; set; }
            public string Stage2HighScoreInitials { get; set; }
            public UInt32 Stage3HighScore { get; set; }
            public bool Stage3ScoreIsEasyMode { get; set; }
            public string Stage3HighScoreInitials { get; set; }
            public UInt32 Stage4HighScore { get; set; }
            public bool Stage4ScoreIsEasyMode { get; set; }
            public string Stage4HighScoreInitials { get; set; }
            public UInt32 Stage5HighScore { get; set; }
            public bool Stage5ScoreIsEasyMode { get; set; }
            public string Stage5HighScoreInitials { get; set; }
            public Byte[] Unk_0x30 { get; set; }

            public TigerTigerData(Byte[] data)
            {
                Unk_0x00 = data.GetByteSubArray(0, 8);

                UInt32 mask = (UInt32)(Math.Pow(2, 31));

                UInt32 s1 = BitConverter.ToUInt32(data.GetByteSubArray(8, 4), 0);
                Stage1HighScore = s1 & ~mask;
                Stage1ScoreIsEasyMode = (s1 >> 31) == 1;
                Stage1HighScoreInitials = Encoding.ASCII.GetString(data.GetByteSubArray(0xC, 3));

                UInt32 s2 = BitConverter.ToUInt32(data.GetByteSubArray(0x10, 4), 0);
                Stage2HighScore = s2 & ~mask;
                Stage2ScoreIsEasyMode = (s2 >> 31) == 1;
                Stage2HighScoreInitials = Encoding.ASCII.GetString(data.GetByteSubArray(0x14, 3));

                UInt32 s3 = BitConverter.ToUInt32(data.GetByteSubArray(0x18, 4), 0);
                Stage3HighScore = s3 & ~mask;
                Stage3ScoreIsEasyMode = (s3 >> 31) == 1;
                Stage3HighScoreInitials = Encoding.ASCII.GetString(data.GetByteSubArray(0x1C, 3));

                UInt32 s4 = BitConverter.ToUInt32(data.GetByteSubArray(0x20, 4), 0);
                Stage4HighScore = s4 & ~mask;
                Stage4ScoreIsEasyMode = (s4 >> 31) == 1;
                Stage4HighScoreInitials = Encoding.ASCII.GetString(data.GetByteSubArray(0x24, 3));

                UInt32 s5 = BitConverter.ToUInt32(data.GetByteSubArray(0x28, 4), 0);
                Stage5HighScore = s5 & ~mask;
                Stage5ScoreIsEasyMode = (s5 >> 31) == 1;
                Stage5HighScoreInitials = Encoding.ASCII.GetString(data.GetByteSubArray(0x2C, 3));

                Unk_0x30 = data.GetByteSubArray(0x30, 0xD0);
            }

            public Byte[] ToRawData()
            {
                Byte[] initials = new Byte[] { 0, 0, 0, 0 };

                List<Byte> result = new List<byte>();
                
                result.AddRange(Unk_0x00);

                result.AddRange(BitConverter.GetBytes((UInt32)(((Stage1ScoreIsEasyMode ? 1 : 0) << 31) + Stage1HighScore)));
                Encoding.ASCII.GetBytes(Stage1HighScoreInitials, 0, 3, initials, 0);
                result.AddRange(initials);

                result.AddRange(BitConverter.GetBytes((UInt32)(((Stage2ScoreIsEasyMode ? 1 : 0) << 31) + Stage2HighScore)));
                Encoding.ASCII.GetBytes(Stage2HighScoreInitials, 0, 3, initials, 0);
                result.AddRange(initials);

                result.AddRange(BitConverter.GetBytes((UInt32)(((Stage3ScoreIsEasyMode ? 1 : 0) << 31) + Stage3HighScore)));
                Encoding.ASCII.GetBytes(Stage3HighScoreInitials, 0, 3, initials, 0);
                result.AddRange(initials);

                result.AddRange(BitConverter.GetBytes((UInt32)(((Stage4ScoreIsEasyMode ? 1 : 0) << 31) + Stage4HighScore)));
                Encoding.ASCII.GetBytes(Stage4HighScoreInitials, 0, 3, initials, 0);
                result.AddRange(initials);

                result.AddRange(BitConverter.GetBytes((UInt32)(((Stage5ScoreIsEasyMode ? 1 : 0) << 31) + Stage5HighScore)));
                Encoding.ASCII.GetBytes(Stage5HighScoreInitials, 0, 3, initials, 0);
                result.AddRange(initials);

                result.AddRange(Unk_0x30);

                if (result.Count != SIZE)
                {
                    string message = "Minigame: SIZE ALL WRONG!!!" + Environment.NewLine +
                    "Size should be " + SIZE + " bytes..." + Environment.NewLine +
                    "...but Size is " + result.Count + " bytes!";

                    throw new Exception(message);
                }

                return result.ToArray();
            }
        }

        private static readonly Dictionary<string, int> LOC = new Dictionary<string, int>()
        {
            { "Magic", 0x0},
            { "ss_field_4", 0x4},
            { "Time", 0x8},
            { "Money", 0x10},
            { "MapJumpId", 0x14},
            { "sg_field_18", 0x18},
            { "sg_field_1C", 0x1C},
            { "MapPosition", 0x20},
            { "sg_field_2C", 0x2C},
            { "LandmarkRotY", 0x30},
            { "isTimeStop", 0x34},
            { "ChapterSaveScenarioFlag", 0x36},
            { "ChapterSaveEventId", 0x38},
            { "sg_field_3A", 0x3A},
            { "Drivers", 0x3C},
            { "Blades", 0x5A3C },
            { "Party", 0xE9894 },
            { "ItemBox", 0xE98E8 },
            { "Flags", 0xFBBD4 },
            //{ "Flags_2Bit", 0xFDBD4 },
            //{ "Flags_4Bit", 0x101BD4 },
            //{ "Flags_8Bit", 0x102BD4 },
            //{ "Flags_16Bit", 0x104BD4 },
            //{ "Flags_32Bit", 0x1063D4 },
            { "ScenarioQuest", 0x1097F4 },
            { "CurrentQuest", 0x1097F8 },
            { "field_1097FC", 0x1097FC },
            { "Map", 0x109800 },
            { "sg_gap_1098C4", 0x1098C4 },
            { "CurrentInGameTime", 0x1098D0 },
            { "PlayTime", 0x1098D4 },
            { "MercenaryTeam1", 0x1098D8 },
            { "MercenaryTeam2", 0x109900 },
            { "MercenaryTeam3", 0x109928 },
            { "MercenaryTeamCount", 0x109950 },
            { "MercenaryTeamPresets", 0x109958 },
            { "CommonBladeIDs", 0x1099B8 },
            { "PlayerCameraDistance", 0x109B38 },
            { "GameClearCount", 0x109B3C },
            { "AchievementTasks", 0x109B40 },
            { "AchievementTasksCount", 0x10A140 },
            { "QuestIDs", 0x10A148 },
            { "QuestCount", 0x10A548 },
            { "Weather", 0x10A54C },
            { "EtherCrystals", 0x10AD4C },
            { "MoveDistance", 0x10AD50 },
            { "MoveDistanceB", 0x10AD54 },
            { "AssurePoint", 0x10AD58 },
            { "AssureCount", 0x10AD5C },
            { "RareBladeAppearType", 0x10AD60 },
            { "field_10AD62", 0x10AD62 },
            { "CoinCount", 0x10AD64 },
            { "SavedEnemyHp", 0x10AD68 },
            { "field_10AD74", 0x10AD74 },
            { "Time2", 0x10AD78 },
            { "CameraHeight", 0x10AD80 },
            { "TigerTigerData", 0x10AD84 },
            { "CameraYaw", 0x10AE84 },
            { "CameraPitch", 0x10AE88 },
            { "CameraFreeMode", 0x10AE8C },
            { "IsHikariCurrent", 0x10AE8D },
            { "AutoEventAfterLoad", 0x10AE8E },
            { "IsCollectFlagNewVersion", 0x10AE90 },
            { "IsEndGameSave", 0x10AE91 },
            { "CameraSide", 0x10AE92 },
            { "gap_10AE93", 0x10AE93 },
            { "Events", 0x10AE94 },
            { "EventsLength", 0x1171E4 },
            { "sg_gap1171E8", 0x1171E8 },
            { "sg_field_117378", 0x117378 },
            { "ContentVersions", 0x11737C },
            { "sg_gap_117390", 0x117390 },
            { "sg_field_117698", 0x117698 },
            { "sg_field_11769C", 0x11769C },
        };

        public Byte[] Magic { get; }
        public Byte[] Unk_0x000004 { get; set; }
        public RealTime TimeSaved { get; set; }
        public UInt32 Money { get; set; }
        public UInt32 LastVisitedLandmarkMapJumpId { get; set; }
        public Byte[] Unk_0x000018 { get; set; }
        public Vec3 LastVisitedLandmarkMapPosition { get; set; }
        public Byte[] Unk_0x00002C { get; set; }
        public float LandmarkRotY { get; set; }
        public bool TimeIsStopped { get; set; }
        public UInt16 ChapterSaveScenarioFlag { get; set; }
        public UInt16 ChapterSaveEventId { get; set; }
        public Byte[] Unk_0x00003A { get; set; }
        public Driver[] Drivers { get; set; }
        public Blade[] Blades { get; set; }
        public XC2Party Party { get; set; }
        public ItemBox ItemBox { get; set; }
        public FlagData Flags { get; set; }
        public UInt32 ScenarioQuest { get; set; }
        public UInt32 CurrentQuest { get; set; }
        public Byte[] Unk_0x1097FC { get; set; }
        public MapData Map { get; set; }
        public Byte[] Unk_0x1098C4 { get; set; }
        public GameTime CurrentInGameTime { get; set; }
        public ElapseTime PlayTime { get; set; }
        public MercGroup MercGroup1 { get; set; }
        public MercGroup MercGroup2 { get; set; }
        public MercGroup MercGroup3 { get; set; }
        public UInt64 MercGroupCount { get; set; }
        public MercGroupPreset[] MercGroupPresets { get; set; }
        public UInt16[] CommonBladeIDs { get; set; }
        public float PlayerCameraDistance { get; set; }
        public UInt32 GameClearCount { get; set; }
        public TaskAchieve[] AchievementTasks { get; set; }
        public UInt64 AchievementTasksCount { get; set; }
        public UInt32[] QuestIDs { get; set; }
        public UInt32 QuestCount { get; set; }
        public WeatherInfo[] Weather { get; set; }
        public UInt32 EtherCrystals { get; set; }
        public float MoveDistance { get; set; }
        public float MoveDistanceB { get; set; }
        public UInt32 AssurePoint { get; set; }
        public UInt32 AssureCount { get; set; }
        public UInt16 RareBladeAppearType { get; set; }
        public Byte[] Unk_0x10AD62 { get; set; }
        public UInt32 CoinCount { get; set; }
        public UInt32[] SavedEnemyHp { get; set; }
        public Byte[] Unk_0x10AD74 { get; set; }
        public RealTime Time2 { get; set; }
        public float CameraHeight { get; set; }
        public TigerTigerData TigerTiger { get; set; }
        public float CameraYaw { get; set; }
        public float CameraPitch { get; set; }
        public Byte CameraFreeMode { get; set; }
        public bool AegisIsMythra { get; set; }
        public UInt16 AutoEventAfterLoad { get; set; }
        public bool IsCollectFlagNewVersion { get; set; }
        public bool IsEndGameSave { get; set; }
        public Byte CameraSide { get; set; }
        public Byte Unk_0x10AE93 { get; set; }
        public Event[] Events { get; set; }
        public UInt32 EventsCount { get; set; }
        public Byte[] Unk_0x1171E8 { get; set; }
        public UInt32[] ContentVersions { get; set; }
        public Byte[] Unk_0x117390 { get; set; }

        public const int SIZE = 0x1176A0;
        public static readonly Byte[] CORRECT_MAGIC = new Byte[] { 0x5B, 0xA1, 0x03, 0x00 };
        public const int COMMON_BLADE_IDS_COUNT = 192;
        public const int QUEST_ID_COUNT = 256;
        public const int CONTENT_VERSION_COUNT = 5;

        public XC2Save(Byte[] data)
        {
            Magic = data.GetByteSubArray(LOC["Magic"], 4);
            Unk_0x000004 = data.GetByteSubArray(LOC["ss_field_4"], 4);
            TimeSaved = new RealTime(data.GetByteSubArray(LOC["Time"], RealTime.SIZE));
            Money = BitConverter.ToUInt32(data.GetByteSubArray(LOC["Money"], 4), 0);
            LastVisitedLandmarkMapJumpId = BitConverter.ToUInt32(data.GetByteSubArray(LOC["MapJumpId"], 4), 0);
            Unk_0x000018 = data.GetByteSubArray(LOC["sg_field_18"], 8);
            LastVisitedLandmarkMapPosition = new Vec3(data.GetByteSubArray(LOC["MapPosition"], Vec3.SIZE));
            Unk_0x00002C = data.GetByteSubArray(LOC["sg_field_2C"], 4);
            LandmarkRotY = BitConverter.ToSingle(data.GetByteSubArray(LOC["LandmarkRotY"], 4), 0);
            TimeIsStopped = BitConverter.ToUInt16(data.GetByteSubArray(LOC["isTimeStop"], 2), 0) == 1;
            ChapterSaveScenarioFlag = BitConverter.ToUInt16(data.GetByteSubArray(LOC["ChapterSaveScenarioFlag"], 2), 0);
            ChapterSaveEventId = BitConverter.ToUInt16(data.GetByteSubArray(LOC["ChapterSaveEventId"], 2), 0);
            Unk_0x00003A = data.GetByteSubArray(LOC["sg_field_3A"], 2);

            Drivers = new Driver[16];
            for (int i = 0; i < Drivers.Length; i++)
                Drivers[i] = new Driver(data.GetByteSubArray(LOC["Drivers"] + (i * Driver.SIZE), Driver.SIZE));

            Blades = new Blade[422];
            for (int i = 0; i < Blades.Length; i++)
                Blades[i] = new Blade(data.GetByteSubArray(LOC["Blades"] + (i * Blade.SIZE), Blade.SIZE));

            Party = new XC2Party(data.GetByteSubArray(LOC["Party"], XC2Party.SIZE));
            ItemBox = new ItemBox(data.GetByteSubArray(LOC["ItemBox"], ItemBox.SIZE));
            Flags = new FlagData(data.GetByteSubArray(LOC["Flags"], 0xDC20));
            ScenarioQuest = BitConverter.ToUInt32(data.GetByteSubArray(LOC["ScenarioQuest"], 4), 0);
            CurrentQuest = BitConverter.ToUInt32(data.GetByteSubArray(LOC["CurrentQuest"], 4), 0);
            Unk_0x1097FC = data.GetByteSubArray(LOC["field_1097FC"], 4);
            Map = new MapData(data.GetByteSubArray(LOC["Map"], MapData.SIZE));
            Unk_0x1098C4 = data.GetByteSubArray(LOC["sg_gap_1098C4"], 12);
            CurrentInGameTime = new GameTime(data.GetByteSubArray(LOC["CurrentInGameTime"], GameTime.SIZE));
            PlayTime = new ElapseTime(data.GetByteSubArray(LOC["PlayTime"], ElapseTime.SIZE));
            MercGroup1 = new MercGroup(data.GetByteSubArray(LOC["MercenaryTeam1"], MercGroup.SIZE));
            MercGroup2 = new MercGroup(data.GetByteSubArray(LOC["MercenaryTeam2"], MercGroup.SIZE));
            MercGroup3 = new MercGroup(data.GetByteSubArray(LOC["MercenaryTeam3"], MercGroup.SIZE));
            MercGroupCount = BitConverter.ToUInt64(data.GetByteSubArray(LOC["MercenaryTeamCount"], 8), 0);

            MercGroupPresets = new MercGroupPreset[8];
            for (int i = 0; i < MercGroupPresets.Length; i++)
                MercGroupPresets[i] = new MercGroupPreset(data.GetByteSubArray(LOC["MercenaryTeamPresets"] + (i * MercGroupPreset.SIZE), MercGroupPreset.SIZE));

            CommonBladeIDs = new UInt16[COMMON_BLADE_IDS_COUNT];
            for (int i = 0; i < CommonBladeIDs.Length; i++)
                CommonBladeIDs[i] = BitConverter.ToUInt16(data.GetByteSubArray(LOC["CommonBladeIDs"] + (i * 2), 2), 0);

            PlayerCameraDistance = BitConverter.ToSingle(data.GetByteSubArray(LOC["PlayerCameraDistance"], 4), 0);
            GameClearCount = BitConverter.ToUInt32(data.GetByteSubArray(LOC["GameClearCount"], 4), 0);

            AchievementTasks = new TaskAchieve[128];
            for (int i = 0; i < AchievementTasks.Length; i++)
                AchievementTasks[i] = new TaskAchieve(data.GetByteSubArray(LOC["AchievementTasks"] + (i * TaskAchieve.SIZE), TaskAchieve.SIZE));

            AchievementTasksCount = BitConverter.ToUInt64(data.GetByteSubArray(LOC["AchievementTasksCount"], 8), 0);

            QuestIDs = new UInt32[QUEST_ID_COUNT];
            for (int i = 0; i < QuestIDs.Length; i++)
                QuestIDs[i] = BitConverter.ToUInt32(data.GetByteSubArray(LOC["QuestIDs"] + (i * 4), 4), 0);

            QuestCount = BitConverter.ToUInt32(data.GetByteSubArray(LOC["QuestCount"], 4), 0);

            Weather = new WeatherInfo[64];
            for (int i = 0; i < Weather.Length; i++)
                Weather[i] = new WeatherInfo(data.GetByteSubArray(LOC["Weather"] + (i * WeatherInfo.SIZE), WeatherInfo.SIZE));

            EtherCrystals = BitConverter.ToUInt32(data.GetByteSubArray(LOC["EtherCrystals"], 4), 0);
            MoveDistance = BitConverter.ToSingle(data.GetByteSubArray(LOC["MoveDistance"], 4), 0);
            MoveDistanceB = BitConverter.ToSingle(data.GetByteSubArray(LOC["MoveDistanceB"], 4), 0);
            AssurePoint = BitConverter.ToUInt32(data.GetByteSubArray(LOC["AssurePoint"], 4), 0);
            AssureCount = BitConverter.ToUInt32(data.GetByteSubArray(LOC["AssureCount"], 4), 0);
            RareBladeAppearType = BitConverter.ToUInt16(data.GetByteSubArray(LOC["RareBladeAppearType"], 4), 0);
            Unk_0x10AD62 = data.GetByteSubArray(LOC["field_10AD62"], 2);
            CoinCount = BitConverter.ToUInt32(data.GetByteSubArray(LOC["CoinCount"], 4), 0);

            SavedEnemyHp = new UInt32[3];
            for (int i = 0; i < SavedEnemyHp.Length; i++)
                SavedEnemyHp[i] = BitConverter.ToUInt32(data.GetByteSubArray(LOC["SavedEnemyHp"] + (i * 4), 4), 0);
            
            Unk_0x10AD74 = data.GetByteSubArray(LOC["field_10AD74"], 4);
            Time2 = new RealTime(data.GetByteSubArray(LOC["Time2"], RealTime.SIZE));
            CameraHeight = BitConverter.ToSingle(data.GetByteSubArray(LOC["CameraHeight"], 4), 0);
            TigerTiger = new TigerTigerData(data.GetByteSubArray(LOC["TigerTigerData"], TigerTigerData.SIZE));
            CameraYaw = BitConverter.ToSingle(data.GetByteSubArray(LOC["CameraYaw"], 4), 0);
            CameraPitch = BitConverter.ToSingle(data.GetByteSubArray(LOC["CameraPitch"], 4), 0);
            CameraFreeMode = data[LOC["CameraFreeMode"]];
            AegisIsMythra = data[LOC["IsHikariCurrent"]] == 1;
            AutoEventAfterLoad = BitConverter.ToUInt16(data.GetByteSubArray(LOC["AutoEventAfterLoad"], 4), 0);
            IsCollectFlagNewVersion = data[LOC["IsCollectFlagNewVersion"]] == 1;
            IsEndGameSave = data[LOC["IsEndGameSave"]] == 1;
            CameraSide = data[LOC["CameraSide"]];
            Unk_0x10AE93 = data[LOC["gap_10AE93"]];

            Events = new Event[500];
            for (int i = 0; i < Events.Length; i++)
                Events[i] = new Event(data.GetByteSubArray(LOC["Events"] + (i * Event.SIZE), Event.SIZE));

            EventsCount = BitConverter.ToUInt32(data.GetByteSubArray(LOC["EventsLength"], 4), 0);
            Unk_0x1171E8 = data.GetByteSubArray(LOC["sg_gap1171E8"], 404);

            ContentVersions = new UInt32[CONTENT_VERSION_COUNT];
            for (int i = 0; i < ContentVersions.Length; i++)
                ContentVersions[i] = BitConverter.ToUInt32(data.GetByteSubArray(LOC["ContentVersions"] + (i * 4), 4), 0);
            
            Unk_0x117390 = data.GetByteSubArray(LOC["sg_gap_117390"], 784);
        }

        public virtual byte[] ToRawData()
        {
            List<Byte> result = new List<byte>();

            result.AddRange(Magic);
            result.AddRange(Unk_0x000004);
            result.AddRange(TimeSaved.ToRawData());
            result.AddRange(BitConverter.GetBytes(Money));
            result.AddRange(BitConverter.GetBytes(LastVisitedLandmarkMapJumpId));
            result.AddRange(Unk_0x000018);
            result.AddRange(LastVisitedLandmarkMapPosition.ToRawData());
            result.AddRange(Unk_0x00002C);
            result.AddRange(BitConverter.GetBytes(LandmarkRotY));
            result.AddRange(BitConverter.GetBytes((UInt16)(TimeIsStopped ? 1 : 0)));
            result.AddRange(BitConverter.GetBytes(ChapterSaveScenarioFlag));
            result.AddRange(BitConverter.GetBytes(ChapterSaveEventId));
            result.AddRange(Unk_0x00003A);

            foreach (Driver d in Drivers)
                result.AddRange(d.ToRawData());

            foreach (Blade b in Blades)
                result.AddRange(b.ToRawData());

            result.AddRange(Party.ToRawData());
            result.AddRange(ItemBox.ToRawData());
            result.AddRange(Flags.ToRawData());
            result.AddRange(BitConverter.GetBytes(ScenarioQuest));
            result.AddRange(BitConverter.GetBytes(CurrentQuest));
            result.AddRange(Unk_0x1097FC);
            result.AddRange(Map.ToRawData());
            result.AddRange(Unk_0x1098C4);
            result.AddRange(CurrentInGameTime.ToRawData());
            result.AddRange(PlayTime.ToRawData());
            result.AddRange(MercGroup1.ToRawData());
            result.AddRange(MercGroup2.ToRawData());
            result.AddRange(MercGroup3.ToRawData());

            result.AddRange(BitConverter.GetBytes(MercGroupCount));

            foreach (MercGroupPreset mtp in MercGroupPresets)
                result.AddRange(mtp.ToRawData());

            foreach (UInt16 u in CommonBladeIDs)
                result.AddRange(BitConverter.GetBytes(u));

            result.AddRange(BitConverter.GetBytes(PlayerCameraDistance));
            result.AddRange(BitConverter.GetBytes(GameClearCount));

            foreach (TaskAchieve ta in AchievementTasks)
                result.AddRange(ta.ToRawData());

            result.AddRange(BitConverter.GetBytes(AchievementTasksCount));

            foreach (UInt32 u in QuestIDs)
                result.AddRange(BitConverter.GetBytes(u));

            result.AddRange(BitConverter.GetBytes(QuestCount));

            foreach (WeatherInfo wi in Weather)
                result.AddRange(wi.ToRawData());

            result.AddRange(BitConverter.GetBytes(EtherCrystals));
            result.AddRange(BitConverter.GetBytes(MoveDistance));
            result.AddRange(BitConverter.GetBytes(MoveDistanceB));
            result.AddRange(BitConverter.GetBytes(AssurePoint));
            result.AddRange(BitConverter.GetBytes(AssureCount));
            result.AddRange(BitConverter.GetBytes(RareBladeAppearType));
            result.AddRange(Unk_0x10AD62);
            result.AddRange(BitConverter.GetBytes(CoinCount));

            foreach (UInt32 u in SavedEnemyHp)
                result.AddRange(BitConverter.GetBytes(u));
            
            result.AddRange(Unk_0x10AD74);
            result.AddRange(Time2.ToRawData());
            result.AddRange(BitConverter.GetBytes(CameraHeight));
            result.AddRange(TigerTiger.ToRawData());
            result.AddRange(BitConverter.GetBytes(CameraYaw));
            result.AddRange(BitConverter.GetBytes(CameraPitch));
            result.Add(CameraFreeMode);
            result.Add((Byte)(AegisIsMythra ? 1 : 0));
            result.AddRange(BitConverter.GetBytes(AutoEventAfterLoad));
            result.Add((Byte)(IsCollectFlagNewVersion ? 1 : 0));
            result.Add((Byte)(IsEndGameSave ? 1 : 0));
            result.Add(CameraSide);
            result.Add(Unk_0x10AE93);

            foreach (Event e in Events)
                result.AddRange(e.ToRawData());

            result.AddRange(BitConverter.GetBytes(EventsCount));
            result.AddRange(Unk_0x1171E8);

            foreach (UInt32 u in ContentVersions)
                result.AddRange(BitConverter.GetBytes(u));

            result.AddRange(Unk_0x117390);

            if (result.Count != SIZE)
            {
                string message = "XC2Save: SIZE ALL WRONG!!!" + Environment.NewLine +
                "Size should be " + SIZE + " bytes..." + Environment.NewLine +
                "...but Size is " + result.Count + " bytes!";

                throw new Exception(message);
            }

            return result.ToArray();
        }
    }

    public class XC2Save150 : XC2Save, IXC2SaveObject
    {
        public new const int SIZE = 0x126D90;

        // ALL PROVISIONAL, SUPER HACKY
        // 1.5.0+ DATA NEEDS TO BE FIGURED OUT
        public Byte[] Unk_0x1176A0 { get; set; }
        public Driver.Weapon[] Driver1Weapons150 { get; set; }
        public Byte[] Unk_0x117728 { get; set; }
        public Driver.Weapon[] Driver2Weapons150 { get; set; }
        public Byte[] Unk_0x117A4C { get; set; }
        public Driver.Weapon[] Driver3Weapons150 { get; set; }
        public Byte[] Unk_0x117D70 { get; set; }
        public Driver.Weapon[] Driver6Weapons150 { get; set; }
        public Byte[] Unk_0x1186DC { get; set; }

        private static readonly Dictionary<string, int> LOC = new Dictionary<string, int>()
        {
            { "Unk_0x1176A0", 0x1176A0 },
            { "Driver1Weapons150", 0x117700 },
            { "Unk_0x117728", 0x117728 },
            { "Driver2Weapons150", 0x117A24 },
            { "Unk_0x117A4C", 0x117A4C },
            { "Driver3Weapons150", 0x117D48 },
            { "Unk_0x117D70", 0x117D70 },
            { "Driver6Weapons150", 0x1186B4 },
            { "Unk_0x1186DC", 0x1186DC }
        };

        public XC2Save150(Byte[] data) : base(data)
        {
            Unk_0x1176A0 = data.GetByteSubArray(LOC["Unk_0x1176A0"], 0x60);

            Driver1Weapons150 = new Driver.Weapon[2];
            for (int i = 0; i < Driver1Weapons150.Length; i++)
                Driver1Weapons150[i] = new Driver.Weapon(data.GetByteSubArray(LOC["Driver1Weapons150"] + (i * Driver.Weapon.SIZE), Driver.Weapon.SIZE));

            Unk_0x117728 = data.GetByteSubArray(LOC["Unk_0x117728"], 0x2FC);

            Driver2Weapons150 = new Driver.Weapon[2];
            for (int i = 0; i < Driver2Weapons150.Length; i++)
                Driver2Weapons150[i] = new Driver.Weapon(data.GetByteSubArray(LOC["Driver2Weapons150"] + (i * Driver.Weapon.SIZE), Driver.Weapon.SIZE));

            Unk_0x117A4C = data.GetByteSubArray(LOC["Unk_0x117A4C"], 0x2FC);

            Driver3Weapons150 = new Driver.Weapon[2];
            for (int i = 0; i < Driver3Weapons150.Length; i++)
                Driver3Weapons150[i] = new Driver.Weapon(data.GetByteSubArray(LOC["Driver3Weapons150"] + (i * Driver.Weapon.SIZE), Driver.Weapon.SIZE));

            Unk_0x117D70 = data.GetByteSubArray(LOC["Unk_0x117D70"], 0x944);

            Driver6Weapons150 = new Driver.Weapon[2];
            for (int i = 0; i < Driver6Weapons150.Length; i++)
                Driver6Weapons150[i] = new Driver.Weapon(data.GetByteSubArray(LOC["Driver6Weapons150"] + (i * Driver.Weapon.SIZE), Driver.Weapon.SIZE));

            Unk_0x1186DC = data.GetByteSubArray(LOC["Unk_0x1186DC"], 0xE6B4);
        }

        public override Byte[] ToRawData()
        {
            
            List<Byte> result = new List<Byte>();

            result.AddRange(base.ToRawData());
            result.AddRange(Unk_0x1176A0);

            foreach (Driver.Weapon w in Driver1Weapons150)
                result.AddRange(w.ToRawData());

            result.AddRange(Unk_0x117728);

            foreach (Driver.Weapon w in Driver2Weapons150)
                result.AddRange(w.ToRawData());

            result.AddRange(Unk_0x117A4C);

            foreach (Driver.Weapon w in Driver3Weapons150)
                result.AddRange(w.ToRawData());

            result.AddRange(Unk_0x117D70);

            foreach (Driver.Weapon w in Driver6Weapons150)
                result.AddRange(w.ToRawData());

            result.AddRange(Unk_0x1186DC);

            if (result.Count != SIZE)
            {
                string message = "XC2Save150: SIZE ALL WRONG!!!" + Environment.NewLine +
                "Size should be " + SIZE + " bytes..." + Environment.NewLine +
                "...but Size is " + result.Count + " bytes!";

                throw new Exception(message);
            }

            return result.ToArray();
        }
    }

    public static class XC2SaveDeserializer
    {
        public static XC2Save Deserialize(Byte[] data)
        {
            Byte[] magic = data.GetByteSubArray(0, 4);

            if (!(magic.SequenceEqual(XC2Save.CORRECT_MAGIC)))
            {
                string message =
                    "ERROR: Given Data has incorrect Magic value!" + Environment.NewLine +
                    Environment.NewLine +
                    "Ecpercted Magic Value: " + XC2Save.CORRECT_MAGIC[0].ToString("X2") + XC2Save.CORRECT_MAGIC[1].ToString("X2") + XC2Save.CORRECT_MAGIC[2].ToString("X2") + XC2Save.CORRECT_MAGIC[3].ToString("X2") + Environment.NewLine +
                    "Actual Magic Value: " + magic[0].ToString("X2") + magic[1].ToString("X2") + magic[2].ToString("X2") + magic[3].ToString("X2") + Environment.NewLine +
                    Environment.NewLine +
                    "Data is most likely not a valid Xenoblade Chronicles 2 Save File!";
                throw new Exception(message);
            }

            switch (data.Length)
            {
                case XC2Save.SIZE:
                    Console.WriteLine("XC2Save (old) detected.");
                    return new XC2Save(data);

                case XC2Save150.SIZE:
                    Console.WriteLine("XC2Save150 detected.");
                    return new XC2Save150(data);

                default:
                    string message = "Given save data has incorrect filesize!" + Environment.NewLine +
                        Environment.NewLine +
                        "Expected Size:" + Environment.NewLine +
                        String.Format("0x{0:X} for old save data.", XC2Save.SIZE) + Environment.NewLine +
                        String.Format("0x{0:X} for ver 1.5.0 save data.", XC2Save150.SIZE) + Environment.NewLine +
                        Environment.NewLine +
                        "Actual Size: 0x" + data.Length.ToString("X");
                    throw new Exception(message);
            }
        }
    }
}
