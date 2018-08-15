using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XC2SaveNETThingy
{
    public class MercGroup : IXC2SaveObject
    {
        public const int SIZE = 0x28;

        public UInt16[] TeamMemberIDs { get; set; }
        public Byte[] Unk_0x0C { get; set; }
        public UInt32 TeamID { get; set; }
        public UInt32 MissionID { get; set; }
        public Byte[] Unk_0x18 { get; set; }
        public UInt32 MissionTime { get; set; }
        public UInt32 MissionTimeOriginal { get; set; }
        public Byte[] Unk_0x24 { get; set; }

        private static readonly Dictionary<string, int> LOC = new Dictionary<string, int>()
            {
                { "TeamMemberIds", 0x0 },
                { "field_C", 0xC },
                { "TeamId", 0x10 },
                { "MissionId", 0x14 },
                { "field_18", 0x18 },
                { "MissionTime", 0x1C },
                { "MissionTimeOriginal", 0x20 },
                { "field_24", 0x24 }
            };

        public MercGroup(Byte[] data)
        {
            TeamMemberIDs = new UInt16[6];
            for (int i = 0; i < TeamMemberIDs.Length; i++)
                TeamMemberIDs[i] = BitConverter.ToUInt16(data.GetByteSubArray(LOC["TeamMemberIds"] + (i * 2), 2), 0);

            Unk_0x0C = data.GetByteSubArray(LOC["field_C"], 4);
            TeamID = BitConverter.ToUInt32(data.GetByteSubArray(LOC["TeamId"], 4), 0);
            MissionID = BitConverter.ToUInt32(data.GetByteSubArray(LOC["MissionId"], 4), 0);
            Unk_0x18 = data.GetByteSubArray(LOC["field_18"], 4);
            MissionTime = BitConverter.ToUInt32(data.GetByteSubArray(LOC["MissionTime"], 4), 0);
            MissionTimeOriginal = BitConverter.ToUInt32(data.GetByteSubArray(LOC["MissionTimeOriginal"], 4), 0);
            Unk_0x24 = data.GetByteSubArray(LOC["field_24"], 4);
        }

        public Byte[] ToRawData()
        {
            List<Byte> result = new List<Byte>();

            foreach (UInt16 tm in TeamMemberIDs)
                result.AddRange(BitConverter.GetBytes(tm));

            result.AddRange(Unk_0x0C);
            result.AddRange(BitConverter.GetBytes(TeamID));
            result.AddRange(BitConverter.GetBytes(MissionID));
            result.AddRange(Unk_0x18);
            result.AddRange(BitConverter.GetBytes(MissionTime));
            result.AddRange(BitConverter.GetBytes(MissionTimeOriginal));
            result.AddRange(Unk_0x24);

            if (result.Count != SIZE)
            {
                string message = "MercenaryTeam: SIZE ALL WRONG!!!" + Environment.NewLine +
                "Size should be " + SIZE + " bytes..." + Environment.NewLine +
                "...but Size is " + result.Count + " bytes!";

                throw new Exception(message);
            }

            return result.ToArray();
        }
    }
}
