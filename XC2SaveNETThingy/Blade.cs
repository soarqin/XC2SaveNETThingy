using System;
using System.Collections.Generic;
using System.Text;

namespace XC2SaveNETThingy
{
    public class BladeArt : IXC2SaveObject
    {
        public const int SIZE = 0xC;

        public UInt16 ID { get; set; }
        public UInt16 RecastRev { get; set; }
        public Byte[] Unk_0x04 { get; set; }
        public Byte Level { get; set; }
        public Byte MaxLevel { get; set; }
        public Byte[] Unk_0x09 { get; set; }

        public BladeArt(Byte[] data)
        {
            ID = BitConverter.ToUInt16(data.GetByteSubArray(0x0, 2), 0);
            RecastRev = BitConverter.ToUInt16(data.GetByteSubArray(0x2, 2), 0);
            Unk_0x04 = data.GetByteSubArray(0x4, 3);
            Level = data[0x7];
            MaxLevel = data[0x8];
            Unk_0x09 = data.GetByteSubArray(0x9, 3);
        }

        public Byte[] ToRawData()
        {
            List<Byte> result = new List<Byte>();

            result.AddRange(BitConverter.GetBytes(ID));
            result.AddRange(BitConverter.GetBytes(RecastRev));
            result.AddRange(Unk_0x04);
            result.Add(Level);
            result.Add(MaxLevel);
            result.AddRange(Unk_0x09);

            if (result.Count != SIZE)
            {
                string message = "BladeArts: SIZE ALL WRONG!!!" + Environment.NewLine +
                "Size should be " + SIZE + " bytes..." + Environment.NewLine +
                "...but Size is " + result.Count + " bytes!";

                throw new Exception(message);
            }

            return result.ToArray();
        }
    }

    public class BladeSkill : IXC2SaveObject
    {
        public const int SIZE = 0x6;

        public UInt16 ID { get; set; }
        public Byte Unk_0x02 { get; set; }
        public Byte Level { get; set; }
        public Byte MaxLevel { get; set; }
        public Byte Unk_0x05 { get; set; }

        public BladeSkill(Byte[] data)
        {
            ID = BitConverter.ToUInt16(data.GetByteSubArray(0, 2), 0);
            Unk_0x02 = data[2];
            Level = data[3];
            MaxLevel = data[4];
            Unk_0x05 = data[5];
        }

        public Byte[] ToRawData()
        {
            List<Byte> result = new List<byte>();

            result.AddRange(BitConverter.GetBytes(ID));
            result.Add(Unk_0x02);
            result.Add(Level);
            result.Add(MaxLevel);
            result.Add(Unk_0x05);

            return result.ToArray();
        }
    }

    public class AchieveQuest : IXC2SaveObject
    {
        public const int SIZE = 0x1C;

        public UInt16 QuestID { get; set; }
        public UInt16 field_2 { get; set; }
        public UInt32 Count { get; set; }
        public UInt32 MaxCount { get; set; }
        public UInt16 field_C { get; set; }
        public UInt16 StatsID { get; set; }
        public UInt16 TaskType { get; set; }
        public UInt16 Column { get; set; }
        public UInt16 Row { get; set; }
        public UInt16 BladeId { get; set; }
        public UInt16 AchievementId { get; set; }
        public UInt16 Alignment { get; set; }

        public AchieveQuest(Byte[] data)
        {
            QuestID = BitConverter.ToUInt16(data.GetByteSubArray(0x0, 2), 0);
            field_2 = BitConverter.ToUInt16(data.GetByteSubArray(0x2, 2), 0);
            Count = BitConverter.ToUInt32(data.GetByteSubArray(0x4, 4), 0);
            MaxCount = BitConverter.ToUInt32(data.GetByteSubArray(0x8, 4), 0);
            field_C = BitConverter.ToUInt16(data.GetByteSubArray(0xC, 2), 0);
            StatsID = BitConverter.ToUInt16(data.GetByteSubArray(0xE, 2), 0);
            TaskType = BitConverter.ToUInt16(data.GetByteSubArray(0x10, 2), 0);
            Column = BitConverter.ToUInt16(data.GetByteSubArray(0x12, 2), 0);
            Row = BitConverter.ToUInt16(data.GetByteSubArray(0x14, 2), 0);
            BladeId = BitConverter.ToUInt16(data.GetByteSubArray(0x16, 2), 0);
            AchievementId = BitConverter.ToUInt16(data.GetByteSubArray(0x18, 2), 0);
            Alignment = BitConverter.ToUInt16(data.GetByteSubArray(0x1A, 2), 0);
        }

