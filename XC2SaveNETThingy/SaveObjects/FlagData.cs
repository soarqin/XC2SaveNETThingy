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

        public static readonly Dictionary<int, string> NAMES_1BIT = new Dictionary<int, string>()
        {
            { 35323, "Pneuma Name (UnChk=Pyra, Chk=Mythra)" },
            { 46988, "Roc's Awakening Viewed" },
            { 46989, "Aegaeon's Awakening Viewed" },
            { 46990, "Godfrey's Awakening Viewed" },
            { 46991, "Wulfric's Awakening Viewed" },
            { 46992, "Perceval's Awakening Viewed" },
            { 46993, "Vale's Awakening Viewed" },
            { 46994, "Agate's Awakening Viewed" },
            { 46995, "Gorg's Awakening Viewed" },
            { 46996, "Boreas's Awakening Viewed" },
            { 46997, "Dagas's Awakening Viewed" },
            { 46998, "Kasandra's Awakening Viewed" },
            { 46999, "Praxis's Awakening Viewed" },
            { 47000, "Theory's Awakening Viewed" },
            { 47001, "Perun's Awakening Viewed" },
            { 47002, "Kora's Awakening Viewed" },
            { 47003, "Azami's Awakening Viewed" },
            { 47004, "Ursula's Awakening Viewed" },
            { 47005, "Newt's Awakening Viewed" },
            { 47006, "Nim's Awakening Viewed" },
            { 47007, "Sheba's Awakening Viewed" },
            { 47008, "Vess's Awakening Viewed" },
            { 47009, "Adenine's Awakening Viewed" },
            { 47010, "Electra's Awakening Viewed" },
            { 47011, "Zenobia's Awakening Viewed" },
            { 47012, "Finch's Awakening Viewed" },
            { 47013, "Floren's Awakening Viewed" },
            { 47014, "KOS-MOS's Awakening Viewed" },
            { 47015, "Herald's Awakening Viewed" },
            { 47016, "Dahlia's Awakening Viewed" },
            { 47017, "Akhos's Awakening Viewed" },
            { 47018, "Patroka's Awakening Viewed" },
            { 47019, "Sever's Awakening Viewed" },
            { 47020, "Cressidus's Awakening Viewed" },
            { 47021, "Perdido's Awakening Viewed" },
            { 47022, "Obrona's Awakening Viewed" },
            { 47023, "Greataxe Male's Awakening Viewed" },
            { 47024, "Megalance Male's Awakening Viewed" },
            { 47025, "Ether Cannon Male's Awakening Viewed" },
            { 47026, "Shield Hammer Male's Awakening Viewed" },
            { 47027, "Chroma Katana Male's Awakening Viewed" },
            { 47028, "Bitball Male's Awakening Viewed" },
            { 47029, "Knuckle Claws Male's Awakening Viewed" },
            { 47030, "Greataxe Female's Awakening Viewed" },
            { 47031, "Megalance Female's Awakening Viewed" },
            { 47032, "Ether Cannon Female's Awakening Viewed" },
            { 47033, "Shield Hammer Female's Awakening Viewed" },
            { 47034, "Chroma Katana Female's Awakening Viewed" },
            { 47035, "Bitball Female's Awakening Viewed" },
            { 47036, "Knuckle Claws Female's Awakening Viewed" },
            { 47037, "Greataxe Brute's Awakening Viewed" },
            { 47038, "Megalance Brute's Awakening Viewed" },
            { 47039, "Ether Cannon Brute's Awakening Viewed" },
            { 47040, "Shield Hammer Brute's Awakening Viewed" },
            { 47041, "Chroma Katana Brute's Awakening Viewed" },
            { 47042, "Bitball Brute's Awakening Viewed" },
            { 47043, "Knuckle Claws Brute's Awakening Viewed" },
            { 47044, "Twin Rings Beast's Awakening Viewed" },
            { 47045, "T-elos's Awakening Viewed" },
            { 47046, "Corvin's Awakening Viewed" },
            { 47047, "Crossette's Awakening Viewed" }
        };

        public static readonly Dictionary<int, string> NAMES_8BIT = new Dictionary<int, string>()
        {
            { 2127, "Ursula Vocal Level" },
            { 2128, "Ursula Looks Level" },
            { 2129, "Ursula Soul Level" },
            { 2132, "Salvager Points" }
        };

        public static readonly Dictionary<int, string> NAMES_16BIT = new Dictionary<int, string>()
        {
            { 1550, "Ursula Fans Count" }
        };

        public static readonly Dictionary<int, string> NAMES_32BIT = new Dictionary<int, string>()
        {
            //{ 0, "" },
            { 258, "No. of Blades Created" },
            { 259, "No. of G Obtained" },
            { 260, "No. of Peds Walked" },
            { 261, "No. of Peds Swam" },
            { 262, "No. of Enemies Defeated" },
            { 263, "No. of Critical Hits Dealt" },
            { 264, "STAT_116" },
            { 265, "STAT_KIZUNA_TOTAL" },
            { 266, "STAT_120" },
            { 267, "No. of Driver Combo Finishers Performed" },
            { 268, "STAT_124" },
            { 269, "No. Blade Combos Finished" },
            { 270, "No. of Chain Attacks Performed" },
            { 271, "No. of Button Challenges Completed" },
            { 272, "No. of Times Team Members Helped Up" },
            { 273, "STAT_POT_GET_TOTAL" },
            { 274, "No. of Attacks Dodged" },
            { 275, "No. of Attacks Blocked" },
            { 276, "No. of Blade Swaps Performed" },
            { 277, "STAT_DAMAGE_ADD_TOTAL" },
            { 278, "STAT_DAMAGE_TOTAL" },
            { 279, "STAT_146" },
            { 280, "STAT_RECOVER_TOTAL" },
            { 281, "No. of Landmarks Discovered" },
            { 282, "No. of Locations Discovered" },
            { 283, "No. of Secret Areas Discovered" },
            { 284, "STAT_TALK_TOTAL" },
            { 285, "STAT_JUMP" },
            { 286, "STAT_SALVAGE" },
            { 287, "STAT_TALK_FIRST" },
            { 288, "STAT_MISSION_CLEAR" },
            { 289, "STAT_BUY" },
            { 290, "STAT_SELL" },
            { 291, "STAT_COLLECT_TOTAL" },
            { 292, "STAT_COLLECT_TRESURE" },
            { 293, "STAT_PORCH" },
            { 294, "No. of Quests Completed" },
            { 295, "No. of G Spent" },
            { 296, "No. of G Obtained from Sales" },
            { 297, "STAT_SALVAGE_SHOP" },
            { 298, "STAT_BUY_SALVAGE" },
            { 299, "STAT_STAY" },
            { 300, "No. of Treasure Troves Opened" },
            { 301, "STAT_BATTLE_TOTAL" },
            { 302, "STAT_COLLECT_GET_TOTAL" },
            { 303, "STAT_BATTLE_COUNT1" },
            { 304, "STAT_BATTLE_COUNT2" },
            { 305, "STAT_BATTLE_COUNT3" },
            { 306, "STAT_BATTLE_COUNT4" },
            { 307, "STAT_RINNE_GOLD" },
            { 308, "No. of Dishes Cooked by Pyra"},
            { 309, "STAT_MAKE_COUNT_2" },
            { 310, "STAT_MAKE_COUNT_3" },
            { 311, "STAT_MAKE_COUNT_4" },
            { 312, "STAT_MAKE_COUNT_5" },
            { 313, "STAT_KIZUNATALK_TOTAL" },
            { 314, "STAT_CB_CUBE_TOTAL" },
            { 315, "STAT_CB_TOTAL" },
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
