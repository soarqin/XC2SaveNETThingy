using System;
using System.Collections.Generic;
using System.Linq;

namespace XC2SaveNETThingy
{
    public sealed class LanguageDefinition
    {
        public LanguageDefinition(string code, string nativeName, string bdatCode, string region)
        {
            Code = code;
            NativeName = nativeName;
            BdatCode = bdatCode;
            Region = region;
        }

        public string Code { get; }
        public string NativeName { get; }
        public string BdatCode { get; }
        public string Region { get; }
    }

    public static class LanguageCatalog
    {
        private static readonly IReadOnlyList<LanguageDefinition> definitions = new[]
        {
            new LanguageDefinition("en", "English", "gb", "Western"),
            new LanguageDefinition("ja", "日本語", "jp", "Asian"),
            new LanguageDefinition("ko", "한국어", "kr", "Western/Asian"),
            new LanguageDefinition("zh-Hans", "简体中文", "cn", "Asian"),
            new LanguageDefinition("zh-Hant", "繁體中文", "tw", "Asian"),
            new LanguageDefinition("fr", "Français", "fr", "Western"),
            new LanguageDefinition("de", "Deutsch", "ge", "Western"),
            new LanguageDefinition("it", "Italiano", "it", "Western"),
            new LanguageDefinition("es", "Español", "sp", "Western")
        };

        public static IReadOnlyList<LanguageDefinition> All => definitions;

        public static LanguageDefinition Find(string code)
        {
            return definitions.FirstOrDefault(d => string.Equals(d.Code, code, StringComparison.OrdinalIgnoreCase))
                ?? definitions[0];
        }

        public static bool IsKnown(string code)
        {
            return definitions.Any(d => string.Equals(d.Code, code, StringComparison.OrdinalIgnoreCase));
        }

        public static string Normalize(string code)
        {
            return Find(code).Code;
        }

        public static XC2Data.LANG ToCurrentDataLanguage(string code)
        {
            return XC2Data.ParseLanguage(code);
        }

        public static string MigrateLegacy(string code)
        {
            if (string.Equals(code, "EN", StringComparison.OrdinalIgnoreCase)) return "en";
            if (string.Equals(code, "CN", StringComparison.OrdinalIgnoreCase)) return "zh-Hans";
            return Normalize(code);
        }
    }

    public static class EditorSettings
    {
        public const string InterfaceLanguageKey = "InterfaceLanguage";
        public const string DataLanguageKey = "DataLanguage";
        public const string LegacyLanguageKey = "Language";
        public const string EditReadOnlyDataKey = "EditReadOnlyData";

        public static Dictionary<string, string> CreateDefaults()
        {
            return new Dictionary<string, string>
            {
                { InterfaceLanguageKey, "en" },
                { DataLanguageKey, "en" },
                { EditReadOnlyDataKey, "false" }
            };
        }

        public static void Migrate(Dictionary<string, string> settings)
        {
            if (settings == null) throw new ArgumentNullException(nameof(settings));

            string legacy;
            if (!settings.ContainsKey(InterfaceLanguageKey) && settings.TryGetValue(LegacyLanguageKey, out legacy))
                settings[InterfaceLanguageKey] = LanguageCatalog.MigrateLegacy(legacy);
            if (!settings.ContainsKey(DataLanguageKey) && settings.TryGetValue(LegacyLanguageKey, out legacy))
                settings[DataLanguageKey] = LanguageCatalog.MigrateLegacy(legacy);

            if (!settings.ContainsKey(InterfaceLanguageKey)) settings[InterfaceLanguageKey] = "en";
            if (!settings.ContainsKey(DataLanguageKey)) settings[DataLanguageKey] = "en";
            settings[InterfaceLanguageKey] = LanguageCatalog.Normalize(settings[InterfaceLanguageKey]);
            settings[DataLanguageKey] = LanguageCatalog.Normalize(settings[DataLanguageKey]);
            if (!settings.ContainsKey(EditReadOnlyDataKey)) settings[EditReadOnlyDataKey] = "false";
        }
    }
}