        public Byte[] ToRawData()
        {
            List<Byte> result = new List<byte>();

            result.AddRange(BitConverter.GetBytes(QuestID));
            result.AddRange(BitConverter.GetBytes(field_2));
            result.AddRange(BitConverter.GetBytes(Count));
            result.AddRange(BitConverter.GetBytes(MaxCount));
            result.AddRange(BitConverter.GetBytes(field_C));
            result.AddRange(BitConverter.GetBytes(StatsID));
            result.AddRange(BitConverter.GetBytes(TaskType));
            result.AddRange(BitConverter.GetBytes(Column));
            result.AddRange(BitConverter.GetBytes(Row));
            result.AddRange(BitConverter.GetBytes(BladeId));
            result.AddRange(BitConverter.GetBytes(AchievementId));
            result.AddRange(BitConverter.GetBytes(Alignment));

            if (result.Count != SIZE)
            {
                string message = "AchieveQuest: SIZE ALL WRONG!!!" + Environment.NewLine +
                "Size should be " + SIZE + " bytes..." + Environment.NewLine +
                "...but Size is " + result.Count + " bytes!";

                throw new Exception(message);
            }

            return result.ToArray();
        }
    }

    public class AchieveInfo : IXC2SaveObject
    {
        public const int SIZE = 0x90;

        public UInt16 Id { get; set; }
        public UInt16 Alignment { get; set; }
        public AchieveQuest[] AchieveQuests { get; set; }

        public AchieveInfo(Byte[] data)
        {
            Id = BitConverter.ToUInt16(data.GetByteSubArray(0, 2), 0);
            Alignment = BitConverter.ToUInt16(data.GetByteSubArray(2, 2), 0);

            AchieveQuests = new AchieveQuest[5];
            for (int i = 0; i < AchieveQuests.Length; i++)
                AchieveQuests[i] = new AchieveQuest(data.GetByteSubArray(4 + (i * AchieveQuest.SIZE), AchieveQuest.SIZE));
        }

        public Byte[] ToRawData()
        {
            List<Byte> result = new List<Byte>();

            result.AddRange(BitConverter.GetBytes(Id));
            result.AddRange(BitConverter.GetBytes(Alignment));
            
            foreach (AchieveQuest aq in AchieveQuests)
                result.AddRange(aq.ToRawData());

            if (result.Count != SIZE)
            {
                string message = "AchieveInfo: SIZE ALL WRONG!!!" + Environment.NewLine +
                "Size should be " + SIZE + " bytes..." + Environment.NewLine +
                "...but Size is " + result.Count + " bytes!";

                throw new Exception(message);
            }

            return result.ToArray();
        }
    }

    public class ArtsEnhance : IXC2SaveObject
    {
        public const int SIZE = 0xC;

        private static readonly Dictionary<UInt16, string> NAMES = new Dictionary<UInt16, string>()
        {
            {0, "(none)" },
        };

        public UInt16 EnhanceID { get; set; }
        public UInt16 RecastRev { get; set; }
        public UInt16 ItemID { get; set; }
        public UInt16 Unk_0x06 { get; set; }
        public ItemHandle16 Unk_0x08 { get; set; }
        public ItemHandle16 Unk_0x0A { get; set; }

        public string Name
        {
            get
            {
                if (NAMES.ContainsKey(EnhanceID))
                    return EnhanceID.ToString() + ": " + NAMES[EnhanceID];
                else
                    return EnhanceID.ToString();
            }
        }

        public ArtsEnhance(Byte[] data)
        {
            EnhanceID = BitConverter.ToUInt16(data.GetByteSubArray(0x0, 2), 0);
            RecastRev = BitConverter.ToUInt16(data.GetByteSubArray(0x2, 2), 0);
            ItemID = BitConverter.ToUInt16(data.GetByteSubArray(0x4, 2), 0);
            Unk_0x06 = BitConverter.ToUInt16(data.GetByteSubArray(0x6, 2), 0);
            Unk_0x08 = new ItemHandle16(data.GetByteSubArray(0x8, ItemHandle16.SIZE));
            Unk_0x0A = new ItemHandle16(data.GetByteSubArray(0xA, ItemHandle16.SIZE));
        }

