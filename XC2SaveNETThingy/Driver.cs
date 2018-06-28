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

        public static readonly Dictionary<UInt16, string> IDNAMES = new Dictionary<UInt16, string>()
        {
            { 0, "(none)" },
            { 1, "Rex" },
            { 2, "Nia" },
            { 3, "Zeke" },
            { 4, "Tora" },
            { 5, "Vandham" },
            { 6, "Mòrag" },
            { 7, "Jin (Ch 1 Guest)" },
            { 8, "Malos (Ch 1 Guest)" },
            { 9, "Jin (Ch 8 Guest)" },
            { 10, "Unknown1" },
            { 11, "Unknown2" },
            { 12, "Unknown3" },
            { 13, "Unknown4" },
            { 14, "Unknown5" },
            { 15, "Unknown6" },
            { 16, "Unknown7" }
        };

        public static readonly Dictionary<int, string> STATUS = new Dictionary<int, string>()
        {
            { 0, "Not Active" },
            { 1, "In Party" },
            { 2, "In Mercenary Team" }
        };

        public static readonly Dictionary<int, string> SETBLADE = new Dictionary<int, string>()
        {
            { -1, "(none)" },
            { 0, "1st Blade" },
            { 1, "2nd Blade" },
            { 2, "3rd Blade" },
            { 3, "Unknown (Mythra+Rex only?)" }
        };

        public static readonly Dictionary<UInt16, string> DRIVER_SKILLS = new Dictionary<UInt16, string>()
        {
            { 0, "(none)" },
            { 1, "Reflex Response" },
            { 2, "Rapid Attack II" },
            { 3, "Picture of Health" },
            { 4, "Lively Lad" },
            { 5, "Topple Champ" },
            { 6, "Fight or Flight" },
            { 7, "Eyes on the Prize" },
            { 8, "Combat Boost" },
            { 9, "Child of Steel" },
            { 10, "Relentless Spirit" },
            { 11, "Rapid Attack I" },
            { 12, "Muscle Builder" },
            { 13, "Combo Enhance" },
            { 14, "Arts Chain" },
            { 15, "Drive to Win" },
            { 16, "Shadow Strike" },
            { 17, "Prediction" },
            { 18, "Inherit Muscle" },
            { 19, "Pre-empt Moves" },
            { 20, "Heart Recharge" },
            { 21, "Ether Studies" },
            { 22, "Rescue" },
            { 23, "Gramps' Blessing" },
            { 24, "Rapid Attack III" },
            { 25, "Untimate Relay" },
            { 26, "Further Ether Studies" },
            { 27, "Cheer Up" },
            { 28, "Herbalism" },
            { 29, "Solidarity" },
            { 30, "Gramps' Auspices" },
            { 31, "Catgirl" },
            { 32, "Big Cat" },
            { 33, "Feral Awakening III" },
            { 34, "Natural Selection" },
            { 35, "Hard Hitter" },
            { 36, "Focused Technique" },
            { 37, "Wild Life" },
            { 38, "Tigergirl" },
            { 39, "Sore Loser" },
            { 40, "Adrenaline" },
            { 41, "Kinetic Vision" },
            { 42, "Feral Awakening II" },
            { 43, "Tooth and Claw" },
            { 44, "Panther Pounce" },
            { 45, "Bestial Pursuit" },
            { 46, "Lithe Strength" },
            { 47, "Tiger's Eye" },
            { 48, "Transmit Power" },
            { 49, "Bear Knuckles" },
            { 50, "Road to Victory" },
            { 51, "Soothing Hand" },
            { 52, "Ether Talent" },
            { 53, "Herb Synthesis" },
            { 54, "Mutual Happiness" },
            { 55, "Feral Awakening I" },
            { 56, "Chance Recovery" },
            { 57, "Lucky Cat" },
            { 58, "Cheer Leader" },
            { 59, "Inner Strength" },
            { 60, "Lady of Luck" },
            { 61, "Raring to Go I" },
            { 62, "Spongy Bod" },
            { 63, "Health Bomb" },
            { 64, "Super Spongy Bod" },
            { 65, "Grin and Bear It" },
            { 66, "Keen Recovery" },
            { 67, "Restless Foot" },
            { 68, "Nopon Topple Arts" },
            { 69, "Look Over There!" },
            { 70, "Super Restless Foot" },
            { 71, "Good With Hands" },
            { 72, "Raring to Go III" },
            { 73, "Tiger Muscles" },
            { 74, "Nopon Chain Arts" },
            { 75, "Crazy With Hands" },
            { 76, "Forced Initiative" },
            { 77, "Nopon Chain Boost" },
            { 78, "Inherit Protein" },
            { 79, "Poppi Care" },
            { 80, "Mood Maker" },
            { 81, "Luckypon" },
            { 82, "Placebo" },
            { 83, "Raring to Go II" },
            { 84, "Double-Luckypon" },
            { 85, "Rapture" },
            { 86, "Nopon Rescue Service" },
            { 87, "Hidden Talent" },
            { 88, "Hope for the Best" },
            { 89, "Team Mascot" },
            { 90, "Burgeoning Talent...?" },
            { 91, "Tough Nut" },
            { 92, "Veteran" },
            { 93, "Guardian" },
            { 94, "Muscle Accelerator II" },
            { 95, "Last Man Standing" },
            { 96, "Power Lift" },
            { 97, "Quick Muscles" },
            { 98, "Recuperation" },
            { 99, "Pumped Dodge" },
            { 100, "Physical Fitness" },
            { 101, "10,000 Push-Ups" },
            { 102, "Burly Chain" },
            { 103, "Muscle Accelerator I" },
            { 104, "Beefcake Flurry" },
            { 105, "Elite Muscles" },
            { 106, "Knack for Battle" },
            { 107, "Anything for a Mate" },
            { 108, "Dependable Guy" },
            { 109, "Honed to Perfection" },
            { 110, "World's Stringest Man" },
            { 111, "Ether Powder" },
            { 112, "Muscle Accelerator III" },
            { 113, "Big Strong Hands" },
            { 114, "Ether Muscle" },
            { 115, "Serious Recovery" },
            { 116, "Thank the Heavens" },
            { 117, "Booming Aid" },
            { 118, "Buff Buff" },
            { 119, "Higher Powers" },
            { 120, "Greatest Work" },
            { 121, "Imperial Footwork" },
            { 122, "Basic Discipline" },
            { 123, "Initiave II" },
            { 124, "Martial Prowess" },
            { 125, "Ultimate Footwork" },
            { 126, "Instant Chain" },
            { 127, "Survival Training" },
            { 128, "Warrior's Resolve" },
            { 129, "Life's Flow" },
            { 130, "Imperial Pride" },
            { 131, "Arts Relay" },
            { 132, "Initiave I" },
            { 133, "Vigilance" },
            { 134, "Physical Discipline" },
            { 135, "Imperial Pursuit" },
            { 136, "Silent Bloodshed" },
            { 137, "Blade Alliance" },
            { 138, "Cool Focus" },
            { 139, "Battle Stance" },
            { 140, "Legendary Skill" },
            { 141, "Etherology" },
            { 142, "Alchemy" },
            { 143, "Charmed Life" },
            { 144, "Imperial First AId" },
            { 145, "Initiave III" },
            { 146, "Inspiration" },
            { 147, "Ultimate Etherology" },
            { 148, "Divination" },
            { 149, "Cheer Master" },
            { 150, "Golden Opportunity" },
            { 151, "Adamant Heart" },
            { 152, "Flash Step" },
            { 153, "Mach Prep II" },
            { 154, "Critical Pro" },
            { 155, "Air Extender" },
            { 156, "Natural Healing" },
            { 157, "Heart of Gold" },
            { 158, "Zeal Heal" },
            { 159, "Fleetfoot Flit" },
            { 160, "Second Wind" },
            { 161, "Mach Prep I" },
            { 162, "Brawn over Brain" },
            { 163, "Camraderie" },
            { 164, "Herculean Strength" },
            { 165, "Move Chaining" },
            { 166, "Feindish Pursuit" },
            { 167, "Professional Hit" },
            { 168, "Hotshot Hit" },
            { 169, "Obsidian Slash" },
            { 170, "A Hero's Work" },
            { 171, "Lucky Duck" },
            { 172, "Ether Prodigy" },
            { 173, "Tantal Courage" },
            { 174, "Mach Prep III" },
            { 175, "Histrionic Hero" },
            { 176, "Ether Autarch" },
            { 177, "Medicinal Maestro" },
            { 178, "Unleash Shining Justice" },
            { 179, "Raucous Cheer" },
            { 180, "As One" }
        };

        public class DriverSkill : IXC2SaveObject
        {
            public const int SIZE = 0xA;

            public UInt16[] IDs { get; set; }
            public Int16 ColumnNo { get; set; }
            public UInt16 Level { get; set; }

            public DriverSkill(Byte[] data)
            {
                IDs = new UInt16[3];
                for (int i = 0; i < IDs.Length; i++)
                    IDs[i] = BitConverter.ToUInt16(data.GetByteSubArray(i * 2, 2), 0);

                ColumnNo = BitConverter.ToInt16(data.GetByteSubArray(0x6, 2), 0);
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
            public static readonly Dictionary<int, string> NAMES = new Dictionary<int, string>()
            {
                { 0, "(none)"},
                { 1, "Aegis Sword"},
                { 2, "Catalyst Scimitar"},
                { 3, "Twin Rings"},
                { 4, "Drill Shield"},
                { 5, "Mech Arms"},
                { 6, "Variable Saber"},
                { 7, "Whipswords"},
                { 8, "Big Bang Edge"},
                { 9, "Dual Scythes"},
                { 10, "Greataxe"},
                { 11, "Megalance"},
                { 12, "Ether Cannon"},
                { 13, "Shield Hammer"},
                { 14, "Chroma Katana"},
                { 15, "Bitball"},
                { 16, "Knuckle Claws"},
                { 17, "Broadsword"},
                { 18, "JIN_WEAPON"},
                { 19, "MALOS_WEAPON"},
                { 20, "Calamity Scythe"},
                { 21, "Cobra Bardiche"},
                { 22, "22"},
                { 23, "Brilliant Twinblades"},
                { 24, "24"},
                { 25, "25"},
                { 26, "26"}
            };

            public const int SIZE = 0x14;

            public UInt32[] ArtIds { get; set; }
            public int WeaponPoints { get; set; }
            public int TotalWeaponPoints { get; set; }

            public Weapon(Byte[] data)
            {
                ArtIds = new UInt32[3];
                for (int i = 0; i < ArtIds.Length; i++)
                    ArtIds[i] = BitConverter.ToUInt32(data.GetByteSubArray(i * 4, 4), 0);

                WeaponPoints = BitConverter.ToInt32(data.GetByteSubArray(0xC, 4), 0);
                TotalWeaponPoints = BitConverter.ToInt32(data.GetByteSubArray(0x10, 4), 0);
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

            float Time { get; set; }
            UInt16 ItemId { get; set; }
            UInt16 IsEnabled { get; set; }

            public Pouch(Byte[] data)
            {
                Time = BitConverter.ToSingle(data.GetByteSubArray(0, 4), 0);
                ItemId = BitConverter.ToUInt16(data.GetByteSubArray(4, 2), 0);
                IsEnabled = BitConverter.ToUInt16(data.GetByteSubArray(6, 2), 0);
            }

            public Byte[] ToRawData()
            {
                List<Byte> result = new List<Byte>();

                result.AddRange(BitConverter.GetBytes(Time));
                result.AddRange(BitConverter.GetBytes(ItemId));
                result.AddRange(BitConverter.GetBytes(IsEnabled));

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
            { "gap_4FD", 0x4FD },
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
        public UInt32 gap_2C { get; set; }
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
        public UInt32 field_A8 { get; set; }
        public Byte[] gap_AC { get; set; }
        public Byte field_AF { get; set; }
        public UInt32 BonusExp { get; set; }
        public UInt32 BattleExp { get; set; }
        public UInt32 CurrentSkillPoints { get; set; }
        public UInt32 TotalSkillPoints { get; set; }
        public Weapon[] Weapons { get; set; }
        public Byte[] gap_2DC { get; set; }
        public UInt16 AccessoryId2 { get; set; }
        public Byte[] gap_2EA { get; set; }
        public ItemHandle AccessoryHandle2 { get; set; }
        public Byte[] DriverArtLevels { get; set; }
        public Byte[] gap_4FD { get; set; }
        public ItemHandle AccessoryHandle0 { get; set; }
        public UInt16 AccessoryId0 { get; set; }
        public UInt16 gap_57A { get; set; }
        public ItemHandle AccessoryHandle1 { get; set; }
        public UInt16 AccessoryId1 { get; set; }
        public UInt16 gap_582 { get; set; }
        public Pouch[] PouchInfo { get; set; }
        public Byte[] gap_field_59C { get; set; }

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

            gap_2C = BitConverter.ToUInt32(data.GetByteSubArray(LOC["gap_2C"], 4), 0);

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
            field_A8 = BitConverter.ToUInt32(data.GetByteSubArray(LOC["field_A8"], 4), 0);
            gap_AC = data.GetByteSubArray(LOC["gap_AC"], 3);
            field_AF = data[LOC["field_AF"]];
            BonusExp = BitConverter.ToUInt32(data.GetByteSubArray(LOC["Exp"], 4), 0);
            BattleExp = BitConverter.ToUInt32(data.GetByteSubArray(LOC["BattleExp"], 4), 0);
            CurrentSkillPoints = BitConverter.ToUInt32(data.GetByteSubArray(LOC["SkillPoints"], 4), 0);
            TotalSkillPoints = BitConverter.ToUInt32(data.GetByteSubArray(LOC["TotalSkillPoints"], 4), 0);

            Weapons = new Weapon[27];
            for (int i = 0; i < Weapons.Length; i++)
                Weapons[i] = new Weapon(data.GetByteSubArray(LOC["Weapons"] + (i * Weapon.SIZE), Weapon.SIZE));

            gap_2DC = data.GetByteSubArray(LOC["gap_2DC"], 12);
            AccessoryId2 = BitConverter.ToUInt16(data.GetByteSubArray(LOC["AccessoryId2"], 2), 0);
            gap_2EA = data.GetByteSubArray(LOC["gap_2EA"], 2);
            AccessoryHandle2 = new ItemHandle32(data.GetByteSubArray(LOC["AccessoryHandle2"], ItemHandle32.SIZE));
            DriverArtLevels = data.GetByteSubArray(LOC["DriverArtLevels"], 525);
            gap_4FD = data.GetByteSubArray(LOC["gap_4FD"], 119);
            AccessoryHandle0 = new ItemHandle32(data.GetByteSubArray(LOC["AccessoryHandle0"], ItemHandle32.SIZE));
            AccessoryId0 = BitConverter.ToUInt16(data.GetByteSubArray(LOC["AccessoryId0"], 2), 0);
            gap_57A = BitConverter.ToUInt16(data.GetByteSubArray(LOC["gap_57A"], 2), 0);
            AccessoryHandle1 = new ItemHandle32(data.GetByteSubArray(LOC["AccessoryHandle1"], ItemHandle32.SIZE));
            AccessoryId1 = BitConverter.ToUInt16(data.GetByteSubArray(LOC["AccessoryId1"], 2), 0);
            gap_582 = BitConverter.ToUInt16(data.GetByteSubArray(LOC["gap_582"], 2), 0);

            PouchInfo = new Pouch[3];
            for (int i = 0; i < PouchInfo.Length; i++)
                PouchInfo[i] = new Pouch(data.GetByteSubArray(LOC["PouchInfo"] + (i * Pouch.SIZE), Pouch.SIZE));

            gap_field_59C = data.GetByteSubArray(LOC["gap_field_59C"], 4);
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

            result.AddRange(BitConverter.GetBytes(gap_2C));

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
            result.AddRange(BitConverter.GetBytes(field_A8));
            result.AddRange(gap_AC);
            result.Add(field_AF);
            result.AddRange(BitConverter.GetBytes(BonusExp));
            result.AddRange(BitConverter.GetBytes(BattleExp));
            result.AddRange(BitConverter.GetBytes(CurrentSkillPoints));
            result.AddRange(BitConverter.GetBytes(TotalSkillPoints));

            foreach (Weapon w in Weapons)
                result.AddRange(w.ToRawData());

            result.AddRange(gap_2DC);
            result.AddRange(BitConverter.GetBytes(AccessoryId2));
            result.AddRange(gap_2EA);
            result.AddRange(AccessoryHandle2.ToRawData());
            result.AddRange(DriverArtLevels);
            result.AddRange(gap_4FD);
            result.AddRange(AccessoryHandle0.ToRawData());
            result.AddRange(BitConverter.GetBytes(AccessoryId0));
            result.AddRange(BitConverter.GetBytes(gap_57A));
            result.AddRange(AccessoryHandle1.ToRawData());
            result.AddRange(BitConverter.GetBytes(AccessoryId1));
            result.AddRange(BitConverter.GetBytes(gap_582));

            foreach (Pouch p in PouchInfo)
                result.AddRange(p.ToRawData());

            result.AddRange(gap_field_59C);

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
            if (IDNAMES.ContainsKey(ID))
                return ID + ": " + IDNAMES[ID];
            else
                return "INVALID ID";
        }
    }
}
