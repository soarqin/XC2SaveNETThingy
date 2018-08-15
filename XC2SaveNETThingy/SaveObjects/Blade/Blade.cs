using System;
using System.Collections.Generic;
using System.Text;

namespace XC2SaveNETThingy
{
    public class Blade : IXC2SaveObject
    {
        public const int SIZE = 0x8A4;
        public const int AFF_CHART_SIZE = 0x5A0;

        public bool IsEnabled { get; set; }
        public UInt16 Creator { get; set; }
        public UInt16 SetDriver { get; set; }
        public UInt16 ID { get; set; }
        public ElapseTime BornTime { get; set; }
        public UInt32 BraveryPoints { get; set; }
        public UInt32 BraveryLevel { get; set; }
        public UInt32 TruthPoints { get; set; }
        public UInt32 TruthLevel { get; set; }
        public UInt32 CompassionPoints { get; set; }
        public UInt32 CompassionLevel { get; set; }
        public UInt32 JusticePoints { get; set; }
        public UInt32 JusticeLevel { get; set; }
        public Byte[] Unk_0x002C { get; set; }
        public UInt32 TrustPoints { get; set; }
        public UInt32 TrustRank { get; set; }
        public BladeArt[] BArts { get; set; }
        public BladeArt[] NArts { get; set; }
        public UInt16 BArtExId { get; set; }
        public UInt16 BladeArtsExH { get; set; }
        public Byte[] Unk_0x0088 { get; set; }
        public UInt16 BArtEx2Id { get; set; }
        public UInt16 BladeArtsEx2H { get; set; }
        public Byte[] Unk_0x0094 { get; set; }
        public BladeSkill[] BattleSkills { get; set; }
        public BladeSkill[] FieldSkills { get; set; }
        public Byte[] ExtraParts2 { get; set; }
        public AchieveInfo[] BArtsAchieve { get; set; }
        public AchieveInfo[] BSkillsAchievement { get; set; }
        public AchieveInfo[] FSkillsAchievement { get; set; }
        public AchieveInfo KeyAchievement { get; set; }
        public UInt16 KeyReleaseLevel { get; set; }
        public UInt16 FavoriteCategory0 { get; set; }
        public UInt16 FavoriteItem0 { get; set; }
        public bool FavoriteCategory0Unlocked { get; set; }
        public bool FavoriteItem0Unlocked { get; set; }
        public UInt16 FavoriteCategory1 { get; set; }
        public UInt16 FavoriteItem1 { get; set; }
        public bool FavoriteCategory1Unlocked { get; set; }
        public bool FavoriteItem1Unlocked { get; set; }
        public Byte[] Unk_0x0676 { get; set; }
        public UInt16[] AuxCores { get; set; }
        public Byte[] Unk_0x067E { get; set; }
        public ItemHandle32[] AuxCoreItemHandles { get; set; }
        public PoppiSwapData Poppiswap { get; set; }
        public Byte Race { get; set; }
        public Byte Gender { get; set; }
        public UInt16 Still { get; set; }
        public PaddedString16 ModelResourceName { get; set; }
        public PaddedString16 Model2Name { get; set; }
        public PaddedString16 MotionResourceName { get; set; }
        public PaddedString16 Motion2Name { get; set; }
        public PaddedString16 AddMotionName { get; set; }
        public PaddedString16 VoiceName { get; set; }
        public Byte[] ClipEvent { get; set; }
        public PaddedString16 Com_SE { get; set; }
        public PaddedString16 EffectResourceName { get; set; }
        public PaddedString16 Com_Vo { get; set; }
        public PaddedString16 CenterBone { get; set; }
        public PaddedString16 CamBone { get; set; }
        public PaddedString32 SeResourceName { get; set; }
        public Byte BladeSize { get; set; }
        public Byte WeaponType { get; set; }
        public Byte OrbNum { get; set; }
        public Byte Atr { get; set; }
        public Byte Personality { get; set; }
        public Byte ExtraParts { get; set; }
        public Byte EyeRot { get; set; }
        public Byte Unk_0x0827 { get; set; }
        public UInt16 CoolTime { get; set; }
        public UInt16 Condition { get; set; }
        public UInt16 DefWeapon { get; set; }
        public UInt16 ChestHeight { get; set; }
        public UInt16 LandingHeight { get; set; }
        public UInt16 RareNameId { get; set; }
        public UInt16 CommonNameId { get; set; }
        public UInt16 Scale { get; set; }
        public UInt16 WpnScale { get; set; }
        public UInt16 OffsetID { get; set; }
        public UInt16 CollisionId { get; set; }
        public Byte PhysDef { get; set; }
        public Byte EtherDef { get; set; }
        public Byte MaxHPMod { get; set; }
        public Byte StrengthMod { get; set; }
        public Byte EtherMod { get; set; }
        public Byte DexterityMod { get; set; }
        public Byte AgilityMod { get; set; }
        public Byte LuckMod { get; set; }
        public Byte QuestRace { get; set; }
        public bool ReleaseLock { get; set; }
        public Byte FootStep { get; set; }
        public Byte FootStepEffect { get; set; }
        public UInt16 KizunaLinkSet { get; set; }
        public UInt16 NormalTalk { get; set; }
        public Byte[] Unk_0x084E { get; set; }
        public UInt32 CreateEventID { get; set; }
        public UInt16 MountPoint { get; set; }
        public UInt16 MountObject { get; set; }
        public Byte[] Name { get; set; } // max len 64 bytes, strange garbage bytes after string
        public UInt32 NameLength { get; set; }
        public Int16 CommonBladeIndex { get; set; }
        public bool EnableEngageRex { get; set; }
        public Byte BladeReleaseStatus { get; set; }
        public Byte IsUnselect { get; set; }
        public Byte AffinityChartStatus { get; set; }
        public Byte[] Unk_0x08A2 { get; set; }

