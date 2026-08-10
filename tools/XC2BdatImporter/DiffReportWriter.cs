using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace XC2BdatImporter
{
    internal sealed class TableDiff
    {
        public string Table { get; set; }
        public int OldRowCount { get; set; }
        public int NewRowCount { get; set; }
        public List<string> AddedIds { get; set; } = new List<string>();
        public List<string> RemovedIds { get; set; } = new List<string>();
        public List<string> ChangedIds { get; set; } = new List<string>();
        public int PreservedRows { get; set; }
        public int FilteredRows { get; set; }
        public List<string> Warnings { get; set; } = new List<string>();
    }

    internal static class DiffReportWriter
    {
        internal static IReadOnlyList<TableDiff> CompareDirectories(string oldDirectory, string newDirectory)
        {
            HashSet<string> names = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (string path in Directory.GetFiles(oldDirectory, "*.csv")) names.Add(Path.GetFileName(path));
            foreach (string path in Directory.GetFiles(newDirectory, "*.csv")) names.Add(Path.GetFileName(path));
            List<TableDiff> result = new List<TableDiff>();
            foreach (string name in names.OrderBy(value => value, StringComparer.OrdinalIgnoreCase))
            {
                ProjectCsvDocument oldDocument = TryRead(Path.Combine(oldDirectory, name));
                ProjectCsvDocument newDocument = TryRead(Path.Combine(newDirectory, name));
                result.Add(Compare(name, oldDocument, newDocument));
            }
            return result;
        }

        internal static TableDiff Compare(string table, ProjectCsvDocument oldDocument, ProjectCsvDocument newDocument)
        {
            TableDiff diff = new TableDiff { Table = table };
            Dictionary<string, IReadOnlyList<string>> oldRows = ToRows(oldDocument);
            Dictionary<string, IReadOnlyList<string>> newRows = ToRows(newDocument);
            diff.OldRowCount = oldRows.Count;
            diff.NewRowCount = newRows.Count;
            foreach (KeyValuePair<string, IReadOnlyList<string>> pair in newRows)
            {
                IReadOnlyList<string> oldRow;
                if (!oldRows.TryGetValue(pair.Key, out oldRow)) diff.AddedIds.Add(pair.Key);
                else if (!RowsEqual(oldRow, pair.Value)) diff.ChangedIds.Add(pair.Key);
                else diff.PreservedRows++;
            }
            foreach (string id in oldRows.Keys)
            {
                if (!newRows.ContainsKey(id)) diff.RemovedIds.Add(id);
            }
            return diff;
        }

        internal static void WriteJson(string path, IReadOnlyList<TableDiff> diffs, int foreignKeyBaseline, int foreignKeyCurrent)
        {
            object report = new
            {
                generatedAtUtc = DateTime.UtcNow,
                foreignKeyBaseline,
                foreignKeyCurrent,
                tables = diffs
            };
            File.WriteAllText(path, JsonSerializer.Serialize(report, new JsonSerializerOptions { WriteIndented = true }));
        }

        internal static void WriteMarkdown(string path, IReadOnlyList<TableDiff> diffs, int foreignKeyBaseline, int foreignKeyCurrent)
        {
            using (StreamWriter writer = new StreamWriter(path, false))
            {
                writer.WriteLine("# XC2 data diff");
                writer.WriteLine();
                writer.WriteLine("Foreign-key violations: baseline " + foreignKeyBaseline + ", candidate " + foreignKeyCurrent + ".");
                writer.WriteLine();
                writer.WriteLine("| Table | Old | New | Added | Removed | Changed | Preserved |");
                writer.WriteLine("| --- | ---: | ---: | ---: | ---: | ---: | ---: |");
                foreach (TableDiff diff in diffs)
                {
                    writer.WriteLine("| " + diff.Table + " | " + diff.OldRowCount + " | " + diff.NewRowCount + " | " + diff.AddedIds.Count + " | " + diff.RemovedIds.Count + " | " + diff.ChangedIds.Count + " | " + diff.PreservedRows + " |");
                }
            }
        }

        private static ProjectCsvDocument TryRead(string path)
        {
            return File.Exists(path) ? ProjectDataWriter.ReadCsv(path) : null;
        }

        private static Dictionary<string, IReadOnlyList<string>> ToRows(ProjectCsvDocument document)
        {
            Dictionary<string, IReadOnlyList<string>> rows = new Dictionary<string, IReadOnlyList<string>>(StringComparer.Ordinal);
            if (document == null) return rows;
            foreach (IReadOnlyList<string> row in document.Rows)
            {
                if (row.Count == 0 || string.IsNullOrWhiteSpace(row[0])) continue;
                rows[row[0]] = row;
            }
            return rows;
        }

        private static bool RowsEqual(IReadOnlyList<string> left, IReadOnlyList<string> right)
        {
            if (left.Count != right.Count) return false;
            for (int index = 0; index < left.Count; index++)
            {
                if (!string.Equals(left[index], right[index], StringComparison.Ordinal)) return false;
            }
            return true;
        }
    }
}