        public Byte[] ToRawData()
        {
            List<Byte> result = new List<Byte>();

            result.AddRange(BitConverter.GetBytes(EnhanceID));
            result.AddRange(BitConverter.GetBytes(RecastRev));
            result.AddRange(BitConverter.GetBytes(ItemID));
            result.AddRange(BitConverter.GetBytes(Unk_0x06));
            result.AddRange(Unk_0x08.ToRawData());
            result.AddRange(Unk_0x0A.ToRawData());

            if (result.Count != SIZE)
            {
                string message = "ArtsEnhance: SIZE ALL WRONG!!!" + Environment.NewLine +
                "Size should be " + SIZE + " bytes..." + Environment.NewLine +
                "...but Size is " + result.Count + " bytes!";

                throw new Exception(message);
            }

            return result.ToArray();
        }

        public override string ToString()
        {
            return Name;
        }
    };

    public class HanaCircuits : IXC2SaveObject
    {
        public const int SIZE = 0x2;
        //uint16 Specials0 : 1;
        //uint16 Specials1 : 1;
        //uint16 Specials2 : 1;
        //uint16 Skill0 : 1;
        //uint16 Skill1 : 1;
        //uint16 Skill2 : 1;

        public bool Specials0_Enabled { get; set; }
        public bool Specials1_Enabled { get; set; }
        public bool Specials2_Enabled { get; set; }
        public bool Skill0_Enabled { get; set; }
        public bool Skill1_Enabled { get; set; }
        public bool Skill2_Enabled { get; set; }

        public HanaCircuits(byte[] data)
        {
            Specials0_Enabled = (data[0] & 1) == 1;
            Specials1_Enabled = ((data[0] >> 1) & 1) == 1;
            Specials2_Enabled = ((data[0] >> 2) & 1) == 1;
            Skill0_Enabled = ((data[0] >> 3) & 1) == 1;
            Skill1_Enabled = ((data[0] >> 4) & 1) == 1;
            Skill2_Enabled = ((data[0] >> 5) & 1) == 1;
        }

        public Byte[] ToRawData()
        {
            Byte data = 0;
            data += (Byte)(Specials0_Enabled ? 1 : 0);
            data += (Byte)((Specials1_Enabled ? 1 : 0) << 1);
            data += (Byte)((Specials2_Enabled ? 1 : 0) << 2);
            data += (Byte)((Skill0_Enabled ? 1 : 0) << 3);
            data += (Byte)((Skill1_Enabled ? 1 : 0) << 4);
            data += (Byte)((Skill2_Enabled ? 1 : 0) << 5);

            return new Byte[] { data, 0x00 };
        }
    };

    public class PoppiSwapData : IXC2SaveObject
    {
        public const int SIZE = 0x7c;

        private static readonly Dictionary<string, int> LOC = new Dictionary<string, int>()
        {
            { "RoleItem", 0x00 },
            { "AtrItem", 0x02 },
            { "BArtsEnhance", 0x04 },
            { "NArts", 0x28 },
            { "Skills", 0x4C },
            { "field_58", 0x58 },
            { "PowerCapacity", 0x78 },
            { "OpenCircuits", 0x7A }
        };

        public UInt16 RoleItem { get; set; }
        public UInt16 AtrItem { get; set; }
        public ArtsEnhance[] BArtsEnhance { get; set; }
        public ArtsEnhance[] NArts { get; set; }
        public UInt16[] Skills { get; set; }
        public ItemHandle32[] field_58 { get; set; }
        public UInt16 PowerCapacity { get; set; }
        public HanaCircuits OpenCircuits { get; set; }

        public PoppiSwapData(Byte[] data)
        {
            RoleItem = BitConverter.ToUInt16(data.GetByteSubArray(LOC["RoleItem"], 2), 0);
            AtrItem = BitConverter.ToUInt16(data.GetByteSubArray(LOC["AtrItem"], 2), 0);

            BArtsEnhance = new ArtsEnhance[3];
            for (int i = 0; i < BArtsEnhance.Length; i++)
                BArtsEnhance[i] = new ArtsEnhance(data.GetByteSubArray(LOC["BArtsEnhance"] + (i * ArtsEnhance.SIZE), ArtsEnhance.SIZE));

            NArts = new ArtsEnhance[3];
            for (int i = 0; i < NArts.Length; i++)
                NArts[i] = new ArtsEnhance(data.GetByteSubArray(LOC["NArts"] + (i * ArtsEnhance.SIZE), ArtsEnhance.SIZE));

            Skills = new UInt16[6];
            for (int i = 0; i < Skills.Length; i++)
                Skills[i] = BitConverter.ToUInt16(data.GetByteSubArray(LOC["Skills"] + (i * 2), 2), 0);

            field_58 = new ItemHandle32[8];
            for (int i = 0; i < field_58.Length; i++)
                field_58[i] = new ItemHandle32(data.GetByteSubArray(LOC["field_58"] + (i * ItemHandle32.SIZE), ItemHandle32.SIZE));

            PowerCapacity = BitConverter.ToUInt16(data.GetByteSubArray(LOC["PowerCapacity"], 2), 0);
            OpenCircuits = new HanaCircuits(data.GetByteSubArray(LOC["OpenCircuits"], HanaCircuits.SIZE));
        }