        private static Dictionary<string, int> LOC = new Dictionary<string, int>()
        {
            { "DataType", 0x00 },
            { "Creator", 0x02 },
            { "SetDriver", 0x04 },
            { "BladeId", 0x06 },
            { "BornTime", 0x08 },
            { "BraveryPoints", 0x0C },
            { "BraveryLevel", 0x10 },
            { "TruthPoints", 0x14 },
            { "TruthLevel", 0x18 },
            { "CompassionPoints", 0x1C },
            { "CompassionLevel", 0x20 },
            { "JusticePoints", 0x24 },
            { "JusticeLevel", 0x28 },
            { "Unk_0x002C", 0x2C },
            { "TrustPoints", 0x34 },
            { "TrustRank", 0x38 },
            { "BArts", 0x3C },
            { "NArts", 0x60 },
            { "BArtExId", 0x84 },
            { "BladeArtsExH", 0x86 },
            { "Unk_0x0088", 0x88 },
            { "BArtEx2Id", 0x90 },
            { "BladeArtsEx2H", 0x92 },
            { "Unk_0x0094", 0x94 },
            { "BladeSkills", 0x9C },
            { "FieldSkills", 0xAE },
            { "ExtraParts2", 0xC0 },
            { "BArtsAchieve", 0xC4 },
            { "BSkillsAchievement", 0x274 },
            { "FSkillsAchievement", 0x424 },
            { "KeyAchievement", 0x5D4 },
            { "KeyReleaseLevel", 0x664 },
            { "FavoriteCategory0", 0x666 },
            { "FavoriteItem0", 0x668 },
            { "FavoriteCategory0Unlocked", 0x66A },
            { "FavoriteItem0Unlocked", 0x66C },
            { "FavoriteCategory1", 0x66E },
            { "FavoriteItem1", 0x670 },
            { "FavoriteCategory1Unlocked", 0x672 },
            { "FavoriteItem1Unlocked", 0x674 },
            { "Unk_0x0676", 0x676 },
            { "AuxCores", 0x678 },
            { "Unk_0x067E", 0x67E },
            { "AuxCoreItemHandles", 0x680 },
            { "HanaEquip", 0x68C },
            { "Race", 0x708 },
            { "Gender", 0x709 },
            { "Still", 0x70A },
            { "ModelResourceName", 0x70C },
            { "Model2Name", 0x720 },
            { "MotionResourceName", 0x734 },
            { "Motion2Name", 0x748 },
            { "AddMotionName", 0x75C },
            { "VoiceName", 0x770 },
            { "ClipEvent", 0x784 },
            { "Com_SE", 0x798 },
            { "EffectResourceName", 0x7AC },
            { "Com_Vo", 0x7C0 },
            { "CenterBone", 0x7D4 },
            { "CamBone", 0x7E8 },
            { "SeResourceName", 0x7FC },
            { "BladeSize", 0x820 },
            { "WeaponType", 0x821 },
            { "OrbNum", 0x822 },
            { "Atr", 0x823 },
            { "Personality", 0x824 },
            { "ExtraParts", 0x825 },
            { "EyeRot", 0x826 },
            { "Unk_0x0827", 0x827 },
            { "CoolTime", 0x828 },
            { "Condition", 0x82A },
            { "DefWeapon", 0x82C },
            { "ChestHeight", 0x82E },
            { "LandingHeight", 0x830 },
            { "RareNameId", 0x832 },
            { "CommonNameId", 0x834 },
            { "Scale", 0x836 },
            { "WpnScale", 0x838 },
            { "OffsetID", 0x83A },
            { "CollisionId", 0x83C },
            { "PArmor", 0x83E },
            { "EArmor", 0x83F },
            { "HpMaxRev", 0x840 },
            { "StrengthRev", 0x841 },
            { "PowEtherRev", 0x842 },
            { "DexRev", 0x843 },
            { "AgilityRev", 0x844 },
            { "LuckRev", 0x845 },
            { "QuestRace", 0x846 },
            { "ReleaseLock", 0x847 },
            { "FootStep", 0x848 },
            { "FootStepEffect", 0x849 },
            { "KizunaLinkSet", 0x84A },
            { "NormalTalk", 0x84C },
            { "Unk_0x084E", 0x84E },
            { "CreateEventID", 0x850 },
            { "MountPoint", 0x854 },
            { "MountObject", 0x856 },
            { "Name", 0x858 },
            { "NameLength", 0x898 },
            { "CommonBladeIndex", 0x89C },
            { "EnableEngageRex", 0x89E },
            { "BladeReleaseStatus", 0x89F },
            { "isUnselect", 0x8A0 },
            { "AffinityChartStatus", 0x8A1 },
            { "Unk_0x08A2", 0x8A2 },
        };

