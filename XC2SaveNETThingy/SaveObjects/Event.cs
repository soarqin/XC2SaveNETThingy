using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XC2SaveNETThingy
{
    public class Event : IXC2SaveObject
    {
        public const int SIZE = 0x64;
        public const int COUNT = 500;

        private static readonly Dictionary<string, int> LOC = new Dictionary<string, int>()
            {
                { "EventID", 0x0 },
                { "Creator", 0x2 },
                { "PlayBladeID", 0x4 },
                { "VoiceID", 0x6 },
                { "Attribute", 0x8 },
                { "ExtraParts", 0xA },
                { "Unk_0x0C", 0xC },
                { "Weapons", 0xE },
                { "Unk_0x22", 0x22 },
                { "BladeIDs", 0x2E },
                { "Unk_0x42", 0x42 },
                { "Time", 0x4C },
                { "CurrentMapWeatherID", 0x50 },
                { "CurrentWeatherType", 0x52 },
                { "Unk_0x54", 0x54 }
            };

        public UInt16 EventID { get; set; }
        public UInt16 Creator { get; set; }
        public UInt16 PlayBladeID { get; set; }
        public UInt16 VoiceID { get; set; }
        public UInt16 Attribute { get; set; }
        public UInt16 ExtraParts { get; set; }
        public Byte[] Unk_0x0C { get; set; }
        public UInt16[] Weapons { get; set; }
        public Byte[] Unk_0x22 { get; set; }
        public UInt16[] BladeIDs { get; set; }
        public Byte[] Unk_0x42 { get; set; }
        public GameTime Time { get; set; }
        public UInt16 CurrentMapWeatherID { get; set; }
        public UInt16 CurrentWeatherType { get; set; }
        public Byte[] Unk_0x54 { get; set; }

        public Event(Byte[] data)
        {
            EventID = BitConverter.ToUInt16(data.GetByteSubArray(LOC["EventID"], 2), 0);
            Creator = BitConverter.ToUInt16(data.GetByteSubArray(LOC["Creator"], 2), 0);
            PlayBladeID = BitConverter.ToUInt16(data.GetByteSubArray(LOC["PlayBladeID"], 2), 0);
            VoiceID = BitConverter.ToUInt16(data.GetByteSubArray(LOC["VoiceID"], 2), 0);
            Attribute = BitConverter.ToUInt16(data.GetByteSubArray(LOC["Attribute"], 2), 0);
            ExtraParts = BitConverter.ToUInt16(data.GetByteSubArray(LOC["ExtraParts"], 2), 0);
            Unk_0x0C = data.GetByteSubArray(LOC["Unk_0x0C"], 2);

            Weapons = new UInt16[10];
            for (int i = 0; i < Weapons.Length; i++)
                Weapons[i] = BitConverter.ToUInt16(data.GetByteSubArray(LOC["Weapons"] + (i * 2), 2), 0);

            Unk_0x22 = data.GetByteSubArray(LOC["Unk_0x22"], 0x0C);

            BladeIDs = new UInt16[10];
            for (int i = 0; i < BladeIDs.Length; i++)
                BladeIDs[i] = BitConverter.ToUInt16(data.GetByteSubArray(LOC["BladeIDs"] + (i * 2), 2), 0);

            Unk_0x42 = data.GetByteSubArray(LOC["Unk_0x42"], 0x0A);
            Time = new GameTime(data.GetByteSubArray(LOC["Time"], 4));
            CurrentMapWeatherID = BitConverter.ToUInt16(data.GetByteSubArray(LOC["CurrentMapWeatherID"], 2), 0);
            CurrentWeatherType = BitConverter.ToUInt16(data.GetByteSubArray(LOC["CurrentWeatherType"], 2), 0);
            Unk_0x54 = data.GetByteSubArray(LOC["Unk_0x54"], 0x10);
        }

        public Byte[] ToRawData()
        {
            List<Byte> result = new List<Byte>();

            result.AddRange(BitConverter.GetBytes(EventID));
            result.AddRange(BitConverter.GetBytes(Creator));
            result.AddRange(BitConverter.GetBytes(PlayBladeID));
            result.AddRange(BitConverter.GetBytes(VoiceID));
            result.AddRange(BitConverter.GetBytes(Attribute));
            result.AddRange(BitConverter.GetBytes(ExtraParts));
            result.AddRange(Unk_0x0C);

            foreach (UInt16 w in Weapons)
                result.AddRange(BitConverter.GetBytes(w));

            result.AddRange(Unk_0x22);

            foreach (UInt16 u in BladeIDs)
                result.AddRange(BitConverter.GetBytes(u));

            result.AddRange(Unk_0x42);
            result.AddRange(Time.ToRawData());
            result.AddRange(BitConverter.GetBytes(CurrentMapWeatherID));
            result.AddRange(BitConverter.GetBytes(CurrentWeatherType));
            result.AddRange(Unk_0x54);

            if (result.Count != SIZE)
            {
                string message = "Event: SIZE ALL WRONG!!!" + Environment.NewLine +
                "Size should be " + SIZE + " bytes..." + Environment.NewLine +
                "...but Size is " + result.Count + " bytes!";

                throw new Exception(message);
            }

            return result.ToArray();
        }
    }
}
