using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XC2SaveNETThingy
{
    public class Item : IXC2SaveObject
    {
        public enum TYPE
        {
            NONE = 0,
            CoreChip = 1,
            Accessory = 2,
            AuxCore = 3,
            UNKNOWN_4 = 4,
            Cylinder = 5,
            KeyItem = 6,
            Collectible = 7,
            Treasure = 8,
            UnrefinedAuxCore = 9,
            PouchItem = 10,
            CoreCrystal = 11,
            Booster = 12,
            UNKNOWN_13 = 13,
            InfoItem = 14,
            UNKNOWN_15 = 15,
            PoppiRoleCPU = 16,
            PoppiElementCore = 17,
            PoppiSpecialsEnhancingRAM = 18,
            PoppiArtsCard = 19,
            PoppiSkillRAM = 20
        }

        public const int SIZE = 0xC;

        private UInt16 _oldId;
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

        private string _name;
        public string Name
        {
            get
            {
                if (ID != _oldId)
                {
                    SetName();
                }

                return _name;
            }
            private set
            {
                _name = value;
            }
        }

        // janky properties to allow DataGridView to access Time sub-properties
        public UInt32 Time_Hours
        {
            get
            {
                return Time.Hours;
            }

            set
            {
                Time.Hours = value;
            }
        }

        public Byte Time_Minutes
        {
            get
            {
                return Time.Minutes;
            }

            set
            {
                Time.Minutes = value;
            }
        }

        public Byte Time_Seconds
        {
            get
            {
                return Time.Seconds;
            }

            set
            {
                Time.Seconds = value;
            }
        }


        public Item(Byte[] data)
        {
            UInt32 pt1 = BitConverter.ToUInt32(data.GetByteSubArray(0, 4), 0);

            ID = (UInt16)(pt1 & (UInt16)(Math.Pow(2, ID_BITS) - 1));
            _oldId = ID;
            Type = (Byte)((pt1 >> ID_BITS) & (Byte)((Math.Pow(2, TYPE_BITS) - 1)));
            Qty = (UInt16)((pt1 >> (ID_BITS + TYPE_BITS)) & (UInt16)(Math.Pow(2, QTY_BITS) - 1));
            Unk1 = ((pt1 >> (ID_BITS + TYPE_BITS + QTY_BITS)) & (UInt32)(Math.Pow(2, UNK1_BITS) - 1)) == 1;
            Equipped = ((pt1 >> (ID_BITS + TYPE_BITS + QTY_BITS + UNK1_BITS)) & (UInt32)(Math.Pow(2, EQUIPPED_BITS) - 1)) == 1;
            Unk2 = (pt1 >> (32 - UNK2_BITS)) == 1;

            Time = new ElapseTime(data.GetByteSubArray(4, 4));

            UInt32 pt2 = BitConverter.ToUInt32(data.GetByteSubArray(8, 4), 0);

            Serial = (UInt32)(pt2 & (UInt32)(Math.Pow(2, SERIAL_BITS) - 1));
            Unk3 = (Byte)(pt2 >> (32 - UNK3_BITS));

            //SetName();
        }

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

        private void SetName()
        {
            switch ((TYPE)Type)
            {
                case TYPE.NONE:
                    Name = "(none)";
                    break;

                case TYPE.Accessory:
                    Name =  (string)XC2Data.GetAccessories().Select("ID = " + ID)[0]["Name"];
                    break;

                case TYPE.AuxCore:
                    Name = (string)XC2Data.GetAuxCores().Select("ID = " + ID)[0]["Name"];
                    break;

                case TYPE.Booster:
                    Name = (string)XC2Data.GetBoosters().Select("ID = " + ID)[0]["Name"];
                    break;

                case TYPE.Collectible:
                    Name = (string)XC2Data.GetCollectibles().Select("ID = " + ID)[0]["Name"];
                    break;

                case TYPE.CoreChip:
                    Name = (string)XC2Data.GetCoreChips().Select("ID = " + ID)[0]["Name"];
                    break;

                case TYPE.CoreCrystal:
                    Name = (string)XC2Data.GetCoreCrystals().Select("ID = " + ID)[0]["Name"];
                    break;

                case TYPE.Cylinder:
                    Name = (string)XC2Data.GetCylinders().Select("ID = " + ID)[0]["Name"];
                    break;

                case TYPE.InfoItem:
                    Name = (string)XC2Data.GetInfoItems().Select("ID = " + ID)[0]["Name"];
                    break;


                case TYPE.KeyItem:
                    Name = (string)XC2Data.GetKeyItems().Select("ID = " + ID)[0]["Name"];
                    break;

                case TYPE.PoppiArtsCard:
                    Name = (string)XC2Data.GetPoppiArtsCards().Select("ID = " + ID)[0]["Name"];
                    break;

                case TYPE.PoppiElementCore:
                    Name = (string)XC2Data.GetPoppiElementCores().Select("ID = " + ID)[0]["Name"];
                    break;

                case TYPE.PoppiRoleCPU:
                    Name = (string)XC2Data.GetPoppiRoleCPU().Select("ID = " + ID)[0]["Name"];
                    break;

                case TYPE.PoppiSkillRAM:
                    Name = (string)XC2Data.GetPoppiSkillRAM().Select("ID = " + ID)[0]["Name"];
                    break;

                case TYPE.PoppiSpecialsEnhancingRAM:
                    Name = (string)XC2Data.GetPoppiSpecialsEnhancingRAM().Select("ID = " + ID)[0]["Name"];
                    break;

                case TYPE.PouchItem:
                    Name = (string)XC2Data.GetPouchItems().Select("ID = " + ID)[0]["Name"];
                    break;

                case TYPE.Treasure:
                    Name = (string)XC2Data.GetTreasure().Select("ID = " + ID)[0]["Name"];
                    break;

                case TYPE.UnrefinedAuxCore:
                    Name = (string)XC2Data.GetUnrefinedAuxCores().Select("ID = " + ID)[0]["Name"];
                    break;

                default:
                    Name = "UNKNOWN";
                    break;
            }
        }

        public override string ToString()
        {
            return Name;
        }

        public bool IsEmptyItem()
        {
            return (
                ID == 0 &&
                Type == 0 &&
                Qty == 0 &&
                !Unk1 &&
                !Equipped &&
                !Unk2 &&
                Time.Hours == 0 &&
                Time.Minutes == 0 &&
                Time.Seconds == 0 &&
                Serial == 0 &&
                Unk3 == 0
                );
        }

        public enum CMP_FIELD
        {
            ID,
            NAME,
            QTY,
            EQUIPPED,
            TIME,
            SERIAL
        }

        public static IComparer GetComparer(CMP_FIELD f, bool desc)
        {
            switch (f)
            {
                case CMP_FIELD.ID:
                default:
                    if (desc)
                        return new ItemComparerIDDesc();
                    else
                        return new ItemComparerIDAsc();

                case CMP_FIELD.NAME:
                    if (desc)
                        return new ItemComparerNameDesc();
                    else
                        return new ItemComparerNameAsc();

                case CMP_FIELD.QTY:
                    if (desc)
                        return new ItemComparerQtyDesc();
                    else
                        return new ItemComparerQtyAsc();

                case CMP_FIELD.EQUIPPED:
                    if (desc)
                        return new ItemComparerNotEquip();
                    else
                        return new ItemComparerEquip();

                case CMP_FIELD.TIME:
                    if (desc)
                        return new ItemComparerTimeDesc();
                    else
                        return new ItemComparerTimeAsc();

                case CMP_FIELD.SERIAL:
                    if (desc)
                        return new ItemComparerSerialDesc();
                    else
                        return new ItemComparerSerialAsc();
            }
        }

        private class ItemComparerIDAsc : Comparer<Item>
        {
            public override int Compare(Item x, Item y)
            {
                if (x.IsEmptyItem())
                    return 1;
                else if (y.IsEmptyItem())
                    return -1;
                else
                    if (x.ID < y.ID)
                    return -1;
                else if (x.ID > y.ID)
                    return 1;
                else
                    return 0;
            }
        }

        private class ItemComparerIDDesc : Comparer<Item>
        {
            public override int Compare(Item x, Item y)
            {
                if (x.IsEmptyItem())
                    return 1;
                else if (y.IsEmptyItem())
                    return -1;
                else
                    if (x.ID > y.ID)
                        return -1;
                    else if (x.ID < y.ID)
                        return 1;
                    else
                        return 0;
            }
        }

        //private class ItemComparerNameAsc : Comparer<Item>
        //{
        //    public override int Compare(Item x, Item y)
        //    {
        //        if (x.IsEmptyItem())
        //            return 1;
        //        else if (y.IsEmptyItem())
        //            return -1;
        //        else
        //            return String.Compare(x.Name, y.Name);
        //    }
        //}

        //private class ItemComparerNameDesc : Comparer<Item>
        //{
        //    public override int Compare(Item x, Item y)
        //    {
        //        if (x.IsEmptyItem())
        //            return 1;
        //        else if (y.IsEmptyItem())
        //            return -1;
        //        else
        //            return String.Compare(x.Name, y.Name) * -1;
        //    }
        //}

        private class ItemComparerQtyAsc : Comparer<Item>
        {
            public override int Compare(Item x, Item y)
            {
                if (x.IsEmptyItem())
                    return 1;
                else if (y.IsEmptyItem())
                    return -1;
                else
                    if (x.Qty < y.Qty)
                        return -1;
                    else if (x.Qty > y.Qty)
                        return 1;
                    else
                        return 0;
            }
        }

        private class ItemComparerQtyDesc : Comparer<Item>
        {
            public override int Compare(Item x, Item y)
            {
                if (x.IsEmptyItem())
                    return 1;
                else if (y.IsEmptyItem())
                    return -1;
                else
                    if (x.Qty > y.Qty)
                    return -1;
                else if (x.Qty < y.Qty)
                    return 1;
                else
                    return 0;
            }
        }

        private class ItemComparerTimeAsc : Comparer<Item>
        {
            public override int Compare(Item x, Item y)
            {
                if (x.IsEmptyItem())
                    return 1;
                else if (y.IsEmptyItem())
                    return -1;
                else
                    return x.Time.CompareTo(y.Time);
            }
        }

        private class ItemComparerEquip : Comparer<Item>
        {
            public override int Compare(Item x, Item y)
            {
                if (x.IsEmptyItem())
                    return 1;
                else if (y.IsEmptyItem())
                    return -1;
                else
                    if (x.Equipped && y.Equipped)
                        return 0;
                    else if (x.Equipped)
                        return -1;
                    else
                        return 1;
            }
        }
        private class ItemComparerNotEquip : Comparer<Item>
        {
            public override int Compare(Item x, Item y)
            {
                if (x.IsEmptyItem())
                    return 1;
                else if (y.IsEmptyItem())
                    return -1;
                else
                    if (!x.Equipped && !y.Equipped)
                        return 0;
                    else if (!x.Equipped)
                        return -1;
                    else
                        return 1;
            }
        }

        private class ItemComparerTimeDesc : Comparer<Item>
        {
            public override int Compare(Item x, Item y)
            {
                if (x.IsEmptyItem())
                    return 1;
                else if (y.IsEmptyItem())
                    return -1;
                else
                    return x.Time.CompareTo(y.Time) * -1;
            }
        }

        private class ItemComparerSerialAsc : Comparer<Item>
        {
            public override int Compare(Item x, Item y)
            {
                if (x.IsEmptyItem())
                    return 1;
                else if (y.IsEmptyItem())
                    return -1;
                else
                    if (x.Serial < y.Serial)
                    return -1;
                else if (x.Serial > y.Serial)
                    return 1;
                else
                    return 0;
            }
        }

        private class ItemComparerSerialDesc : Comparer<Item>
        {
            public override int Compare(Item x, Item y)
            {
                if (x.IsEmptyItem())
                    return 1;
                else if (y.IsEmptyItem())
                    return -1;
                else
                    if (x.Serial > y.Serial)
                    return -1;
                else if (x.Serial < y.Serial)
                    return 1;
                else
                    return 0;
            }
        }

        public class ItemComparerNameAsc : Comparer<string>
        {
            public override int Compare(string x, string y)
            {
                if (x == "" || x.StartsWith("("))
                    return 1;
                else if (y == "" || y.StartsWith("("))
                    return -1;
                else
                    return x.ToLower().CompareTo(y.ToLower());
            }
        }

        public class ItemComparerNameDesc : Comparer<string>
        {
            public override int Compare(string x, string y)
            {
                if (x == "" || x.StartsWith("("))
                    return 1;
                else if (y == "" || y.StartsWith("("))
                    return -1;
                else
                    return x.ToLower().CompareTo(y.ToLower()) * -1;
            }
        }
    }
}