        public Blade(Byte[] data)
        {
            if (data.Length != SIZE)
            {
                string message = "IMPORT Blade: SIZE ALL WRONG!!!" + Environment.NewLine +
                    "Size should be " + SIZE + " bytes..." + Environment.NewLine +
                    "...but Size is " + data.Length + " bytes!";

                throw new Exception(message);
            }

            IsEnabled = BitConverter.ToUInt16(data.GetByteSubArray(LOC["DataType"], 2), 0) == 1;
            Creator = BitConverter.ToUInt16(data.GetByteSubArray(LOC["Creator"], 2), 0);
            SetDriver = BitConverter.ToUInt16(data.GetByteSubArray(LOC["SetDriver"], 2), 0);
            ID = BitConverter.ToUInt16(data.GetByteSubArray(LOC["BladeId"], 2), 0);
            BornTime =new ElapseTime(data.GetByteSubArray(LOC["BornTime"], 4));
            BraveryPoints = BitConverter.ToUInt32(data.GetByteSubArray(LOC["BraveryPoints"], 4), 0);
            BraveryLevel = BitConverter.ToUInt32(data.GetByteSubArray(LOC["BraveryLevel"], 4), 0);
            TruthPoints = BitConverter.ToUInt32(data.GetByteSubArray(LOC["TruthPoints"], 4), 0);
            TruthLevel = BitConverter.ToUInt32(data.GetByteSubArray(LOC["TruthLevel"], 4), 0);
            CompassionPoints = BitConverter.ToUInt32(data.GetByteSubArray(LOC["CompassionPoints"], 4), 0);
            CompassionLevel = BitConverter.ToUInt32(data.GetByteSubArray(LOC["CompassionLevel"], 4), 0);
            JusticePoints = BitConverter.ToUInt32(data.GetByteSubArray(LOC["JusticePoints"], 4), 0);
            JusticeLevel = BitConverter.ToUInt32(data.GetByteSubArray(LOC["JusticeLevel"], 4), 0);
            Unk_0x002C = data.GetByteSubArray(LOC["Unk_0x002C"], 8);
            TrustPoints = BitConverter.ToUInt32(data.GetByteSubArray(LOC["TrustPoints"], 4), 0);
            TrustRank = BitConverter.ToUInt32(data.GetByteSubArray(LOC["TrustRank"], 4), 0);

            BArts = new BladeArt[3];
            for (int i = 0; i < BArts.Length; i++)
                BArts[i] = new BladeArt(data.GetByteSubArray(LOC["BArts"] + (i * BladeArt.SIZE), BladeArt.SIZE));

            NArts = new BladeArt[3];
            for (int i = 0; i < NArts.Length; i++)
                NArts[i] = new BladeArt(data.GetByteSubArray(LOC["NArts"] + (i * BladeArt.SIZE), BladeArt.SIZE));

            BArtExId = BitConverter.ToUInt16(data.GetByteSubArray(LOC["BArtExId"], 2), 0);
            BladeArtsExH = BitConverter.ToUInt16(data.GetByteSubArray(LOC["BladeArtsExH"], 2), 0);
            Unk_0x0088 = data.GetByteSubArray(LOC["Unk_0x0088"], 8);
            BArtEx2Id = BitConverter.ToUInt16(data.GetByteSubArray(LOC["BArtEx2Id"], 2), 0);
            BladeArtsEx2H = BitConverter.ToUInt16(data.GetByteSubArray(LOC["BladeArtsEx2H"], 2), 0);
            Unk_0x0094 = data.GetByteSubArray(LOC["Unk_0x0094"], 8);

            BattleSkills = new BladeSkill[3];
            for (int i = 0; i < BattleSkills.Length; i++)
                BattleSkills[i] = new BladeSkill(data.GetByteSubArray(LOC["BladeSkills"] + (i * BladeSkill.SIZE), BladeSkill.SIZE));

            FieldSkills = new BladeSkill[3];
            for (int i = 0; i < FieldSkills.Length; i++)
                FieldSkills[i] = new BladeSkill(data.GetByteSubArray(LOC["FieldSkills"] + (i * BladeSkill.SIZE), BladeSkill.SIZE));

            ExtraParts2 = data.GetByteSubArray(LOC["ExtraParts2"], 4);

            BArtsAchieve = new AchieveInfo[3];
            for (int i = 0; i < BArtsAchieve.Length; i++)
                BArtsAchieve[i] = new AchieveInfo(data.GetByteSubArray(LOC["BArtsAchieve"] + (i * AchieveInfo.SIZE), AchieveInfo.SIZE));

            BSkillsAchievement = new AchieveInfo[3];
            for (int i = 0; i < BSkillsAchievement.Length; i++)
                BSkillsAchievement[i] = new AchieveInfo(data.GetByteSubArray(LOC["BSkillsAchievement"] + (i * AchieveInfo.SIZE), AchieveInfo.SIZE));

            FSkillsAchievement = new AchieveInfo[3];
            for (int i = 0; i < FSkillsAchievement.Length; i++)
                FSkillsAchievement[i] = new AchieveInfo(data.GetByteSubArray(LOC["FSkillsAchievement"] + (i * AchieveInfo.SIZE), AchieveInfo.SIZE));

            KeyAchievement = new AchieveInfo(data.GetByteSubArray(LOC["KeyAchievement"], AchieveInfo.SIZE));
            KeyReleaseLevel = BitConverter.ToUInt16(data.GetByteSubArray(LOC["KeyReleaseLevel"], 2), 0);
            FavoriteCategory0 = BitConverter.ToUInt16(data.GetByteSubArray(LOC["FavoriteCategory0"], 2), 0);
            FavoriteItem0 = BitConverter.ToUInt16(data.GetByteSubArray(LOC["FavoriteItem0"], 2), 0);
            FavoriteCategory0Unlocked = BitConverter.ToUInt16(data.GetByteSubArray(LOC["FavoriteCategory0Unlocked"], 2), 0) == 1;
            FavoriteItem0Unlocked = BitConverter.ToUInt16(data.GetByteSubArray(LOC["FavoriteItem0Unlocked"], 2), 0) == 1;
            FavoriteCategory1 = BitConverter.ToUInt16(data.GetByteSubArray(LOC["FavoriteCategory1"], 2), 0);
            FavoriteItem1 = BitConverter.ToUInt16(data.GetByteSubArray(LOC["FavoriteItem1"], 2), 0);
            FavoriteCategory1Unlocked = BitConverter.ToUInt16(data.GetByteSubArray(LOC["FavoriteCategory1Unlocked"], 2), 0) == 1;
            FavoriteItem1Unlocked = BitConverter.ToUInt16(data.GetByteSubArray(LOC["FavoriteItem1Unlocked"], 2), 0) == 1;

            Unk_0x0676 = data.GetByteSubArray(LOC["Unk_0x0676"], 2);

            AuxCores = new UInt16[3];
            for (int i = 0; i < AuxCores.Length; i++)
                AuxCores[i] = BitConverter.ToUInt16(data.GetByteSubArray(LOC["AuxCores"] + (i * 2), 2), 0);

            Unk_0x067E = data.GetByteSubArray(LOC["Unk_0x067E"], 2);

            AuxCoreItemHandles = new ItemHandle32[3];
            for (int i = 0; i < AuxCoreItemHandles.Length; i++)
                AuxCoreItemHandles[i] = new ItemHandle32(data.GetByteSubArray(LOC["AuxCoreItemHandles"] + (i * ItemHandle32.SIZE), ItemHandle32.SIZE));

            

            Poppiswap = new PoppiSwapData(data.GetByteSubArray(LOC["HanaEquip"], PoppiSwapData.SIZE));
            Race = data[LOC["Race"]];
            Gender = data[LOC["Gender"]];
            Still = BitConverter.ToUInt16(data.GetByteSubArray(LOC["Still"], 2), 0);
            ModelResourceName = new PaddedString16(data.GetByteSubArray(LOC["ModelResourceName"], PaddedString16.SIZE));
            Model2Name = new PaddedString16(data.GetByteSubArray(LOC["Model2Name"], PaddedString16.SIZE));
            MotionResourceName = new PaddedString16(data.GetByteSubArray(LOC["MotionResourceName"], PaddedString16.SIZE));
            Motion2Name = new PaddedString16(data.GetByteSubArray(LOC["Motion2Name"], PaddedString16.SIZE));
            AddMotionName = new PaddedString16(data.GetByteSubArray(LOC["AddMotionName"], PaddedString16.SIZE));
            VoiceName = new PaddedString16(data.GetByteSubArray(LOC["VoiceName"], PaddedString16.SIZE));
            ClipEvent = data.GetByteSubArray(LOC["ClipEvent"], 0x14);
            Com_SE = new PaddedString16(data.GetByteSubArray(LOC["Com_SE"], PaddedString16.SIZE));
            EffectResourceName = new PaddedString16(data.GetByteSubArray(LOC["EffectResourceName"], PaddedString16.SIZE));
            Com_Vo = new PaddedString16(data.GetByteSubArray(LOC["Com_Vo"], PaddedString16.SIZE));
            CenterBone = new PaddedString16(data.GetByteSubArray(LOC["CenterBone"], PaddedString16.SIZE));
            CamBone = new PaddedString16(data.GetByteSubArray(LOC["CamBone"], PaddedString16.SIZE));
            SeResourceName = new PaddedString32(data.GetByteSubArray(LOC["SeResourceName"], PaddedString32.SIZE));
            BladeSize = data[LOC["BladeSize"]];
            WeaponType = data[LOC["WeaponType"]];
            OrbNum = data[LOC["OrbNum"]];
            Atr = data[LOC["Atr"]];
            Personality = data[LOC["Personality"]];
            ExtraParts = data[LOC["ExtraParts"]];
            EyeRot = data[LOC["EyeRot"]];
            Unk_0x0827 = data[LOC["Unk_0x0827"]];
            CoolTime = BitConverter.ToUInt16(data.GetByteSubArray(LOC["CoolTime"], 2), 0);
            Condition = BitConverter.ToUInt16(data.GetByteSubArray(LOC["Condition"], 2), 0);
            DefWeapon = BitConverter.ToUInt16(data.GetByteSubArray(LOC["DefWeapon"], 2), 0);
            ChestHeight = BitConverter.ToUInt16(data.GetByteSubArray(LOC["ChestHeight"], 2), 0);
            LandingHeight = BitConverter.ToUInt16(data.GetByteSubArray(LOC["LandingHeight"], 2), 0);
            RareNameId = BitConverter.ToUInt16(data.GetByteSubArray(LOC["RareNameId"], 2), 0);
            CommonNameId = BitConverter.ToUInt16(data.GetByteSubArray(LOC["CommonNameId"], 2), 0);
            Scale = BitConverter.ToUInt16(data.GetByteSubArray(LOC["Scale"], 2), 0);
            WpnScale = BitConverter.ToUInt16(data.GetByteSubArray(LOC["WpnScale"], 2), 0);
            OffsetID = BitConverter.ToUInt16(data.GetByteSubArray(LOC["OffsetID"], 2), 0);
            CollisionId = BitConverter.ToUInt16(data.GetByteSubArray(LOC["CollisionId"], 2), 0);
            PhysDef = data[LOC["PArmor"]];
            EtherDef = data[LOC["EArmor"]];
            MaxHPMod = data[LOC["HpMaxRev"]];
            StrengthMod = data[LOC["StrengthRev"]];
            EtherMod = data[LOC["PowEtherRev"]];
            DexterityMod = data[LOC["DexRev"]];
            AgilityMod = data[LOC["AgilityRev"]];
            LuckMod = data[LOC["LuckRev"]];
            QuestRace = data[LOC["QuestRace"]];
            ReleaseLock = data[LOC["ReleaseLock"]] == 1;
            FootStep = data[LOC["FootStep"]];
            FootStepEffect = data[LOC["FootStepEffect"]];
            KizunaLinkSet = BitConverter.ToUInt16(data.GetByteSubArray(LOC["KizunaLinkSet"], 2), 0);
            NormalTalk = BitConverter.ToUInt16(data.GetByteSubArray(LOC["NormalTalk"], 2), 0);
            Unk_0x084E = data.GetByteSubArray(LOC["Unk_0x084E"], 2);
            CreateEventID = BitConverter.ToUInt16(data.GetByteSubArray(LOC["CreateEventID"], 4), 0);
            MountPoint = BitConverter.ToUInt16(data.GetByteSubArray(LOC["MountPoint"], 2), 0);
            MountObject = BitConverter.ToUInt16(data.GetByteSubArray(LOC["MountObject"], 2), 0);
            Name = data.GetByteSubArray(LOC["Name"], 64); // max len 64 bytes, strange value bytes in padding after string
            NameLength = BitConverter.ToUInt16(data.GetByteSubArray(LOC["NameLength"], 4), 0);
            CommonBladeIndex = BitConverter.ToInt16(data.GetByteSubArray(LOC["CommonBladeIndex"], 2), 0);
            EnableEngageRex = data[LOC["EnableEngageRex"]] == 1;
            BladeReleaseStatus = data[LOC["BladeReleaseStatus"]];
            IsUnselect = data[LOC["isUnselect"]];
            AffinityChartStatus = data[LOC["AffinityChartStatus"]];
            Unk_0x08A2 = data.GetByteSubArray(LOC["Unk_0x08A2"], 2);
        }

