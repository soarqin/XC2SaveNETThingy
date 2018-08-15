using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XC2SaveNETThingy
{
    public class TigerTigerData : IXC2SaveObject
    {
        public const int SIZE = 0x100;

        public Byte[] Unk_0x00 { get; set; }
        public UInt32 Stage1HighScore { get; set; }
        public bool Stage1ScoreIsEasyMode { get; set; }
        public string Stage1HighScoreInitials { get; set; }
        public UInt32 Stage2HighScore { get; set; }
        public bool Stage2ScoreIsEasyMode { get; set; }
        public string Stage2HighScoreInitials { get; set; }
        public UInt32 Stage3HighScore { get; set; }
        public bool Stage3ScoreIsEasyMode { get; set; }
        public string Stage3HighScoreInitials { get; set; }
        public UInt32 Stage4HighScore { get; set; }
        public bool Stage4ScoreIsEasyMode { get; set; }
        public string Stage4HighScoreInitials { get; set; }
        public UInt32 Stage5HighScore { get; set; }
        public bool Stage5ScoreIsEasyMode { get; set; }
        public string Stage5HighScoreInitials { get; set; }
        public Byte[] Unk_0x30 { get; set; }

        public TigerTigerData(Byte[] data)
        {
            Unk_0x00 = data.GetByteSubArray(0, 8);

            UInt32 mask = (UInt32)(Math.Pow(2, 31));

            UInt32 s1 = BitConverter.ToUInt32(data.GetByteSubArray(8, 4), 0);
            Stage1HighScore = s1 & ~mask;
            Stage1ScoreIsEasyMode = (s1 >> 31) == 1;
            Stage1HighScoreInitials = Encoding.ASCII.GetString(data.GetByteSubArray(0xC, 3));

            UInt32 s2 = BitConverter.ToUInt32(data.GetByteSubArray(0x10, 4), 0);
            Stage2HighScore = s2 & ~mask;
            Stage2ScoreIsEasyMode = (s2 >> 31) == 1;
            Stage2HighScoreInitials = Encoding.ASCII.GetString(data.GetByteSubArray(0x14, 3));

            UInt32 s3 = BitConverter.ToUInt32(data.GetByteSubArray(0x18, 4), 0);
            Stage3HighScore = s3 & ~mask;
            Stage3ScoreIsEasyMode = (s3 >> 31) == 1;
            Stage3HighScoreInitials = Encoding.ASCII.GetString(data.GetByteSubArray(0x1C, 3));

            UInt32 s4 = BitConverter.ToUInt32(data.GetByteSubArray(0x20, 4), 0);
            Stage4HighScore = s4 & ~mask;
            Stage4ScoreIsEasyMode = (s4 >> 31) == 1;
            Stage4HighScoreInitials = Encoding.ASCII.GetString(data.GetByteSubArray(0x24, 3));

            UInt32 s5 = BitConverter.ToUInt32(data.GetByteSubArray(0x28, 4), 0);
            Stage5HighScore = s5 & ~mask;
            Stage5ScoreIsEasyMode = (s5 >> 31) == 1;
            Stage5HighScoreInitials = Encoding.ASCII.GetString(data.GetByteSubArray(0x2C, 3));

            Unk_0x30 = data.GetByteSubArray(0x30, 0xD0);
        }

        public Byte[] ToRawData()
        {
            Byte[] initials = new Byte[] { 0, 0, 0, 0 };

            List<Byte> result = new List<byte>();

            result.AddRange(Unk_0x00);

            result.AddRange(BitConverter.GetBytes((UInt32)(((Stage1ScoreIsEasyMode ? 1 : 0) << 31) + Stage1HighScore)));
            Encoding.ASCII.GetBytes(Stage1HighScoreInitials, 0, 3, initials, 0);
            result.AddRange(initials);

            result.AddRange(BitConverter.GetBytes((UInt32)(((Stage2ScoreIsEasyMode ? 1 : 0) << 31) + Stage2HighScore)));
            Encoding.ASCII.GetBytes(Stage2HighScoreInitials, 0, 3, initials, 0);
            result.AddRange(initials);

            result.AddRange(BitConverter.GetBytes((UInt32)(((Stage3ScoreIsEasyMode ? 1 : 0) << 31) + Stage3HighScore)));
            Encoding.ASCII.GetBytes(Stage3HighScoreInitials, 0, 3, initials, 0);
            result.AddRange(initials);

            result.AddRange(BitConverter.GetBytes((UInt32)(((Stage4ScoreIsEasyMode ? 1 : 0) << 31) + Stage4HighScore)));
            Encoding.ASCII.GetBytes(Stage4HighScoreInitials, 0, 3, initials, 0);
            result.AddRange(initials);

            result.AddRange(BitConverter.GetBytes((UInt32)(((Stage5ScoreIsEasyMode ? 1 : 0) << 31) + Stage5HighScore)));
            Encoding.ASCII.GetBytes(Stage5HighScoreInitials, 0, 3, initials, 0);
            result.AddRange(initials);

            result.AddRange(Unk_0x30);

            if (result.Count != SIZE)
            {
                string message = "Minigame: SIZE ALL WRONG!!!" + Environment.NewLine +
                "Size should be " + SIZE + " bytes..." + Environment.NewLine +
                "...but Size is " + result.Count + " bytes!";

                throw new Exception(message);
            }

            return result.ToArray();
        }
    }
}
