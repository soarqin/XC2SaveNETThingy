using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XC2SaveNETThingy
{
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
    }
}
