using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XC2SaveNETThingy
{
    public class ItemBox : IXC2SaveObject
    {
        public static readonly Dictionary<int, string> BOOSTERS = new Dictionary<int, string>()
        {
            { 0, "(none)" },
            { 1, "Bravery Booster" },
            { 2, "Truth Booster" },
            { 3, "Compassion Booster" },
            { 4, "Justice Booster" }
        };

        public static readonly Dictionary<int, string> CYLINDERS = new Dictionary<int, string>()
        {
            { 1, "Normal Cylinder" },
            { 2, "Silver Cylinder" },
            { 3, "Golden  Cylinder" },
            { 4, "Premium Cylinder" }
        };

        public static readonly Dictionary<int, string> HANA_ROLECPU = new Dictionary<int, string>()
        {
            { 4, "Tank Mod IV" },
            { 9, "Evasion Mod IV" },
            { 12, "Muscle Mod II" },
            { 6, "Evasion Mod I" },
            { 2, "Tank Mod II" },
            { 1, "Tank Mod I" },
        };

        public static readonly Dictionary<int, string> HANA_ElementCores = new Dictionary<int, string>()
        {
            { 1, "Elemental Core: Fire" },
            { 2, "Elemental Core: Water" },
            { 3, "Elemental Core: Electric" },
            { 4, "Elemental Core: Earth" },
            { 5, "Elemental Core: Wind" },
            { 6, "Elemental Core: Ice" },
            { 7, "Elemental Core: Light" },
            { 8, "Elemental Core: Dark" }
        };

        public static readonly Dictionary<int, string> HANA_SpecialsEnhancingRam = new Dictionary<int, string>()
        {
            { 98, "AB HP Potion III" },
            { 71, "AB Avenger I" },
            { 61, "AB Launch I" },
            { 63, "AB Launch III" },
            { 38, "AB HP Absorb Arts III" },
            { 46, "AB HP Potion III" }
        };

        public static readonly Dictionary<int, string> HANA_ArtsCards = new Dictionary<int, string>()
        {
            { 43, "AC Aggro Draw III" },
            { 6, "AC Accuracy Up I" },
            { 11, "AC Arts Boost I" },
            { 21, "AC Damage Absorb I" },
            { 26, "AC Reaction Nullify I" },
            { 16, "AC Recharge Boost I" },
            { 41, "AC Aggro Draw I" },
            { 8, "AC Accuracy Up III" }
        };

        public const int SIZE = 0x122EC;

        public Item[] PcWpnChipBox { get; set; }
        public Item[] PcEquipBox { get; set; }
        public Item[] EquipOrbBox { get; set; }
        public Item[] SalvageBox { get; set; }
        public Item[] PreciousBox { get; set; }
        public Item[] InfoBox { get; set; }
        public Item[] EventBox { get; set; }
        public Item[] CollectionListBox { get; set; }
        public Item[] TreasureBox { get; set; }
        public Item[] EmptyOrbBox { get; set; }
        public Item[] FavoriteBox { get; set; }
        public Item[] CrystalListBox { get; set; }
        public Item[] BoosterBox { get; set; }
        public Item[] HanaRoleBox { get; set; }
        public Item[] HanaAtrBox { get; set; }
        public Item[] HanaArtsBox { get; set; }
        public Item[] HanaNArtsBox { get; set; }
        public Item[] HanaAssistBox { get; set; }
        public UInt32[] Serials { get; set; }

        private static readonly Dictionary<string, int> LOC = new Dictionary<string, int>()
        {
            { "PcWpnChipBox", 0x0 },
            { "PcEquipBox", 0x960 },
            { "EquipOrbBox", 0x3390 },
            { "SalvageBox", 0x4B00 },
            { "PreciousBox", 0x5460 },
            { "InfoBox", 0x6BD0 },
            { "EventBox", 0x7530 },
            { "CollectionListBox", 0x79E0 },
            { "TreasureBox", 0x9150 },
            { "EmptyOrbBox", 0x9AB0 },
            { "FavoriteBox", 0xB220 },
            { "CrystalListBox", 0xC990 },
            { "BoosterBox", 0xD2F0 },
            { "HanaRoleBox", 0xDC50 },
            { "HanaAtrBox", 0xE5B0 },
            { "HanaArtsBox", 0xEF10 },
            { "HanaNArtsBox", 0xF870 },
            { "HanaAssistBox", 0x101D0 },
            { "Serials", 0x122A0 }
        };

        public ItemBox(Byte[] data)
        {
            PcWpnChipBox = new Item[200];
            for (int i = 0; i < PcWpnChipBox.Length; i++)
                PcWpnChipBox[i] = new Item(data.GetByteSubArray(LOC["PcWpnChipBox"] + (i * Item.SIZE), Item.SIZE));

            PcEquipBox = new Item[900];
            for (int i = 0; i < PcEquipBox.Length; i++)
                PcEquipBox[i] = new Item(data.GetByteSubArray(LOC["PcEquipBox"] + (i * Item.SIZE), Item.SIZE));

            EquipOrbBox = new Item[500];
            for (int i = 0; i < EquipOrbBox.Length; i++)
                EquipOrbBox[i] = new Item(data.GetByteSubArray(LOC["EquipOrbBox"] + (i * Item.SIZE), Item.SIZE));

            SalvageBox = new Item[200];
            for (int i = 0; i < SalvageBox.Length; i++)
                SalvageBox[i] = new Item(data.GetByteSubArray(LOC["SalvageBox"] + (i * Item.SIZE), Item.SIZE));

            PreciousBox = new Item[500];
            for (int i = 0; i < PreciousBox.Length; i++)
                PreciousBox[i] = new Item(data.GetByteSubArray(LOC["PreciousBox"] + (i * Item.SIZE), Item.SIZE));

            InfoBox = new Item[200];
            for (int i = 0; i < InfoBox.Length; i++)
                InfoBox[i] = new Item(data.GetByteSubArray(LOC["InfoBox"] + (i * Item.SIZE), Item.SIZE));

            EventBox = new Item[100];
            for (int i = 0; i < EventBox.Length; i++)
                EventBox[i] = new Item(data.GetByteSubArray(LOC["EventBox"] + (i * Item.SIZE), Item.SIZE));

            CollectionListBox = new Item[500];
            for (int i = 0; i < CollectionListBox.Length; i++)
                CollectionListBox[i] = new Item(data.GetByteSubArray(LOC["CollectionListBox"] + (i * Item.SIZE), Item.SIZE));

            TreasureBox = new Item[200];
            for (int i = 0; i < TreasureBox.Length; i++)
                TreasureBox[i] = new Item(data.GetByteSubArray(LOC["TreasureBox"] + (i * Item.SIZE), Item.SIZE));

            EmptyOrbBox = new Item[500];
            for (int i = 0; i < EmptyOrbBox.Length; i++)
                EmptyOrbBox[i] = new Item(data.GetByteSubArray(LOC["EmptyOrbBox"] + (i * Item.SIZE), Item.SIZE));

            FavoriteBox = new Item[500];
            for (int i = 0; i < FavoriteBox.Length; i++)
                FavoriteBox[i] = new Item(data.GetByteSubArray(LOC["FavoriteBox"] + (i * Item.SIZE), Item.SIZE));

            CrystalListBox = new Item[200];
            for (int i = 0; i < CrystalListBox.Length; i++)
                CrystalListBox[i] = new Item(data.GetByteSubArray(LOC["CrystalListBox"] + (i * Item.SIZE), Item.SIZE));

            BoosterBox = new Item[200];
            for (int i = 0; i < BoosterBox.Length; i++)
                BoosterBox[i] = new Item(data.GetByteSubArray(LOC["BoosterBox"] + (i * Item.SIZE), Item.SIZE));
                
            HanaRoleBox = new Item[200];
            for (int i = 0; i < HanaRoleBox.Length; i++)
                HanaRoleBox[i] = new Item(data.GetByteSubArray(LOC["HanaRoleBox"] + (i * Item.SIZE), Item.SIZE));

            HanaAtrBox = new Item[200];
            for (int i = 0; i < HanaAtrBox.Length; i++)
                HanaAtrBox[i] = new Item(data.GetByteSubArray(LOC["HanaAtrBox"] + (i * Item.SIZE), Item.SIZE));

            HanaArtsBox = new Item[200];
            for (int i = 0; i < HanaArtsBox.Length; i++)
                HanaArtsBox[i] = new Item(data.GetByteSubArray(LOC["HanaArtsBox"] + (i * Item.SIZE), Item.SIZE));

            HanaNArtsBox = new Item[200];
            for (int i = 0; i < HanaNArtsBox.Length; i++)
                HanaNArtsBox[i] = new Item(data.GetByteSubArray(LOC["HanaNArtsBox"] + (i * Item.SIZE), Item.SIZE));

            HanaAssistBox = new Item[700];
            for (int i = 0; i < HanaAssistBox.Length; i++)
                HanaAssistBox[i] = new Item(data.GetByteSubArray(LOC["HanaAssistBox"] + (i * Item.SIZE), Item.SIZE));

            Serials = new UInt32[19];
            for (int i = 0; i < Serials.Length; i++)
                Serials[i] = BitConverter.ToUInt32(data.GetByteSubArray(LOC["Serials"] + (i * 4), 4), 0);
        }

        public Byte[] ToRawData()
        {
            List<Byte> result = new List<Byte>();

            foreach (Item i in PcWpnChipBox)
                result.AddRange(i.ToRawData());

            foreach (Item i in PcEquipBox)
                result.AddRange(i.ToRawData());

            foreach (Item i in EquipOrbBox)
                result.AddRange(i.ToRawData());

            foreach (Item i in SalvageBox)
                result.AddRange(i.ToRawData());

            foreach (Item i in PreciousBox)
                result.AddRange(i.ToRawData());

            foreach (Item i in InfoBox)
                result.AddRange(i.ToRawData());

            foreach (Item i in EventBox)
                result.AddRange(i.ToRawData());

            foreach (Item i in CollectionListBox)
                result.AddRange(i.ToRawData());

            foreach (Item i in TreasureBox)
                result.AddRange(i.ToRawData());

            foreach (Item i in EmptyOrbBox)
                result.AddRange(i.ToRawData());

            foreach (Item i in FavoriteBox)
                result.AddRange(i.ToRawData());

            foreach (Item i in CrystalListBox)
                result.AddRange(i.ToRawData());

            foreach (Item i in BoosterBox)
                result.AddRange(i.ToRawData());

            foreach (Item i in HanaRoleBox)
                result.AddRange(i.ToRawData());

            foreach (Item i in HanaAtrBox)
                result.AddRange(i.ToRawData());

            foreach (Item i in HanaArtsBox)
                result.AddRange(i.ToRawData());

            foreach (Item i in HanaNArtsBox)
                result.AddRange(i.ToRawData());

            foreach (Item i in HanaAssistBox)
                result.AddRange(i.ToRawData());

            foreach (UInt32 u in Serials)
                result.AddRange(BitConverter.GetBytes(u));

            if (result.Count != SIZE)
            {
                string message = "ItemBox: SIZE ALL WRONG!!!" + Environment.NewLine +
                "Size should be " + SIZE + " bytes..." + Environment.NewLine +
                "...but Size is " + result.Count + " bytes!";

                throw new Exception(message);
            }

            return result.ToArray();
        }

        public class Item : IXC2SaveObject
        {
            public const int SIZE = 0xC;

            public UInt16 ID { get; set; }
            private const int ID_BITS = 13;

            public Byte Type { get; set; }
            private const int TYPE_BITS = 6;

            public UInt16 Qty { get; set; }
            private const int QTY_BITS = 10;

            public bool Unk1 { get; set; }
            private const int UNK1_BITS = 1;

            public bool Equipped { get; set; }
            private const int EQUIPPED_BITS = 1;

            public bool Unk2 { get; set; }
            private const int UNK2_BITS = 1;

            public ElapseTime Time { get; set; }

            public UInt32 Serial { get; set; }
            private const int SERIAL_BITS = 26;

            public Byte Unk3 { get; set; }
            private const int UNK3_BITS = 6;

            public string Name { get { return ToString(); } }


            public Item(Byte[] data)
            {
                UInt32 pt1 = BitConverter.ToUInt32(data.GetByteSubArray(0, 4), 0);

                ID = (UInt16)(pt1 & (UInt16)(Math.Pow(2, ID_BITS) - 1));
                Type = (Byte)((pt1 >> ID_BITS) & (Byte)((Math.Pow(2, TYPE_BITS) - 1)));
                Qty = (UInt16)((pt1 >> (ID_BITS + TYPE_BITS)) & (UInt16)(Math.Pow(2, QTY_BITS) - 1));
                Unk1 = ((pt1 >> (ID_BITS + TYPE_BITS + QTY_BITS)) & (UInt32)(Math.Pow(2, UNK1_BITS) - 1)) == 1;
                Equipped = ((pt1 >> (ID_BITS + TYPE_BITS + QTY_BITS + UNK1_BITS)) & (UInt32)(Math.Pow(2, EQUIPPED_BITS) - 1)) == 1;
                Unk2 = (pt1 >> (32 - UNK2_BITS)) == 1;

                Time = new ElapseTime(data.GetByteSubArray(4, 4));

                UInt32 pt2 = BitConverter.ToUInt32(data.GetByteSubArray(8, 4), 0);

                Serial = (UInt32)(pt2 & (UInt32)(Math.Pow(2, SERIAL_BITS) - 1));
                Unk3 = (Byte)(pt2 >> (32 - UNK3_BITS));
            }

            //public Item()
            //{
            //    ID = 0;
            //    Type = 0;
            //    Qty = 0;
            //    Unk1 = false;
            //    Equipped = false;
            //    Unk2 = false;
            //    Time = new gf_ElapseTime();
            //    Serial = 0;
            //    Unk3 = 0;
            //}

            public Byte[] ToRawData()
            {
                UInt32 pt1 = (UInt32)((Unk2 ? 1 : 0) << (31 - UNK2_BITS)) +
                    (UInt32)((Equipped ? 1 : 0) << (ID_BITS + TYPE_BITS + QTY_BITS + UNK1_BITS)) +
                    (UInt32)((Unk1 ? 1 : 0) << (ID_BITS + TYPE_BITS + QTY_BITS)) +
                    (UInt32)(Qty << (ID_BITS + TYPE_BITS)) +
                    (UInt32)(Type << ID_BITS) +
                    ID;

                UInt32 pt2 = (UInt32)(Unk3 << SERIAL_BITS) + Serial;

                List<Byte> result = new List<byte>();

                result.AddRange(BitConverter.GetBytes(pt1));
                result.AddRange(Time.ToRawData());
                result.AddRange(BitConverter.GetBytes(pt2));

                if (result.Count != SIZE)
                {
                    string message = "ItemBox.Item: SIZE ALL WRONG!!!" + Environment.NewLine +
                    "Size should be " + SIZE + " bytes..." + Environment.NewLine +
                    "...but Size is " + result.Count + " bytes!";

                    throw new Exception(message);
                }

                return result.ToArray();
            }

            public override string ToString()
            {
                switch (Type)
                {
                    case 12:
                        return BOOSTERS[ID];
                }

                return ID.ToString();
            }
        }

    }
}
