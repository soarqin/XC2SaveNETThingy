using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Ionic.Zlib;

namespace XC2SaveNETThingy
{
    public class XC2Save : IXC2SaveObject
    {
        public const int SIZE = 0x1176A0;
        public static readonly Byte[] CORRECT_MAGIC = new Byte[] { 0x5B, 0xA1, 0x03, 0x00 };
        public const int COMMON_BLADE_IDS_COUNT = 192;
        public const int QUEST_ID_COUNT = 256;
        public const int CONTENT_VERSION_COUNT = 5;

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
        public static int DRIVER150EXTS_COUNT = 16;
        private static readonly Dictionary<string, int> LOC = new Dictionary<string, int>()
        {
            { "Driver150Exts", 0x11769C },
            { "Unk_0x11A8DC", 0x11A8DC },
        };

        public Driver150Ext[] Driver150Exts { get; set; }
        public Byte[] Unk_0x11A8DC { get; set; }

        public XC2Save150(Byte[] data) : base(data)
        {
            Driver150Exts = new Driver150Ext[16];
            for (int i = 0; i < Driver150Exts.Length; i++)
            {
                Driver150Exts[i] = new Driver150Ext(data.GetByteSubArray(LOC["Driver150Exts"] + (i * Driver150Ext.SIZE), Driver150Ext.SIZE));
                Drivers[i].Weapons.AddRange(Driver150Exts[i].Weapons150);
                Drivers[i].DriverArtLevels.AddRange(Driver150Exts[i].DriverArtLevels150);
            }

            Unk_0x11A8DC = data.GetByteSubArray(LOC["Unk_0x11A8DC"], 0xC4B4);
        }
        public override Byte[] ToRawData()
        {   
            List<Byte> result = new List<Byte>();

            result.AddRange(base.ToRawData());
            result.RemoveRange(0x11769C, 4);

            for (int i = 0; i < Drivers.Length; i++)
            {
                Driver150Exts[i].Weapons150 = Drivers[i].Weapons.GetRange(Driver.WEAPONS_COUNT, Driver150Ext.WEAPONS_COUNT);
                Driver150Exts[i].DriverArtLevels150 = Drivers[i].DriverArtLevels.GetRange(Driver.DRIVER_ARTS_COUNT, Driver150Ext.DRIVER_ARTS_COUNT);
            }

            foreach (Driver150Ext de in Driver150Exts)
                result.AddRange(de.ToRawData());

            result.AddRange(Unk_0x11A8DC);


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

    public class XC2SaveIra : XC2Save150, IXC2SaveObject
    {
        public new const int SIZE = 0x137B10;
        private static readonly Dictionary<string, int> LOC = new Dictionary<string, int>()
        {
            { "DriversIra", 0x126D90 },
            { "DriverIraExts", 0x134850 },
            { "Unk_0x137A90", 0x137A90 },
        };

        public DriverIra[] DriversIra { get; set; }
        public DriverIraExt[] DriverIraExts { get; set; }
        public Byte[] Unk_0x137A90 { get; set; }
        
        public XC2SaveIra(byte[] data) : base(data)
        {
            DriversIra = new DriverIra[16];
            for (int i = 0; i < DriversIra.Length; i++)
                DriversIra[i] = new DriverIra(data.GetByteSubArray(LOC["DriversIra"] + (i * DriverIra.SIZE), DriverIra.SIZE));

            DriverIraExts = new DriverIraExt[16];
            for (int i = 0; i < Driver150Exts.Length; i++)
            {
                DriverIraExts[i] = new DriverIraExt(data.GetByteSubArray(LOC["DriverIraExts"] + (i * DriverIraExt.SIZE), DriverIraExt.SIZE));
                DriversIra[i].Weapons.AddRange(DriverIraExts[i].WeaponsIra);
                DriversIra[i].DriverArtLevels.AddRange(DriverIraExts[i].DriverArtLevelsIra);
            }

            Unk_0x137A90 = data.GetByteSubArray(LOC["Unk_0x137A90"], 0x80);
        }
        public override Byte[] ToRawData()
        {
            List<Byte> result = new List<Byte>();

            result.AddRange(base.ToRawData());

            foreach (DriverIra d in DriversIra)
                result.AddRange(d.ToRawData());

            for (int i = 0; i < DriversIra.Length; i++)
            {
                DriverIraExts[i].WeaponsIra = DriversIra[i].Weapons.GetRange(Driver.WEAPONS_COUNT, DriverIraExt.WEAPONS_COUNT);
                DriverIraExts[i].DriverArtLevelsIra = DriversIra[i].DriverArtLevels.GetRange(Driver.DRIVER_ARTS_COUNT, DriverIraExt.DRIVER_ARTS_COUNT);
            }

            foreach (DriverIraExt de in DriverIraExts)
                result.AddRange(de.ToRawData());

            result.AddRange(Unk_0x137A90);

            return result.ToArray();
        }
    }

    public static class XC2SaveSerialization
    {
        private static Byte[] CMP_HEADER = new Byte[] { 0x7E, 0xC0, 0xE2, 0xC9, 0x45, 0x98, 0xCE, 0x03, 0xA7, 0x98, 0xF1, 0x33, 0x6A, 0xE7, 0x25, 0x80 };

        public static XC2Save Deserialize(Byte[] data)
        {
            if (data.GetByteSubArray(0, CMP_HEADER.Length).SequenceEqual(CMP_HEADER))
                data = Decompress(data);

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

                case XC2SaveIra.SIZE:
                    Console.WriteLine("XC2SaveIra detected.");
                    return new XC2SaveIra(data);

                default:
                    string message = "Given save data has incorrect filesize!" + Environment.NewLine +
                        Environment.NewLine +
                        "Expected Size:" + Environment.NewLine +
                        String.Format("0x{0:X} for old save data.", XC2Save.SIZE) + Environment.NewLine +
                        String.Format("0x{0:X} for ver 1.5.0 save data.", XC2Save150.SIZE) + Environment.NewLine +
                        String.Format("0x{0:X} for Torna Golden Country DLC save data.", XC2SaveIra.SIZE) + Environment.NewLine +
                        Environment.NewLine +
                        "Actual Size: 0x" + data.Length.ToString("X");
                    throw new Exception(message);
            }
        }
        public static Byte[] Serialize(XC2Save save, bool compress = false)
        {
            if (compress)
                return Compress(save.ToRawData());
            else
                return save.ToRawData();
        }
        private static Byte[] Decompress(Byte[] data)
        {
            Byte[] result;

            using (ZlibStream compStream = new ZlibStream(new MemoryStream(data, CMP_HEADER.Length, data.Length - CMP_HEADER.Length), CompressionMode.Decompress))
            using (MemoryStream decStream = new MemoryStream())
            {
                compStream.CopyTo(decStream);
                result = decStream.ToArray();
            }

            return result;
        }
        private static Byte[] Compress(Byte[] data)
        {
            List<Byte> result = new List<Byte>();
            result.AddRange(CMP_HEADER);

            using (ZlibStream decStream = new ZlibStream(new MemoryStream(data, 0, data.Length), CompressionMode.Compress))
            using (MemoryStream compStream = new MemoryStream())
            {
                decStream.CopyTo(compStream);
                result.AddRange(compStream.ToArray());
            }

            return result.ToArray();
        }
    }
}
