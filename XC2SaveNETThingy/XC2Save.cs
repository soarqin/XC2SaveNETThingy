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
            public UInt16 Unk_0x02 { get; set; }
            public UInt16 Unk_0x04 { get; set; }

            public PartyMember(Byte[] data)
            {
                DriverId = BitConverter.ToUInt16(data.GetByteSubArray(0, 2), 0);
                Unk_0x02 = BitConverter.ToUInt16(data.GetByteSubArray(2, 2), 0);
                Unk_0x04 = BitConverter.ToUInt16(data.GetByteSubArray(4, 2), 0);
            }

            public Byte[] ToRawData()
            {
                List<Byte> result = new List<byte>();

                result.AddRange(BitConverter.GetBytes(DriverId));
                result.AddRange(BitConverter.GetBytes(Unk_0x02));
                result.AddRange(BitConverter.GetBytes(Unk_0x04));

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
                { "gap3C", 0x3C },
                { "PartyLeader", 0x40 },
                { "field_44", 0x44 },
                { "field_48", 0x48 },
                { "PartyGauge", 0x4C },
                { "field_4E", 0x4E },
                { "field_50", 0x50 }
            };

            public PartyMember[] Members { get; set; }
            public Byte[] gap3C { get; set; }
            public UInt32 PartyLeader { get; set; }
            public UInt32 field_44 { get; set; }
            public UInt32 field_48 { get; set; }
            public UInt16 PartyGauge { get; set; }
            public UInt16 field_4E { get; set; }
            public UInt32 field_50 { get; set; }

            public XC2Party(Byte[] data)
            {
                Members = new PartyMember[10];
                for (int i = 0; i < Members.Length; i++)
                    Members[i] = new PartyMember(data.GetByteSubArray(LOC["Members"] + (i * PartyMember.SIZE), PartyMember.SIZE));

                gap3C = new Byte[4];
                for (int i = 0; i < gap3C.Length; i++)
                    gap3C[i] = data[LOC["gap3C"] + i];

                PartyLeader = BitConverter.ToUInt32(data.GetByteSubArray(LOC["PartyLeader"], 4), 0);
                field_44 = BitConverter.ToUInt32(data.GetByteSubArray(LOC["field_44"], 4), 0);
                field_48 = BitConverter.ToUInt32(data.GetByteSubArray(LOC["field_48"], 4), 0);
                PartyGauge = BitConverter.ToUInt16(data.GetByteSubArray(LOC["PartyGauge"], 2), 0);
                field_4E = BitConverter.ToUInt16(data.GetByteSubArray(LOC["field_4E"], 2), 0);
                field_50 = BitConverter.ToUInt32(data.GetByteSubArray(LOC["field_50"], 4), 0);
            }

            public Byte[] ToRawData()
            {
                List<Byte> result = new List<Byte>();

                foreach (PartyMember m in Members)
                    result.AddRange(m.ToRawData());

                foreach (Byte b in gap3C)
                    result.Add(b);

                result.AddRange(BitConverter.GetBytes(PartyLeader));
                result.AddRange(BitConverter.GetBytes(field_44));
                result.AddRange(BitConverter.GetBytes(field_48));
                result.AddRange(BitConverter.GetBytes(PartyGauge));
                result.AddRange(BitConverter.GetBytes(field_4E));
                result.AddRange(BitConverter.GetBytes(field_50));

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

        public class SDataMap : IXC2SaveObject
        {
            public const int SIZE = 0xC4;

            public Vec3Padded[] DriverPositions { get; set; }
            public Vec3Padded[] BladePositions { get; set; }
            public Vec3Padded[] DriverRotations { get; set; }
            public Vec3Padded[] BladeRotations { get; set; }
            public UInt32 MapJumpId { get; set; }

            public SDataMap(Byte[] data)
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

                MapJumpId = BitConverter.ToUInt32(data.GetByteSubArray(0xC0, 4), 0);
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

                result.AddRange(BitConverter.GetBytes(MapJumpId));

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

        public class MercenaryTeam : IXC2SaveObject
        {
            public const int SIZE = 0x28;

            public UInt16[] TeamMemberIds { get; set; }
            public UInt32 field_C { get; set; }
            public UInt32 TeamId { get; set; }
            public UInt32 MissionId { get; set; }
            public UInt32 field_18 { get; set; }
            public UInt32 MissionTime { get; set; }
            public UInt32 MissionTimeOriginal { get; set; }
            public UInt32 field_24 { get; set; }

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

            public MercenaryTeam(Byte[] data)
            {
                TeamMemberIds = new UInt16[6];
                for (int i = 0; i < TeamMemberIds.Length; i++)
                    TeamMemberIds[i] = BitConverter.ToUInt16(data.GetByteSubArray(LOC["TeamMemberIds"] + (i * 2), 2), 0);

                field_C = BitConverter.ToUInt32(data.GetByteSubArray(LOC["field_C"], 4), 0);
                TeamId = BitConverter.ToUInt32(data.GetByteSubArray(LOC["TeamId"], 4), 0);
                MissionId = BitConverter.ToUInt32(data.GetByteSubArray(LOC["MissionId"], 4), 0);
                field_18 = BitConverter.ToUInt32(data.GetByteSubArray(LOC["field_18"], 4), 0);
                MissionTime = BitConverter.ToUInt32(data.GetByteSubArray(LOC["MissionTime"], 4), 0);
                MissionTimeOriginal = BitConverter.ToUInt32(data.GetByteSubArray(LOC["MissionTimeOriginal"], 4), 0);
                field_24 = BitConverter.ToUInt32(data.GetByteSubArray(LOC["field_24"], 4), 0);
            }

            public Byte[] ToRawData()
            {
                List<Byte> result = new List<Byte>();

                foreach (UInt16 tm in TeamMemberIds)
                    result.AddRange(BitConverter.GetBytes(tm));

                result.AddRange(BitConverter.GetBytes(field_C));
                result.AddRange(BitConverter.GetBytes(TeamId));
                result.AddRange(BitConverter.GetBytes(MissionId));
                result.AddRange(BitConverter.GetBytes(field_18));
                result.AddRange(BitConverter.GetBytes(MissionTime));
                result.AddRange(BitConverter.GetBytes(MissionTimeOriginal));
                result.AddRange(BitConverter.GetBytes(field_24));

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

        public class MercenaryTeamPreset : IXC2SaveObject
        {
            public const int SIZE = 0xC;

            public UInt16[] Members { get; set; }

            public MercenaryTeamPreset(Byte[] data)
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

            public UInt16 Unk_0x0 { get; set; }
            public UInt16 Unk_0x2 { get; set; }
            public UInt16 Unk_0x4 { get; set; }
            public UInt16 Unk_0x6 { get; set; }
            public UInt32 Unk_0x8 { get; set; }

            public TaskAchieve(Byte[] data)
            {
                Unk_0x0 = BitConverter.ToUInt16(data.GetByteSubArray(0, 2), 0);
                Unk_0x2 = BitConverter.ToUInt16(data.GetByteSubArray(2, 2), 0);
                Unk_0x4 = BitConverter.ToUInt16(data.GetByteSubArray(4, 2), 0);
                Unk_0x6 = BitConverter.ToUInt16(data.GetByteSubArray(6, 2), 0);
                Unk_0x8 = BitConverter.ToUInt32(data.GetByteSubArray(8, 4), 0);
            }

            public Byte[] ToRawData()
            {
                List<Byte> result = new List<byte>();

                result.AddRange(BitConverter.GetBytes(Unk_0x0));
                result.AddRange(BitConverter.GetBytes(Unk_0x2));
                result.AddRange(BitConverter.GetBytes(Unk_0x4));
                result.AddRange(BitConverter.GetBytes(Unk_0x6));
                result.AddRange(BitConverter.GetBytes(Unk_0x8));

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

            public string Name { get; set; }
            public UInt32 Unk_0x04 { get; set; }
            public UInt32 Unk_0x08 { get; set; }
            public UInt32 Unk_0x0C { get; set; }
            public float Unk_0x10 { get; set; }
            public float Unk_0x14 { get; set; }
            public UInt32 Unk_0x18 { get; set; }
            public UInt32 Unk_0x1C { get; set; }

            public WeatherInfo(Byte[] data)
            {
                Name = Encoding.ASCII.GetString(data.GetByteSubArray(0x0, 4));
                Unk_0x04 = BitConverter.ToUInt32(data.GetByteSubArray(0x4, 4), 0);
                Unk_0x08 = BitConverter.ToUInt32(data.GetByteSubArray(0x8, 4), 0);
                Unk_0x0C = BitConverter.ToUInt32(data.GetByteSubArray(0xC, 4), 0);
                Unk_0x10 = BitConverter.ToSingle(data.GetByteSubArray(0x10, 4), 0);
                Unk_0x14 = BitConverter.ToSingle(data.GetByteSubArray(0x14, 4), 0);
                Unk_0x18 = BitConverter.ToUInt32(data.GetByteSubArray(0x18, 4), 0);
                Unk_0x1C = BitConverter.ToUInt32(data.GetByteSubArray(0x1C, 4), 0);
            }

            public Byte[] ToRawData()
            {
                List<Byte> result = new List<byte>();

                result.AddRange(Encoding.ASCII.GetBytes(Name));
                result.AddRange(BitConverter.GetBytes(Unk_0x04));
                result.AddRange(BitConverter.GetBytes(Unk_0x08));
                result.AddRange(BitConverter.GetBytes(Unk_0x0C));
                result.AddRange(BitConverter.GetBytes(Unk_0x10));
                result.AddRange(BitConverter.GetBytes(Unk_0x14));
                result.AddRange(BitConverter.GetBytes(Unk_0x18));
                result.AddRange(BitConverter.GetBytes(Unk_0x1C));

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
            public UInt16 Unk_0x0C { get; set; }
            public UInt16[] Weapons { get; set; }
            public UInt16[] Unk_0x22 { get; set; }
            public UInt16[] BladeIDs { get; set; }
            public UInt16[] Unk_0x42 { get; set; }
            public GameTime Time { get; set; }
            public UInt16 CurrentMapWeatherID { get; set; }
            public UInt16 CurrentWeatherType { get; set; }
            public UInt16[] Unk_0x54 { get; set; }

            public Event(Byte[] data)
            {
                EventID = BitConverter.ToUInt16(data.GetByteSubArray(LOC["EventID"], 2), 0);
                Creator = BitConverter.ToUInt16(data.GetByteSubArray(LOC["Creator"], 2), 0);
                PlayBladeID = BitConverter.ToUInt16(data.GetByteSubArray(LOC["PlayBladeID"], 2), 0);
                VoiceID = BitConverter.ToUInt16(data.GetByteSubArray(LOC["VoiceID"], 2), 0);
                Attribute = BitConverter.ToUInt16(data.GetByteSubArray(LOC["Attribute"], 2), 0);
                ExtraParts = BitConverter.ToUInt16(data.GetByteSubArray(LOC["ExtraParts"], 2), 0);
                Unk_0x0C = BitConverter.ToUInt16(data.GetByteSubArray(LOC["Unk_0x0C"], 2), 0);

                Weapons = new UInt16[10];
                for (int i = 0; i < Weapons.Length; i++)
                    Weapons[i] = BitConverter.ToUInt16(data.GetByteSubArray(LOC["Weapons"] + (i * 2), 2), 0);

                Unk_0x22 = new UInt16[6];
                for (int i = 0; i < Unk_0x22.Length; i++)
                    Unk_0x22[i] = BitConverter.ToUInt16(data.GetByteSubArray(LOC["Unk_0x22"] + (i * 2), 2), 0);

                BladeIDs = new UInt16[10];
                for (int i = 0; i < BladeIDs.Length; i++)
                    BladeIDs[i] = BitConverter.ToUInt16(data.GetByteSubArray(LOC["BladeIDs"] + (i * 2), 2), 0);

                Unk_0x42 = new UInt16[5];
                for (int i = 0; i < Unk_0x42.Length; i++)
                    Unk_0x42[i] = BitConverter.ToUInt16(data.GetByteSubArray(LOC["Unk_0x42"] + (i * 2), 2), 0);

                Time = new GameTime(data.GetByteSubArray(LOC["Time"], 4));
                CurrentMapWeatherID = BitConverter.ToUInt16(data.GetByteSubArray(LOC["CurrentMapWeatherID"], 2), 0);
                CurrentWeatherType = BitConverter.ToUInt16(data.GetByteSubArray(LOC["CurrentWeatherType"], 2), 0);

                Unk_0x54 = new UInt16[8];
                for (int i = 0; i < Unk_0x54.Length; i++)
                    Unk_0x54[i] = BitConverter.ToUInt16(data.GetByteSubArray(LOC["Unk_0x54"], 2), 0);
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
                result.AddRange(BitConverter.GetBytes(Unk_0x0C));

                foreach (UInt16 w in Weapons)
                    result.AddRange(BitConverter.GetBytes(w));

                foreach (UInt16 u in Unk_0x22)
                    result.AddRange(BitConverter.GetBytes(u));

                foreach (UInt16 u in BladeIDs)
                    result.AddRange(BitConverter.GetBytes(u));

                foreach (UInt16 u in Unk_0x42)
                    result.AddRange(BitConverter.GetBytes(u));

                result.AddRange(Time.ToRawData());
                result.AddRange(BitConverter.GetBytes(CurrentMapWeatherID));
                result.AddRange(BitConverter.GetBytes(CurrentWeatherType));

                foreach (UInt16 u in Unk_0x54)
                    result.AddRange(BitConverter.GetBytes(u));

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

        public class Minigame : IXC2SaveObject
        {
            public const int SIZE = 0x100;

            public UInt32 Unk_0x00 { get; set; }
            public UInt32 Unk_0x04 { get; set; }
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

            public Minigame(Byte[] data)
            {
                Unk_0x00 = BitConverter.ToUInt32(data.GetByteSubArray(0, 4), 0);
                Unk_0x04 = BitConverter.ToUInt32(data.GetByteSubArray(4, 4), 0);

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
                
                result.AddRange(BitConverter.GetBytes(Unk_0x00));
                result.AddRange(BitConverter.GetBytes(Unk_0x04));

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

        public UInt32 Magic { get; }
        public UInt32 ss_field_4 { get; set; }
        public RealTime Time { get; set; }
        public UInt32 Money { get; set; }
        public UInt32 MapJumpId { get; set; }
        public UInt32 sg_field_18 { get; set; }
        public UInt32 sg_field_1C { get; set; }
        public Vec3 MapPosition { get; set; }
        public float sg_field_2C { get; set; }
        public float LandmarkRotY { get; set; }
        public UInt16 isTimeStop { get; set; }
        public UInt16 ChapterSaveScenarioFlag { get; set; }
        public UInt16 ChapterSaveEventId { get; set; }
        public UInt16 sg_field_3A { get; set; }
        public Driver[] Drivers { get; set; }
        public Blade[] Blades { get; set; }
        public XC2Party Party { get; set; }
        public ItemBox ItemBox { get; set; }

        //GameFlag Flags;
        public Byte[] Flags { get; set; }
        public UInt32 ScenarioQuest { get; set; }
        public UInt32 CurrentQuest { get; set; }
        public UInt32 field_1097FC { get; set; }
        public SDataMap Map { get; set; }
        public Byte[] sg_gap_1098C4 { get; set; }
        public GameTime CurrentInGameTime { get; set; }
        public ElapseTime PlayTime { get; set; }
        public MercenaryTeam MercenaryTeam1 { get; set; }
        public MercenaryTeam MercenaryTeam2 { get; set; }
        public MercenaryTeam MercenaryTeam3 { get; set; }
        public UInt64 MercenaryTeamCount { get; set; }
        public MercenaryTeamPreset[] MercenaryTeamPresets { get; set; }
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
        public UInt16 field_10AD62 { get; set; }
        public UInt32 CoinCount { get; set; }
        public UInt32[] SavedEnemyHp { get; set; }
        public UInt32 field_10AD74 { get; set; }
        public RealTime Time2 { get; set; }
        public float CameraHeight { get; set; }
        public Minigame TigerTigerData { get; set; }
        public float CameraYaw { get; set; }
        public float CameraPitch { get; set; }
        public Byte CameraFreeMode { get; set; }
        public Byte IsHikariCurrent { get; set; }
        public UInt16 AutoEventAfterLoad { get; set; }
        public Byte IsCollectFlagNewVersion { get; set; }
        public Byte IsEndGameSave { get; set; }
        public Byte CameraSide { get; set; }
        public Byte gap_10AE93 { get; set; }
        public Event[] Events { get; set; }
        public UInt32 EventsLength { get; set; }
        public Byte[] sg_gap1171E8 { get; set; }
        public float sg_field_117378 { get; set; }
        public UInt32[] ContentVersions { get; set; }
        public Byte[] sg_gap_117390 { get; set; }
        public UInt32 sg_field_117698 { get; set; }
        public UInt32 sg_field_11769C { get; set; }

        public const int SIZE = 0x1176A0;
        public XC2Save(Byte[] data)
        {
            Magic = BitConverter.ToUInt32(data.GetByteSubArray(LOC["Magic"], 4), 0);
            ss_field_4 = BitConverter.ToUInt32(data.GetByteSubArray(LOC["ss_field_4"], 4), 0);
            Time = new RealTime(data.GetByteSubArray(LOC["Time"], RealTime.SIZE));
            Money = BitConverter.ToUInt32(data.GetByteSubArray(LOC["Money"], 4), 0);
            MapJumpId = BitConverter.ToUInt32(data.GetByteSubArray(LOC["MapJumpId"], 4), 0);
            sg_field_18 = BitConverter.ToUInt32(data.GetByteSubArray(LOC["sg_field_18"], 4), 0);
            sg_field_1C = BitConverter.ToUInt32(data.GetByteSubArray(LOC["sg_field_1C"], 4), 0);
            MapPosition = new Vec3(data.GetByteSubArray(LOC["MapPosition"], Vec3.SIZE));
            sg_field_2C = BitConverter.ToSingle(data.GetByteSubArray(LOC["sg_field_2C"], 4), 0);
            LandmarkRotY = BitConverter.ToSingle(data.GetByteSubArray(LOC["LandmarkRotY"], 4), 0);
            isTimeStop = BitConverter.ToUInt16(data.GetByteSubArray(LOC["isTimeStop"], 2), 0);
            ChapterSaveScenarioFlag = BitConverter.ToUInt16(data.GetByteSubArray(LOC["ChapterSaveScenarioFlag"], 2), 0);
            ChapterSaveEventId = BitConverter.ToUInt16(data.GetByteSubArray(LOC["ChapterSaveEventId"], 2), 0);
            sg_field_3A = BitConverter.ToUInt16(data.GetByteSubArray(LOC["sg_field_3A"], 2), 0);

            Drivers = new Driver[16];
            for (int i = 0; i < Drivers.Length; i++)
                Drivers[i] = new Driver(data.GetByteSubArray(LOC["Drivers"] + (i * Driver.SIZE), Driver.SIZE));

            Blades = new Blade[422];
            for (int i = 0; i < Blades.Length; i++)
                Blades[i] = new Blade(data.GetByteSubArray(LOC["Blades"] + (i * Blade.SIZE), Blade.SIZE));

            Party = new XC2Party(data.GetByteSubArray(LOC["Party"], XC2Party.SIZE));
            ItemBox = new ItemBox(data.GetByteSubArray(LOC["ItemBox"], ItemBox.SIZE));
            Flags = data.GetByteSubArray(LOC["Flags"], 0xDC20);
            ScenarioQuest = BitConverter.ToUInt32(data.GetByteSubArray(LOC["ScenarioQuest"], 4), 0);
            CurrentQuest = BitConverter.ToUInt32(data.GetByteSubArray(LOC["CurrentQuest"], 4), 0);
            field_1097FC = BitConverter.ToUInt32(data.GetByteSubArray(LOC["field_1097FC"], 4), 0);
            Map = new SDataMap(data.GetByteSubArray(LOC["Map"], SDataMap.SIZE));
            sg_gap_1098C4 = data.GetByteSubArray(LOC["sg_gap_1098C4"], 12);
            CurrentInGameTime = new GameTime(data.GetByteSubArray(LOC["CurrentInGameTime"], GameTime.SIZE));
            PlayTime = new ElapseTime(data.GetByteSubArray(LOC["PlayTime"], ElapseTime.SIZE));
            MercenaryTeam1 = new MercenaryTeam(data.GetByteSubArray(LOC["MercenaryTeam1"], MercenaryTeam.SIZE));
            MercenaryTeam2 = new MercenaryTeam(data.GetByteSubArray(LOC["MercenaryTeam2"], MercenaryTeam.SIZE));
            MercenaryTeam3 = new MercenaryTeam(data.GetByteSubArray(LOC["MercenaryTeam3"], MercenaryTeam.SIZE));
            MercenaryTeamCount = BitConverter.ToUInt64(data.GetByteSubArray(LOC["MercenaryTeamCount"], 8), 0);

            MercenaryTeamPresets = new MercenaryTeamPreset[8];
            for (int i = 0; i < MercenaryTeamPresets.Length; i++)
                MercenaryTeamPresets[i] = new MercenaryTeamPreset(data.GetByteSubArray(LOC["MercenaryTeamPresets"] + (i * MercenaryTeamPreset.SIZE), MercenaryTeamPreset.SIZE));

            CommonBladeIDs = new UInt16[192];
            for (int i = 0; i < CommonBladeIDs.Length; i++)
                CommonBladeIDs[i] = BitConverter.ToUInt16(data.GetByteSubArray(LOC["CommonBladeIDs"] + (i * 2), 2), 0);

            PlayerCameraDistance = BitConverter.ToSingle(data.GetByteSubArray(LOC["PlayerCameraDistance"], 4), 0);
            GameClearCount = BitConverter.ToUInt32(data.GetByteSubArray(LOC["GameClearCount"], 4), 0);

            AchievementTasks = new TaskAchieve[128];
            for (int i = 0; i < AchievementTasks.Length; i++)
                AchievementTasks[i] = new TaskAchieve(data.GetByteSubArray(LOC["AchievementTasks"] + (i * TaskAchieve.SIZE), TaskAchieve.SIZE));

            AchievementTasksCount = BitConverter.ToUInt64(data.GetByteSubArray(LOC["AchievementTasksCount"], 8), 0);

            QuestIDs = new UInt32[256];
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
            field_10AD62 = BitConverter.ToUInt16(data.GetByteSubArray(LOC["field_10AD62"], 4), 0);
            CoinCount = BitConverter.ToUInt32(data.GetByteSubArray(LOC["CoinCount"], 4), 0);

            SavedEnemyHp = new UInt32[3];
            for (int i = 0; i < SavedEnemyHp.Length; i++)
                SavedEnemyHp[i] = BitConverter.ToUInt32(data.GetByteSubArray(LOC["SavedEnemyHp"] + (i * 4), 4), 0);

            field_10AD74 = BitConverter.ToUInt32(data.GetByteSubArray(LOC["field_10AD74"], 4), 0);
            Time2 = new RealTime(data.GetByteSubArray(LOC["Time2"], RealTime.SIZE));
            CameraHeight = BitConverter.ToSingle(data.GetByteSubArray(LOC["CameraHeight"], 4), 0);
            TigerTigerData = new Minigame(data.GetByteSubArray(LOC["TigerTigerData"], Minigame.SIZE));
            CameraYaw = BitConverter.ToSingle(data.GetByteSubArray(LOC["CameraYaw"], 4), 0);
            CameraPitch = BitConverter.ToSingle(data.GetByteSubArray(LOC["CameraPitch"], 4), 0);
            CameraFreeMode = data[LOC["CameraFreeMode"]];
            IsHikariCurrent = data[LOC["IsHikariCurrent"]];
            AutoEventAfterLoad = BitConverter.ToUInt16(data.GetByteSubArray(LOC["AutoEventAfterLoad"], 4), 0);
            IsCollectFlagNewVersion = data[LOC["IsCollectFlagNewVersion"]];
            IsEndGameSave = data[LOC["IsEndGameSave"]];
            CameraSide = data[LOC["CameraSide"]];
            gap_10AE93 = data[LOC["gap_10AE93"]];

            Events = new Event[500];
            for (int i = 0; i < Events.Length; i++)
                Events[i] = new Event(data.GetByteSubArray(LOC["Events"] + (i * Event.SIZE), Event.SIZE));

            EventsLength = BitConverter.ToUInt32(data.GetByteSubArray(LOC["EventsLength"], 4), 0);
            sg_gap1171E8 = data.GetByteSubArray(LOC["sg_gap1171E8"], 400);
            sg_field_117378 = BitConverter.ToSingle(data.GetByteSubArray(LOC["sg_field_117378"], 4), 0);

            ContentVersions = new UInt32[5];
            for (int i = 0; i < ContentVersions.Length; i++)
                ContentVersions[i] = BitConverter.ToUInt32(data.GetByteSubArray(LOC["ContentVersions"] + (i * 4), 4), 0);

            sg_gap_117390 = data.GetByteSubArray(LOC["sg_gap_117390"], 776);
            sg_field_117698 = BitConverter.ToUInt32(data.GetByteSubArray(LOC["sg_field_117698"], 4), 0);
            sg_field_11769C = BitConverter.ToUInt32(data.GetByteSubArray(LOC["sg_field_11769C"], 4), 0);
        }

        public virtual byte[] ToRawData()
        {
            List<Byte> result = new List<byte>();

            result.AddRange(BitConverter.GetBytes(Magic));
            result.AddRange(BitConverter.GetBytes(ss_field_4));
            result.AddRange(Time.ToRawData());
            result.AddRange(BitConverter.GetBytes(Money));
            result.AddRange(BitConverter.GetBytes(MapJumpId));
            result.AddRange(BitConverter.GetBytes(sg_field_18));
            result.AddRange(BitConverter.GetBytes(sg_field_1C));
            result.AddRange(MapPosition.ToRawData());
            result.AddRange(BitConverter.GetBytes(sg_field_2C));
            result.AddRange(BitConverter.GetBytes(LandmarkRotY));
            result.AddRange(BitConverter.GetBytes(isTimeStop));
            result.AddRange(BitConverter.GetBytes(ChapterSaveScenarioFlag));
            result.AddRange(BitConverter.GetBytes(ChapterSaveEventId));
            result.AddRange(BitConverter.GetBytes(sg_field_3A));

            foreach (Driver d in Drivers)
                result.AddRange(d.ToRawData());

            foreach (Blade b in Blades)
                result.AddRange(b.ToRawData());

            result.AddRange(Party.ToRawData());
            result.AddRange(ItemBox.ToRawData());
            result.AddRange(Flags);
            result.AddRange(BitConverter.GetBytes(ScenarioQuest));
            result.AddRange(BitConverter.GetBytes(CurrentQuest));
            result.AddRange(BitConverter.GetBytes(field_1097FC));
            result.AddRange(Map.ToRawData());
            result.AddRange(sg_gap_1098C4);
            result.AddRange(CurrentInGameTime.ToRawData());
            result.AddRange(PlayTime.ToRawData());
            result.AddRange(MercenaryTeam1.ToRawData());
            result.AddRange(MercenaryTeam2.ToRawData());
            result.AddRange(MercenaryTeam3.ToRawData());

            result.AddRange(BitConverter.GetBytes(MercenaryTeamCount));

            foreach (MercenaryTeamPreset mtp in MercenaryTeamPresets)
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
            result.AddRange(BitConverter.GetBytes(field_10AD62));
            result.AddRange(BitConverter.GetBytes(CoinCount));

            foreach (UInt32 u in SavedEnemyHp)
                result.AddRange(BitConverter.GetBytes(u));

            result.AddRange(BitConverter.GetBytes(field_10AD74));
            result.AddRange(Time2.ToRawData());
            result.AddRange(BitConverter.GetBytes(CameraHeight));
            result.AddRange(TigerTigerData.ToRawData());
            result.AddRange(BitConverter.GetBytes(CameraYaw));
            result.AddRange(BitConverter.GetBytes(CameraPitch));
            result.Add(CameraFreeMode);
            result.Add(IsHikariCurrent);
            result.AddRange(BitConverter.GetBytes(AutoEventAfterLoad));
            result.Add(IsCollectFlagNewVersion);
            result.Add(IsEndGameSave);

            result.Add(CameraSide);
            result.Add(gap_10AE93);

            foreach (Event e in Events)
                result.AddRange(e.ToRawData());

            result.AddRange(BitConverter.GetBytes(EventsLength));
            result.AddRange(sg_gap1171E8);
            result.AddRange(BitConverter.GetBytes(sg_field_117378));

            foreach (UInt32 u in ContentVersions)
                result.AddRange(BitConverter.GetBytes(u));

            result.AddRange(sg_gap_117390);
            result.AddRange(BitConverter.GetBytes(sg_field_117698));
            result.AddRange(BitConverter.GetBytes(sg_field_11769C));

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
        public Byte[] Unk_0x1176A0 { get; set; }

        public XC2Save150(Byte[] data) : base(data)
        {
            Unk_0x1176A0 = data.GetByteSubArray(0x1176A0, SIZE - XC2Save.SIZE);
        }

        public override Byte[] ToRawData()
        {
            List<Byte> result = new List<Byte>();

            result.AddRange(base.ToRawData());
            result.AddRange(Unk_0x1176A0);

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

    public static class XC2SaveParser
    {
        public static XC2Save Deserialize(Byte[] data)
        {
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
                        "Actual Size: 0x" + data.Length.ToString("X") + Environment.NewLine +
                        "Data has not been loaded.";
                    throw new Exception(message);
            }
        }
    }
}
