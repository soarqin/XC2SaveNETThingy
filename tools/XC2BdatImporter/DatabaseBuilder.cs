using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Text.RegularExpressions;

namespace XC2BdatImporter
{
    internal static class DatabaseBuilder
    {
        private static readonly Regex CreateTablePattern = new Regex(
            @"CREATE\s+TABLE\s+\[([^\]]+)\]", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

        internal static void Build(string ddlPathOrDirectory, string csvDirectory, string outputPath)
        {
            if (string.IsNullOrWhiteSpace(ddlPathOrDirectory) || string.IsNullOrWhiteSpace(csvDirectory) || string.IsNullOrWhiteSpace(outputPath))
            {
                throw new ArgumentException("DDL, CSV, and output paths are required.");
            }

            string ddlPath = Path.GetFullPath(ddlPathOrDirectory);
            string csvRoot = Path.GetFullPath(csvDirectory);
            string destination = Path.GetFullPath(outputPath);
            string destinationDirectory = Path.GetDirectoryName(destination);
            if (!Directory.Exists(csvRoot))
            {
                throw new DirectoryNotFoundException("CSV directory was not found: " + csvRoot);
            }

            List<string> ddlFiles = File.Exists(ddlPath)
                ? new List<string> { ddlPath }
                : new List<string>(Directory.GetFiles(ddlPath, "*.sql"));
            ddlFiles.Sort(StringComparer.OrdinalIgnoreCase);
            if (ddlFiles.Count == 0)
            {
                throw new DatabaseBuildException("No DDL files were found: " + ddlPath);
            }

            string tempPath = destination + ".tmp-" + Guid.NewGuid().ToString("N");
            try
            {
                if (!string.IsNullOrWhiteSpace(destinationDirectory))
                {
                    Directory.CreateDirectory(destinationDirectory);
                }

                using (SQLiteConnection connection = new SQLiteConnection("Data Source=" + tempPath + ";Version=3;New=True;"))
                {
                    connection.Open();
                    using (SQLiteTransaction transaction = connection.BeginTransaction())
                    {
                        foreach (string ddlFile in ddlFiles)
                        {
                            BuildTable(connection, transaction, ddlFile, csvRoot);
                        }

                        transaction.Commit();
                    }
                }

                if (File.Exists(destination))
                {
                    File.Delete(destination);
                }

                File.Move(tempPath, destination);
            }
            catch (DatabaseBuildException)
            {
                DeleteCandidate(tempPath);
                throw;
            }
            catch (Exception ex)
            {
                DeleteCandidate(tempPath);
                throw new DatabaseBuildException("SQLite database build failed.", ex);
            }
        }

        internal static DatabaseQueryResult ReadSingleTextAndInteger(string databasePath, string tableName, long id)
        {
            using (SQLiteConnection connection = new SQLiteConnection("Data Source=" + Path.GetFullPath(databasePath) + ";Read Only=True;"))
            {
                connection.Open();
                using (SQLiteCommand command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT [Text], [Count] FROM [" + tableName.Replace("]", "]]", StringComparison.Ordinal) + "] WHERE [ID] = @id";
                    command.Parameters.AddWithValue("@id", id);
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        if (!reader.Read())
                        {
                            throw new DatabaseBuildException("Expected test row was not found.");
                        }

                        return new DatabaseQueryResult(reader.GetString(0), reader.GetInt64(1));
                    }
                }
            }
        }

        private static void BuildTable(SQLiteConnection connection, SQLiteTransaction transaction, string ddlFile, string csvRoot)
        {
            string sql = File.ReadAllText(ddlFile);
            Match match = CreateTablePattern.Match(sql);
            if (!match.Success)
            {
                throw new DatabaseBuildException("DDL does not contain CREATE TABLE: " + ddlFile);
            }

            string tableName = match.Groups[1].Value;
            string csvPath = Path.Combine(csvRoot, tableName + ".csv");
            if (!File.Exists(csvPath))
            {
                throw new DatabaseBuildException("CSV for table '" + tableName + "' was not found: " + csvPath);
            }

            ProjectCsvDocument document = ProjectDataWriter.ExpandLanguageColumns(ProjectDataWriter.ReadCsv(csvPath));
            using (SQLiteCommand ddlCommand = connection.CreateCommand())
            {
                ddlCommand.Transaction = transaction;
                ddlCommand.CommandText = sql;
                ddlCommand.ExecuteNonQuery();
            }
            AddMissingColumns(connection, transaction, tableName, sql, document.Columns);

            string columnList = string.Join(", ", EscapeColumns(document.Columns));
            string parameterList = string.Join(", ", ParameterNames(document.Columns.Count));
            using (SQLiteCommand insert = connection.CreateCommand())
            {
                insert.Transaction = transaction;
                insert.CommandText = "INSERT INTO [" + tableName.Replace("]", "]]", StringComparison.Ordinal) + "] (" + columnList + ") VALUES (" + parameterList + ")";
                for (int index = 0; index < document.Columns.Count; index++)
                {
                    insert.Parameters.Add(new SQLiteParameter("@p" + index));
                }

                foreach (IReadOnlyList<string> row in document.Rows)
                {
                    for (int index = 0; index < row.Count; index++)
                    {
                        insert.Parameters[index].Value = ConvertValue(row[index]);
                    }

                    try
                    {
                        insert.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        throw new DatabaseBuildException(
                            "Failed inserting row into table '" + tableName + "': " + ex.Message +
                            "; row=" + string.Join(" | ", row),
                            ex);
                    }
                }
            }
        }

        private static void AddMissingColumns(SQLiteConnection connection, SQLiteTransaction transaction, string tableName, string sql, IReadOnlyList<string> columns)
        {
            foreach (string column in columns)
            {
                if (Regex.IsMatch(sql, "\\[" + Regex.Escape(column) + "\\]", RegexOptions.IgnoreCase)) continue;
                using (SQLiteCommand alter = connection.CreateCommand())
                {
                    alter.Transaction = transaction;
                    alter.CommandText = "ALTER TABLE [" + tableName.Replace("]", "]]", StringComparison.Ordinal) + "] ADD COLUMN [" + column.Replace("]", "]]", StringComparison.Ordinal) + "] text NOT NULL DEFAULT ''";
                    alter.ExecuteNonQuery();
                }
            }
        }

        private static IEnumerable<string> EscapeColumns(IReadOnlyList<string> columns)
        {
            for (int index = 0; index < columns.Count; index++)
            {
                yield return "[" + columns[index].Replace("]", "]]", StringComparison.Ordinal) + "]";
            }
        }

        private static IEnumerable<string> ParameterNames(int count)
        {
            for (int index = 0; index < count; index++)
            {
                yield return "@p" + index;
            }
        }

        private static object ConvertValue(string value)
        {
            long integer;
            if (!string.IsNullOrEmpty(value) && long.TryParse(value, out integer))
            {
                return integer;
            }

            return value ?? string.Empty;
        }

        private static void DeleteCandidate(string path)
        {
            try
            {
                if (File.Exists(path))
                {
                    File.Delete(path);
                }
            }
            catch
            {
            }
        }
    }

    internal sealed class DatabaseBuildException : Exception
    {
        internal DatabaseBuildException(string message) : base(message) { }
        internal DatabaseBuildException(string message, Exception innerException) : base(message, innerException) { }
    }
}
