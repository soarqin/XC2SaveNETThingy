using System;
using System.Collections.Generic;
using System.Text.Json;

namespace XC2BdatImporter
{
    internal static class ReservedItemSelfTests
    {
        internal static void Run()
        {
            JapaneseEnglishNameIsReserved();
            EmptyEnglishNameIsReserved();
            UnknownNameIsReserved();
            AsianProvisionalMarkerIsReserved();
            SourceSortMetadataIsReserved();
            MissingSourceNamePointerIsReserved();
            JapaneseMaskWordIsNotAProvisionalMarker();
            FullyTranslatedItemIsNotReserved();
        }

        private static void JapaneseEnglishNameIsReserved()
        {
            ReservedItemSignals signals = ReservedItemAnalyzer.Analyze(
                "ニアの情報",
                "ニアの情報",
                "ニアの情報",
                "ニアの情報");
            AssertTrue(signals.IsReserved, "Japanese English name should be reserved");
            AssertContains(signals.ReasonCode, "untranslated", "Japanese English reason");
        }

        private static void EmptyEnglishNameIsReserved()
        {
            ReservedItemSignals signals = ReservedItemAnalyzer.Analyze(string.Empty, string.Empty, string.Empty, string.Empty);
            AssertTrue(signals.IsReserved, "empty English name should be reserved");
            AssertContains(signals.ReasonCode, "empty", "empty-name reason");
        }

        private static void UnknownNameIsReserved()
        {
            ReservedItemSignals signals = ReservedItemAnalyzer.Analyze("UNKNOWN_25004", string.Empty, string.Empty, string.Empty);
            AssertTrue(signals.IsReserved, "UNKNOWN item should be reserved");
            AssertContains(signals.ReasonCode, "unknown", "UNKNOWN reason");
        }

        private static void AsianProvisionalMarkerIsReserved()
        {
            ReservedItemSignals signals = ReservedItemAnalyzer.Analyze(
                "Gorilla Hair Bristle",
                "ゴリラの剛毛(仮)",
                "ゴリラの剛毛(仮)",
                "ゴリラの剛毛(仮)");
            AssertTrue(signals.IsReserved, "Asian provisional marker should be reserved");
            AssertContains(signals.ReasonCode, "provisional", "provisional reason");
        }

        private static void SourceSortMetadataIsReserved()
        {
            ReservedItemSignals signals = ReservedItemAnalyzer.Analyze(
                "Cloud Sea Compass EX",
                "スーパー雲海羅針盤",
                "スーパー雲海羅針盤",
                "スーパー雲海羅針盤",
                CreateSourceRow(25162, 317, 0, 50));
            AssertTrue(signals.IsReserved, "missing Western sort index should be reserved");
            AssertContains(signals.ReasonCode, "western-sort", "Western sort reason");

            signals = ReservedItemAnalyzer.Analyze(
                "fav_cap_036",
                "fav_cap_036",
                "fav_cap_036",
                "fav_cap_036",
                CreateSourceRow(40350, 339, 0, 0));
            AssertTrue(signals.IsReserved, "missing all locale sort indexes should be reserved");
            AssertContains(signals.ReasonCode, "no-locale-sort", "all locale sort reason");
        }

        private static void MissingSourceNamePointerIsReserved()
        {
            ReservedItemSignals signals = ReservedItemAnalyzer.Analyze(
                "UNKNOWN_25004",
                string.Empty,
                string.Empty,
                string.Empty,
                CreateSourceRow(25004, 0, 1, 1));
            AssertTrue(signals.IsReserved, "missing BDAT name pointer should be reserved");
            AssertContains(signals.ReasonCode, "source-name", "missing source name reason");
        }

        private static BdatRow CreateSourceRow(long id, long name, long sortGb, long sortKr)
        {
            Dictionary<string, JsonElement> fields = new Dictionary<string, JsonElement>(StringComparer.Ordinal)
            {
                ["Name"] = JsonDocument.Parse(name.ToString()).RootElement.Clone(),
                ["sortJP"] = JsonDocument.Parse("0").RootElement.Clone(),
                ["sortGE"] = JsonDocument.Parse("0").RootElement.Clone(),
                ["sortFR"] = JsonDocument.Parse("0").RootElement.Clone(),
                ["sortSP"] = JsonDocument.Parse("0").RootElement.Clone(),
                ["sortIT"] = JsonDocument.Parse("0").RootElement.Clone(),
                ["sortGB"] = JsonDocument.Parse(sortGb.ToString()).RootElement.Clone(),
                ["sortCN"] = JsonDocument.Parse("0").RootElement.Clone(),
                ["sortTW"] = JsonDocument.Parse("0").RootElement.Clone(),
                ["sortKR"] = JsonDocument.Parse(sortKr.ToString()).RootElement.Clone()
            };
            return new BdatRow(id, fields);
        }

        private static void JapaneseMaskWordIsNotAProvisionalMarker()
        {
            ReservedItemSignals signals = ReservedItemAnalyzer.Analyze(
                "Platinum Nopon Mask",
                "白金诺彭面具",
                "プラチナノポンの仮面",
                "白金諾彭面具");
            AssertFalse(signals.IsReserved, "仮面 must not be treated as 仮 marker");
        }

        private static void FullyTranslatedItemIsNotReserved()
        {
            ReservedItemSignals signals = ReservedItemAnalyzer.Analyze(
                "Abyss Vest",
                "深渊背心",
                "アビスベスト",
                "深淵背心");
            AssertFalse(signals.IsReserved, "translated item should not be reserved");
        }

        private static void AssertTrue(bool value, string description)
        {
            if (!value) throw new InvalidOperationException(description);
        }

        private static void AssertFalse(bool value, string description)
        {
            if (value) throw new InvalidOperationException(description);
        }

        private static void AssertContains(string actual, string expected, string description)
        {
            if (actual.IndexOf(expected, StringComparison.OrdinalIgnoreCase) < 0)
            {
                throw new InvalidOperationException(description + " expected '" + expected + "' in '" + actual + "'.");
            }
        }
    }
}
