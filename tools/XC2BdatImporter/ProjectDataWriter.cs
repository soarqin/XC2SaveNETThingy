using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace XC2BdatImporter
{
    internal sealed class ProjectCsvDocument
    {
        internal ProjectCsvDocument(IReadOnlyList<string> preamble, IReadOnlyList<string> columns, IReadOnlyList<IReadOnlyList<string>> rows, IReadOnlyList<string> postamble)
        {
            Preamble = preamble;
            Columns = columns;
            Rows = rows;
            Postamble = postamble;
        }

        internal IReadOnlyList<string> Preamble { get; }
        internal IReadOnlyList<string> Columns { get; }
        internal IReadOnlyList<IReadOnlyList<string>> Rows { get; }
        internal IReadOnlyList<string> Postamble { get; }
    }

    internal static class ProjectDataWriter
    {
        private static readonly Encoding ProjectEncoding = new UnicodeEncoding(false, true);
        private static readonly string[] LanguageSuffixes =
        {
            "EN", "CN", "JA", "KO", "ZH_HANS", "ZH_HANT", "FR", "DE", "IT", "ES"
        };

        internal static ProjectCsvDocument ReadCsv(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentException("A CSV path is required.", nameof(path));
            }

            string fullPath = Path.GetFullPath(path);
            if (!File.Exists(fullPath))
            {
                throw new FileNotFoundException("Project CSV was not found.", fullPath);
            }

            string text = File.ReadAllText(fullPath, ProjectEncoding);
            List<string> lines = SplitLines(text);
            List<string> preamble = new List<string>();
            string header = null;
            int headerIndex = -1;
            for (int index = 0; index < lines.Count; index++)
            {
                string line = lines[index];
                if (IsBlankLine(line) || line.TrimStart().StartsWith("//", StringComparison.Ordinal))
                {
                    preamble.Add(line);
                    continue;
                }

                header = line;
                headerIndex = index;
                break;
            }

            if (header == null)
            {
                throw new InvalidDataException("CSV has no header: " + fullPath);
            }

            List<string> columns = ParseLine(header, fullPath, headerIndex + 1);
            if (columns.Count == 0)
            {
                throw new InvalidDataException("CSV header is empty: " + fullPath);
            }

            List<IReadOnlyList<string>> rows = new List<IReadOnlyList<string>>();
            List<string> postamble = new List<string>();
            for (int index = headerIndex + 1; index < lines.Count; index++)
            {
                string line = lines[index];
                if (IsBlankLine(line) || line.TrimStart().StartsWith("//", StringComparison.Ordinal))
                {
                    postamble.Add(line);
                    continue;
                }

                List<string> values = ParseLine(line, fullPath, index + 1);
                if (values.Count != columns.Count)
                {
                    throw new InvalidDataException(
                        "CSV row " + (index + 1) + " has " + values.Count +
                        " fields; expected " + columns.Count + ": " + fullPath);
                }

                rows.Add(values);
            }

            return new ProjectCsvDocument(preamble, columns, rows, postamble);
        }

        internal static void WriteCsv(string path, ProjectCsvDocument document)
        {
            if (document == null)
            {
                throw new ArgumentNullException(nameof(document));
            }

            string fullPath = Path.GetFullPath(path);
            string parent = Path.GetDirectoryName(fullPath);
            if (!string.IsNullOrWhiteSpace(parent))
            {
                Directory.CreateDirectory(parent);
            }

            StringBuilder output = new StringBuilder();
            for (int index = 0; index < document.Preamble.Count; index++)
            {
                output.Append(document.Preamble[index]).Append("\r\n");
            }

            AppendFields(output, document.Columns);
            output.Append("\r\n");
            for (int rowIndex = 0; rowIndex < document.Rows.Count; rowIndex++)
            {
                AppendFields(output, document.Rows[rowIndex]);
                output.Append("\r\n");
            }

            for (int index = 0; index < document.Postamble.Count; index++)
            {
                output.Append(document.Postamble[index]).Append("\r\n");
            }

            File.WriteAllText(fullPath, output.ToString(), ProjectEncoding);
        }

        internal static ProjectCsvDocument ExpandLanguageColumns(ProjectCsvDocument source)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));

            List<string> columns = new List<string>(source.Columns);
            List<string> prefixes = new List<string>();
            foreach (string column in source.Columns)
            {
                if (!column.EndsWith("_EN", StringComparison.Ordinal)) continue;
                string prefix = column.Substring(0, column.Length - 2);
                if (!prefixes.Contains(prefix)) prefixes.Add(prefix);
                foreach (string suffix in LanguageSuffixes)
                {
                    string localized = prefix + suffix;
                    if (!columns.Contains(localized)) columns.Add(localized);
                }
            }

            Dictionary<string, int> sourceIndexes = new Dictionary<string, int>(StringComparer.Ordinal);
            for (int index = 0; index < source.Columns.Count; index++)
                sourceIndexes[source.Columns[index]] = index;

            List<IReadOnlyList<string>> rows = new List<IReadOnlyList<string>>();
            foreach (IReadOnlyList<string> sourceRow in source.Rows)
            {
                List<string> row = new List<string>(columns.Count);
                foreach (string column in columns)
                {
                    int index;
                    row.Add(sourceIndexes.TryGetValue(column, out index) && index < sourceRow.Count
                        ? sourceRow[index]
                        : string.Empty);
                }

                foreach (string prefix in prefixes)
                {
                    int englishIndex = columns.IndexOf(prefix + "EN");
                    if (englishIndex < 0) continue;

                    int legacyChineseIndex = columns.IndexOf(prefix + "CN");
                    int simplifiedChineseIndex = columns.IndexOf(prefix + "ZH_HANS");
                    if (legacyChineseIndex >= 0 && simplifiedChineseIndex >= 0 &&
                        string.IsNullOrWhiteSpace(row[simplifiedChineseIndex]) &&
                        !string.IsNullOrWhiteSpace(row[legacyChineseIndex]))
                    {
                        row[simplifiedChineseIndex] = row[legacyChineseIndex];
                    }

                    foreach (string suffix in LanguageSuffixes)
                    {
                        if (suffix == "EN") continue;
                        int localizedIndex = columns.IndexOf(prefix + suffix);
                        if (localizedIndex >= 0 && string.IsNullOrWhiteSpace(row[localizedIndex]))
                            row[localizedIndex] = row[englishIndex];
                    }

                    if (legacyChineseIndex >= 0 && simplifiedChineseIndex >= 0)
                        row[legacyChineseIndex] = row[simplifiedChineseIndex];
                }

                rows.Add(row);
            }

            return new ProjectCsvDocument(source.Preamble, columns, rows, source.Postamble);
        }

        internal static void ExpandLanguageColumns(string directory)
        {
            if (!Directory.Exists(directory)) throw new DirectoryNotFoundException(directory);
            foreach (string path in Directory.GetFiles(directory, "*.csv"))
                WriteCsv(path, ExpandLanguageColumns(ReadCsv(path)));
        }

        private static List<string> SplitLines(string text)
        {
            List<string> lines = new List<string>();
            int start = 0;
            for (int index = 0; index < text.Length; index++)
            {
                if (text[index] != '\n')
                {
                    continue;
                }

                int length = index - start;
                if (length > 0 && text[index - 1] == '\r')
                {
                    length--;
                }

                lines.Add(text.Substring(start, length));
                start = index + 1;
            }

            if (start < text.Length)
            {
                lines.Add(text.Substring(start).TrimEnd('\r'));
            }

            return lines;
        }

        private static bool IsBlankLine(string line)
        {
            return string.IsNullOrWhiteSpace(line);
        }

        private static List<string> ParseLine(string line, string path, int lineNumber)
        {
            List<string> values = new List<string>();
            StringBuilder value = new StringBuilder();
            bool quoted = false;
            for (int index = 0; index < line.Length; index++)
            {
                char current = line[index];
                if (current == '"')
                {
                    if (quoted && index + 1 < line.Length && line[index + 1] == '"')
                    {
                        value.Append('"');
                        index++;
                    }
                    else
                    {
                        quoted = !quoted;
                    }
                }
                else if (current == '\t' && !quoted)
                {
                    values.Add(value.ToString());
                    value.Clear();
                }
                else
                {
                    value.Append(current);
                }
            }

            if (quoted)
            {
                throw new InvalidDataException("Unterminated quoted field at " + path + ":" + lineNumber);
            }

            values.Add(value.ToString());
            return values;
        }

        private static void AppendFields(StringBuilder output, IReadOnlyList<string> fields)
        {
            for (int index = 0; index < fields.Count; index++)
            {
                if (index != 0)
                {
                    output.Append('\t');
                }

                string value = fields[index] ?? string.Empty;
                bool quote = value.IndexOfAny(new[] { '\t', '\r', '\n', '"' }) >= 0;
                if (quote)
                {
                    output.Append('"').Append(value.Replace("\"", "\"\"", StringComparison.Ordinal)).Append('"');
                }
                else
                {
                    output.Append(value);
                }
            }
        }
    }
}