        public Byte[] ToRawData()
        {
            List<Byte> result = new List<byte>();

            result.AddRange(BitConverter.GetBytes(RoleItem));
            result.AddRange(BitConverter.GetBytes(AtrItem));

            foreach (ArtsEnhance ae in BArtsEnhance)
                result.AddRange(ae.ToRawData());

            foreach (ArtsEnhance ae in NArts)
                result.AddRange(ae.ToRawData());

            foreach (UInt16 u in Skills)
                result.AddRange(BitConverter.GetBytes(u));

            foreach (ItemHandle32 ih in field_58)
                result.AddRange(ih.ToRawData());

            result.AddRange(BitConverter.GetBytes(PowerCapacity));
            result.AddRange(OpenCircuits.ToRawData());

            if (result.Count != SIZE)
            {
                string message = "Blade.Poppiswap: SIZE ALL WRONG!!!" + Environment.NewLine +
                    "Size should be " + SIZE + " bytes..." + Environment.NewLine +
                    "...but Size is " + result.Count + " bytes!";

                throw new Exception(message);
            }

            return result.ToArray();
        }
    }

    public class Blade : IXC2SaveObject
    {
        public const int SIZE = 0x8A4;

        public UInt16 DataType { get; set; }
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
        public UInt32 Unk_0x2C { get; set; }
        public Byte Unk_0x30 { get; set; }
        public Byte[] gap_31 { get; set; }
        public UInt32 TrustPoints { get; set; }
        public UInt32 TrustRank { get; set; }
        public BladeArt[] BArts { get; set; }
        public BladeArt[] NArts { get; set; }
        public UInt16 BArtExId { get; set; }
        public UInt16 BladeArtsExH { get; set; }
        public UInt16 field_88 { get; set; }
        public UInt16 field_8A { get; set; }
        public Byte field_8C { get; set; }
        public Byte gap_8D { get; set; }
        public UInt16 field_8E { get; set; }
        public UInt16 BArtEx2Id { get; set; }
        public UInt16 BladeArtsEx2H { get; set; }
        public UInt16 field_94 { get; set; }
        public UInt16 field_96 { get; set; }
        public Byte field_98 { get; set; }
        public Byte gap_99 { get; set; }
        public UInt16 field_9A { get; set; }
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
        public Byte[] gap676 { get; set; }
        public UInt32 field_678 { get; set; }
        public UInt16 field_67C { get; set; }
        public Byte[] gap67E { get; set; }
        public UInt64 field_680 { get; set; }
        public UInt32 field_688 { get; set; }
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
        //public PaddedString16 ClipEvent { get; set; }
        public Byte[] ClipEvent { get; set; }
        //public PaddedString16 Com_SE { get; set; }
        public Byte[] Com_SE { get; set; }
        public PaddedString16 EffectResourceName { get; set; }
        //public PaddedString16 Com_Vo { get; set; }
        public Byte[] Com_Vo { get; set; }
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
        public Byte gap_827 { get; set; }
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
        public Byte PArmor { get; set; }
        public Byte EArmor { get; set; }
        public Byte HpMaxRev { get; set; }
        public Byte StrengthRev { get; set; }
        public Byte PowEtherRev { get; set; }
        public Byte DexRev { get; set; }
        public Byte AgilityRev { get; set; }
        public Byte LuckRev { get; set; }
        public Byte QuestRace { get; set; }
        public Byte ReleaseLock { get; set; }
        public Byte FootStep { get; set; }
        public Byte FootStepEffect { get; set; }
        public UInt16 KizunaLinkSet { get; set; }
        public UInt16 NormalTalk { get; set; }
        public UInt16 gap_84E { get; set; }
        public UInt32 CreateEventID { get; set; }
        public UInt16 MountPoint { get; set; }
        public UInt16 MountObject { get; set; }
        public Byte[] Name { get; set; } // max len 64 bytes, strange garbage bytes after string
        public UInt32 NameLength { get; set; }
        public Int16 CommonBladeIndex { get; set; }
        public Byte EnableEngageRex { get; set; }
        public Byte BladeReleaseStatus { get; set; }
        public Byte isUnselect { get; set; }
        public Byte AffinityChartStatus { get; set; }
        public UInt16 field_8A2 { get; set; }

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
            { "field_2C", 0x2C },
            { "field_30", 0x30 },
            { "gap_31", 0x31 },
            { "TrustPoints", 0x34 },
            { "TrustRank", 0x38 },
            { "BArts", 0x3C },
            { "NArts", 0x60 },
            { "BArtExId", 0x84 },
            { "BladeArtsExH", 0x86 },
            { "field_88", 0x88 },
            { "field_8A", 0x8A },
            { "field_8C", 0x8C },
            { "gap_8D", 0x8D },
            { "field_8E", 0x8E },
            { "BArtEx2Id", 0x90 },
            { "BladeArtsEx2H", 0x92 },
            { "field_94", 0x94 },
            { "field_96", 0x96 },
            { "field_98", 0x98 },
            { "gap_99", 0x99 },
            { "field_9A", 0x9A },
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
            { "gap676", 0x676 },
            { "field_678", 0x678 },
            { "field_67C", 0x67C },
            { "gap67E", 0x67E },
            { "field_680", 0x680 },
            { "field_688", 0x688 },
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
            { "gap_827", 0x827 },
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
            { "gap_84E", 0x84E },
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
            { "field_8A2", 0x8A2 },
        };

