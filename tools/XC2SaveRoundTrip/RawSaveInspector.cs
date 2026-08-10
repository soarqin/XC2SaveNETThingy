using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using XC2SaveNETThingy;
using ZLIB;

namespace XC2SaveRoundTrip
{
    internal sealed class RoundTripResult
    {
        internal string ModelType { get; set; }
        internal bool WasCompressed { get; set; }
        internal int SourceLength { get; set; }
        internal int SerializedLength { get; set; }
        internal ushort SaveVersionValue { get; set; }
        internal IReadOnlyList<string> Differences { get; set; }
        internal bool Passed { get { return Differences.Count == 0; } }
    }

    internal static class RawSaveInspector
    {
        private static readonly byte[] CompressionHeader = new byte[]
        {
            0x7E, 0xC0, 0xE2, 0xC9, 0x45, 0x98, 0xCE, 0x03,
            0xA7, 0x98, 0xF1, 0x33, 0x6A, 0xE7, 0x25, 0x80
        };

        internal static RoundTripResult Inspect(string inputPath, string outputPath, bool compress)
        {
            byte[] source = File.ReadAllBytes(inputPath);
            bool wasCompressed = HasCompressionHeader(source);
            byte[] rawSource = ToRawData(source, wasCompressed);
            XC2Save first = XC2SaveSerialization.Deserialize(source);
            byte[] serializedRaw = XC2SaveSerialization.Serialize(first, false);
            XC2Save second = XC2SaveSerialization.Deserialize(serializedRaw);
            List<string> differences = new List<string>(CompareKeyRegions(rawSource, serializedRaw));

            if (first.GetType() != second.GetType())
                differences.Add("ModelType: " + first.GetType().Name + " -> " + second.GetType().Name);
            if (!first.ContentVersions.SequenceEqual(second.ContentVersions))
                differences.Add("ContentVersions: model values changed");
            if (first.SaveVersionValue != second.SaveVersionValue)
                differences.Add("SaveVersionValue: " + first.SaveVersionValue + " -> " + second.SaveVersionValue);

            byte[] output = XC2SaveSerialization.Serialize(first, compress);
            File.WriteAllBytes(outputPath, output);
            return new RoundTripResult
            {
                ModelType = first.GetType().Name,
                WasCompressed = wasCompressed,
                SourceLength = rawSource.Length,
                SerializedLength = serializedRaw.Length,
                SaveVersionValue = first.SaveVersionValue,
                Differences = differences
            };
        }

        internal static IReadOnlyList<string> CompareKeyRegions(byte[] expected, byte[] actual)
        {
            if (expected == null) throw new ArgumentNullException(nameof(expected));
            if (actual == null) throw new ArgumentNullException(nameof(actual));
            List<string> differences = new List<string>();
            if (expected.Length != actual.Length)
                differences.Add("Length: 0x" + expected.Length.ToString("X") + " -> 0x" + actual.Length.ToString("X"));

            CompareRegion(differences, "Header", expected, actual, 0, 0x3C);
            CompareRegion(differences, "Drivers", expected, actual, 0x3C, Driver.SIZE * 16);
            CompareRegion(differences, "Blades", expected, actual, 0x5A3C, Blade.SIZE * 422);
            CompareRegion(differences, "ItemBox", expected, actual, 0xE98E8, ItemBox.SIZE);
            CompareRegion(differences, "ContentVersions", expected, actual, 0x11737C, XC2Save.CONTENT_VERSION_COUNT * 4);
            CompareRegion(differences, "SaveVersion", expected, actual, 0x117390, 784);
            return differences;
        }

        private static void CompareRegion(List<string> differences, string name, byte[] expected, byte[] actual, int offset, int length)
        {
            if (offset < 0 || offset + length > expected.Length || offset + length > actual.Length)
            {
                differences.Add(name + ": region is outside one of the buffers");
                return;
            }

            int mismatchCount = 0;
            for (int index = 0; index < length; index++)
            {
                if (expected[offset + index] != actual[offset + index]) mismatchCount++;
            }

            if (mismatchCount != 0)
                differences.Add(name + ": " + mismatchCount + " byte(s) differ");
        }

        private static bool HasCompressionHeader(byte[] data)
        {
            if (data.Length < CompressionHeader.Length) return false;
            for (int index = 0; index < CompressionHeader.Length; index++)
            {
                if (data[index] != CompressionHeader[index]) return false;
            }

            return true;
        }

        private static byte[] ToRawData(byte[] data, bool compressed)
        {
            if (!compressed) return data;
            using (MemoryStream compressedStream = new MemoryStream(data, CompressionHeader.Length, data.Length - CompressionHeader.Length))
            using (ZLIBStream zlibStream = new ZLIBStream(compressedStream, CompressionMode.Decompress, true))
            using (MemoryStream rawStream = new MemoryStream())
            {
                zlibStream.CopyTo(rawStream);
                return rawStream.ToArray();
            }
        }
    }
}
