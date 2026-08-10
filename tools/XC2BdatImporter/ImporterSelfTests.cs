using System;
using System.IO;
using System.Text.Json;

namespace XC2BdatImporter
{
    internal static class ImporterSelfTests
    {
        internal static void Run()
        {
            string fixtureDirectory = TemporaryExtractionDirectory.Create();
            try
            {
                ReadValidTablePreservesIdsAndUnknownFields(fixtureDirectory);
                MissingRequiredColumnReportsContext(fixtureDirectory);
                DuplicateIdIsRejected(fixtureDirectory);
                NullStringIsAccepted(fixtureDirectory);
                InconsistentFieldTypeIsRejected(fixtureDirectory);
            }
            finally
            {
                TemporaryExtractionDirectory.DeleteExact(fixtureDirectory);
            }
        }

        private static void ReadValidTablePreservesIdsAndUnknownFields(string fixtureDirectory)
        {
            string path = WriteFixture(
                fixtureDirectory,
                "valid.json",
                """
                {
                  "schema": [
                    { "name": "style", "type": 2 },
                    { "name": "name", "type": 7 }
                  ],
                  "rows": [
                    { "$id": 1, "style": 0, "name": "Rex" },
                    { "$id": 2, "style": 36, "name": "Team Lora", "future": true }
                  ]
                }
                """);

            BdatTable table = BdatJsonReader.ReadTable(path, new[] { "style", "name" });
            AssertEqual(2, table.Rows.Count, "valid row count");
            AssertEqual(2, table.Schema.Count, "valid schema count");
            AssertEqual(1L, table.Rows[0].Id, "first row id");
            AssertEqual(JsonValueKind.True, table.Rows[1].Fields["future"].ValueKind, "unknown field");
        }

        private static void MissingRequiredColumnReportsContext(string fixtureDirectory)
        {
            string path = WriteFixture(
                fixtureDirectory,
                "missing-column.json",
                """
                {
                  "schema": [{ "name": "style", "type": 2 }],
                  "rows": [{ "$id": 1, "style": 0 }]
                }
                """);

            BdatSchemaException exception = AssertThrows<BdatSchemaException>(
                () => BdatJsonReader.ReadTable(path, new[] { "style", "name" }),
                "missing required column");
            AssertContains(exception.Message, Path.GetFullPath(path), "missing-column file context");
            AssertContains(exception.Message, "missing-column", "missing-column table context");
            AssertContains(exception.Message, "name", "missing-column field context");
        }

        private static void DuplicateIdIsRejected(string fixtureDirectory)
        {
            string path = WriteFixture(
                fixtureDirectory,
                "duplicate-id.json",
                """
                {
                  "schema": [{ "name": "name", "type": 7 }],
                  "rows": [
                    { "$id": 7, "name": "Rex" },
                    { "$id": 7, "name": "Nia" }
                  ]
                }
                """);

            BdatSchemaException exception = AssertThrows<BdatSchemaException>(
                () => BdatJsonReader.ReadTable(path, new[] { "name" }),
                "duplicate id");
            AssertContains(exception.Message, "$id", "duplicate id field context");
        }

        private static void NullStringIsAccepted(string fixtureDirectory)
        {
            string path = WriteFixture(
                fixtureDirectory,
                "null-string.json",
                """
                {
                  "schema": [{ "name": "name", "type": 7 }],
                  "rows": [
                    { "$id": 1, "name": null },
                    { "$id": 2, "name": "Nia" }
                  ]
                }
                """);

            BdatTable table = BdatJsonReader.ReadTable(path, new[] { "name" });
            AssertEqual(JsonValueKind.Null, table.Rows[0].Fields["name"].ValueKind, "null string");
        }

        private static void InconsistentFieldTypeIsRejected(string fixtureDirectory)
        {
            string path = WriteFixture(
                fixtureDirectory,
                "bad-type.json",
                """
                {
                  "schema": [{ "name": "name", "type": 7 }],
                  "rows": [
                    { "$id": 1, "name": "Rex" },
                    { "$id": 2, "name": 42 }
                  ]
                }
                """);

            BdatSchemaException exception = AssertThrows<BdatSchemaException>(
                () => BdatJsonReader.ReadTable(path, new[] { "name" }),
                "inconsistent field type");
            AssertContains(exception.Message, "name", "bad type field context");
        }

        private static string WriteFixture(string directory, string fileName, string json)
        {
            string path = Path.Combine(directory, fileName);
            File.WriteAllText(path, json);
            return path;
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

        private static void AssertContains(string actual, string expected, string description)
        {
            if (actual.IndexOf(expected, StringComparison.OrdinalIgnoreCase) < 0)
            {
                throw new InvalidOperationException(
                    description + " expected '" + expected + "' in '" + actual + "'.");
            }
        }

        private static void AssertEqual<T>(T expected, T actual, string description)
        {
            if (!object.Equals(expected, actual))
            {
                throw new InvalidOperationException(
                    description + " expected '" + expected + "' but got '" + actual + "'.");
            }
        }
    }
}