        public Blade(Byte[] data)
        {
            DataType = BitConverter.ToUInt16(data.GetByteSubArray(LOC["DataType"], 2), 0);
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
            Unk_0x2C = BitConverter.ToUInt32(data.GetByteSubArray(LOC["field_2C"], 4), 0);
            Unk_0x30 = data[LOC["field_30"]];
            gap_31 = data.GetByteSubArray(LOC["gap_31"], 3);
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
            field_88 = BitConverter.ToUInt16(data.GetByteSubArray(LOC["field_88"], 2), 0);
            field_8A = BitConverter.ToUInt16(data.GetByteSubArray(LOC["field_8A"], 2), 0);
            field_8C = data[LOC["field_8C"]];
            gap_8D = data[LOC["gap_8D"]];
            field_8E = BitConverter.ToUInt16(data.GetByteSubArray(LOC["field_8E"], 2), 0);
            BArtEx2Id = BitConverter.ToUInt16(data.GetByteSubArray(LOC["BArtEx2Id"], 2), 0);
            BladeArtsEx2H = BitConverter.ToUInt16(data.GetByteSubArray(LOC["BladeArtsEx2H"], 2), 0);
            field_94 = BitConverter.ToUInt16(data.GetByteSubArray(LOC["field_94"], 2), 0);
            field_96 = BitConverter.ToUInt16(data.GetByteSubArray(LOC["field_96"], 2), 0);
            field_98 = data[LOC["field_98"]];
            gap_99 = data[LOC["gap_99"]];
            field_9A = BitConverter.ToUInt16(data.GetByteSubArray(LOC["field_9A"], 2), 0);

            BattleSkills = new BladeSkill[3];
            for (int i = 0; i < BattleSkills.Length; i++)
                BattleSkills[i] = new BladeSkill(data.GetByteSubArray(LOC["BladeSkills"] + (i * BladeSkill.SIZE), BladeSkill.SIZE));

            FieldSkills = new BladeSkill[3];
            for (int i = 0; i < FieldSkills.Length; i++)
                FieldSkills[i] = new BladeSkill(data.GetByteSubArray(LOC["FieldSkills"] + (i * BladeSkill.SIZE), BladeSkill.SIZE));

            ExtraParts2 = new Byte[4];
            for (int i = 0; i < ExtraParts2.Length; i++)
                ExtraParts2[i] = data[LOC["ExtraParts2"] + i];

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
            //FavoriteCategory0Unlocked = BitConverter.ToUInt16(data.GetByteSubArray(LOC["FavoriteCategory0Unlocked"], 2), 0);
            //FavoriteItem0Unlocked = BitConverter.ToUInt16(data.GetByteSubArray(LOC["FavoriteItem0Unlocked"], 2), 0);

            FavoriteCategory0Unlocked = BitConverter.ToUInt16(data.GetByteSubArray(LOC["FavoriteCategory0Unlocked"], 2), 0) == 1;
            FavoriteItem0Unlocked = BitConverter.ToUInt16(data.GetByteSubArray(LOC["FavoriteItem0Unlocked"], 2), 0) == 1;

            FavoriteCategory1 = BitConverter.ToUInt16(data.GetByteSubArray(LOC["FavoriteCategory1"], 2), 0);
            FavoriteItem1 = BitConverter.ToUInt16(data.GetByteSubArray(LOC["FavoriteItem1"], 2), 0);
            //FavoriteCategory1Unlocked = BitConverter.ToUInt16(data.GetByteSubArray(LOC["FavoriteCategory1Unlocked"], 2), 0);
            //FavoriteItem1Unlocked = BitConverter.ToUInt16(data.GetByteSubArray(LOC["FavoriteItem1Unlocked"], 2), 0);

            FavoriteCategory1Unlocked = BitConverter.ToUInt16(data.GetByteSubArray(LOC["FavoriteCategory1Unlocked"], 2), 0) == 1;
            FavoriteItem1Unlocked = BitConverter.ToUInt16(data.GetByteSubArray(LOC["FavoriteItem1Unlocked"], 2), 0) == 1;
            gap676 = data.GetByteSubArray(LOC["gap676"], 2);
            field_678 = BitConverter.ToUInt32(data.GetByteSubArray(LOC["field_678"], 4), 0);
            field_67C = BitConverter.ToUInt16(data.GetByteSubArray(LOC["field_67C"], 2), 0);
            gap67E = data.GetByteSubArray(LOC["gap67E"], 2);
            field_680 = BitConverter.ToUInt64(data.GetByteSubArray(LOC["field_680"], 8), 0);
            field_688 = BitConverter.ToUInt32(data.GetByteSubArray(LOC["field_688"], 4), 0);
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

            //ClipEvent = new PaddedString16(data.GetByteSubArray(LOC["ClipEvent"], PaddedString16.SIZE));
            ClipEvent = data.GetByteSubArray(LOC["ClipEvent"], 0x14);

            //Com_SE = new PaddedString16(data.GetByteSubArray(LOC["Com_SE"], PaddedString16.SIZE));
            Com_SE = data.GetByteSubArray(LOC["Com_SE"], 0x14);

            EffectResourceName = new PaddedString16(data.GetByteSubArray(LOC["EffectResourceName"], PaddedString16.SIZE));

            //Com_Vo = new PaddedString16(data.GetByteSubArray(LOC["Com_Vo"], PaddedString16.SIZE));
            Com_Vo = data.GetByteSubArray(LOC["Com_Vo"], 0x14);

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
            gap_827 = data[LOC["gap_827"]];
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
            PArmor = data[LOC["PArmor"]];
            EArmor = data[LOC["EArmor"]];
            HpMaxRev = data[LOC["HpMaxRev"]];
            StrengthRev = data[LOC["StrengthRev"]];
            PowEtherRev = data[LOC["PowEtherRev"]];
            DexRev = data[LOC["DexRev"]];
            AgilityRev = data[LOC["AgilityRev"]];
            LuckRev = data[LOC["LuckRev"]];
            QuestRace = data[LOC["QuestRace"]];
            ReleaseLock = data[LOC["ReleaseLock"]];
            FootStep = data[LOC["FootStep"]];
            FootStepEffect = data[LOC["FootStepEffect"]];
            KizunaLinkSet = BitConverter.ToUInt16(data.GetByteSubArray(LOC["KizunaLinkSet"], 2), 0);
            NormalTalk = BitConverter.ToUInt16(data.GetByteSubArray(LOC["NormalTalk"], 2), 0);
            gap_84E = BitConverter.ToUInt16(data.GetByteSubArray(LOC["gap_84E"], 2), 0);
            CreateEventID = BitConverter.ToUInt16(data.GetByteSubArray(LOC["CreateEventID"], 4), 0);
            MountPoint = BitConverter.ToUInt16(data.GetByteSubArray(LOC["MountPoint"], 2), 0);
            MountObject = BitConverter.ToUInt16(data.GetByteSubArray(LOC["MountObject"], 2), 0);
            Name = data.GetByteSubArray(LOC["Name"], 64); // max len 64 bytes, strange value bytes in padding after string
            NameLength = BitConverter.ToUInt16(data.GetByteSubArray(LOC["NameLength"], 4), 0);
            CommonBladeIndex = BitConverter.ToInt16(data.GetByteSubArray(LOC["CommonBladeIndex"], 2), 0);
            EnableEngageRex = data[LOC["EnableEngageRex"]];
            BladeReleaseStatus = data[LOC["BladeReleaseStatus"]];
            isUnselect = data[LOC["isUnselect"]];
            AffinityChartStatus = data[LOC["AffinityChartStatus"]];
            field_8A2 = BitConverter.ToUInt16(data.GetByteSubArray(LOC["field_8A2"], 2), 0);
        }

