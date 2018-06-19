using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XC2SaveNETThingy
{
    public class Vec3
    {
        public float X;
        public float Y;
        public float Z;

        public Vec3(float X = 0, float Y = 0, float Z = 0)
        {
            this.X = X;
            this.Y = Y;
            this.Z = Z;
        }
    }

    class Vec3Padded : Vec3
    {
        float Padding;

        public Vec3Padded(float X = 0, float Y = 0, float Z = 0, float Padding = 0) : base(X, Y, Z)
        {
            this.Padding = Padding;
        }
    }
    public class XC2Save
    {
        private byte[] saveData;

        class gf_GameTime
        {
            UInt32 rawData;
            //UInt32 Seconds, 6;
            //UInt32 Minutes : 6;
            //UInt32 Hours : 5;
            //UInt32 Days : 15;

            Byte Seconds;
            Byte Minutes;
            Byte Hours;
            UInt16 Days;

            public gf_GameTime(UInt32 data)
            {
                Seconds = (Byte)(data << 26);

                Minutes = (Byte)(data << 20);
                Minutes = (Byte)(Minutes & ~0b111111000000);

                Hours = (Byte)(data << 15);
                Hours = (Byte)(data & ~0b11111111111100000);

                Days = (UInt16)(data & ~0b11111111111111111000000000000000);
            }
        };

        public class gf_RealTime : IXC2SaveObject
        {
            // 13 bits
            public UInt16 Year;
            private const int YEAR_BITS = 13;

            // 4 bits
            public Byte Month;
            private const int MONTH_BITS = 4;

            // 9 bits
            public UInt16 UnkB;
            private const int UNKB_BITS = 9;

            // 5 bits
            public Byte Day;
            private const int DAY_BITS = 5;

            // 5 bits
            public Byte Hour;
            private const int HOUR_BITS = 5;

            // 6 bits
            public Byte Minute;
            private const int MINUTE_BITS = 6;

            // 6 bits 
            public Byte Second;
            private const int SECOND_BITS = 6;

            // 10 bits
            public UInt16 MSecond;
            private const int MSECOND_BITS = 10;

            // 6 bits
            public Byte UnkA;
            private const int UNKA_BITS = 6;

            public gf_RealTime(Byte[] data)
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
                derp += (UInt64)Hour << (64 - YEAR_BITS - MONTH_BITS- UNKB_BITS - DAY_BITS - HOUR_BITS);
                derp += (UInt64)Minute << (64 - YEAR_BITS - MONTH_BITS - UNKB_BITS - DAY_BITS - HOUR_BITS- MINUTE_BITS);
                derp += (UInt64)Second << (64 - YEAR_BITS - MONTH_BITS - UNKB_BITS - DAY_BITS - HOUR_BITS - MINUTE_BITS - SECOND_BITS);
                derp += (UInt64)MSecond << (64 - YEAR_BITS - MONTH_BITS - UNKB_BITS - DAY_BITS - HOUR_BITS - MINUTE_BITS - SECOND_BITS - MSECOND_BITS);
                derp += (UInt64)UnkA;


                Byte[] value = BitConverter.GetBytes(derp);
                Array.Reverse(value);

                return value;
            }

            public override string ToString()
            {
                return String.Format("{0:0000}-{1:00}-{2:00} {3:00}:{4:00}:{5:00}.{6:000}", Year, Month, Day, Hour, Minute, Second, MSecond);
            }
        }

        public class PartyMember : IXC2SaveObject
        {
            public const int SIZE = 0x6;

            public UInt16 DriverId;
            public UInt16 field_2;
            public UInt16 field_4;

            public PartyMember(Byte[] data)
            {
                DriverId = BitConverter.ToUInt16(data.GetByteSubArray(0, 2), 0);
                field_2 = BitConverter.ToUInt16(data.GetByteSubArray(2, 2), 0);
                field_4 = BitConverter.ToUInt16(data.GetByteSubArray(4, 2), 0);
            }

            public Byte[] ToRawData()
            {
                List<Byte> result = new List<byte>();

                result.AddRange(BitConverter.GetBytes(DriverId));
                result.AddRange(BitConverter.GetBytes(field_2));
                result.AddRange(BitConverter.GetBytes(field_4));

                return result.ToArray();
            }
        }

        public class XC2Party : IXC2SaveObject
        {
            public const int SIZE = 0x54;

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

            public PartyMember[] Members;
            public Byte[] gap3C;
            public UInt32 PartyLeader;
            public UInt32 field_44;
            public UInt32 field_48;
            public UInt16 PartyGauge;
            public UInt16 field_4E;
            public UInt32 field_50;

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

                return result.ToArray();
            }
        }

        class SDataMap
        {
            Vec3Padded[] DriverPositions = new Vec3Padded[3];
            Vec3Padded[] BladePositions = new Vec3Padded[3];
            Vec3Padded[] DriverRotations = new Vec3Padded[3];
            Vec3Padded[] BladeRotations = new Vec3Padded[3];
            UInt32 MapJumpId;
        }

        class MercenaryTeam
        {
            UInt16[] MemberIds = new UInt16[6];
            UInt32 field_C;
            UInt32 TeamId;
            UInt32 MissionId;
            UInt32 field_18;
            UInt32 MissionTime;
            UInt32 MissionTimeOriginal;
            UInt32 field_24;
        }

        class FixedVector_MercenaryTeam_3
        {
            MercenaryTeam[] Data = new MercenaryTeam[3];
            UInt64 Length;
        }

        class MercenaryTeamPreset
        {
            UInt16[] Members = new UInt16[6];
        }
        class TaskAchieve
        {
            UInt16 taska_field_0;
            UInt16 taska_field_2;
            UInt16 taska_field_4;
            UInt16 taska_field_6;
            UInt16 taska_field_8;
        }

        class FixedVector_TaskAchieve_128
        {
            TaskAchieve[] Data = new TaskAchieve[128];
            UInt64 Length;
        }

        class FixedVector_QuestId_256
        {
            UInt32[] Data = new UInt32[256];
            UInt32 Length;
        }
        class SDataWeather
        {
            char[] Name = new char[4];
            UInt32 field_4;
            UInt32 field_8;
            UInt32 field_C;
            float field_10;
            float field_14;
            UInt32 field_18;
            UInt32 field_1C;
        }

        public class Event
        {
            UInt16 EventId;
            UInt16 Creator;
            UInt16 PlayBladeId;
            UInt16 VoiceID;
            UInt16 Attribute;
            UInt16 ExtraParts;
            UInt16 A;
            UInt16[] Weapons = new UInt16[10];
            UInt16[] B = new UInt16[6];
            private UInt16[] Blades = new UInt16[10];
            UInt16[] C = new UInt16[5];
            gf_GameTime GameTime;
            UInt16 CurrentMapWeatherID;
            UInt16 CurrentWtrType;
            UInt16[] D = new UInt16[8];
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
            { "Party", 0xE9894 }
        };

        public UInt32 Magic;
        public UInt32 ss_field_4;
        public gf_RealTime Time;
        public UInt32 Money;
        public UInt32 MapJumpId;
        public UInt32 sg_field_18;
        public UInt32 sg_field_1C;
        public Vec3 MapPosition;
        public float sg_field_2C;
        public float LandmarkRotY;
        public UInt16 isTimeStop;
        public UInt16 ChapterSaveScenarioFlag;
        public UInt16 ChapterSaveEventId;
        public UInt16 sg_field_3A;
        public Driver[] Drivers;
        public Blade[] Blades;
        public XC2Party Party;
        public ItemBox ItemBox;

        //GameFlag Flags;
        Byte[] Flags = new Byte[0xDC20];

        UInt32 ScenarioQuest;
        UInt32 CurrentQuest;
        UInt32 field_1097EC;
        SDataMap Map;
        Byte[] sg_gap_1098B4 = new Byte[12];
        gf_GameTime GameTime;
        gf_ElapseTime ElapseTime;
        FixedVector_MercenaryTeam_3 MercTeams;
        MercenaryTeamPreset[] MercPresets = new MercenaryTeamPreset[8];
        UInt16[] CommonBladeIds = new UInt16[192];
        float PlayerCameraDistance;
        UInt32 GameClearCount;
        FixedVector_TaskAchieve_128 AchievementTasks;
        FixedVector_QuestId_256 Quests;
        SDataWeather[] Weather = new SDataWeather[64];
        UInt32 EtherCrystals;
        float MoveDistance;
        float MoveDistanceB;
        UInt32 AssurePoint;
        UInt32 AssureCount;
        UInt16 RareBladeAppearType;
        UInt16 field_10AD52;
        UInt32 CoinCount;
        UInt32[] SavedEnemyHp = new UInt32[3];
        UInt32 field_10AD64;
        gf_RealTime Time2;
        float CameraHeight;
        Byte[] Minigame = new Byte[256];
        float CameraYaw;
        float CameraPitch;
        Byte CameraFreeMode;
        Byte IsHikariCurrent;
        UInt16 AutoEventAfterLoad;
        Byte IsCollectFlagNewVersion;
        Byte IsEndGameSave;
        Byte CameraSide;
        Byte gap_10AE83;
        Event[] Events = new Event[500];
        UInt32 EventsLength;
        Byte[] sg_gap1171D8 = new Byte[400];
        float sg_field_117368;
        UInt32[] ContentVersions = new UInt32[5];
        Byte[] sg_gap_117380 = new Byte[776];
        UInt32 sg_field_117688;
        UInt32 sg_field_11768C;

        public XC2Save(string path)
        {
            saveData = File.ReadAllBytes(path);

            Magic = BitConverter.ToUInt32(saveData.GetByteSubArray(LOC["Magic"], 4), 0);
            ss_field_4 = BitConverter.ToUInt32(saveData.GetByteSubArray(LOC["ss_field_4"], 4), 0);
            Time = new gf_RealTime(saveData.GetByteSubArray(LOC["Time"], 8));
            Money = BitConverter.ToUInt32(saveData.GetByteSubArray(LOC["Money"], 4), 0);
            MapJumpId = BitConverter.ToUInt32(saveData.GetByteSubArray(LOC["MapJumpId"], 4), 0);
            sg_field_18 = BitConverter.ToUInt32(saveData.GetByteSubArray(LOC["sg_field_18"], 4), 0);
            sg_field_1C = BitConverter.ToUInt32(saveData.GetByteSubArray(LOC["sg_field_1C"], 4), 0);
            MapPosition = new Vec3(
                BitConverter.ToSingle(saveData.GetByteSubArray(LOC["MapPosition"], 4), 0),
                BitConverter.ToSingle(saveData.GetByteSubArray(LOC["MapPosition"] + 4, 4), 0),
                BitConverter.ToSingle(saveData.GetByteSubArray(LOC["MapPosition"] + 8, 4), 0)
                );
            sg_field_2C = BitConverter.ToSingle(saveData.GetByteSubArray(LOC["sg_field_2C"], 4), 0);
            LandmarkRotY = BitConverter.ToSingle(saveData.GetByteSubArray(LOC["LandmarkRotY"], 4), 0);
            isTimeStop = BitConverter.ToUInt16(saveData.GetByteSubArray(LOC["isTimeStop"], 2), 0);
            ChapterSaveScenarioFlag = BitConverter.ToUInt16(saveData.GetByteSubArray(LOC["ChapterSaveScenarioFlag"], 2), 0);
            ChapterSaveEventId = BitConverter.ToUInt16(saveData.GetByteSubArray(LOC["ChapterSaveEventId"], 2), 0);
            sg_field_3A = BitConverter.ToUInt16(saveData.GetByteSubArray(LOC["sg_field_3A"], 2), 0);

            Drivers = new Driver[16];
            for (int i = 0; i < Drivers.Length; i++)
            {
                Byte[] driverBytes = new Byte[Driver.SIZE];

                for (int j = 0; j < driverBytes.Length; j++)
                {
                    driverBytes[j] = saveData[LOC["Drivers"] + (Driver.SIZE * i) + j];
                }

                Drivers[i] = new Driver(driverBytes);
            }

            Blades = new Blade[422];
            for (int i = 0; i < Blades.Length; i++)
            {
                Blades[i] = new Blade(saveData.GetByteSubArray(LOC["Blades"] + (i * Blade.SIZE), Blade.SIZE));
            }

            Party = new XC2Party(saveData.GetByteSubArray(LOC["Party"], XC2Party.SIZE));
            ItemBox ItemBox;

        }
    }
}
