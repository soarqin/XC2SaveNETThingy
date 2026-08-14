using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XC2SaveNETThingy
{
    public class Driver : IXC2SaveObject
    {
        public static int SIZE = 0x5A0;
        public const int DRIVER_ARTS_COUNT = 640;
        public const int WEAPONS_COUNT = 27;

        private static Dictionary<string, int> LOC = new Dictionary<string, int>()
        {
            { "BraveryPoints", 0x0 },
            { "BraveryLevel", 0x4 },
            { "TruthPoints", 0x8 },
            { "TruthLevel", 0xC },
            { "CompassionPoints", 0x10 },
            { "CompassionLevel", 0x14 },
            { "JusticePoints", 0x18 },
            { "JusticeLevel", 0x1C },
            { "IsActive", 0x20 },
            { "DriverId", 0x22 },
            { "SetBlade", 0x24 },
            { "EquippedBlades", 0x26 },
            { "gap_2C", 0x2C },
            { "SkillsRound1", 0x30 },
            { "SkillsRound2", 0x62 },
            { "Level", 0x94 },
            { "HpMax", 0x96 },
            { "Strength", 0x98 },
            { "PowEther", 0x9A },
            { "Dex", 0x9C },
            { "Agility", 0x9E },
            { "Luck", 0xA0 },
            { "PArmor", 0xA2 },
            { "EArmor", 0xA4 },
            { "CritRate", 0xA6 },
            { "GuardRate", 0xA7 },
            { "field_A8", 0xA8 },
            { "gap_AC", 0xAC },
            { "field_AF", 0xAF },
            { "Exp", 0xB0 },
            { "BattleExp", 0xB4 },
            { "SkillPoints", 0xB8 },
            { "TotalSkillPoints", 0xBC },
            { "Weapons", 0xC0 },
            { "gap_2DC", 0x2DC },
            { "AccessoryId2", 0x2E8 },
            { "gap_2EA", 0x2EA },
            { "AccessoryHandle2", 0x2EC },
            { "DriverArtLevels", 0x2F0 },
            { "Unk_0x0570", 0x570 },
            { "AccessoryHandle0", 0x574 },
            { "AccessoryId0", 0x578 },
            { "gap_57A", 0x57A },
            { "AccessoryHandle1", 0x57C },
            { "AccessoryId1", 0x580 },
            { "gap_582", 0x582 },
            { "PouchInfo", 0x584 },
            { "gap_field_59C", 0x59C }
        };

        public UInt32 BraveryPoints { get; set; }
        public UInt32 BraveryLevel { get; set; }
        public UInt32 TruthPoints { get; set; }
        public UInt32 TruthLevel { get; set; }
        public UInt32 CompassionPoints { get; set; }
        public UInt32 CompassionLevel { get; set; }
        public UInt32 JusticePoints { get; set; }
        public UInt32 JusticeLevel { get; set; }
        public bool IsInParty { get; set; }
        public UInt16 ID { get; set; }
        public Int16 SetBlade { get; set; }
        public UInt16[] EquippedBlades { get; set; }
        public Byte[] Unk_0x002C { get; set; }
        public DriverSkill[] OvertSkills { get; set; }
        public DriverSkill[] HiddenSkills { get; set; }
        public UInt16 Level { get; set; }
        public UInt16 HP { get; set; }
        public UInt16 Strength { get; set; }
        public UInt16 Ether { get; set; }
        public UInt16 Dexterity { get; set; }
        public UInt16 Agility { get; set; }
        public UInt16 Luck { get; set; }
        public UInt16 PhysDef { get; set; }
        public UInt16 EtherDef { get; set; }
        public Byte CriticalRate { get; set; }
        public Byte GuardRate { get; set; }
        public Byte[] Unk_0x00A8 { get; set; }
        public UInt32 BonusExp { get; set; }
        public UInt32 BattleExp { get; set; }
        public UInt32 CurrentSkillPoints { get; set; }
        public UInt32 TotalSkillPoints { get; set; }
        public List<Weapon> Weapons { get; set; }
        public Byte[] Unk_0x02DC { get; set; }
        public UInt16 AccessoryId2 { get; set; }
        public Byte[] Unk_0x02EA { get; set; }
        public ItemHandle AccessoryHandle2 { get; set; }
        public List<Byte> DriverArtLevels { get; set; }
        public Byte[] Unk_0x0570 { get; set; }
        public ItemHandle AccessoryHandle0 { get; set; }
        public UInt16 AccessoryId0 { get; set; }
        public Byte[] Unk_0x057A { get; set; }
        public ItemHandle AccessoryHandle1 { get; set; }
        public UInt16 AccessoryId1 { get; set; }
        public Byte[] Unk_0x0582 { get; set; }
        public Pouch[] PouchInfo { get; set; }
        public Byte[] Unk_0x059C { get; set; }

        public string Name_EN
        {
            get
            {
                return ToString(XC2Data.LANG.EN);
            }
        }

        public string Name_CN
        {
            get
            {
                return ToString(XC2Data.LANG.CN);
            }
        }

        public string Name_JA { get { return ToString(XC2Data.LANG.JA); } }
        public string Name_KO { get { return ToString(XC2Data.LANG.KO); } }
        public string Name_ZH_HANS { get { return ToString(XC2Data.LANG.ZH_HANS); } }
        public string Name_ZH_HANT { get { return ToString(XC2Data.LANG.ZH_HANT); } }
        public string Name_FR { get { return ToString(XC2Data.LANG.FR); } }
        public string Name_DE { get { return ToString(XC2Data.LANG.DE); } }
        public string Name_IT { get { return ToString(XC2Data.LANG.IT); } }
        public string Name_ES { get { return ToString(XC2Data.LANG.ES); } }

        public Driver(Byte[] data)
        {
            BraveryPoints = BitConverter.ToUInt32(data.GetByteSubArray(LOC["BraveryPoints"], 4), 0);
            BraveryLevel = BitConverter.ToUInt32(data.GetByteSubArray(LOC["BraveryLevel"], 4), 0);
            TruthPoints = BitConverter.ToUInt32(data.GetByteSubArray(LOC["TruthPoints"], 4), 0);
            TruthLevel = BitConverter.ToUInt32(data.GetByteSubArray(LOC["TruthLevel"], 4), 0);
            CompassionPoints = BitConverter.ToUInt32(data.GetByteSubArray(LOC["CompassionPoints"], 4), 0);
            CompassionLevel = BitConverter.ToUInt32( data.GetByteSubArray(LOC["CompassionLevel"], 4), 0);
            JusticePoints = BitConverter.ToUInt32(data.GetByteSubArray(LOC["JusticePoints"], 4), 0);
            JusticeLevel = BitConverter.ToUInt32(data.GetByteSubArray(LOC["JusticeLevel"], 4), 0);
            IsInParty = BitConverter.ToUInt16(data.GetByteSubArray(LOC["IsActive"], 2), 0) == 1;
            ID = BitConverter.ToUInt16(data.GetByteSubArray(LOC["DriverId"], 2), 0);
            SetBlade = BitConverter.ToInt16(data.GetByteSubArray(LOC["SetBlade"], 2), 0);

            EquippedBlades = new UInt16[3];
            for (int i = 0; i < EquippedBlades.Length; i++)
                EquippedBlades[i] = BitConverter.ToUInt16(data.GetByteSubArray(LOC["EquippedBlades"] + (i * 2), 2), 0);

            Unk_0x002C = data.GetByteSubArray(LOC["gap_2C"], 4);

            OvertSkills = new DriverSkill[5];
            for (int i = 0; i < OvertSkills.Length; i++)
                OvertSkills[i] = new DriverSkill(data.GetByteSubArray(LOC["SkillsRound1"] + (i * DriverSkill.SIZE), DriverSkill.SIZE));

            HiddenSkills = new DriverSkill[5];
            for (int i = 0; i < HiddenSkills.Length; i++)
                HiddenSkills[i] = new DriverSkill(data.GetByteSubArray(LOC["SkillsRound2"] + (i * DriverSkill.SIZE), DriverSkill.SIZE));

            Level = BitConverter.ToUInt16(data.GetByteSubArray(LOC["Level"], 2), 0);
            HP = BitConverter.ToUInt16(data.GetByteSubArray(LOC["HpMax"], 2), 0);
            Strength = BitConverter.ToUInt16(data.GetByteSubArray(LOC["Strength"], 2), 0);
            Ether = BitConverter.ToUInt16(data.GetByteSubArray(LOC["PowEther"], 2), 0);
            Dexterity = BitConverter.ToUInt16(data.GetByteSubArray(LOC["Dex"], 2), 0);
            Agility = BitConverter.ToUInt16(data.GetByteSubArray(LOC["Agility"], 2), 0);
            Luck = BitConverter.ToUInt16(data.GetByteSubArray(LOC["Luck"], 2), 0);
            PhysDef = BitConverter.ToUInt16(data.GetByteSubArray(LOC["PArmor"], 2), 0);
            EtherDef = BitConverter.ToUInt16(data.GetByteSubArray(LOC["EArmor"], 2), 0);
            CriticalRate = data[LOC["CritRate"]];
            GuardRate = data[LOC["GuardRate"]];
            Unk_0x00A8 = data.GetByteSubArray(LOC["field_A8"], 8);
            BonusExp = BitConverter.ToUInt32(data.GetByteSubArray(LOC["Exp"], 4), 0);
            BattleExp = BitConverter.ToUInt32(data.GetByteSubArray(LOC["BattleExp"], 4), 0);
            CurrentSkillPoints = BitConverter.ToUInt32(data.GetByteSubArray(LOC["SkillPoints"], 4), 0);
            TotalSkillPoints = BitConverter.ToUInt32(data.GetByteSubArray(LOC["TotalSkillPoints"], 4), 0);

            Weapons = new List<Weapon>();
            for (int i = 0; i < WEAPONS_COUNT; i++)
                Weapons.Add(new Weapon(data.GetByteSubArray(LOC["Weapons"] + (i * Weapon.SIZE), Weapon.SIZE)));

            Unk_0x02DC = data.GetByteSubArray(LOC["gap_2DC"], 12);
            AccessoryId2 = BitConverter.ToUInt16(data.GetByteSubArray(LOC["AccessoryId2"], 2), 0);
            Unk_0x02EA = data.GetByteSubArray(LOC["gap_2EA"], 2);
            AccessoryHandle2 = new ItemHandle32(data.GetByteSubArray(LOC["AccessoryHandle2"], ItemHandle32.SIZE));
            DriverArtLevels = new List<Byte>();
            DriverArtLevels.AddRange(data.GetByteSubArray(LOC["DriverArtLevels"], DRIVER_ARTS_COUNT));
            Unk_0x0570 = data.GetByteSubArray(LOC["Unk_0x0570"], 4);
            AccessoryHandle0 = new ItemHandle32(data.GetByteSubArray(LOC["AccessoryHandle0"], ItemHandle32.SIZE));
            AccessoryId0 = BitConverter.ToUInt16(data.GetByteSubArray(LOC["AccessoryId0"], 2), 0);
            Unk_0x057A = data.GetByteSubArray(LOC["gap_57A"], 2);
            AccessoryHandle1 = new ItemHandle32(data.GetByteSubArray(LOC["AccessoryHandle1"], ItemHandle32.SIZE));
            AccessoryId1 = BitConverter.ToUInt16(data.GetByteSubArray(LOC["AccessoryId1"], 2), 0);
            Unk_0x0582 = data.GetByteSubArray(LOC["gap_582"], 2);

            PouchInfo = new Pouch[3];
            for (int i = 0; i < PouchInfo.Length; i++)
                PouchInfo[i] = new Pouch(data.GetByteSubArray(LOC["PouchInfo"] + (i * Pouch.SIZE), Pouch.SIZE));

            Unk_0x059C = data.GetByteSubArray(LOC["gap_field_59C"], 4);
        }

        public virtual Byte[] ToRawData()
        {
            List<Byte> result = new List<Byte>();

            result.AddRange(BitConverter.GetBytes(BraveryPoints));
            result.AddRange(BitConverter.GetBytes(BraveryLevel));
            result.AddRange(BitConverter.GetBytes(TruthPoints));
            result.AddRange(BitConverter.GetBytes(TruthLevel));
            result.AddRange(BitConverter.GetBytes(CompassionPoints));
            result.AddRange(BitConverter.GetBytes(CompassionLevel));
            result.AddRange(BitConverter.GetBytes(JusticePoints));
            result.AddRange(BitConverter.GetBytes(JusticeLevel));
            result.AddRange(BitConverter.GetBytes((UInt16)(IsInParty ? 1 : 0)));
            result.AddRange(BitConverter.GetBytes(ID));
            result.AddRange(BitConverter.GetBytes(SetBlade));

            foreach (UInt16 u in EquippedBlades)
                result.AddRange(BitConverter.GetBytes(u));

            result.AddRange(Unk_0x002C);

            foreach (DriverSkill ds in OvertSkills)
                result.AddRange(ds.ToRawData());

            foreach (DriverSkill ds in HiddenSkills)
                result.AddRange(ds.ToRawData());

            result.AddRange(BitConverter.GetBytes(Level));
            result.AddRange(BitConverter.GetBytes(HP));
            result.AddRange(BitConverter.GetBytes(Strength));
            result.AddRange(BitConverter.GetBytes(Ether));
            result.AddRange(BitConverter.GetBytes(Dexterity));
            result.AddRange(BitConverter.GetBytes(Agility));
            result.AddRange(BitConverter.GetBytes(Luck));
            result.AddRange(BitConverter.GetBytes(PhysDef));
            result.AddRange(BitConverter.GetBytes(EtherDef));
            result.Add(CriticalRate);
            result.Add(GuardRate);
            result.AddRange(Unk_0x00A8);
            result.AddRange(BitConverter.GetBytes(BonusExp));
            result.AddRange(BitConverter.GetBytes(BattleExp));
            result.AddRange(BitConverter.GetBytes(CurrentSkillPoints));
            result.AddRange(BitConverter.GetBytes(TotalSkillPoints));

            for (int i = 0; i < WEAPONS_COUNT; i++)
                result.AddRange(Weapons[i].ToRawData());

            result.AddRange(Unk_0x02DC);
            result.AddRange(BitConverter.GetBytes(AccessoryId2));
            result.AddRange(Unk_0x02EA);
            result.AddRange(AccessoryHandle2.ToRawData());
            result.AddRange(DriverArtLevels.GetRange(0, DRIVER_ARTS_COUNT));
            result.AddRange(Unk_0x0570);
            result.AddRange(AccessoryHandle0.ToRawData());
            result.AddRange(BitConverter.GetBytes(AccessoryId0));
            result.AddRange(Unk_0x057A);
            result.AddRange(AccessoryHandle1.ToRawData());
            result.AddRange(BitConverter.GetBytes(AccessoryId1));
            result.AddRange(Unk_0x0582);

            foreach (Pouch p in PouchInfo)
                result.AddRange(p.ToRawData());

            result.AddRange(Unk_0x059C);

            if (result.Count != SIZE)
            {
                string message = "Driver: SIZE ALL WRONG!!!" + Environment.NewLine +
                "Size should be " + SIZE + " bytes..." + Environment.NewLine +
                "...but Size is " + result.Count + " bytes!";

                throw new Exception(message);
            }

            return result.ToArray();
        }

        public string ToString(XC2Data.LANG l)
        {
            if (!XC2Data.ContainsDriver(ID)) return ID + ": INVALID ID";
            return ID + ": " + XC2Data.Drivers(l).Select("ID = " + ID)[0]["Name"];
        }

        public override string ToString()
        {
            return ToString(XC2Data.LANG.EN);
        }
    }

    public class Driver150Ext : IXC2SaveObject
    {
        public static int SIZE = 0x324;
        public const int WEAPONS_COUNT = 9;
        public const int DRIVER_ARTS_COUNT = 103;
        private static Dictionary<string, int> LOC = new Dictionary<string, int>()
        {
            { "Weapons150", 0x0000 },
            { "Unk_0x00B4", 0x00B4 },
            { "DriverArtLevels150", 0x01E0 },
            { "Unk_0x0247", 0x0247 }
        };

        public List<Weapon> Weapons150 { get; set; }
        public Byte[] Unk_0x00B4 { get; set; }
        public List<Byte> DriverArtLevels150 { get; set; }
        public Byte[] Unk_0x0247 { get; set; }

        public Driver150Ext(Byte[] data)
        {
            Weapons150 = new List<Weapon>();
            for(int i = 0; i < WEAPONS_COUNT; i++)
                Weapons150.Add(new Weapon(data.GetByteSubArray(LOC["Weapons150"] + (i * Weapon.SIZE), Weapon.SIZE)));

            Unk_0x00B4 = data.GetByteSubArray(LOC["Unk_0x00B4"], 0x12C);

            DriverArtLevels150 = new List<Byte>();
            DriverArtLevels150.AddRange(data.GetByteSubArray(LOC["DriverArtLevels150"], DRIVER_ARTS_COUNT));

            Unk_0x0247 = data.GetByteSubArray(LOC["Unk_0x0247"], 0xDD);
        }
        public byte[] ToRawData()
        {
            List<Byte> result = new List<Byte>();

            foreach (Weapon w in Weapons150)
                result.AddRange(w.ToRawData());
            result.AddRange(Unk_0x00B4);
            result.AddRange(DriverArtLevels150);
            result.AddRange(Unk_0x0247);

            return result.ToArray();
        }
    }

    public class DriverIra : Driver, IXC2SaveObject
    {
        public static new int SIZE = 0xDAC;
        public static new int WEAPONS_COUNT = 37;
        private static Dictionary<string, int> LOC = new Dictionary<string, int>()
        {
            { "WeaponAccessoryHandle", 0x05A0 },
            { "WeaponAccessoryID", 0x05A4 },
            { "Unk_0x05A6", 0x05A6 },
            { "RearGuardWeapons", 0x05B0 },
            { "Unk_0x0894", 0x0894 },
            { "SwitchTalentWeapons", 0x09AC },
            { "Unk_0x0C90", 0x0C90 },
        };

        public ItemHandle AccessoryWeaponHandle { get; set; }
        public UInt16 AccessoryWeaponID { get; set; }
        public Byte[] Unk_0x05A6 { get; set; }
        public Weapon[] WeaponsRearGuard { get; set; }
        public Byte[] Unk_0x0894 { get; set; }
        public Weapon[] WeaponsSwitchTalent { get; set; }
        public Byte[] Unk_0x0C90 { get; set; }

        public DriverIra(Byte[] data) : base(data)
        {
            AccessoryWeaponHandle = new ItemHandle32(data.GetByteSubArray(LOC["WeaponAccessoryHandle"], ItemHandle32.SIZE));
            AccessoryWeaponID = BitConverter.ToUInt16(data.GetByteSubArray(LOC["WeaponAccessoryID"], 2), 0);
            Unk_0x05A6 = data.GetByteSubArray(LOC["Unk_0x05A6"], 0xA);

            WeaponsRearGuard = new Weapon[WEAPONS_COUNT];
            for (int i = 0; i < WeaponsRearGuard.Length; i++)
                WeaponsRearGuard[i] = new Weapon(data.GetByteSubArray(LOC["RearGuardWeapons"] + (i * Weapon.SIZE), Weapon.SIZE));

            Unk_0x0894 = data.GetByteSubArray(LOC["Unk_0x0894"], 0x118);

            WeaponsSwitchTalent = new Weapon[WEAPONS_COUNT];
            for (int i = 0; i < WeaponsSwitchTalent.Length; i++)
                WeaponsSwitchTalent[i] = new Weapon(data.GetByteSubArray(LOC["SwitchTalentWeapons"] + (i * Weapon.SIZE), Weapon.SIZE));

            Unk_0x0C90 = data.GetByteSubArray(LOC["Unk_0x0C90"], 0x11C);
        }
        public override Byte[] ToRawData()
        {
            List<Byte> result = new List<Byte>();
            result.AddRange(base.ToRawData());

            result.AddRange(AccessoryWeaponHandle.ToRawData());
            result.AddRange(BitConverter.GetBytes(AccessoryWeaponID));
            result.AddRange(Unk_0x05A6);

            foreach (Weapon w in WeaponsRearGuard)
                result.AddRange(w.ToRawData());

            result.AddRange(Unk_0x0894);

            foreach (Weapon w in WeaponsSwitchTalent)
                result.AddRange(w.ToRawData());

            result.AddRange(Unk_0x0C90);

            return result.ToArray();
        }
    }

    public class DriverIraExt : IXC2SaveObject
    {
        public static int SIZE = 0x324;
        public const int WEAPONS_COUNT = 9;
        public const int DRIVER_ARTS_COUNT = 103;
        private static Dictionary<string, int> LOC = new Dictionary<string, int>()
        {
            { "Unk_0x0000", 0x0000 },
            { "WeaponsIra", 0x0004 },
            { "Unk_0x00B8", 0x00B8 },
            { "DriverArtLevelsIra", 0x01E4 },
            { "Unk_0x024B", 0x024B }
        };

        public Byte[] Unk_0x0000 { get; set; }
        public List<Weapon> WeaponsIra { get; set; }
        public Byte[] Unk_0x00B8 { get; set; }
        public List<Byte> DriverArtLevelsIra { get; set; }
        public Byte[] Unk_0x024B { get; set; }

        public DriverIraExt(Byte[] data)
        {
            Unk_0x0000 = data.GetByteSubArray(LOC["Unk_0x0000"], 4);

            WeaponsIra = new List<Weapon>();
            for (int i = 0; i < WEAPONS_COUNT; i++)
                WeaponsIra.Add(new Weapon(data.GetByteSubArray(LOC["WeaponsIra"] + (i * Weapon.SIZE), Weapon.SIZE)));

            Unk_0x00B8 = data.GetByteSubArray(LOC["Unk_0x00B8"], 0x12C);
            DriverArtLevelsIra = new List<Byte>();
            DriverArtLevelsIra.AddRange(data.GetByteSubArray(LOC["DriverArtLevelsIra"], DRIVER_ARTS_COUNT));
            Unk_0x024B = data.GetByteSubArray(LOC["Unk_0x024B"], 0xD9);
        }
        public byte[] ToRawData()
        {
            List<Byte> result = new List<Byte>();

            result.AddRange(Unk_0x0000);
            foreach (Weapon w in WeaponsIra)
                result.AddRange(w.ToRawData());
            result.AddRange(Unk_0x00B8);
            result.AddRange(DriverArtLevelsIra);
            result.AddRange(Unk_0x024B);

            return result.ToArray();
        }
    }
}