        public Byte[] ToRawData()
        {
            List<Byte> result = new List<byte>();

            result.AddRange(BitConverter.GetBytes(DataType));
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
            result.AddRange(BitConverter.GetBytes(Unk_0x2C));
            result.Add(Unk_0x30);
            result.AddRange(gap_31);
            result.AddRange(BitConverter.GetBytes(TrustPoints));
            result.AddRange(BitConverter.GetBytes(TrustRank));

            foreach (BladeArt ba in BArts)
                result.AddRange(ba.ToRawData());

            foreach (BladeArt ba in NArts)
                result.AddRange(ba.ToRawData());

            result.AddRange(BitConverter.GetBytes(BArtExId));
            result.AddRange(BitConverter.GetBytes(BladeArtsExH));
            result.AddRange(BitConverter.GetBytes(field_88));
            result.AddRange(BitConverter.GetBytes(field_8A));
            result.Add(field_8C);
            result.Add(gap_8D);
            result.AddRange(BitConverter.GetBytes(field_8E));
            result.AddRange(BitConverter.GetBytes(BArtEx2Id));
            result.AddRange(BitConverter.GetBytes(BladeArtsEx2H));
            result.AddRange(BitConverter.GetBytes(field_94));
            result.AddRange(BitConverter.GetBytes(field_96));
            result.Add(field_98);
            result.Add(gap_99);
            result.AddRange(BitConverter.GetBytes(field_9A));

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
            result.AddRange(gap676);
            result.AddRange(BitConverter.GetBytes(field_678));
            result.AddRange(BitConverter.GetBytes(field_67C));
            result.AddRange(gap67E);
            result.AddRange(BitConverter.GetBytes(field_680));
            result.AddRange(BitConverter.GetBytes(field_688));
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
            //result.AddRange(ClipEvent.ToRawData());
            result.AddRange(ClipEvent);
            //result.AddRange(Com_SE.ToRawData());
            result.AddRange(Com_SE);
            result.AddRange(EffectResourceName.ToRawData());
            //result.AddRange(Com_Vo.ToRawData());
            result.AddRange(Com_Vo);
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
            result.Add(gap_827);
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
            result.Add(PArmor);
            result.Add(EArmor);
            result.Add(HpMaxRev);
            result.Add(StrengthRev);
            result.Add(PowEtherRev);
            result.Add(DexRev);
            result.Add(AgilityRev);
            result.Add(LuckRev);
            result.Add(QuestRace);
            result.Add(ReleaseLock);
            result.Add(FootStep);
            result.Add(FootStepEffect);
            result.AddRange(BitConverter.GetBytes(KizunaLinkSet));
            result.AddRange(BitConverter.GetBytes(NormalTalk));
            result.AddRange(BitConverter.GetBytes(gap_84E));
            result.AddRange(BitConverter.GetBytes(CreateEventID));
            result.AddRange(BitConverter.GetBytes(MountPoint));
            result.AddRange(BitConverter.GetBytes(MountObject));
            result.AddRange(Name);
            result.AddRange(BitConverter.GetBytes(NameLength));
            result.AddRange(BitConverter.GetBytes(CommonBladeIndex));
            result.Add(EnableEngageRex);
            result.Add(BladeReleaseStatus);
            result.Add(isUnselect);
            result.Add(AffinityChartStatus);
            result.AddRange(BitConverter.GetBytes(field_8A2));

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
            string str = "";

            str = Encoding.UTF8.GetString(Name.GetByteSubArray(0, (int)NameLength));

            return ID + ": " + str;
        }
    }
}
