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
        public const int DRIVER_ARTS_COUNT = 526;

        public class DriverSkill : IXC2SaveObject
        {
            public const int SIZE = 0xA;

            public UInt16[] IDs { get; set; }
            public UInt16 ColumnNo { get; set; }
            public UInt16 Level { get; set; }

            public DriverSkill(Byte[] data)
            {
                IDs = new UInt16[3];
                for (int i = 0; i < IDs.Length; i++)
                    IDs[i] = BitConverter.ToUInt16(data.GetByteSubArray(i * 2, 2), 0);

                ColumnNo = BitConverter.ToUInt16(data.GetByteSubArray(0x6, 2), 0);
                Level = BitConverter.ToUInt16(data.GetByteSubArray(0x8, 2), 0);
            }

            public Byte[] ToRawData()
            {
                List<Byte> result = new List<byte>();

                foreach (UInt16 u in IDs)
                    result.AddRange(BitConverter.GetBytes(u));

                result.AddRange(BitConverter.GetBytes(ColumnNo));
                result.AddRange(BitConverter.GetBytes(Level));

                if (result.Count != SIZE)
                {
                    string message = "DriverSkill: SIZE ALL WRONG!!!" + Environment.NewLine +
                    "Size should be " + SIZE + " bytes..." + Environment.NewLine +
                    "...but Size is " + result.Count + " bytes!";

                    throw new Exception(message);
                }

                return result.ToArray();
            }
        }

        public class Weapon : IXC2SaveObject
        {
            public const int SIZE = 0x14;

            public UInt32[] ArtIds { get; set; }
            public UInt32 WeaponPoints { get; set; }
            public UInt32 TotalWeaponPoints { get; set; }

            public Weapon(Byte[] data)
            {
                ArtIds = new UInt32[3];
                for (int i = 0; i < ArtIds.Length; i++)
                    ArtIds[i] = BitConverter.ToUInt32(data.GetByteSubArray(i * 4, 4), 0);

                WeaponPoints = BitConverter.ToUInt32(data.GetByteSubArray(0xC, 4), 0);
                TotalWeaponPoints = BitConverter.ToUInt32(data.GetByteSubArray(0x10, 4), 0);
            }

            public Byte[] ToRawData()
            {
                List<Byte> result = new List<Byte>();

                foreach (UInt32 u in ArtIds)
                    result.AddRange(BitConverter.GetBytes(u));

                result.AddRange(BitConverter.GetBytes(WeaponPoints));
                result.AddRange(BitConverter.GetBytes(TotalWeaponPoints));

                if (result.Count != SIZE)
                {
                    string message = "Weapon: SIZE ALL WRONG!!!" + Environment.NewLine +
                    "Size should be " + SIZE + " bytes..." + Environment.NewLine +
                    "...but Size is " + result.Count + " bytes!";

                    throw new Exception(message);
                }

                return result.ToArray();
            }
        }

        public class Pouch : IXC2SaveObject
        {
            public const int SIZE = 0x8;
            public const int COUNT = 3;

            public float Time { get; set; }
            public UInt16 ItemId { get; set; }
            public bool IsEnabled { get; set; }

            public Pouch(Byte[] data)
            {
                Time = BitConverter.ToSingle(data.GetByteSubArray(0, 4), 0);
                ItemId = BitConverter.ToUInt16(data.GetByteSubArray(4, 2), 0);
                IsEnabled = BitConverter.ToUInt16(data.GetByteSubArray(6, 2), 0) == 1;
            }

            public Byte[] ToRawData()
            {
                List<Byte> result = new List<Byte>();

                result.AddRange(BitConverter.GetBytes(Time));
                result.AddRange(BitConverter.GetBytes(ItemId));
                result.AddRange(BitConverter.GetBytes((UInt16)(IsEnabled ? 1 : 0)));

                if (result.Count != SIZE)
                {
                    string message = "Pouch: SIZE ALL WRONG!!!" + Environment.NewLine +
                    "Size should be " + SIZE + " bytes..." + Environment.NewLine +
                    "...but Size is " + result.Count + " bytes!";

                    throw new Exception(message);
                }

                return result.ToArray();
            }
        }

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
            { "gap_536", 0x536 },
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
        public UInt16 IsActive { get; set; }
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
        public Weapon[] Weapons { get; set; }
        public Byte[] Unk_0x02DC { get; set; }
        public UInt16 AccessoryId2 { get; set; }
        public Byte[] Unk_0x02EA { get; set; }
        public ItemHandle AccessoryHandle2 { get; set; }
        public Byte[] DriverArtLevels { get; set; }
        public Byte[] Unk_0x0536 { get; set; }
        public ItemHandle AccessoryHandle0 { get; set; }
        public UInt16 AccessoryId0 { get; set; }
        public Byte[] Unk_0x057A { get; set; }
        public ItemHandle AccessoryHandle1 { get; set; }
        public UInt16 AccessoryId1 { get; set; }
        public Byte[] Unk_0x0582 { get; set; }
        public Pouch[] PouchInfo { get; set; }
        public Byte[] Unk_0x059C { get; set; }

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
            IsActive = BitConverter.ToUInt16(data.GetByteSubArray(LOC["IsActive"], 2), 0);
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

            Weapons = new Weapon[27];
            for (int i = 0; i < Weapons.Length; i++)
                Weapons[i] = new Weapon(data.GetByteSubArray(LOC["Weapons"] + (i * Weapon.SIZE), Weapon.SIZE));

            Unk_0x02DC = data.GetByteSubArray(LOC["gap_2DC"], 12);
            AccessoryId2 = BitConverter.ToUInt16(data.GetByteSubArray(LOC["AccessoryId2"], 2), 0);
            Unk_0x02EA = data.GetByteSubArray(LOC["gap_2EA"], 2);
            AccessoryHandle2 = new ItemHandle32(data.GetByteSubArray(LOC["AccessoryHandle2"], ItemHandle32.SIZE));
            DriverArtLevels = data.GetByteSubArray(LOC["DriverArtLevels"], 582);
            Unk_0x0536 = data.GetByteSubArray(LOC["gap_536"], 62);
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

        public Byte[] ToRawData()
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
            result.AddRange(BitConverter.GetBytes(IsActive));
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

            foreach (Weapon w in Weapons)
                result.AddRange(w.ToRawData());

            result.AddRange(Unk_0x02DC);
            result.AddRange(BitConverter.GetBytes(AccessoryId2));
            result.AddRange(Unk_0x02EA);
            result.AddRange(AccessoryHandle2.ToRawData());
            result.AddRange(DriverArtLevels);
            result.AddRange(Unk_0x0536);
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

        public override string ToString()
        {
            if (XC2Data.ContainsDriver(ID))
                return ID + ": " + XC2Data.Drivers().Select("ID = " + ID)[0]["Name"];
            else
                return "INVALID ID";
        }
    }
}
