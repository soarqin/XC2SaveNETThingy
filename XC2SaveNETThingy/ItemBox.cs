using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XC2SaveNETThingy
{
    public class ItemBox : IXC2SaveObject
    {
        public Item[] PcWpnChipBox;
        public Item[] PcEquipBox;
        public Item[] EquipOrbBox;
        public Item[] SalvageBox;
        public Item[] PreciousBox;
        public Item[] InfoBox;
        public Item[] EventBox;
        public Item[] CollectionListBox;
        public Item[] TreasureVox;
        public Item[] EmptyOrbBox;
        public Item[] FavoriteBox;
        public Item[] CrystalListBox;
        public Item[] BoosterBox;
        public Item[] HanaRoleBox;
        public Item[] HanaAtrBox;
        public Item[] HanaArtsBox;
        public Item[] HanaNArtsBox;
        public Item[] HanaAssistBox;
        public UInt32[] Serials;

        private static readonly Dictionary<string, int> LOC = new Dictionary<string, int>()
        {
            { "PcWpnChipBox", 0x0 },
            { "PcEquipBox", 200 * Item.SIZE },
            { "EquipOrbBox", 900 * Item.SIZE },
            { "SalvageBox", 500 * Item.SIZE },
            { "PreciousBox", 200 * Item.SIZE },
            { "InfoBox", 500 * Item.SIZE },
            { "EventBox", 200 * Item.SIZE },
            { "CollectionListBox", 100 * Item.SIZE },
            { "BoosterBox", 500 * Item.SIZE },
            { "HanaRoleBox", 200 * Item.SIZE },
            { "HanaAtrBox", 200 * Item.SIZE },
            { "HanaArtsBox", 200 * Item.SIZE },
            { "HanaNArtsBox", 200 * Item.SIZE },
            { "HanaAssistBox", 200 * Item.SIZE },
            { "Serials", 700 * Item.SIZE }
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
            throw new NotImplementedException();
        }

        public class Item : IXC2SaveObject
        {
            public const int SIZE = 0xC;

            //uint32 Id : 13;
            //uint32 Type : 6;
            //uint32 Count : 10;
            //uint32 Unk1 : 1;
            //uint32 Equipped : 1;
            //uint32 Unk2 : 1;
            //gf_ElapseTime Time;
            //uint32 Serial : 26;
            //uint32 Unk3 : 6;

            UInt16 ID;
            static int ID_BITS = 13;

            Byte Type;
            static int TYPE_BITS = 6;

            UInt16 Qty;
            static int QTY_BITS = 10;

            bool Unk1;
            static int UNK1_BITS = 1;

            bool Equipped;
            static int EQUIPPED_BITS = 1;

            bool Unk2;
            static int UNK2_BITS = 1;

            gf_ElapseTime Time;

            UInt32 Serial;
            static int SERIAL_BITS = 26;

            Byte Unk3;
            static int UNK3_BITS = 6;


            public Item(Byte[] data)
            {
                UInt32 v1 = BitConverter.ToUInt32(new Byte[] { data[0], data[1], data[2], data[3] }, 0);

                ID = (UInt16)(v1 & ((2 ^ ID_BITS) - 1));
                Type = (Byte)((v1 >> ID_BITS) & ((2 ^ TYPE_BITS) - 1));
                Qty = (UInt16)((v1 >> (ID_BITS + TYPE_BITS)) & ((2 ^ QTY_BITS) - 1));
                Unk1 = ((v1 >> (ID_BITS + TYPE_BITS + QTY_BITS)) & ((2 ^ UNK1_BITS) - 1)) == 1;
                Equipped = ((v1 >> (ID_BITS + TYPE_BITS + QTY_BITS + UNK1_BITS)) & ((2 ^ EQUIPPED_BITS) - 1)) == 1;
                Unk2 = (v1 >> (32 - UNK2_BITS)) == 1;

                Time = new gf_ElapseTime(new Byte[] { data[4], data[5], data[6], data[7] });

                UInt32 v2 = BitConverter.ToUInt32(new Byte[] { data[11], data[10], data[9], data[8] }, 0);

                Serial = (UInt32)(v2 & ((2 ^ SERIAL_BITS) - 1));
                Unk3 = (Byte)(v2 >> (32 - UNK3_BITS));
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
                throw new NotImplementedException();
            }
        }

    }
}
