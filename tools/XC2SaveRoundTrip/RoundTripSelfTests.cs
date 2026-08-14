using System;
using System.ComponentModel;
using System.Data;
using System.Collections.Generic;
using System.Reflection;
using System.Windows.Forms;
using XC2Bf2SaveNETThingy;
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
            ItemGridDisplayColumnsDoNotFormatNumericCells();
            DeferredTabPathKeepsSiblingPagesDeferred();
            LanguageCatalogSelfTests.Run();
            PaddedStringPreservesOriginalBytesUntilEdited();
            Bf2FixtureRoundTripsWithoutChangingRawPayload();
            Bf2FormLoadDoesNotMutateRawPayload();
        }

        private static void PaddedStringPreservesOriginalBytesUntilEdited()
        {
            byte[] source = new byte[PaddedString16.SIZE];
            source[0] = (byte)'A';
            source[1] = (byte)'B';
            source[2] = 0xCC;
            source[16] = 2;
            source[17] = 0xAA;
            source[18] = 0xBB;
            source[19] = 0xDD;

            PaddedString16 value = new PaddedString16(source);
            AssertSequenceEqual(source, value.ToRawData(), "unedited padded string bytes");

            value.Str = "C";
            byte[] edited = value.ToRawData();
            AssertByteEqual((byte)'C', edited[0], "edited padded string value");
            AssertByteEqual(1, edited[16], "edited padded string length");
            AssertByteEqual(0, edited[2], "edited padded string padding");
            AssertByteEqual(0, edited[17], "edited padded string trailing metadata");
        }

        private static void Bf2FixtureRoundTripsWithoutChangingRawPayload()
        {
            string fixturePath = System.IO.Path.Combine(Environment.CurrentDirectory, "bf2savefile.sav");
            if (!System.IO.File.Exists(fixturePath))
                throw new InvalidOperationException("Bf2 round-trip fixture was not found: " + fixturePath);

            string outputPath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "xc2-bf2-roundtrip-regression.sav");
            RoundTripResult result = RawSaveInspector.Inspect(fixturePath, outputPath, false);
            AssertEqual(0, result.Differences.Count, "Bf2 fixture raw payload preservation");
        }

        private static void Bf2FormLoadDoesNotMutateRawPayload()
        {
            string fixturePath = System.IO.Path.Combine(Environment.CurrentDirectory, "bf2savefile.sav");
            string previousThrowSetting = Environment.GetEnvironmentVariable("XC2_PERF_THROW_LOAD_ERRORS");
            Environment.SetEnvironmentVariable("XC2_PERF_THROW_LOAD_ERRORS", "1");

            try
            {
                using (frmMain form = new frmMain())
                {
                    MethodInfo formLoad = typeof(frmMain).GetMethod("frmMain_Load", BindingFlags.Instance | BindingFlags.NonPublic);
                    if (formLoad == null)
                        throw new InvalidOperationException("Bf2 frmMain_Load method was not found.");

                    formLoad.Invoke(form, new object[] { form, EventArgs.Empty });

                    MethodInfo loadSaveData = typeof(frmMain).GetMethod("LoadSaveData", BindingFlags.Instance | BindingFlags.NonPublic);
                    if (loadSaveData == null)
                        throw new InvalidOperationException("Bf2 LoadSaveData method was not found.");

                    loadSaveData.Invoke(form, new object[] { fixturePath });
                    Application.DoEvents();

                    FieldInfo saveDataField = typeof(frmMain).GetField("SAVEDATA", BindingFlags.Instance | BindingFlags.NonPublic);
                    XC2Save loadedSave = saveDataField == null ? null : saveDataField.GetValue(form) as XC2Save;
                    if (loadedSave == null)
                        throw new InvalidOperationException("Bf2 form did not retain loaded save data.");

                    string difference = RawSaveInspector.CompareAllBytes(
                        RawSaveInspector.ReadRawData(fixturePath),
                        XC2SaveSerialization.Serialize(loadedSave, false));
                    if (difference != null)
                        throw new InvalidOperationException("Bf2 form load changed the save payload: " + difference);

                    FieldInfo mapPositionXField = typeof(frmMain).GetField("nudLastVisitedLandmarkMapPositionX", BindingFlags.Instance | BindingFlags.NonPublic);
                    NumericUpDown mapPositionX = mapPositionXField == null ? null : mapPositionXField.GetValue(form) as NumericUpDown;
                    if (mapPositionX == null)
                        throw new InvalidOperationException("Bf2 map position control was not found.");

                    decimal editedValue = mapPositionX.Value < mapPositionX.Maximum
                        ? mapPositionX.Value + 1
                        : mapPositionX.Value - 1;
                    mapPositionX.Value = editedValue;
                    AssertEqual((float)editedValue, loadedSave.LastVisitedLandmarkMapPosition.X, "Bf2 post-load float edit");
                }
            }
            finally
            {
                Environment.SetEnvironmentVariable("XC2_PERF_THROW_LOAD_ERRORS", previousThrowSetting);
            }
        }

        private static void ItemGridDisplayColumnsDoNotFormatNumericCells()
        {
            DataTable itemNames = new DataTable();
            itemNames.Columns.Add("ID", typeof(UInt32));
            itemNames.Columns.Add("Name", typeof(string));
            itemNames.Rows.Add(12U, "Core Chip");

            DataTable itemTypes = new DataTable();
            itemTypes.Columns.Add("ID", typeof(UInt32));
            itemTypes.Columns.Add("Name", typeof(string));
            itemTypes.Rows.Add(6U, "Weapon");

            Item item = new Item(new byte[Item.SIZE])
            {
                ID = 12,
                Type = 6,
                Qty = 7
            };
            ItemGridBinding binding = new ItemGridBinding(new[] { item }, itemNames, itemTypes);

            AssertDisplayValue(binding, 1, "Core Chip", "item name column");
            AssertDisplayValue(binding, 3, "Weapon", "item type name column");
            AssertNoDisplayValue(binding, 4, "quantity column");
        }

        private static void AssertDisplayValue(ItemGridBinding binding, int columnIndex, string expected, string description)
        {
            if (!binding.TryGetDisplayValue(0, columnIndex, out string actual))
                throw new InvalidOperationException(description + " did not provide a display value.");

            AssertEqual(expected, actual, description);
        }

        private static void AssertNoDisplayValue(ItemGridBinding binding, int columnIndex, string description)
        {
            if (binding.TryGetDisplayValue(0, columnIndex, out string actual))
                throw new InvalidOperationException(description + " unexpectedly formatted as '" + actual + "'.");
        }

        private static void DeferredTabPathKeepsSiblingPagesDeferred()
        {
            using (TabControl root = new TabControl())
            {
                TabPage mainPage = new TabPage("Main");
                TabPage siblingPage = new TabPage("Sibling");
                TabControl nested = new TabControl();
                TabPage firstChild = new TabPage("First");
                TabPage secondChild = new TabPage("Second");
                nested.TabPages.Add(firstChild);
                nested.TabPages.Add(secondChild);
                nested.SelectedIndex = 0;
                mainPage.Controls.Add(nested);
                root.TabPages.Add(mainPage);
                root.TabPages.Add(siblingPage);
                root.SelectedIndex = 0;

                HashSet<TabPage> deferredPages = new HashSet<TabPage>
                {
                    mainPage,
                    siblingPage,
                    firstChild,
                    secondChild
                };

                DeferredTabPageState.MarkVisiblePathReady(mainPage, deferredPages);

                AssertFalse(deferredPages.Contains(mainPage), "selected parent page remains deferred");
                AssertFalse(deferredPages.Contains(firstChild), "selected child page remains deferred");
                AssertTrue(deferredPages.Contains(secondChild), "unselected child page was removed from deferred state");
                AssertTrue(deferredPages.Contains(siblingPage), "unselected parent page was removed from deferred state");
            }
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

        private static void AssertEqual(string expected, string actual, string description)
        {
            if (!String.Equals(expected, actual, StringComparison.Ordinal))
                throw new InvalidOperationException(description + " expected '" + expected + "' but got '" + actual + "'.");
        }

        private static void AssertEqual(float expected, float actual, string description)
        {
            if (expected != actual)
                throw new InvalidOperationException(description + " expected " + expected + " but got " + actual + ".");
        }

        private static void AssertByteEqual(byte expected, byte actual, string description)
        {
            if (expected != actual)
                throw new InvalidOperationException(description + " expected 0x" + expected.ToString("X2") + " but got 0x" + actual.ToString("X2") + ".");
        }

        private static void AssertSequenceEqual(byte[] expected, byte[] actual, string description)
        {
            if (expected == null || actual == null || expected.Length != actual.Length)
                throw new InvalidOperationException(description + " changed length.");

            for (int index = 0; index < expected.Length; index++)
            {
                if (expected[index] != actual[index])
                    throw new InvalidOperationException(
                        description + " changed offset 0x" + index.ToString("X") +
                        " from 0x" + expected[index].ToString("X2") +
                        " to 0x" + actual[index].ToString("X2") + ".");
            }
        }

        private static void AssertTrue(bool value, string description)
        {
            if (!value)
                throw new InvalidOperationException(description + " expected true.");
        }

        private static void AssertFalse(bool value, string description)
        {
            if (value)
                throw new InvalidOperationException(description + " expected false.");
        }
    }
}