        public Byte[] ToRawData()
        {
            List<Byte> result = new List<byte>();

            result.AddRange(BitConverter.GetBytes((UInt16)(IsEnabled ? 1 : 0)));
            result.AddRange(BitConverter.GetBytes(Creator));
            result.AddRange(BitConverter.GetBytes(SetDriver));
            result.AddRange(BitConverter.GetBytes(ID));
            result.AddRange(BornTime.ToRawData());
            result.AddRange(BitConverter.GetBytes(BraveryPoints));
            result.AddRange(BitConverter.GetBytes(BraveryLevel));
            result.AddRange(BitConverter.GetBytes(TruthPoints));
            result.AddRange(BitConverter.GetBytes(TruthLevel));
            result.AddRange(BitConverter.GetBytes(CompassionPoints));
            result.AddRange(BitConverter.GetBytes(CompassionLevel));
            result.AddRange(BitConverter.GetBytes(JusticePoints));
            result.AddRange(BitConverter.GetBytes(JusticeLevel));
            result.AddRange(Unk_0x002C);
            result.AddRange(BitConverter.GetBytes(TrustPoints));
            result.AddRange(BitConverter.GetBytes(TrustRank));

            foreach (BladeArt ba in BArts)
                result.AddRange(ba.ToRawData());

            foreach (BladeArt ba in NArts)
                result.AddRange(ba.ToRawData());

            result.AddRange(BitConverter.GetBytes(BArtExId));
            result.AddRange(BitConverter.GetBytes(BladeArtsExH));
            result.AddRange(Unk_0x0088);
            result.AddRange(BitConverter.GetBytes(BArtEx2Id));
            result.AddRange(BitConverter.GetBytes(BladeArtsEx2H));
            result.AddRange(Unk_0x0094);

            foreach (BladeSkill bs in BattleSkills)
                result.AddRange(bs.ToRawData());

            foreach (BladeSkill bs in FieldSkills)
                result.AddRange(bs.ToRawData());

            result.AddRange(ExtraParts2);

            foreach (AchieveInfo ai in BArtsAchieve)
                result.AddRange(ai.ToRawData());

            foreach (AchieveInfo ai in BSkillsAchievement)
                result.AddRange(ai.ToRawData());

            foreach (AchieveInfo ai in FSkillsAchievement)
                result.AddRange(ai.ToRawData());

            result.AddRange(KeyAchievement.ToRawData());
            result.AddRange(BitConverter.GetBytes(KeyReleaseLevel));
            result.AddRange(BitConverter.GetBytes(FavoriteCategory0));
            result.AddRange(BitConverter.GetBytes(FavoriteItem0));
            result.AddRange(BitConverter.GetBytes((UInt16)(FavoriteCategory0Unlocked ? 1 : 0)));
            result.AddRange(BitConverter.GetBytes((UInt16)(FavoriteItem0Unlocked ? 1 : 0)));
            result.AddRange(BitConverter.GetBytes(FavoriteCategory1));
            result.AddRange(BitConverter.GetBytes(FavoriteItem1));
            result.AddRange(BitConverter.GetBytes((UInt16)(FavoriteCategory1Unlocked ? 1 : 0)));
            result.AddRange(BitConverter.GetBytes((UInt16)(FavoriteItem1Unlocked ? 1 : 0)));
            result.AddRange(Unk_0x0676);

            foreach (UInt16 u in AuxCores)
                result.AddRange(BitConverter.GetBytes(u));

            result.AddRange(Unk_0x067E);

            foreach (ItemHandle32 ih in AuxCoreItemHandles)
                result.AddRange(ih.ToRawData());

            result.AddRange(Poppiswap.ToRawData());
            result.Add(Race);
            result.Add(Gender);
            result.AddRange(BitConverter.GetBytes(Still));
            result.AddRange(ModelResourceName.ToRawData());
            result.AddRange(Model2Name.ToRawData());
            result.AddRange(MotionResourceName.ToRawData());
            result.AddRange(Motion2Name.ToRawData());
            result.AddRange(AddMotionName.ToRawData());
            result.AddRange(VoiceName.ToRawData());
            result.AddRange(ClipEvent);
            result.AddRange(Com_SE.ToRawData());
            result.AddRange(EffectResourceName.ToRawData());
            result.AddRange(Com_Vo.ToRawData());
            result.AddRange(CenterBone.ToRawData());
            result.AddRange(CamBone.ToRawData());
            result.AddRange(SeResourceName.ToRawData());
            result.Add(BladeSize);
            result.Add(WeaponType);
            result.Add(OrbNum);
            result.Add(Atr);
            result.Add(Personality);
            result.Add(ExtraParts);
            result.Add(EyeRot);
            result.Add(Unk_0x0827);
            result.AddRange(BitConverter.GetBytes(CoolTime));
            result.AddRange(BitConverter.GetBytes(Condition));
            result.AddRange(BitConverter.GetBytes(DefWeapon));
            result.AddRange(BitConverter.GetBytes(ChestHeight));
            result.AddRange(BitConverter.GetBytes(LandingHeight));
            result.AddRange(BitConverter.GetBytes(RareNameId));
            result.AddRange(BitConverter.GetBytes(CommonNameId));
            result.AddRange(BitConverter.GetBytes(Scale));
            result.AddRange(BitConverter.GetBytes(WpnScale));
            result.AddRange(BitConverter.GetBytes(OffsetID));
            result.AddRange(BitConverter.GetBytes(CollisionId));
            result.Add(PhysDef);
            result.Add(EtherDef);
            result.Add(MaxHPMod);
            result.Add(StrengthMod);
            result.Add(EtherMod);
            result.Add(DexterityMod);
            result.Add(AgilityMod);
            result.Add(LuckMod);
            result.Add(QuestRace);
            result.Add((Byte)(ReleaseLock ? 1 : 0));
            result.Add(FootStep);
            result.Add(FootStepEffect);
            result.AddRange(BitConverter.GetBytes(KizunaLinkSet));
            result.AddRange(BitConverter.GetBytes(NormalTalk));
            result.AddRange(Unk_0x084E);
            result.AddRange(BitConverter.GetBytes(CreateEventID));
            result.AddRange(BitConverter.GetBytes(MountPoint));
            result.AddRange(BitConverter.GetBytes(MountObject));
            result.AddRange(Name);
            result.AddRange(BitConverter.GetBytes(NameLength));
            result.AddRange(BitConverter.GetBytes(CommonBladeIndex));
            result.Add((Byte)(EnableEngageRex ? 1 : 0));
            result.Add(BladeReleaseStatus);
            result.Add(IsUnselect);
            result.Add(AffinityChartStatus);
            result.AddRange(Unk_0x08A2);

            if (result.Count != SIZE)
            {
                string message = "Blade: SIZE ALL WRONG!!!" + Environment.NewLine +
                    "Size should be " + SIZE + " bytes..." + Environment.NewLine +
                    "...but Size is " + result.Count + " bytes!";

                throw new Exception(message);
            }

            return result.ToArray();
        }

