using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace XC2BdatImporter
{
    internal sealed class MappingConfig
    {
        private readonly Dictionary<string, List<MappingDefinition>> byTarget;
        private readonly Dictionary<string, List<MappingDefinition>> byCsv;

        private MappingConfig(IReadOnlyList<MappingDefinition> mappings)
        {
            Mappings = mappings;
            byTarget = new Dictionary<string, List<MappingDefinition>>(StringComparer.Ordinal);
            byCsv = new Dictionary<string, List<MappingDefinition>>(StringComparer.Ordinal);
            foreach (MappingDefinition mapping in mappings)
            {
                if (mapping == null || string.IsNullOrWhiteSpace(mapping.TargetTable))
                {
                    throw new InvalidDataException("Each mapping must define targetTable.");
                }

                Add(byTarget, mapping.TargetTable, mapping);
                Add(byCsv, mapping.TargetCsv, mapping);
            }

            foreach (KeyValuePair<string, List<MappingDefinition>> pair in byCsv)
            {
                if (pair.Value.Count <= 1)
                {
                    continue;
                }

                string targetTable = pair.Value[0].TargetTable;
                for (int index = 0; index < pair.Value.Count; index++)
                {
                    MappingDefinition mapping = pair.Value[index];
                    if (!string.Equals(mapping.TargetTable, targetTable, StringComparison.Ordinal) ||
                        !mapping.PreserveExistingRows)
                    {
                        throw new InvalidDataException(
                            "Mappings sharing targetCsv '" + pair.Key +
                            "' must target the same table and preserve existing rows.");
                    }
                }
            }
        }

        internal IReadOnlyList<MappingDefinition> Mappings { get; }

        internal MappingDefinition GetMapping(string targetTableOrCsv)
        {
            IReadOnlyList<MappingDefinition> mappings = GetMappings(targetTableOrCsv);
            return mappings.Count == 0 ? null : mappings[0];
        }

        internal MappingDefinition GetMapping(string targetTable, string targetCsv)
        {
            foreach (MappingDefinition mapping in GetMappings(targetTable))
            {
                if (string.Equals(mapping.TargetCsv, targetCsv, StringComparison.Ordinal))
                {
                    return mapping;
                }
            }

            return null;
        }

        internal IReadOnlyList<MappingDefinition> GetMappings(string targetTableOrCsv)
        {
            if (string.IsNullOrWhiteSpace(targetTableOrCsv))
            {
                return Array.Empty<MappingDefinition>();
            }

            List<MappingDefinition> mappings;
            if (byTarget.TryGetValue(targetTableOrCsv, out mappings))
            {
                return mappings;
            }

            return byCsv.TryGetValue(targetTableOrCsv, out mappings)
                ? mappings
                : Array.Empty<MappingDefinition>();
        }

        private static void Add(
            IDictionary<string, List<MappingDefinition>> index,
            string key,
            MappingDefinition mapping)
        {
            List<MappingDefinition> mappings;
            if (!index.TryGetValue(key, out mappings))
            {
                mappings = new List<MappingDefinition>();
                index.Add(key, mappings);
            }

            mappings.Add(mapping);
        }

        internal static MappingConfig Load(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentException("A mapping config path is required.", nameof(path));
            }

            string fullPath = Path.GetFullPath(path);
            if (!File.Exists(fullPath))
            {
                throw new FileNotFoundException("Mapping config was not found.", fullPath);
            }

            try
            {
                JsonSerializerOptions options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                List<MappingDefinition> mappings = JsonSerializer.Deserialize<List<MappingDefinition>>(
                    File.ReadAllText(fullPath), options);
                if (mappings == null || mappings.Count == 0)
                {
                    throw new InvalidDataException("Mapping config must contain at least one mapping.");
                }

                foreach (MappingDefinition mapping in mappings)
                {
                    if (mapping == null)
                    {
                        throw new InvalidDataException("Mapping config cannot contain null entries.");
                    }

                    mapping.Validate();
                }

                return new MappingConfig(mappings);
            }
            catch (JsonException ex)
            {
                throw new InvalidDataException("Mapping config is not valid JSON.", ex);
            }
        }
    }

    internal sealed class MappingDefinition
    {
        public string SourceFile { get; set; }
        public string SourceTable { get; set; }
        public string LocalizedFile { get; set; }
        public string LocalizedTable { get; set; }
        public string TargetCsv { get; set; }
        public string TargetTable { get; set; }
        public string KeyColumn { get; set; }
        public string LocalizedKeyColumn { get; set; }
        public int SourceKeyOffset { get; set; }
        public string IdExpression { get; set; }
        public int? ItemTypeId { get; set; }
        public bool PreserveExistingRows { get; set; }
        public List<long> PreserveExistingIds { get; set; }
        public string Filter { get; set; }
        public int IdOffset { get; set; }

        internal void Validate()
        {
            if (string.IsNullOrWhiteSpace(SourceFile) || string.IsNullOrWhiteSpace(SourceTable) ||
                string.IsNullOrWhiteSpace(LocalizedFile) || string.IsNullOrWhiteSpace(LocalizedTable) ||
                string.IsNullOrWhiteSpace(TargetCsv) || string.IsNullOrWhiteSpace(TargetTable) ||
                string.IsNullOrWhiteSpace(KeyColumn) || string.IsNullOrWhiteSpace(LocalizedKeyColumn) ||
                string.IsNullOrWhiteSpace(IdExpression))
            {
                throw new InvalidDataException("Mapping '" + (TargetTable ?? "") + "' is missing a required field.");
            }

            if (IdOffset < 0)
            {
                throw new InvalidDataException("Mapping '" + TargetTable + "' has a negative idOffset.");
            }

            if (SourceKeyOffset < -ushort.MaxValue || SourceKeyOffset > ushort.MaxValue)
            {
                throw new InvalidDataException("Mapping '" + TargetTable + "' has an out-of-range sourceKeyOffset.");
            }

            ParseIdExpression(IdExpression, TargetTable);

            if (ItemTypeId.HasValue && (ItemTypeId.Value < 0 || ItemTypeId.Value > ushort.MaxValue))
            {
                throw new InvalidDataException("Mapping '" + TargetTable + "' has an out-of-range itemTypeId.");
            }

            if (PreserveExistingIds != null)
            {
                foreach (long id in PreserveExistingIds)
                {
                    if (id < 0 || id > uint.MaxValue)
                        throw new InvalidDataException("Mapping '" + TargetTable + "' has an out-of-range preserveExistingIds value.");
                }
            }
        }

        internal long ComputeTargetId(long sourceId)
        {
            if (sourceId < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(sourceId));
            }

            IdExpressionParts parts = ParseIdExpression(IdExpression, TargetTable);
            long result;
            try
            {
                result = checked(sourceId + parts.Offset);
            }
            catch (OverflowException ex)
            {
                throw new InvalidDataException(
                    "Mapping '" + TargetTable + "' produced an overflowing target ID.", ex);
            }

            if (result < 0 || result > uint.MaxValue)
            {
                throw new InvalidDataException(
                    "Mapping '" + TargetTable + "' produced an out-of-range target ID " + result + ".");
            }

            return result;
        }

        internal long ComputeSourceId(long targetId)
        {
            if (targetId < 0) throw new ArgumentOutOfRangeException(nameof(targetId));
            IdExpressionParts parts = ParseIdExpression(IdExpression, TargetTable);
            long result;
            try
            {
                result = checked(targetId - parts.Offset);
            }
            catch (OverflowException ex)
            {
                throw new InvalidDataException("Mapping '" + TargetTable + "' produced an overflowing source ID.", ex);
            }

            if (result < 0 || result > uint.MaxValue)
            {
                throw new InvalidDataException("Mapping '" + TargetTable + "' produced an out-of-range source ID " + result + ".");
            }

            return result;
        }

        private static IdExpressionParts ParseIdExpression(string expression, string targetTable)
        {
            string value = (expression ?? string.Empty).Trim();
            const string prefix = "source.id";
            if (!value.StartsWith(prefix, StringComparison.Ordinal))
            {
                throw new InvalidDataException(
                    "Mapping '" + (targetTable ?? string.Empty) +
                    "' has unsupported idExpression '" + value + "'.");
            }

            string suffix = value.Substring(prefix.Length).Trim();
            if (suffix.Length == 0)
            {
                return new IdExpressionParts(0);
            }

            if (suffix.Length < 3 || (suffix[0] != '+' && suffix[0] != '-'))
            {
                throw new InvalidDataException(
                    "Mapping '" + (targetTable ?? string.Empty) +
                    "' has unsupported idExpression '" + value + "'.");
            }

            long offsetMagnitude;
            if (!long.TryParse(suffix.Substring(1).Trim(), out offsetMagnitude) || offsetMagnitude < 0)
            {
                throw new InvalidDataException(
                    "Mapping '" + (targetTable ?? string.Empty) +
                    "' has unsupported idExpression '" + value + "'.");
            }

            return new IdExpressionParts(suffix[0] == '+' ? offsetMagnitude : -offsetMagnitude);
        }

        private readonly struct IdExpressionParts
        {
            internal IdExpressionParts(long offset)
            {
                Offset = offset;
            }

            internal long Offset { get; }
        }
    }
}
