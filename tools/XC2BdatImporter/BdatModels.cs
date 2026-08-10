using System;
using System.Collections.Generic;
using System.Text.Json;

namespace XC2BdatImporter
{
    internal sealed class BdatTable
    {
        internal BdatTable(string sourceFile, string tableName, IReadOnlyList<BdatColumn> schema, IReadOnlyList<BdatRow> rows)
        {
            SourceFile = sourceFile;
            TableName = tableName;
            Schema = schema;
            Rows = rows;
        }

        internal string SourceFile { get; }
        internal string TableName { get; }
        internal IReadOnlyList<BdatColumn> Schema { get; }
        internal IReadOnlyList<BdatRow> Rows { get; }

        internal bool HasColumn(string name)
        {
            for (int index = 0; index < Schema.Count; index++)
            {
                if (string.Equals(Schema[index].Name, name, StringComparison.Ordinal))
                {
                    return true;
                }
            }

            return false;
        }
    }

    internal sealed class BdatColumn
    {
        internal BdatColumn(string name, int type)
        {
            Name = name;
            Type = type;
        }

        internal string Name { get; }
        internal int Type { get; }
    }

    internal sealed class BdatRow
    {
        internal BdatRow(long id, IReadOnlyDictionary<string, JsonElement> fields)
        {
            Id = id;
            Fields = fields;
        }

        internal long Id { get; }
        internal IReadOnlyDictionary<string, JsonElement> Fields { get; }
    }

    internal sealed class BdatSchemaException : Exception
    {
        internal BdatSchemaException(string sourceFile, string tableName, string fieldName, string message)
            : base(BuildMessage(sourceFile, tableName, fieldName, message))
        {
            SourceFile = sourceFile;
            TableName = tableName;
            FieldName = fieldName;
        }

        internal BdatSchemaException(string sourceFile, string tableName, string fieldName, string message, Exception innerException)
            : base(BuildMessage(sourceFile, tableName, fieldName, message), innerException)
        {
            SourceFile = sourceFile;
            TableName = tableName;
            FieldName = fieldName;
        }

        internal string SourceFile { get; }
        internal string TableName { get; }
        internal string FieldName { get; }

        private static string BuildMessage(string sourceFile, string tableName, string fieldName, string message)
        {
            return "BDAT schema error in file '" + sourceFile + "', table '" + tableName +
                "', field '" + (fieldName ?? string.Empty) + "': " + message;
        }
    }
}
