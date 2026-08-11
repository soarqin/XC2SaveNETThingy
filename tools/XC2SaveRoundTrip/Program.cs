using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace XC2SaveRoundTrip
{
    internal static class Program
    {
        [STAThread]
        private static int Main(string[] args)
        {
            try
            {
                Arguments options = Arguments.Parse(args);
                if (options.SelfTest)
                {
                    RoundTripSelfTests.Run();
                    Console.WriteLine("SELF_TEST_OK");
                    return 0;
                }

                if (string.IsNullOrWhiteSpace(options.InputDirectory) || string.IsNullOrWhiteSpace(options.OutputDirectory))
                {
                    Console.Error.WriteLine("Both --input-dir and --output-dir are required.");
                    WriteUsage(Console.Error);
                    return 2;
                }

                if (!Directory.Exists(options.InputDirectory))
                {
                    Console.Error.WriteLine("Input directory was not found: " + Path.GetFullPath(options.InputDirectory));
                    return 1;
                }

                Directory.CreateDirectory(options.OutputDirectory);
                string[] files = Directory.GetFiles(options.InputDirectory, "*", SearchOption.AllDirectories)
                    .OrderBy(path => path, StringComparer.OrdinalIgnoreCase)
                    .ToArray();
                if (files.Length == 0)
                {
                    Console.WriteLine("NO_REAL_SAMPLES; structure-level verifier is ready for an external sample directory.");
                    return 0;
                }

                int failures = 0;
                foreach (string inputPath in files)
                {
                    string outputPath = Path.Combine(options.OutputDirectory, Path.GetFileName(inputPath) + ".roundtrip");
                    try
                    {
                        RoundTripResult result = RawSaveInspector.Inspect(inputPath, outputPath, options.Compress);
                        Console.WriteLine(
                            (result.Passed ? "PASS" : "FAIL") +
                            " input=" + inputPath +
                            " model=" + result.ModelType +
                            " rawLength=0x" + result.SourceLength.ToString("X") +
                            " serializedLength=0x" + result.SerializedLength.ToString("X") +
                            " saveVersion=0x" + result.SaveVersionValue.ToString("X4") +
                            " differences=" + result.Differences.Count);
                        foreach (string difference in result.Differences)
                            Console.WriteLine("  " + difference);
                        if (!result.Passed) failures++;
                    }
                    catch (Exception ex)
                    {
                        failures++;
                        Console.WriteLine("FAIL input=" + inputPath + " reason=" + ex.Message);
                    }
                }

                return failures == 0 ? 0 : 1;
            }
            catch (ArgumentException ex)
            {
                Console.Error.WriteLine(ex.Message);
                WriteUsage(Console.Error);
                return 2;
            }
        }

        private static void WriteUsage(TextWriter writer)
        {
            writer.WriteLine("Usage: XC2SaveRoundTrip --input-dir <dir> --output-dir <dir> [--compress]");
            writer.WriteLine("       XC2SaveRoundTrip --self-test");
        }
    }

    internal sealed class Arguments
    {
        internal string InputDirectory { get; private set; }
        internal string OutputDirectory { get; private set; }
        internal bool Compress { get; private set; }
        internal bool SelfTest { get; private set; }

        internal static Arguments Parse(string[] args)
        {
            Arguments result = new Arguments();
            for (int index = 0; index < args.Length; index++)
            {
                string argument = args[index];
                if (argument == "--self-test") { result.SelfTest = true; continue; }
                if (argument == "--compress") { result.Compress = true; continue; }
                if (argument == "--input-dir") { result.InputDirectory = ReadValue(args, ref index, argument); continue; }
                if (argument == "--output-dir") { result.OutputDirectory = ReadValue(args, ref index, argument); continue; }
                throw new ArgumentException("Unknown option: " + argument);
            }

            if (result.SelfTest && (!string.IsNullOrWhiteSpace(result.InputDirectory) || !string.IsNullOrWhiteSpace(result.OutputDirectory)))
                throw new ArgumentException("--self-test cannot be combined with input or output options.");
            return result;
        }

        private static string ReadValue(string[] args, ref int index, string option)
        {
            if (index + 1 >= args.Length || args[index + 1].StartsWith("--", StringComparison.Ordinal))
                throw new ArgumentException(option + " requires a value.");
            return args[++index];
        }
    }
}
