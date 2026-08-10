using System;

namespace XC2SaveRoundTrip
{
    internal static class RoundTripSelfTests
    {
        internal static void Run()
        {
            byte[] first = new byte[XC2SaveNETThingy.XC2Save.SIZE];
            byte[] second = (byte[])first.Clone();
            AssertEqual(0, RawSaveInspector.CompareKeyRegions(first, second).Count, "identical raw data");

            second[0x3C] = 1;
            AssertContains(RawSaveInspector.CompareKeyRegions(first, second), "Drivers", "driver region change");

            second = (byte[])first.Clone();
            second[0x117390] = 1;
            AssertContains(RawSaveInspector.CompareKeyRegions(first, second), "SaveVersion", "save version change");

            second = (byte[])first.Clone();
            second[0x11737C] = 1;
            AssertContains(RawSaveInspector.CompareKeyRegions(first, second), "ContentVersions", "content version change");
        }

        private static void AssertContains(System.Collections.Generic.IReadOnlyList<string> values, string expected, string description)
        {
            for (int index = 0; index < values.Count; index++)
            {
                if (values[index].IndexOf(expected, StringComparison.Ordinal) >= 0) return;
            }

            throw new InvalidOperationException(description + " did not report '" + expected + "'.");
        }

        private static void AssertEqual(int expected, int actual, string description)
        {
            if (expected != actual)
                throw new InvalidOperationException(description + " expected " + expected + " but got " + actual + ".");
        }
    }
}
