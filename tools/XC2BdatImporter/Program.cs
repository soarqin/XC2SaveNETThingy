using System;
using System.Collections.Generic;
using System.IO;

namespace XC2BdatImporter
{
    internal static class Program
    {
        private static readonly string[] RequiredOptions =
        {
            "base-bdat",
            "base-gb",
            "torna-bdat",
            "torna-gb",
            "toolset",
            "output",
            "report"
        };

        private static int Main(string[] args)
        {
            try
            {
                ImporterArguments options = ImporterArguments.Parse(args);

                if (options.ShowHelp)
                {
                    WriteUsage(Console.Out);
                    return 0;
                }

                if (options.SelfTest)
                {
                    Console.WriteLine("SELF_TEST_PLACEHOLDER");
                    return 0;
                }

                IReadOnlyList<string> missingOptions = options.GetMissingValues(RequiredOptions);
                if (missingOptions.Count != 0)
                {
                    Console.Error.WriteLine(
                        "Missing required option(s): --" + string.Join(", --", missingOptions));
                    WriteUsage(Console.Error);
                    return 2;
                }

                ValidateInputs(options);
                Console.WriteLine("Arguments validated. The BDAT import stages will be added in subsequent tasks.");
                return 0;
            }
            catch (ArgumentException ex)
            {
                Console.Error.WriteLine(ex.Message);
                WriteUsage(Console.Error);
                return 2;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("XC2 BDAT import failed: " + ex.Message);
                return 1;
            }
        }

        private static void ValidateInputs(ImporterArguments options)
        {
            ValidateDirectory(options.GetValue("base-bdat"), "--base-bdat");
            ValidateDirectory(options.GetValue("base-gb"), "--base-gb");
            ValidateDirectory(options.GetValue("torna-bdat"), "--torna-bdat");
            ValidateDirectory(options.GetValue("torna-gb"), "--torna-gb");

            string toolsetPath = options.GetValue("toolset");
            if (!File.Exists(toolsetPath))
            {
                throw new FileNotFoundException("BDAT toolset executable was not found.", toolsetPath);
            }
        }

        private static void ValidateDirectory(string path, string optionName)
        {
            if (!Directory.Exists(path))
            {
                throw new DirectoryNotFoundException(
                    optionName + " directory was not found: " + Path.GetFullPath(path));
            }
        }

        private static void WriteUsage(TextWriter writer)
        {
            writer.WriteLine("Usage:");
            writer.WriteLine("  XC2BdatImporter --base-bdat <dir> --base-gb <dir> --torna-bdat <dir>");
            writer.WriteLine("    --torna-gb <dir> --toolset <file> --output <dir> --report <file>");
            writer.WriteLine("    [--verify-only] [--replace]");
            writer.WriteLine("  XC2BdatImporter --self-test");
        }
    }

    internal sealed class ImporterArguments
    {
        private static readonly HashSet<string> ValueOptions = new HashSet<string>(StringComparer.Ordinal)
        {
            "base-bdat",
            "base-gb",
            "torna-bdat",
            "torna-gb",
            "toolset",
            "output",
            "report"
        };

        private static readonly HashSet<string> FlagOptions = new HashSet<string>(StringComparer.Ordinal)
        {
            "verify-only",
            "replace",
            "self-test",
            "help"
        };

        private readonly Dictionary<string, string> values;
        private readonly HashSet<string> flags;

        private ImporterArguments(Dictionary<string, string> values, HashSet<string> flags)
        {
            this.values = values;
            this.flags = flags;
        }

        internal bool ShowHelp
        {
            get { return flags.Contains("help"); }
        }

        internal bool SelfTest
        {
            get { return flags.Contains("self-test"); }
        }

        internal bool VerifyOnly
        {
            get { return flags.Contains("verify-only"); }
        }

        internal bool Replace
        {
            get { return flags.Contains("replace"); }
        }

        internal static ImporterArguments Parse(string[] args)
        {
            Dictionary<string, string> values = new Dictionary<string, string>(StringComparer.Ordinal);
            HashSet<string> flags = new HashSet<string>(StringComparer.Ordinal);

            for (int index = 0; index < args.Length; index++)
            {
                string argument = args[index];
                if (argument == "-h")
                {
                    argument = "--help";
                }

                if (!argument.StartsWith("--", StringComparison.Ordinal) || argument.Length == 2)
                {
                    throw new ArgumentException("Unexpected argument: " + argument);
                }

                string optionName = argument.Substring(2);
                if (FlagOptions.Contains(optionName))
                {
                    if (!flags.Add(optionName))
                    {
                        throw new ArgumentException("Duplicate option: --" + optionName);
                    }

                    continue;
                }

                if (!ValueOptions.Contains(optionName))
                {
                    throw new ArgumentException("Unknown option: --" + optionName);
                }

                if (values.ContainsKey(optionName))
                {
                    throw new ArgumentException("Duplicate option: --" + optionName);
                }

                if (index + 1 >= args.Length || args[index + 1].StartsWith("--", StringComparison.Ordinal))
                {
                    throw new ArgumentException("Option --" + optionName + " requires a value.");
                }

                values.Add(optionName, args[++index]);
            }

            return new ImporterArguments(values, flags);
        }

        internal string GetValue(string optionName)
        {
            string value;
            return values.TryGetValue(optionName, out value) ? value : null;
        }

        internal IReadOnlyList<string> GetMissingValues(IEnumerable<string> optionNames)
        {
            List<string> missing = new List<string>();
            foreach (string optionName in optionNames)
            {
                string value;
                if (!values.TryGetValue(optionName, out value) || string.IsNullOrWhiteSpace(value))
                {
                    missing.Add(optionName);
                }
            }

            return missing;
        }
    }
}
