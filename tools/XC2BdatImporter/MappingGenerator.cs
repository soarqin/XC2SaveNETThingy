using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace XC2BdatImporter
{
    internal sealed class GeneratedCsvSet
    {
        internal GeneratedCsvSet(IReadOnlyDictionary<string, ProjectCsvDocument> documents)
        {
            Documents = documents;
        }

        internal IReadOnlyDictionary<string, ProjectCsvDocument> Documents { get; }
    }

    internal static class MappingGenerator
    {
        internal static long ResolveWeaponCoreChipId(long weaponId, IReadOnlyDictionary<long, long> index)
        {
            if (weaponId == 0) return 0;
            long chipId;
            return index != null && index.TryGetValue(weaponId, out chipId) ? chipId : 0;
        }

        internal static IReadOnlyDictionary<long, long> BuildWeaponCoreChipIndex(BdatTable chipTable)
        {
            if (chipTable == null) throw new ArgumentNullException(nameof(chipTable));
            Dictionary<long, long> result = new Dictionary<long, long>();
            foreach (BdatRow chip in chipTable.Rows)
            {
                foreach (KeyValuePair<string, JsonElement> field in chip.Fields)
                {
                    if (!field.Key.StartsWith("CreateWpn", StringComparison.Ordinal)) continue;
                    if (field.Value.ValueKind != JsonValueKind.Number) continue;
                    long weaponId = field.Value.GetInt64();
                    if (weaponId == 0) continue;
                    long previousChip;
                    if (result.TryGetValue(weaponId, out previousChip) && previousChip != chip.Id)
                    {
                        throw new InvalidDataException(
                            "Weapon " + weaponId + " is assigned to multiple core chips " + previousChip + " and " + chip.Id + ".");
                    }

                    result[weaponId] = chip.Id;
                }
            }

            return result;
        }

        internal static string ComputeWeaponTypeBaseId(BdatRow row, long fallback)
        {
            if (row == null) throw new ArgumentNullException(nameof(row));
            JsonElement motion;
            if (row.Fields.TryGetValue("Motion", out motion) && motion.ValueKind == JsonValueKind.Number)
            {
                return motion.GetInt64().ToString(CultureInfo.InvariantCulture);
            }

            return GetIntField(row, "WpnBaseID", fallback);
        }

        internal static GeneratedCsvSet Generate(
            MappingConfig config,
            string commonExtractDirectory,
            string localizedExtractDirectory,
            string existingCsvDirectory)
        {
            if (config == null) throw new ArgumentNullException(nameof(config));
            Dictionary<string, ProjectCsvDocument> output = new Dictionary<string, ProjectCsvDocument>(StringComparer.OrdinalIgnoreCase);
            Dictionary<string, ProjectCsvDocument> existing = ReadExisting(existingCsvDirectory);
            BdatTable coreChipTable = ReadExtractedTable(commonExtractDirectory, "ITM_PcWpnChip");
            IReadOnlyDictionary<long, long> weaponCoreChipIndex = BuildWeaponCoreChipIndex(coreChipTable);

            foreach (MappingDefinition mapping in config.Mappings)
            {
                BdatTable source = ReadExtractedTable(commonExtractDirectory, mapping.SourceTable);
                BdatTable localized = ReadExtractedTable(localizedExtractDirectory, mapping.LocalizedTable);
                IReadOnlyDictionary<ulong, string> names = NameJoiner.BuildNameIndex(localized, mapping.LocalizedKeyColumn, "name");
                List<IReadOnlyList<string>> generatedRows = new List<IReadOnlyList<string>>();

                foreach (BdatRow row in source.Rows)
                {
                    if (!ShouldInclude(row, mapping.Filter)) continue;
                    string name;
                    try
                    {
                        name = NameJoiner.ResolveName(row, mapping.KeyColumn, names, mapping.SourceKeyOffset);
                    }
                    catch (NameJoinException ex)
                    {
                        throw new NameJoinException(
                            "Mapping '" + mapping.TargetTable + "' source table '" + mapping.SourceTable +
                            "' row " + row.Id + ": " + ex.Message);
                    }
                    long targetId = mapping.ComputeTargetId(row.Id);
                    generatedRows.Add(CreateTargetRow(mapping, row, targetId, name, weaponCoreChipIndex));
                }

                ProjectCsvDocument generated = MergeRows(mapping, generatedRows, existing, output);
                output[mapping.TargetCsv] = generated;
            }

            output["Items.csv"] = AggregateItems(existing, output);

            return new GeneratedCsvSet(output);
        }

        internal static void WriteAll(GeneratedCsvSet set, string outputDirectory)
        {
            Directory.CreateDirectory(outputDirectory);
            foreach (KeyValuePair<string, ProjectCsvDocument> pair in set.Documents)
            {
                ProjectDataWriter.WriteCsv(Path.Combine(outputDirectory, pair.Key), pair.Value);
            }
        }

        private static Dictionary<string, ProjectCsvDocument> ReadExisting(string directory)
        {
            Dictionary<string, ProjectCsvDocument> result = new Dictionary<string, ProjectCsvDocument>(StringComparer.OrdinalIgnoreCase);
            if (!Directory.Exists(directory)) return result;
            foreach (string path in Directory.GetFiles(directory, "*.csv"))
            {
                result[Path.GetFileName(path)] = ProjectDataWriter.ReadCsv(path);
            }
            return result;
        }

        private static BdatTable ReadExtractedTable(string root, string tableName)
        {
            string fileName = tableName + ".json";
            string path = Directory.GetFiles(root, fileName, SearchOption.AllDirectories)
                .FirstOrDefault(candidate => string.Equals(Path.GetFileName(candidate), fileName, StringComparison.Ordinal));
            if (path == null)
            {
                path = Directory.GetFiles(root, fileName, SearchOption.AllDirectories).FirstOrDefault();
            }
            if (path == null)
            {
                throw new FileNotFoundException("Extracted BDAT table was not found.", Path.Combine(root, fileName));
            }
            return BdatJsonReader.ReadTable(path);
        }

        private static bool ShouldInclude(BdatRow row, string filter)
        {
            if (string.IsNullOrWhiteSpace(filter)) return true;
            string expression = filter.Trim();
            if (expression.EndsWith("!= 0", StringComparison.Ordinal))
            {
                string column = expression.Substring(0, expression.Length - 4).Trim();
                JsonElement value;
                return row.Fields.TryGetValue(column, out value) && value.ValueKind == JsonValueKind.Number && value.GetInt64() != 0;
            }
            throw new InvalidDataException("Unsupported mapping filter: " + filter);
        }

        private static IReadOnlyList<string> CreateTargetRow(MappingDefinition mapping, BdatRow row, long targetId, string name, IReadOnlyDictionary<long, long> weaponCoreChipIndex)
        {
            if (string.Equals(mapping.TargetTable, "Drivers", StringComparison.Ordinal)) return new[] { targetId.ToString(CultureInfo.InvariantCulture), name, string.Empty };
            if (string.Equals(mapping.TargetTable, "DriverSkills", StringComparison.Ordinal) || string.Equals(mapping.TargetTable, "BladeArts", StringComparison.Ordinal) || string.Equals(mapping.TargetTable, "BladeSkills", StringComparison.Ordinal) || string.Equals(mapping.TargetTable, "BladeFieldSkills", StringComparison.Ordinal)) return new[] { targetId.ToString(CultureInfo.InvariantCulture), name, string.Empty };
            if (string.Equals(mapping.TargetTable, "BladeSpecials", StringComparison.Ordinal) || string.Equals(mapping.TargetTable, "BladeSpecialsLv4", StringComparison.Ordinal)) return new[] { targetId.ToString(CultureInfo.InvariantCulture), GetIntField(row, "WpnType", 0), name, string.Empty };
            if (string.Equals(mapping.TargetTable, "WeaponTypes", StringComparison.Ordinal)) return new[] { targetId.ToString(CultureInfo.InvariantCulture), ComputeWeaponTypeBaseId(row, targetId), name, string.Empty };
            if (string.Equals(mapping.TargetTable, "Weapons", StringComparison.Ordinal)) return new[] { targetId.ToString(CultureInfo.InvariantCulture), GetIntField(row, "WpnType", 0), ResolveWeaponCoreChipId(row.Id, weaponCoreChipIndex).ToString(CultureInfo.InvariantCulture), name, string.Empty };
            if (string.Equals(mapping.TargetTable, "DriverArts", StringComparison.Ordinal)) return new[] { targetId.ToString(CultureInfo.InvariantCulture), GetIntField(row, "WpnType", 0), GetIntField(row, "Driver", 0), name, string.Empty };
            if (string.Equals(mapping.TargetTable, "Elements", StringComparison.Ordinal) || string.Equals(mapping.TargetTable, "FavCategories", StringComparison.Ordinal) || string.Equals(mapping.TargetTable, "FavCategoriesIra", StringComparison.Ordinal)) return new[] { targetId.ToString(CultureInfo.InvariantCulture), name, string.Empty };
            if (string.Equals(mapping.TargetTable, "BladeTrustRanks", StringComparison.Ordinal)) return new[] { targetId.ToString(CultureInfo.InvariantCulture), name };
            if (string.Equals(mapping.TargetTable, "Items", StringComparison.Ordinal)) return new[] { targetId.ToString(CultureInfo.InvariantCulture), (mapping.ItemTypeId ?? 0).ToString(CultureInfo.InvariantCulture), name, string.Empty };
            throw new InvalidDataException("No target row shape configured for table '" + mapping.TargetTable + "'.");
        }

        private static string GetIntField(BdatRow row, string field, long fallback)
        {
            JsonElement value;
            if (!row.Fields.TryGetValue(field, out value) || value.ValueKind != JsonValueKind.Number) return fallback.ToString(CultureInfo.InvariantCulture);
            return value.GetInt64().ToString(CultureInfo.InvariantCulture);
        }

        private static ProjectCsvDocument MergeRows(MappingDefinition mapping, List<IReadOnlyList<string>> rows, Dictionary<string, ProjectCsvDocument> existing, Dictionary<string, ProjectCsvDocument> generated)
        {
            ProjectCsvDocument baseDocument;
            if (!existing.TryGetValue(mapping.TargetCsv, out baseDocument))
            {
                baseDocument = new ProjectCsvDocument(Array.Empty<string>(), TargetColumns(mapping), Array.Empty<IReadOnlyList<string>>(), Array.Empty<string>());
            }

            Dictionary<string, IReadOnlyList<string>> byId = new Dictionary<string, IReadOnlyList<string>>(StringComparer.Ordinal);
            foreach (IReadOnlyList<string> row in baseDocument.Rows) byId[row[0]] = row;
            ProjectCsvDocument previous;
            if (generated.TryGetValue(mapping.TargetCsv, out previous))
            {
                foreach (IReadOnlyList<string> row in previous.Rows) byId[row[0]] = row;
            }
            foreach (IReadOnlyList<string> row in rows)
            {
                if (mapping.PreserveExistingIds != null && mapping.PreserveExistingIds.Contains(ParseRowId(row[0])) && byId.ContainsKey(row[0]))
                {
                    continue;
                }

                if (byId.ContainsKey(row[0]) || !mapping.PreserveExistingRows) byId[row[0]] = row;
                else byId.Add(row[0], row);
            }
            List<IReadOnlyList<string>> merged = byId.Values.OrderBy(row => row[0], StringComparer.Ordinal).ToList();
            return new ProjectCsvDocument(baseDocument.Preamble, baseDocument.Columns, merged, baseDocument.Postamble);
        }

        private static long ParseRowId(string value)
        {
            long result;
            return long.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out result) ? result : long.MinValue;
        }

        private static IReadOnlyList<string> TargetColumns(MappingDefinition mapping)
        {
            if (mapping.TargetTable == "DriverArts") return new[] { "ID", "WeaponTypeID", "DriverID", "Name_EN", "Name_CN" };
            if (mapping.TargetTable == "Weapons") return new[] { "ID", "WeaponTypeID", "CoreChipID", "Name_EN", "Name_CN" };
            if (mapping.TargetTable == "BladeSpecials" || mapping.TargetTable == "BladeSpecialsLv4") return new[] { "ID", "WpnBaseID", "Name_EN", "Name_CN" };
            if (mapping.TargetTable == "WeaponTypes") return new[] { "ID", "WpnBaseID", "Name_EN", "Name_CN" };
            if (mapping.TargetTable == "BladeTrustRanks") return new[] { "ID", "Name" };
            if (mapping.TargetTable == "Items") return new[] { "ID", "ItemTypeID", "Name_EN", "Name_CN" };
            return new[] { "ID", "Name_EN", "Name_CN" };
        }

        private static ProjectCsvDocument AggregateItems(
            Dictionary<string, ProjectCsvDocument> existing,
            Dictionary<string, ProjectCsvDocument> generated)
        {
            ProjectCsvDocument baseDocument;
            if (!existing.TryGetValue("Items.csv", out baseDocument))
            {
                baseDocument = new ProjectCsvDocument(
                    Array.Empty<string>(),
                    new[] { "ID", "ItemTypeID", "Name_EN", "Name_CN" },
                    Array.Empty<IReadOnlyList<string>>(),
                    Array.Empty<string>());
            }

            Dictionary<string, IReadOnlyList<string>> byId = new Dictionary<string, IReadOnlyList<string>>(StringComparer.Ordinal);
            foreach (IReadOnlyList<string> row in baseDocument.Rows)
            {
                byId[row[0]] = row;
            }

            foreach (KeyValuePair<string, ProjectCsvDocument> pair in generated)
            {
                if (!pair.Key.StartsWith("Items_", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                foreach (IReadOnlyList<string> row in pair.Value.Rows)
                {
                    byId[row[0]] = row;
                }
            }

            return new ProjectCsvDocument(
                baseDocument.Preamble,
                baseDocument.Columns,
                byId.Values.OrderBy(row => row[0], StringComparer.Ordinal).ToList(),
                baseDocument.Postamble);
        }
    }
}
