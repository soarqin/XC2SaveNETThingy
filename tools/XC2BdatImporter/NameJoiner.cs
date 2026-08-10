using System;
using System.Collections.Generic;
using System.Text.Json;

namespace XC2BdatImporter
{
    internal static class NameJoiner
    {
        internal static IReadOnlyDictionary<ulong, string> BuildNameIndex(
            BdatTable localizedTable,
            string localizedKeyColumn,
            string localizedValueColumn)
        {
            if (localizedTable == null)
            {
                throw new ArgumentNullException(nameof(localizedTable));
            }

            if (string.IsNullOrWhiteSpace(localizedKeyColumn))
            {
                throw new NameJoinException("Localized key column is required.");
            }

            Dictionary<ulong, string> result = new Dictionary<ulong, string>();
            foreach (BdatRow row in localizedTable.Rows)
            {
                if (!row.Fields.TryGetValue(localizedValueColumn, out JsonElement value) || value.ValueKind != JsonValueKind.String)
                {
                    throw new NameJoinException("Localized table '" + localizedTable.TableName + "' row " + row.Id + " has no string '" + localizedValueColumn + "'.");
                }

                ulong key;
                if (string.Equals(localizedKeyColumn, "$id", StringComparison.Ordinal))
                {
                    key = checked((ulong)row.Id);
                }
                else if (!row.Fields.TryGetValue(localizedKeyColumn, out JsonElement keyValue) ||
                    keyValue.ValueKind != JsonValueKind.Number || !keyValue.TryGetUInt64(out key))
                {
                    throw new NameJoinException("Localized table '" + localizedTable.TableName + "' row " + row.Id + " has no unsigned-short index in '" + localizedKeyColumn + "'.");
                }

                if (key > ushort.MaxValue)
                {
                    throw new NameJoinException("Localized table '" + localizedTable.TableName + "' row " + row.Id + " has an out-of-range unsigned-short index " + key + ".");
                }

                if (!result.TryAdd(key, Normalize(value.GetString(), localizedTable, row.Id)))
                {
                    throw new NameJoinException("Localized table '" + localizedTable.TableName + "' contains duplicate index " + key + ".");
                }
            }

            return result;
        }

        internal static string ResolveName(
            BdatRow sourceRow,
            string sourceKeyColumn,
            IReadOnlyDictionary<ulong, string> localizedIndex,
            int sourceKeyOffset = 0)
        {
            if (sourceRow == null || localizedIndex == null)
            {
                throw new ArgumentNullException(sourceRow == null ? nameof(sourceRow) : nameof(localizedIndex));
            }

            ulong key;
            if (string.Equals(sourceKeyColumn, "$id", StringComparison.Ordinal))
            {
                long adjusted = checked(sourceRow.Id + sourceKeyOffset);
                if (adjusted < 0)
                {
                    throw new NameJoinException("Source row " + sourceRow.Id + " produced a negative adjusted name index.");
                }

                key = checked((ulong)adjusted);
            }
            else if (!sourceRow.Fields.TryGetValue(sourceKeyColumn, out JsonElement value) ||
                value.ValueKind != JsonValueKind.Number || !value.TryGetUInt64(out key))
            {
                throw new NameJoinException("Source row " + sourceRow.Id + " has no unsigned-short name index in '" + sourceKeyColumn + "'.");
            }

            if (!string.Equals(sourceKeyColumn, "$id", StringComparison.Ordinal) && sourceKeyOffset != 0)
            {
                long adjusted = checked((long)key + sourceKeyOffset);
                if (adjusted < 0)
                {
                    throw new NameJoinException("Source row " + sourceRow.Id + " produced a negative adjusted name index.");
                }

                key = checked((ulong)adjusted);
            }

            if (key > ushort.MaxValue)
            {
                throw new NameJoinException("Source row " + sourceRow.Id + " has an out-of-range unsigned-short name index " + key + ".");
            }

            string name;
            if (!localizedIndex.TryGetValue(key, out name))
            {
                throw new NameJoinException("Missing localized name index " + key + " for source row " + sourceRow.Id + ".");
            }

            return name;
        }

        private static string Normalize(string value, BdatTable table, long rowId)
        {
            if (value == null)
            {
                throw new NameJoinException("Localized table '" + table.TableName + "' row " + rowId + " has a null name.");
            }

            int markerStart = value.IndexOf("[ML:", StringComparison.Ordinal);
            while (markerStart >= 0)
            {
                int markerEnd = value.IndexOf(']', markerStart);
                if (markerEnd < 0)
                {
                    throw new NameJoinException("Unterminated localization marker in '" + value + "'.");
                }

                string marker = value.Substring(markerStart, markerEnd - markerStart + 1);
                if (!string.Equals(marker, "[ML:Space]", StringComparison.Ordinal))
                {
                    throw new NameJoinException("Unknown localization marker '" + marker + "' in '" + value + "'.");
                }

                value = value.Remove(markerStart, marker.Length).Insert(markerStart, " ");
                markerStart = value.IndexOf("[ML:", StringComparison.Ordinal);
            }

            string normalized = value.Trim();
            if (normalized.Length == 0)
            {
                throw new NameJoinException("Localized table '" + table.TableName + "' row " + rowId + " has an empty name.");
            }

            return normalized;
        }
    }

    internal sealed class NameJoinException : Exception
    {
        internal NameJoinException(string message) : base(message) { }
    }
}