        public override string ToString()
        {
            string name = "";

            if (CommonNameId > 0)
                name = (string)XC2Data.BladeCommonNames().Select("ID = " + CommonNameId)[0]["Name"];

            if (RareNameId > 0)
                name = (string)XC2Data.BladeRareNames().Select("ID = " + RareNameId)[0]["Name"];

            if (name == "")
                name = Encoding.UTF8.GetString(Name.GetByteSubArray(0, (int)NameLength));

            return ID + ": " + name;
        }

        public Byte[] ExportAffinityChart()
        {
            
            List<Byte> result = new List<Byte>();

            foreach (AchieveInfo ai in BArtsAchieve)
                result.AddRange(ai.ToRawData());

            foreach (AchieveInfo ai in BSkillsAchievement)
                result.AddRange(ai.ToRawData());

            foreach (AchieveInfo ai in FSkillsAchievement)
                result.AddRange(ai.ToRawData());

            result.AddRange(KeyAchievement.ToRawData());

            if (result.Count != AFF_CHART_SIZE)
            {
                string message = "EXPORT Blade Affinity Chart: SIZE ALL WRONG!!!" + Environment.NewLine +
                    "Size should be " + AFF_CHART_SIZE + " bytes..." + Environment.NewLine +
                    "...but Size is " + result.Count + " bytes!";

                throw new Exception(message);
            }

            return result.ToArray();
        }

