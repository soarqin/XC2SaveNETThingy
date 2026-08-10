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
                RunImport(options, outputPaths);
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

        private static void RunImport(ImporterArguments options, ValidatedOutputPaths outputPaths)
        {
            string projectRoot = Directory.GetCurrentDirectory();
            string dataDirectory = Path.Combine(projectRoot, "XC2SaveNETThingy", "data");
            string ddlDirectory = Path.Combine(projectRoot, "XC2SaveNETThingy", "db");
            if (!Directory.Exists(dataDirectory) || !Directory.Exists(ddlDirectory))
            {
                throw new DirectoryNotFoundException(
                    "Run the importer from the repository root containing XC2SaveNETThingy/data and XC2SaveNETThingy/db.");
            }

            string mappingPath = Path.Combine(AppContext.BaseDirectory, "mappings.json");
            MappingConfig config = MappingConfig.Load(mappingPath);
            string outputDataDirectory = Path.Combine(outputPaths.OutputDirectory, "data");
            Directory.CreateDirectory(outputDataDirectory);
            foreach (string existingCsv in Directory.GetFiles(dataDirectory, "*.csv"))
            {
                File.Copy(existingCsv, Path.Combine(outputDataDirectory, Path.GetFileName(existingCsv)), true);
            }

            BdatToolsetRunner runner = new BdatToolsetRunner(options.GetValue("toolset"));
            using (BdatExtractionResult common = runner.Extract(Path.Combine(options.GetValue("base-bdat"), "common.bdat")))
            using (BdatExtractionResult localized = runner.Extract(ResolveEnglishDirectory(options.GetValue("base-gb"))))
            {
                GeneratedCsvSet generated = MappingGenerator.Generate(
                    config,
                    common.OutputDirectory,
                    localized.OutputDirectory,
                    dataDirectory);
                MappingGenerator.WriteAll(generated, outputDataDirectory);

                if (HasAllAsianLanguageOptions(options))
                {
                    using (BdatExtractionResult chinese = runner.Extract(options.GetValue("base-cn")))
                    using (BdatExtractionResult japanese = runner.Extract(options.GetValue("base-jp")))
                    using (BdatExtractionResult traditionalChinese = runner.Extract(options.GetValue("base-tw")))
                    {
                        Dictionary<string, BdatTable> sourceTables = new Dictionary<string, BdatTable>(StringComparer.Ordinal);
                        foreach (MappingDefinition mapping in config.Mappings)
                        {
                            if (!sourceTables.ContainsKey(mapping.SourceTable))
                            {
                                sourceTables.Add(
                                    mapping.SourceTable,
                                    MappingGenerator.ReadExtractedTableForAnalysis(common.OutputDirectory, mapping.SourceTable));
                            }
                        }

                        Dictionary<string, IReadOnlyDictionary<string, BdatTable>> localizedTables = new Dictionary<string, IReadOnlyDictionary<string, BdatTable>>(StringComparer.OrdinalIgnoreCase)
                        {
                            ["gb"] = ReadLocalizedTables(config, localized.OutputDirectory),
                            ["cn"] = ReadLocalizedTables(config, chinese.OutputDirectory),
                            ["jp"] = ReadLocalizedTables(config, japanese.OutputDirectory),
                            ["tw"] = ReadLocalizedTables(config, traditionalChinese.OutputDirectory)
                        };
                        ProjectCsvDocument generatedItems = ProjectDataWriter.ReadCsv(Path.Combine(outputDataDirectory, "Items.csv"));
                        IReadOnlyList<ReservedItemRecord> statuses = ReservedItemAnalyzer.AnalyzeProjectItems(
                            generatedItems,
                            config,
                            sourceTables,
                            localizedTables);
                        ReservedItemAnalyzer.WriteStatusCsv(Path.Combine(outputDataDirectory, "ItemStatus.csv"), statuses);
                        Console.WriteLine("Reserved item statuses generated: " + statuses.Count);
                    }
                }
            }

            string candidateDatabase = Path.Combine(outputPaths.OutputDirectory, "XC2Data.db");
            DatabaseBuilder.Build(ddlDirectory, outputDataDirectory, candidateDatabase);
            string projectDatabase = Path.Combine(projectRoot, "XC2SaveNETThingy", "XC2Data.db");
            int baselineForeignKeys = Verifier.CountForeignKeys(projectDatabase);
            VerificationResult verification = Verifier.Verify(candidateDatabase, outputDataDirectory, baselineForeignKeys);
            IReadOnlyList<TableDiff> diffs = DiffReportWriter.CompareDirectories(dataDirectory, outputDataDirectory);
            string jsonReport = Path.ChangeExtension(outputPaths.ReportPath, ".json");
            DiffReportWriter.WriteJson(jsonReport, diffs, baselineForeignKeys, verification.ForeignKeyViolations);
            DiffReportWriter.WriteMarkdown(outputPaths.ReportPath, diffs, baselineForeignKeys, verification.ForeignKeyViolations);
            if (!verification.Passed)
            {
                throw new InvalidDataException("Candidate verification failed: " + string.Join("; ", verification.Errors));
            }

            if (options.Replace)
            {
                ReplaceProjectData(dataDirectory, projectDatabase, outputDataDirectory, candidateDatabase);
            }

            Console.WriteLine("Candidate generated: " + candidateDatabase);
            Console.WriteLine("Report path: " + outputPaths.ReportPath);
        }

        private static string ResolveEnglishDirectory(string path)
        {
            string fullPath = Path.GetFullPath(path);
            string gbDirectory = Path.Combine(fullPath, "gb");
            return Directory.Exists(gbDirectory) ? gbDirectory : fullPath;
        }

        private static bool HasAllAsianLanguageOptions(ImporterArguments options)
        {
            bool any = !string.IsNullOrWhiteSpace(options.GetValue("base-cn")) ||
                !string.IsNullOrWhiteSpace(options.GetValue("base-jp")) ||
                !string.IsNullOrWhiteSpace(options.GetValue("base-tw"));
            if (!any) return false;
            if (string.IsNullOrWhiteSpace(options.GetValue("base-cn")) ||
                string.IsNullOrWhiteSpace(options.GetValue("base-jp")) ||
                string.IsNullOrWhiteSpace(options.GetValue("base-tw")))
            {
                throw new ArgumentException("--base-cn, --base-jp, and --base-tw must be supplied together.");
            }

            return true;
        }

        private static IReadOnlyDictionary<string, BdatTable> ReadLocalizedTables(MappingConfig config, string root)
        {
            Dictionary<string, BdatTable> tables = new Dictionary<string, BdatTable>(StringComparer.Ordinal);
            foreach (MappingDefinition mapping in config.Mappings)
            {
                if (!tables.ContainsKey(mapping.LocalizedTable))
                {
                    tables.Add(mapping.LocalizedTable, MappingGenerator.ReadExtractedTableForAnalysis(root, mapping.LocalizedTable));
                }
            }

            return tables;
        }

        private static void ReplaceProjectData(string projectDataDirectory, string projectDatabase, string candidateDataDirectory, string candidateDatabase)
        {
            string backupRoot = Path.Combine(Path.GetDirectoryName(candidateDatabase), "backup-" + DateTime.UtcNow.ToString("yyyyMMdd-HHmmss", System.Globalization.CultureInfo.InvariantCulture));
            Directory.CreateDirectory(backupRoot);
            foreach (string candidate in Directory.GetFiles(candidateDataDirectory, "*.csv"))
            {
                string target = Path.Combine(projectDataDirectory, Path.GetFileName(candidate));
                if (File.Exists(target)) File.Copy(target, Path.Combine(backupRoot, Path.GetFileName(target)), true);
                File.Copy(candidate, target, true);
            }
            if (File.Exists(projectDatabase)) File.Copy(projectDatabase, Path.Combine(backupRoot, "XC2Data.db"), true);
            string temporaryTarget = projectDatabase + ".new";
            File.Copy(candidateDatabase, temporaryTarget, true);
            File.Move(temporaryTarget, projectDatabase, true);
            Console.WriteLine("Project data replaced; backup: " + backupRoot);
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
            writer.WriteLine("    [--base-cn <dir> --base-jp <dir> --base-tw <dir>]");
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
            "base-cn",
            "base-jp",
            "base-tw",
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
