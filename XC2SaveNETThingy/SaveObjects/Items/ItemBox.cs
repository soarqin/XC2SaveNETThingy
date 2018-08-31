using System;
using System.Collections.Generic;

namespace XC2SaveNETThingy
{
    public class ItemBox : IXC2SaveObject
    {
        public const int SIZE = 0x122EC;

        public Item[] CoreChipBox { get; set; }
        public Item[] AccessoryBox { get; set; }
        public Item[] AuxCoreBox { get; set; }
        public Item[] CylinderBox { get; set; }
        public Item[] KeyItemBox { get; set; }
        public Item[] InfoItemBox { get; set; }
        public Item[] EventBox { get; set; }
        public Item[] CollectibleBox { get; set; }
        public Item[] TreasureBox { get; set; }
        public Item[] UnrefinedAuxCoreBox { get; set; }
        public Item[] PouchItemBox { get; set; }
        public Item[] CoreCrystalBox { get; set; }
        public Item[] BoosterBox { get; set; }
        public Item[] PoppiRoleCPUBox { get; set; }
        public Item[] PoppiElementCoreBox { get; set; }
        public Item[] PoppiSpecialsEnhancingRAMBox { get; set; }
        public Item[] PoppiArtsCardBox { get; set; }
        public Item[] PoppiSkillRAMBox { get; set; }
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
            CoreChipBox = new Item[200];
            for (int i = 0; i < CoreChipBox.Length; i++)
                CoreChipBox[i] = new Item(data.GetByteSubArray(LOC["PcWpnChipBox"] + (i * Item.SIZE), Item.SIZE));

            AccessoryBox = new Item[900];
            for (int i = 0; i < AccessoryBox.Length; i++)
                AccessoryBox[i] = new Item(data.GetByteSubArray(LOC["PcEquipBox"] + (i * Item.SIZE), Item.SIZE));

            AuxCoreBox = new Item[500];
            for (int i = 0; i < AuxCoreBox.Length; i++)
                AuxCoreBox[i] = new Item(data.GetByteSubArray(LOC["EquipOrbBox"] + (i * Item.SIZE), Item.SIZE));

            CylinderBox = new Item[200];
            for (int i = 0; i < CylinderBox.Length; i++)
                CylinderBox[i] = new Item(data.GetByteSubArray(LOC["SalvageBox"] + (i * Item.SIZE), Item.SIZE));

            KeyItemBox = new Item[500];
            for (int i = 0; i < KeyItemBox.Length; i++)
                KeyItemBox[i] = new Item(data.GetByteSubArray(LOC["PreciousBox"] + (i * Item.SIZE), Item.SIZE));

            InfoItemBox = new Item[200];
            for (int i = 0; i < InfoItemBox.Length; i++)
                InfoItemBox[i] = new Item(data.GetByteSubArray(LOC["InfoBox"] + (i * Item.SIZE), Item.SIZE));

            EventBox = new Item[100];
            for (int i = 0; i < EventBox.Length; i++)
                EventBox[i] = new Item(data.GetByteSubArray(LOC["EventBox"] + (i * Item.SIZE), Item.SIZE));

            CollectibleBox = new Item[500];
            for (int i = 0; i < CollectibleBox.Length; i++)
                CollectibleBox[i] = new Item(data.GetByteSubArray(LOC["CollectionListBox"] + (i * Item.SIZE), Item.SIZE));

            TreasureBox = new Item[200];
            for (int i = 0; i < TreasureBox.Length; i++)
                TreasureBox[i] = new Item(data.GetByteSubArray(LOC["TreasureBox"] + (i * Item.SIZE), Item.SIZE));

            UnrefinedAuxCoreBox = new Item[500];
            for (int i = 0; i < UnrefinedAuxCoreBox.Length; i++)
                UnrefinedAuxCoreBox[i] = new Item(data.GetByteSubArray(LOC["EmptyOrbBox"] + (i * Item.SIZE), Item.SIZE));

            PouchItemBox = new Item[500];
            for (int i = 0; i < PouchItemBox.Length; i++)
                PouchItemBox[i] = new Item(data.GetByteSubArray(LOC["FavoriteBox"] + (i * Item.SIZE), Item.SIZE));

