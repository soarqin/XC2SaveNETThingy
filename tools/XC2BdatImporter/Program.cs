using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

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
                    return RunSelfTest();
                }

                string readTablePath = options.GetValue("read-table");
                if (!string.IsNullOrWhiteSpace(readTablePath))
                {
                    BdatTable table = BdatJsonReader.ReadTable(readTablePath);
                    StringBuilder columns = new StringBuilder();
                    for (int index = 0; index < table.Schema.Count; index++)
                    {
                        if (index != 0)
                        {
                            columns.Append(',');
                        }

                        columns.Append(table.Schema[index].Name);
                    }

                    Console.WriteLine(
                        "TABLE_OK table=" + table.TableName +
                        " rows=" + table.Rows.Count +
                        " columns=" + columns);
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

                ValidatedOutputPaths outputPaths = ValidateInputs(options);
                Console.WriteLine("Arguments validated. The BDAT import stages will be added in subsequent tasks.");
                Console.WriteLine("Output directory: " + outputPaths.OutputDirectory);
                Console.WriteLine("Report path: " + outputPaths.ReportPath);
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

        private static ValidatedOutputPaths ValidateInputs(ImporterArguments options)
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

            return ValidatedOutputPaths.Create(
                options.GetValue("output"),
                options.GetValue("report"));
        }

        private static void ValidateDirectory(string path, string optionName)
        {
            if (!Directory.Exists(path))
            {
                throw new DirectoryNotFoundException(
                    optionName + " directory was not found: " + Path.GetFullPath(path));
            }
        }

        private static int RunSelfTest()
        {
            string outputDirectory = TemporaryExtractionDirectory.Create();
            File.WriteAllText(Path.Combine(outputDirectory, "cleanup-check.txt"), "cleanup");
            using (BdatExtractionResult result = new BdatExtractionResult(
                0,
                string.Empty,
                string.Empty,
                outputDirectory))
            {
            }

            if (Directory.Exists(outputDirectory))
            {
                throw new InvalidOperationException("Temporary extraction output was not deleted.");
            }

            bool unsafePathRejected = false;
            try
            {
                TemporaryExtractionDirectory.ValidateExactChild(Path.GetTempPath());
            }
            catch (InvalidOperationException)
            {
                unsafePathRejected = true;
            }

            if (!unsafePathRejected)
            {
                throw new InvalidOperationException("Unsafe temporary deletion target was accepted.");
            }

            ImporterSelfTests.Run();
            Console.WriteLine("SELF_TEST_OK");
            return 0;
        }

        private static void WriteUsage(TextWriter writer)
        {
            writer.WriteLine("Usage:");
            writer.WriteLine("  XC2BdatImporter --base-bdat <dir> --base-gb <dir> --torna-bdat <dir>");
            writer.WriteLine("    --torna-gb <dir> --toolset <file> --output <dir> --report <file>");
            writer.WriteLine("    [--verify-only] [--replace]");
            writer.WriteLine("  XC2BdatImporter --read-table <json>");
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
            "report",
            "read-table"
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

            if (flags.Contains("verify-only") && flags.Contains("replace"))
            {
                throw new ArgumentException("Options --verify-only and --replace cannot be used together.");
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

    internal sealed class ValidatedOutputPaths
    {
        private ValidatedOutputPaths(string outputDirectory, string reportPath)
        {
            OutputDirectory = outputDirectory;
            ReportPath = reportPath;
        }

        internal string OutputDirectory { get; }

        internal string ReportPath { get; }

        internal static ValidatedOutputPaths Create(string outputDirectory, string reportPath)
        {
            if (Path.EndsInDirectorySeparator(reportPath))
            {
                throw new IOException("--report must name a file, not a directory.");
            }

            string fullOutputDirectory = Path.GetFullPath(outputDirectory);
            string fullReportPath = Path.GetFullPath(reportPath);
            string outputRoot = Path.GetPathRoot(fullOutputDirectory);
            if (string.Equals(
                    Path.TrimEndingDirectorySeparator(fullOutputDirectory),
                    Path.TrimEndingDirectorySeparator(outputRoot),
                    PathComparison))
            {
                throw new IOException("--output cannot be a filesystem root.");
            }

            if (File.Exists(fullOutputDirectory))
            {
                throw new IOException("--output points to an existing file: " + fullOutputDirectory);
            }

            if (Directory.Exists(fullReportPath))
            {
                throw new IOException("--report points to an existing directory: " + fullReportPath);
            }

            if (string.Equals(fullOutputDirectory, fullReportPath, PathComparison))
            {
                throw new IOException("--output and --report must resolve to different paths.");
            }

            ValidateExistingAncestor(fullOutputDirectory, "--output");
            string reportDirectory = Path.GetDirectoryName(fullReportPath);
            if (string.IsNullOrWhiteSpace(reportDirectory))
            {
                throw new IOException("--report must have a parent directory.");
            }

            ValidateExistingAncestor(reportDirectory, "--report");
            return new ValidatedOutputPaths(fullOutputDirectory, fullReportPath);
        }

        private static StringComparison PathComparison
        {
            get
            {
                return OperatingSystem.IsWindows()
                    ? StringComparison.OrdinalIgnoreCase
                    : StringComparison.Ordinal;
            }
        }

        private static void ValidateExistingAncestor(string path, string optionName)
        {
            string currentPath = path;
            while (!string.IsNullOrWhiteSpace(currentPath))
            {
                if (Directory.Exists(currentPath))
                {
                    return;
                }

                if (File.Exists(currentPath))
                {
                    throw new IOException(
                        optionName + " has a file where a parent directory is required: " + currentPath);
                }

                currentPath = Path.GetDirectoryName(currentPath);
            }

            throw new IOException(optionName + " has no existing parent directory: " + path);
        }
    }
}
