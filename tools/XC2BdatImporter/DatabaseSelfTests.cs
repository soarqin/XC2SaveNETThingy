using System;
using System.IO;
using System.Text;

namespace XC2BdatImporter
{
    internal static class DatabaseSelfTests
    {
        internal static void Run()
        {
            string directory = TemporaryExtractionDirectory.Create();
            try
            {
                CsvRoundTripPreservesCommentsAndUnicode(directory);
                DatabaseBuildUsesParameterizedValues(directory);
                FailedBuildDoesNotLeaveCandidate(directory);
            }
            finally
            {
                TemporaryExtractionDirectory.DeleteExact(directory);
            }
        }

        private static void CsvRoundTripPreservesCommentsAndUnicode(string directory)
        {
            string input = Path.Combine(directory, "sample.csv");
            string output = Path.Combine(directory, "sample-copy.csv");
            File.WriteAllText(input,
                "// comment\r\n\r\nID\tName_EN\tName_CN\r\n1\tO'Neil\t\r\n2\tMòrag\t中文\r\n",
                Encoding.Unicode);

            ProjectCsvDocument document = ProjectDataWriter.ReadCsv(input);
            AssertEqual(3, document.Columns.Count, "CSV column count");
            AssertEqual(2, document.Rows.Count, "CSV data row count");
            AssertEqual("O'Neil", document.Rows[0][1], "CSV apostrophe");
            AssertEqual(string.Empty, document.Rows[0][2], "CSV trailing empty value");
            AssertEqual("// comment", document.Preamble[0], "CSV comment preservation");

            ProjectDataWriter.WriteCsv(output, document);
            ProjectCsvDocument copy = ProjectDataWriter.ReadCsv(output);
            AssertEqual(document.Columns.Count, copy.Columns.Count, "CSV round-trip columns");
            AssertEqual(document.Rows[1][2], copy.Rows[1][2], "CSV Unicode round-trip");
            AssertEqual(document.Preamble[0], copy.Preamble[0], "CSV preamble round-trip");
        }

        private static void DatabaseBuildUsesParameterizedValues(string directory)
        {
            string ddl = Path.Combine(directory, "Demo.sql");
            string csv = Path.Combine(directory, "Demo.csv");
            string database = Path.Combine(directory, "candidate.db");
            File.WriteAllText(ddl,
                "DROP TABLE IF EXISTS [Demo];\n" +
                "CREATE TABLE [Demo] ([ID] INTEGER NOT NULL PRIMARY KEY, [Text] text NOT NULL, [Count] INTEGER NOT NULL);\n",
                Encoding.UTF8);
            File.WriteAllText(csv,
                "ID\tText\tCount\r\n1\tO'Neil\t7\r\n2\t中文\t0\r\n",
                Encoding.Unicode);

            DatabaseBuilder.Build(ddl, directory, database);
            DatabaseQueryResult result = DatabaseBuilder.ReadSingleTextAndInteger(database, "Demo", 1);
            AssertEqual("O'Neil", result.Text, "SQLite apostrophe value");
            AssertEqual(7L, result.Number, "SQLite integer value");
        }

        private static void FailedBuildDoesNotLeaveCandidate(string directory)
        {
            string ddl = Path.Combine(directory, "Broken.sql");
            string csv = Path.Combine(directory, "Broken.csv");
            string database = Path.Combine(directory, "broken.db");
            File.WriteAllText(ddl,
                "DROP TABLE IF EXISTS [Broken];\n" +
                "CREATE TABLE [Broken] ([ID] INTEGER NOT NULL PRIMARY KEY, [Text] text NOT NULL);\n",
                Encoding.UTF8);
            File.WriteAllText(csv, "ID\tText\r\n1\tOne\r\n1\tDuplicate\r\n", Encoding.Unicode);

            AssertThrows<DatabaseBuildException>(() => DatabaseBuilder.Build(ddl, directory, database), "duplicate primary key");
            if (File.Exists(database))
            {
                throw new InvalidOperationException("Failed database build left a candidate file.");
            }
        }

        private static TException AssertThrows<TException>(Action action, string description)
            where TException : Exception
        {
            try
            {
                action();
            }
            catch (TException exception)
            {
                return exception;
            }

            throw new InvalidOperationException("Expected " + typeof(TException).Name + ": " + description);
        }

        private static void AssertEqual<T>(T expected, T actual, string description)
        {
            if (!object.Equals(expected, actual))
            {
                throw new InvalidOperationException(description + " expected '" + expected + "' but got '" + actual + "'.");
            }
        }
    }

    internal sealed class DatabaseQueryResult
    {
        internal DatabaseQueryResult(string text, long number)
        {
            Text = text;
            Number = number;
        }

        internal string Text { get; }
        internal long Number { get; }
    }
}