            CoreCrystalBox = new Item[200];
            for (int i = 0; i < CoreCrystalBox.Length; i++)
                CoreCrystalBox[i] = new Item(data.GetByteSubArray(LOC["CrystalListBox"] + (i * Item.SIZE), Item.SIZE));

            BoosterBox = new Item[200];
            for (int i = 0; i < BoosterBox.Length; i++)
                BoosterBox[i] = new Item(data.GetByteSubArray(LOC["BoosterBox"] + (i * Item.SIZE), Item.SIZE));
                
            PoppiRoleCPUBox = new Item[200];
            for (int i = 0; i < PoppiRoleCPUBox.Length; i++)
                PoppiRoleCPUBox[i] = new Item(data.GetByteSubArray(LOC["HanaRoleBox"] + (i * Item.SIZE), Item.SIZE));

            PoppiElementCoreBox = new Item[200];
            for (int i = 0; i < PoppiElementCoreBox.Length; i++)
                PoppiElementCoreBox[i] = new Item(data.GetByteSubArray(LOC["HanaAtrBox"] + (i * Item.SIZE), Item.SIZE));

            PoppiSpecialsEnhancingRAMBox = new Item[200];
            for (int i = 0; i < PoppiSpecialsEnhancingRAMBox.Length; i++)
                PoppiSpecialsEnhancingRAMBox[i] = new Item(data.GetByteSubArray(LOC["HanaArtsBox"] + (i * Item.SIZE), Item.SIZE));

            PoppiArtsCardBox = new Item[200];
            for (int i = 0; i < PoppiArtsCardBox.Length; i++)
                PoppiArtsCardBox[i] = new Item(data.GetByteSubArray(LOC["HanaNArtsBox"] + (i * Item.SIZE), Item.SIZE));

            PoppiSkillRAMBox = new Item[700];
            for (int i = 0; i < PoppiSkillRAMBox.Length; i++)
                PoppiSkillRAMBox[i] = new Item(data.GetByteSubArray(LOC["HanaAssistBox"] + (i * Item.SIZE), Item.SIZE));

            Serials = new UInt32[19];
            for (int i = 0; i < Serials.Length; i++)
                Serials[i] = BitConverter.ToUInt32(data.GetByteSubArray(LOC["Serials"] + (i * 4), 4), 0);
        }

        public Byte[] ToRawData()
        {
            List<Byte> result = new List<Byte>();

            foreach (Item i in CoreChipBox)
                result.AddRange(i.ToRawData());

            foreach (Item i in AccessoryBox)
                result.AddRange(i.ToRawData());

            foreach (Item i in AuxCoreBox)
                result.AddRange(i.ToRawData());

            foreach (Item i in CylinderBox)
                result.AddRange(i.ToRawData());

            foreach (Item i in KeyItemBox)
                result.AddRange(i.ToRawData());

            foreach (Item i in InfoItemBox)
                result.AddRange(i.ToRawData());

            foreach (Item i in EventBox)
                result.AddRange(i.ToRawData());

            foreach (Item i in CollectibleBox)
                result.AddRange(i.ToRawData());

            foreach (Item i in TreasureBox)
                result.AddRange(i.ToRawData());

            foreach (Item i in UnrefinedAuxCoreBox)
                result.AddRange(i.ToRawData());

            foreach (Item i in PouchItemBox)
                result.AddRange(i.ToRawData());

            foreach (Item i in CoreCrystalBox)
                result.AddRange(i.ToRawData());

            foreach (Item i in BoosterBox)
                result.AddRange(i.ToRawData());

            foreach (Item i in PoppiRoleCPUBox)
                result.AddRange(i.ToRawData());

            foreach (Item i in PoppiElementCoreBox)
                result.AddRange(i.ToRawData());

            foreach (Item i in PoppiSpecialsEnhancingRAMBox)
                result.AddRange(i.ToRawData());

            foreach (Item i in PoppiArtsCardBox)
                result.AddRange(i.ToRawData());

            foreach (Item i in PoppiSkillRAMBox)
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
