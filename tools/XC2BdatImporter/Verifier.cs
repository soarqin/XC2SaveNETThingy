using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;

namespace XC2BdatImporter
{
    internal sealed class VerificationResult
    {
        internal bool Passed { get; set; }
        internal int ForeignKeyViolations { get; set; }
        internal List<string> Errors { get; } = new List<string>();
    }

    internal static class Verifier
    {
        internal static VerificationResult Verify(string databasePath, string csvDirectory, int foreignKeyBaseline)
        {
            VerificationResult result = new VerificationResult();
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection("Data Source=" + Path.GetFullPath(databasePath) + ";Read Only=True;"))
                {
                    connection.Open();
                    string integrity = ExecuteScalar(connection, "PRAGMA integrity_check;");
                    if (!string.Equals(integrity, "ok", StringComparison.OrdinalIgnoreCase)) result.Errors.Add("integrity_check returned " + integrity);
                    result.ForeignKeyViolations = ExecuteCount(connection, "PRAGMA foreign_key_check;");
                    if (result.ForeignKeyViolations > foreignKeyBaseline) result.Errors.Add("foreign_key_check increased from " + foreignKeyBaseline + " to " + result.ForeignKeyViolations);
                    foreach (string table in GetTables(connection))
                    {
                        string csvPath = Path.Combine(csvDirectory, table + ".csv");
                        if (!File.Exists(csvPath)) continue;
                        ProjectCsvDocument document = ProjectDataWriter.ReadCsv(csvPath);
                        int count = ExecuteCount(connection, "SELECT COUNT(*) FROM [" + table.Replace("]", "]]", StringComparison.Ordinal) + "];");
                        if (count != document.Rows.Count) result.Errors.Add("CSV/SQLite row count mismatch for " + table + ": " + document.Rows.Count + " vs " + count);
                        string primaryKey = GetPrimaryKeyColumn(connection, table);
                        if (!string.IsNullOrWhiteSpace(primaryKey))
                        {
                            int duplicateCount = ExecuteCount(connection, "SELECT COUNT(*) - COUNT(DISTINCT [" + primaryKey.Replace("]", "]]", StringComparison.Ordinal) + "]) FROM [" + table.Replace("]", "]]", StringComparison.Ordinal) + "];");
                            if (duplicateCount != 0) result.Errors.Add("Duplicate primary keys in " + table);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                result.Errors.Add(ex.Message);
            }
            result.Passed = result.Errors.Count == 0;
            return result;
        }

        internal static int CountForeignKeys(string databasePath)
        {
            using (SQLiteConnection connection = new SQLiteConnection("Data Source=" + Path.GetFullPath(databasePath) + ";Read Only=True;"))
            {
                connection.Open();
                return ExecuteCount(connection, "PRAGMA foreign_key_check;");
            }
        }

        private static IEnumerable<string> GetTables(SQLiteConnection connection)
        {
            using (SQLiteCommand command = connection.CreateCommand())
            {
                command.CommandText = "SELECT name FROM sqlite_master WHERE type='table' AND name NOT LIKE 'sqlite_%' ORDER BY name;";
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read()) yield return reader.GetString(0);
                }
            }
        }

        private static string GetPrimaryKeyColumn(SQLiteConnection connection, string table)
        {
            using (SQLiteCommand command = connection.CreateCommand())
            {
                command.CommandText = "PRAGMA table_info([" + table.Replace("]", "]]", StringComparison.Ordinal) + "]);";
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    string fallback = null;
                    while (reader.Read())
                    {
                        string name = reader.GetString(1);
                        int primaryKeyOrder = reader.GetInt32(5);
                        if (fallback == null) fallback = name;
                        if (primaryKeyOrder > 0) return name;
                    }

                    return fallback;
                }
            }
        }

        private static string ExecuteScalar(SQLiteConnection connection, string sql)
        {
            using (SQLiteCommand command = connection.CreateCommand())
            {
                command.CommandText = sql;
                return Convert.ToString(command.ExecuteScalar());
            }
        }

        private static int ExecuteCount(SQLiteConnection connection, string sql)
        {
            using (SQLiteCommand command = connection.CreateCommand())
            {
                command.CommandText = sql;
                if (sql.StartsWith("PRAGMA foreign_key_check", StringComparison.OrdinalIgnoreCase))
                {
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        int count = 0;
                        while (reader.Read()) count++;
                        return count;
                    }
                }
                return Convert.ToInt32(command.ExecuteScalar());
            }
        }
    }
}
