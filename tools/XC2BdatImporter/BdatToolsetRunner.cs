using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace XC2BdatImporter
{
    internal sealed class BdatToolsetRunner
    {
        private readonly string toolsetPath;

        internal BdatToolsetRunner(string toolsetPath)
        {
            if (string.IsNullOrWhiteSpace(toolsetPath))
            {
                throw new ArgumentException("A BDAT toolset path is required.", nameof(toolsetPath));
            }

            this.toolsetPath = Path.GetFullPath(toolsetPath);
            if (!File.Exists(this.toolsetPath))
            {
                throw new FileNotFoundException("BDAT toolset executable was not found.", this.toolsetPath);
            }
        }

        internal BdatExtractionResult Extract(string inputPath)
        {
            if (string.IsNullOrWhiteSpace(inputPath))
            {
                throw new ArgumentException("A BDAT input path is required.", nameof(inputPath));
            }

            string fullInputPath = Path.GetFullPath(inputPath);
            if (!File.Exists(fullInputPath))
            {
                throw new FileNotFoundException("BDAT input file was not found.", fullInputPath);
            }

            string fullOutputDirectory = Path.Combine(
                Path.GetTempPath(),
                "XC2BdatImporter",
                Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(fullOutputDirectory);

            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = toolsetPath,
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };
            startInfo.ArgumentList.Add("extract");
            startInfo.ArgumentList.Add("--out-dir");
            startInfo.ArgumentList.Add(fullOutputDirectory);
            startInfo.ArgumentList.Add("--file-type");
            startInfo.ArgumentList.Add("json");
            startInfo.ArgumentList.Add("--pretty");
            startInfo.ArgumentList.Add(fullInputPath);

            using (Process process = new Process { StartInfo = startInfo })
            {
                if (!process.Start())
                {
                    throw new InvalidOperationException(
                        "Failed to start bdat-toolset for input: " + fullInputPath);
                }

                Task<string> standardOutputTask = process.StandardOutput.ReadToEndAsync();
                Task<string> standardErrorTask = process.StandardError.ReadToEndAsync();
                process.WaitForExit();
                Task.WaitAll(standardOutputTask, standardErrorTask);

                string standardOutput = standardOutputTask.Result;
                string standardError = standardErrorTask.Result;

                if (process.ExitCode != 0)
                {
                    string details = string.IsNullOrWhiteSpace(standardError)
                        ? standardOutput.Trim()
                        : standardError.Trim();
                    throw new InvalidOperationException(
                        "bdat-toolset extract failed for '" + fullInputPath +
                        "' with exit code " + process.ExitCode + ". " + details);
                }

                return new BdatExtractionResult(
                    process.ExitCode,
                    standardOutput,
                    standardError,
                    fullOutputDirectory);
            }
        }
    }

    internal sealed class BdatExtractionResult
    {
        internal BdatExtractionResult(
            int exitCode,
            string standardOutput,
            string standardError,
            string outputDirectory)
        {
            ExitCode = exitCode;
            StandardOutput = standardOutput;
            StandardError = standardError;
            OutputDirectory = outputDirectory;
        }

        internal int ExitCode { get; }

        internal string StandardOutput { get; }

        internal string StandardError { get; }

        internal string OutputDirectory { get; }
    }
}
