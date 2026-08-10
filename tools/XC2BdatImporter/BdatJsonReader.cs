using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace XC2BdatImporter
{
    internal static class BdatJsonReader
    {
        internal static BdatTable ReadTable(string path, IEnumerable<string> requiredColumns = null)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentException("A BDAT JSON path is required.", nameof(path));
            }

            string fullPath = Path.GetFullPath(path);
            if (!File.Exists(fullPath))
            {
                throw new FileNotFoundException("BDAT JSON file was not found.", fullPath);
            }

            string tableName = Path.GetFileNameWithoutExtension(fullPath);
            try
            {
                using JsonDocument document = JsonDocument.Parse(File.ReadAllText(fullPath));
                JsonElement root = document.RootElement;
                JsonElement schemaElement = GetRequiredProperty(root, "schema", fullPath, tableName);
                JsonElement rowsElement = GetRequiredProperty(root, "rows", fullPath, tableName);
                if (schemaElement.ValueKind != JsonValueKind.Array)
                {
                    throw Error(fullPath, tableName, "schema", "The schema must be an array.");
                }

                if (rowsElement.ValueKind != JsonValueKind.Array)
                {
                    throw Error(fullPath, tableName, "rows", "The rows must be an array.");
                }

                List<BdatColumn> schema = ReadSchema(schemaElement, fullPath, tableName);
                ValidateRequiredColumns(schema, requiredColumns, fullPath, tableName);
                List<BdatRow> rows = ReadRows(rowsElement, schema, fullPath, tableName);
                return new BdatTable(fullPath, tableName, schema, rows);
            }
            catch (BdatSchemaException)
            {
                throw;
            }
            catch (JsonException ex)
            {
                throw new BdatSchemaException(fullPath, tableName, string.Empty, "The JSON could not be parsed.", ex);
            }
            catch (InvalidOperationException ex)
            {
                throw new BdatSchemaException(fullPath, tableName, string.Empty, "The JSON structure is invalid.", ex);
            }
        }

        private static List<BdatColumn> ReadSchema(JsonElement schemaElement, string sourceFile, string tableName)
        {
            List<BdatColumn> schema = new List<BdatColumn>();
            HashSet<string> names = new HashSet<string>(StringComparer.Ordinal);
            foreach (JsonElement columnElement in schemaElement.EnumerateArray())
            {
                if (columnElement.ValueKind != JsonValueKind.Object)
                {
                    throw Error(sourceFile, tableName, "schema", "Each schema entry must be an object.");
                }

                JsonElement nameElement = GetRequiredProperty(columnElement, "name", sourceFile, tableName);
                if (nameElement.ValueKind != JsonValueKind.String || string.IsNullOrWhiteSpace(nameElement.GetString()))
                {
                    throw Error(sourceFile, tableName, "name", "Schema column names must be non-empty strings.");
                }

                string name = nameElement.GetString();
                if (!names.Add(name))
                {
                    throw Error(sourceFile, tableName, name, "The schema contains a duplicate column.");
                }

                int type = -1;
                JsonElement typeElement;
                if (columnElement.TryGetProperty("type", out typeElement) &&
                    (typeElement.ValueKind != JsonValueKind.Number || !typeElement.TryGetInt32(out type)))
                {
                    throw Error(sourceFile, tableName, name, "The schema type must be a 32-bit integer.");
                }

                schema.Add(new BdatColumn(name, type));
            }

            if (schema.Count == 0)
            {
                throw Error(sourceFile, tableName, "schema", "The schema must contain at least one column.");
            }

            return schema;
        }

        private static List<BdatRow> ReadRows(JsonElement rowsElement, IReadOnlyList<BdatColumn> schema, string sourceFile, string tableName)
        {
            List<BdatRow> rows = new List<BdatRow>();
            HashSet<long> ids = new HashSet<long>();
            Dictionary<string, JsonValueKind> kinds = new Dictionary<string, JsonValueKind>(StringComparer.Ordinal);
            for (int rowIndex = 0; rowIndex < rowsElement.GetArrayLength(); rowIndex++)
            {
                JsonElement rowElement = rowsElement[rowIndex];
                if (rowElement.ValueKind != JsonValueKind.Object)
                {
                    throw Error(sourceFile, tableName, "$id", "Each row must be an object.");
                }

                JsonElement idElement = GetRequiredProperty(rowElement, "$id", sourceFile, tableName);
                long id;
                if (idElement.ValueKind != JsonValueKind.Number || !idElement.TryGetInt64(out id) || id < 0)
                {
                    throw Error(sourceFile, tableName, "$id", "Row IDs must be non-negative integers.");
                }

                if (!ids.Add(id))
                {
                    throw Error(sourceFile, tableName, "$id", "The row ID is duplicated: " + id + ".");
                }

                Dictionary<string, JsonElement> fields = new Dictionary<string, JsonElement>(StringComparer.Ordinal);
                foreach (JsonProperty property in rowElement.EnumerateObject())
                {
                    if (!property.NameEquals("$id"))
                    {
                        fields[property.Name] = property.Value.Clone();
                    }
                }

                for (int columnIndex = 0; columnIndex < schema.Count; columnIndex++)
                {
                    BdatColumn column = schema[columnIndex];
                    JsonElement value;
                    if (!rowElement.TryGetProperty(column.Name, out value))
                    {
                        throw Error(sourceFile, tableName, column.Name, "A schema column is missing from a row.");
                    }

                    ValidateFieldType(column, value, kinds, sourceFile, tableName);
                }

                rows.Add(new BdatRow(id, fields));
            }

            return rows;
        }

        private static void ValidateFieldType(BdatColumn column, JsonElement value, IDictionary<string, JsonValueKind> kinds, string sourceFile, string tableName)
        {
            if (value.ValueKind == JsonValueKind.Null)
            {
                return;
            }

            JsonValueKind actualKind = NormalizeValueKind(value.ValueKind);
            JsonValueKind knownKind;
            if (kinds.TryGetValue(column.Name, out knownKind) && knownKind != actualKind)
            {
                throw Error(sourceFile, tableName, column.Name, "Field value kinds are inconsistent (expected " + knownKind + ", got " + actualKind + ").");
            }

            if (!kinds.ContainsKey(column.Name))
            {
                kinds.Add(column.Name, actualKind);
            }

            if (column.Type == 7 && value.ValueKind != JsonValueKind.String)
            {
                throw Error(sourceFile, tableName, column.Name, "Schema type 7 requires string values.");
            }
        }

        private static JsonValueKind NormalizeValueKind(JsonValueKind kind)
        {
            return kind == JsonValueKind.False ? JsonValueKind.True : kind;
        }

        private static void ValidateRequiredColumns(IReadOnlyList<BdatColumn> schema, IEnumerable<string> requiredColumns, string sourceFile, string tableName)
        {
            if (requiredColumns == null)
            {
                return;
            }

            foreach (string requiredColumn in requiredColumns)
            {
                if (string.IsNullOrWhiteSpace(requiredColumn) || !ContainsColumn(schema, requiredColumn))
                {
                    throw Error(sourceFile, tableName, requiredColumn ?? string.Empty, "Required column is missing.");
                }
            }
        }

        private static bool ContainsColumn(IReadOnlyList<BdatColumn> schema, string name)
        {
            for (int index = 0; index < schema.Count; index++)
            {
                if (string.Equals(schema[index].Name, name, StringComparison.Ordinal))
                {
                    return true;
                }
            }

            return false;
        }

        private static JsonElement GetRequiredProperty(JsonElement element, string propertyName, string sourceFile, string tableName)
        {
            JsonElement value;
            if (!element.TryGetProperty(propertyName, out value))
            {
                throw Error(sourceFile, tableName, propertyName, "Required property is missing.");
            }

            return value;
        }

        private static BdatSchemaException Error(string sourceFile, string tableName, string fieldName, string message)
        {
            return new BdatSchemaException(sourceFile, tableName, fieldName, message);
        }
    }
}