        public void ImportAffinityChart(Byte[] data)
        {
            if (data.Length != AFF_CHART_SIZE)
            {
                string message = "IMPORT Blade Affinity Chart: SIZE ALL WRONG!!!" + Environment.NewLine +
                    "Size should be " + AFF_CHART_SIZE + " bytes..." + Environment.NewLine +
                    "...but Size is " + data.Length + " bytes!";

                throw new Exception(message);
            }
            else
            {
                BArtsAchieve = new AchieveInfo[3];
                for (int i = 0; i < BArtsAchieve.Length; i++)
                    BArtsAchieve[i] = new AchieveInfo(data.GetByteSubArray(0x0 + (i * AchieveInfo.SIZE), AchieveInfo.SIZE));

                BSkillsAchievement = new AchieveInfo[3];
                for (int i = 0; i < BSkillsAchievement.Length; i++)
                    BSkillsAchievement[i] = new AchieveInfo(data.GetByteSubArray((0x90 * 3) + (i * AchieveInfo.SIZE), AchieveInfo.SIZE));

                FSkillsAchievement = new AchieveInfo[3];
                for (int i = 0; i < FSkillsAchievement.Length; i++)
                    FSkillsAchievement[i] = new AchieveInfo(data.GetByteSubArray((0x90 * 6) + (i * AchieveInfo.SIZE), AchieveInfo.SIZE));

                KeyAchievement = new AchieveInfo(data.GetByteSubArray((0x90 * 9), AchieveInfo.SIZE));
            }

        }
    }
}
