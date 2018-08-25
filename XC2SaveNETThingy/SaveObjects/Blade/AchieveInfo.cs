using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XC2SaveNETThingy
{
    public class AchieveInfo : IXC2SaveObject
    {
        public const int SIZE = 0x90;

        public UInt16 ID { get; set; }
        public UInt16 Alignment { get; set; }
        public AchieveQuest[] AchieveQuests { get; set; }

        public AchieveInfo(Byte[] data)
        {
            ID = BitConverter.ToUInt16(data.GetByteSubArray(0, 2), 0);
            Alignment = BitConverter.ToUInt16(data.GetByteSubArray(2, 2), 0);

            AchieveQuests = new AchieveQuest[5];
            for (int i = 0; i < AchieveQuests.Length; i++)
                AchieveQuests[i] = new AchieveQuest(data.GetByteSubArray(4 + (i * AchieveQuest.SIZE), AchieveQuest.SIZE));
        }

        public Byte[] ToRawData()
        {
            List<Byte> result = new List<Byte>();

            result.AddRange(BitConverter.GetBytes(ID));
            result.AddRange(BitConverter.GetBytes(Alignment));

            foreach (AchieveQuest aq in AchieveQuests)
                result.AddRange(aq.ToRawData());

            if (result.Count != SIZE)
            {
                string message = "AchieveInfo: SIZE ALL WRONG!!!" + Environment.NewLine +
                "Size should be " + SIZE + " bytes..." + Environment.NewLine +
                "...but Size is " + result.Count + " bytes!";

                throw new Exception(message);
            }

            return result.ToArray();
        }
    }
}
