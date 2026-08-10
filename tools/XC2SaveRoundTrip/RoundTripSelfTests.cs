using System;
using System.ComponentModel;
using XC2SaveNETThingy;

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

            BindingListAddRangeProducesOneResetNotification();
        }

        private static void BindingListAddRangeProducesOneResetNotification()
        {
            BindingList<int> values = new BindingList<int>();
            int notificationCount = 0;
            ListChangedType lastChangeType = ListChangedType.ItemAdded;
            values.ListChanged += (sender, e) =>
            {
                notificationCount++;
                lastChangeType = e.ListChangedType;
            };

            values.AddRange(new[] { 10, 20, 30 });

            AssertEqual(3, values.Count, "batch list item count");
            AssertEqual(1, notificationCount, "batch list notification count");
            if (lastChangeType != ListChangedType.Reset)
                throw new InvalidOperationException("batch list notification was not Reset.");
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
