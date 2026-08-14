using System;
using System.Collections.Generic;
using XC2SaveNETThingy;

namespace XC2SaveRoundTrip
{
    internal static class LanguageCatalogSelfTests
    {
        internal static void Run()
        {
            IReadOnlyList<LanguageDefinition> languages = LanguageCatalog.All;
            AssertEqual(9, languages.Count, "language count");

            HashSet<string> codes = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            HashSet<string> bdatCodes = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (LanguageDefinition language in languages)
            {
                if (!codes.Add(language.Code)) throw new InvalidOperationException("Duplicate language code: " + language.Code);
                if (!bdatCodes.Add(language.BdatCode)) throw new InvalidOperationException("Duplicate BDAT code: " + language.BdatCode);
                if (string.IsNullOrWhiteSpace(language.NativeName)) throw new InvalidOperationException("Missing native language name: " + language.Code);
            }

            AssertEqual("en", LanguageCatalog.MigrateLegacy("EN"), "legacy English migration");
            AssertEqual("zh-Hans", LanguageCatalog.MigrateLegacy("CN"), "legacy Chinese migration");
            AssertEqual("en", LanguageCatalog.Normalize("not-a-language"), "unknown language fallback");
            AssertEqual(XC2Data.LANG.ZH_HANS, LanguageCatalog.ToCurrentDataLanguage("zh-Hans"), "current Chinese data mapping");
            AssertEqual(XC2Data.LANG.JA, LanguageCatalog.ToCurrentDataLanguage("ja"), "current Japanese data mapping");
            AssertEqual("zh-Hans", LanguageCatalog.Normalize("zh-CN"), "simplified Chinese normalization");
            AssertEqual("zh-Hant", LanguageCatalog.Normalize("zh-TW"), "traditional Chinese normalization");
            Dictionary<string, string> defaults = EditorSettings.CreateDefaults("ja");
            AssertEqual("ja", defaults[EditorSettings.InterfaceLanguageKey], "default interface language");
            AssertEqual("ja", defaults[EditorSettings.DataLanguageKey], "default data language");
        }

        private static void AssertEqual<T>(T expected, T actual, string description)
        {
            if (!object.Equals(expected, actual))
                throw new InvalidOperationException(description + " expected '" + expected + "' but got '" + actual + "'.");
        }
    }
}
