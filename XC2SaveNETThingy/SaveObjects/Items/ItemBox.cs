using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XC2SaveNETThingy
{
    public class ItemBox : IXC2SaveObject
    {
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
    }
}
