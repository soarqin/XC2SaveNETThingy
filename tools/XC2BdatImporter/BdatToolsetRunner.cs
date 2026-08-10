using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace XC2BdatImporter
{
    internal sealed class BdatToolsetRunner
    {
        private static readonly TimeSpan DefaultTimeout = TimeSpan.FromMinutes(10);

        private readonly string toolsetPath;
        private readonly TimeSpan processTimeout;

        internal BdatToolsetRunner(string toolsetPath, TimeSpan? processTimeout = null)
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

            this.processTimeout = processTimeout ?? DefaultTimeout;
            if (this.processTimeout <= TimeSpan.Zero)
            {
                throw new ArgumentOutOfRangeException(nameof(processTimeout), "The process timeout must be positive.");
            }
        }

        internal BdatExtractionResult Extract(
            string inputPath,
            CancellationToken cancellationToken = default(CancellationToken))
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

            string outputDirectory = TemporaryExtractionDirectory.Create();
            using (Process process = new Process { StartInfo = CreateStartInfo(fullInputPath, outputDirectory) })
            {
                try
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        throw new OperationCanceledException(
                            "bdat-toolset extract was canceled for input: " + fullInputPath,
                            cancellationToken);
                    }
                    try
                    {
                        if (!process.Start())
                        {
                            throw new InvalidOperationException("The process did not start.");
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new InvalidOperationException(
                            "Failed to start bdat-toolset for input: " + fullInputPath,
                            ex);
                    }

                    Task<string> standardOutputTask = process.StandardOutput.ReadToEndAsync();
                    Task<string> standardErrorTask = process.StandardError.ReadToEndAsync();
                    WaitForExit(process, fullInputPath, cancellationToken);
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
                        outputDirectory);
                }
                catch (Exception extractionException)
                {
                    TerminateProcessTree(process);
                    try
                    {
                        TemporaryExtractionDirectory.DeleteExact(outputDirectory);
                    }
                    catch (Exception cleanupException)
                    {
                        throw new AggregateException(
                            "BDAT extraction and temporary-directory cleanup both failed.",
                            extractionException,
                            cleanupException);
                    }

                    throw;
                }
            }
        }

        private ProcessStartInfo CreateStartInfo(string inputPath, string outputDirectory)
        {
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
            startInfo.ArgumentList.Add(outputDirectory);
            startInfo.ArgumentList.Add("--file-type");
            startInfo.ArgumentList.Add("json");
            startInfo.ArgumentList.Add("--pretty");
            startInfo.ArgumentList.Add(inputPath);
            return startInfo;
        }

        private void WaitForExit(Process process, string inputPath, CancellationToken cancellationToken)
        {
            long startedAt = Stopwatch.GetTimestamp();
            while (!process.WaitForExit(250))
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    throw new OperationCanceledException(
                        "bdat-toolset extract was canceled for input: " + inputPath,
                        cancellationToken);
                }

                if (Stopwatch.GetElapsedTime(startedAt) >= processTimeout)
                {
                    throw new TimeoutException(
                        "bdat-toolset extract timed out after " + processTimeout +
                        " for input: " + inputPath);
                }
            }
        }

        private static void TerminateProcessTree(Process process)
        {
            try
            {
                if (!process.HasExited)
                {
                    process.Kill(true);
                    process.WaitForExit(5000);
                }
            }
            catch (InvalidOperationException)
            {
            }
            catch (System.ComponentModel.Win32Exception)
            {
            }
        }
    }

    internal sealed class BdatExtractionResult : IDisposable
    {
        private bool disposed;

        internal BdatExtractionResult(
            int exitCode,
            string standardOutput,
            string standardError,
            string outputDirectory)
        {
            ExitCode = exitCode;
            StandardOutput = standardOutput;
            StandardError = standardError;
            OutputDirectory = TemporaryExtractionDirectory.ValidateExactChild(outputDirectory);
        }

        internal int ExitCode { get; }

        internal string StandardOutput { get; }

        internal string StandardError { get; }

        internal string OutputDirectory { get; }

        public void Dispose()
        {
            if (disposed)
            {
                return;
            }

            TemporaryExtractionDirectory.DeleteExact(OutputDirectory);
            disposed = true;
        }
    }

    internal static class TemporaryExtractionDirectory
    {
        private static readonly string RootDirectory = Path.GetFullPath(
            Path.Combine(Path.GetTempPath(), "XC2BdatImporter"));

        internal static string Create()
        {
            Directory.CreateDirectory(RootDirectory);
            string outputDirectory = ValidateExactChild(
                Path.Combine(RootDirectory, Guid.NewGuid().ToString("N")));
            Directory.CreateDirectory(outputDirectory);
            return outputDirectory;
        }

        internal static string ValidateExactChild(string path)
        {
            string fullPath = Path.TrimEndingDirectorySeparator(Path.GetFullPath(path));
            DirectoryInfo parent = Directory.GetParent(fullPath);
            StringComparison comparison = OperatingSystem.IsWindows()
                ? StringComparison.OrdinalIgnoreCase
                : StringComparison.Ordinal;
            if (parent == null || !string.Equals(
                    Path.TrimEndingDirectorySeparator(parent.FullName),
                    Path.TrimEndingDirectorySeparator(RootDirectory),
                    comparison))
            {
                throw new InvalidOperationException(
                    "Refusing to delete a directory outside the XC2BdatImporter temporary root: " + fullPath);
            }

            return fullPath;
        }

        internal static void DeleteExact(string path)
        {
            string fullPath = ValidateExactChild(path);
            if (!Directory.Exists(fullPath))
            {
                return;
            }

            FileAttributes attributes = File.GetAttributes(fullPath);
            Directory.Delete(fullPath, (attributes & FileAttributes.ReparsePoint) == 0);
        }
    }
}
