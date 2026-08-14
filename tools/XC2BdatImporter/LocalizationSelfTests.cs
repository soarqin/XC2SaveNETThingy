using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace XC2BdatImporter
{
    internal static class LocalizationSelfTests
    {
        internal static void Run()
        {
            Dictionary<string, string> languages = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { "en", "gb" }, { "ja", "jp" }, { "ko", "kr" }, { "zh-Hans", "cn" },
                { "zh-Hant", "tw" }, { "fr", "fr" }, { "de", "ge" }, { "it", "it" }, { "es", "sp" }
            };
            AssertEqual(9, languages.Count, "language catalog count");

            string projectRoot = Directory.GetParent(AppContext.BaseDirectory).Parent.Parent.Parent.Parent.Parent.FullName;
            string[] bdatRoots =
            {
                Path.Combine(projectRoot, "romdata", "3.0", "bdat"),
                Path.Combine(projectRoot, "romdata", "3.0U", "bdat")
            };

            foreach (KeyValuePair<string, string> language in languages)
            {
                if (language.Key == "en" && language.Value != "gb")
                    throw new InvalidOperationException("English must use the gb BDAT directory.");

                bool found = bdatRoots.Any(root => Directory.Exists(Path.Combine(root, language.Value)));
                if (!found)
                    throw new InvalidOperationException("BDAT language directory was not found for " + language.Key + ": " + language.Value);
            }
        }

        private static void AssertEqual<T>(T expected, T actual, string description)
        {
            if (!object.Equals(expected, actual))
                throw new InvalidOperationException(description + " expected '" + expected + "' but got '" + actual + "'.");
        }
    }
}
