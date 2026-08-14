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

        internal static GeneratedCsvSet GenerateAllLanguages(
            MappingConfig config,
            string commonExtractDirectory,
            IReadOnlyDictionary<string, string> localizedExtractDirectories,
            string existingCsvDirectory)
        {
            string[] codes = { "en", "ja", "ko", "zh-Hans", "zh-Hant", "fr", "de", "it", "es" };
            string[] suffixes = { "EN", "JA", "KO", "ZH_HANS", "ZH_HANT", "FR", "DE", "IT", "ES" };
            Dictionary<string, GeneratedCsvSet> generated = new Dictionary<string, GeneratedCsvSet>(StringComparer.OrdinalIgnoreCase);
            foreach (string code in codes)
            {
                string directory;
                if (!localizedExtractDirectories.TryGetValue(code, out directory))
                    throw new DirectoryNotFoundException("Missing localized extraction for " + code + ".");
                generated[code] = Generate(config, commonExtractDirectory, directory, existingCsvDirectory);
            }

            Dictionary<string, ProjectCsvDocument> result = new Dictionary<string, ProjectCsvDocument>(StringComparer.OrdinalIgnoreCase);
            foreach (string fileName in generated["en"].Documents.Keys)
            {
                ProjectCsvDocument english = generated["en"].Documents[fileName];
                List<string> columns = new List<string>(english.Columns);
                foreach (string suffix in suffixes.Skip(1))
                    foreach (string column in english.Columns.Where(c => c.EndsWith("_EN", StringComparison.Ordinal)))
                        columns.Add(column.Substring(0, column.Length - 2) + suffix);
                columns = columns.Distinct(StringComparer.Ordinal).ToList();
                Dictionary<string, List<string>> rows = new Dictionary<string, List<string>>(StringComparer.Ordinal);
                foreach (IReadOnlyList<string> row in english.Rows)
                    rows[row[0]] = NormalizeRow(english.Columns, row, columns).ToList();

                for (int languageIndex = 1; languageIndex < codes.Length; languageIndex++)
                {
                    ProjectCsvDocument localized = generated[codes[languageIndex]].Documents[fileName];
                string suffix = suffixes[languageIndex];
                    foreach (IReadOnlyList<string> row in localized.Rows)
                    {
                        List<string> target = rows.ContainsKey(row[0]) ? rows[row[0]] : NormalizeRow(localized.Columns, row, columns).ToList();
                        for (int columnIndex = 0; columnIndex < localized.Columns.Count; columnIndex++)
                        {
                            string column = localized.Columns[columnIndex];
                            if (!column.EndsWith("_EN", StringComparison.Ordinal)) continue;
                            int targetIndex = columns.IndexOf(column.Substring(0, column.Length - 2) + suffix);
                            if (targetIndex >= 0 && columnIndex < row.Count) target[targetIndex] = row[columnIndex];
                        }
                        rows[row[0]] = target;
                    }
                }
                int legacyCnIndex = columns.IndexOf("Name_CN");
                int simplifiedIndex = columns.IndexOf("Name_ZH_HANS");
                if (legacyCnIndex >= 0 && simplifiedIndex >= 0)
                    foreach (List<string> row in rows.Values)
                        row[legacyCnIndex] = row[simplifiedIndex];
                result[fileName] = new ProjectCsvDocument(english.Preamble, columns, rows.Values.OrderBy(r => r[0], StringComparer.Ordinal).ToList(), english.Postamble);
            }

            MergeLocalizedMessageTable(result, localizedExtractDirectories, existingCsvDirectory, "bld_bladename", "BladeCommonNames.csv");
            MergeLocalizedMessageTable(result, localizedExtractDirectories, existingCsvDirectory, "chr_bl_ms", "BladeRareNames.csv");
            RestoreExistingLocalizedColumns(result, existingCsvDirectory);
            return new GeneratedCsvSet(result);
        }

        private static void RestoreExistingLocalizedColumns(Dictionary<string, ProjectCsvDocument> result, string existingDirectory)
        {
            foreach (string fileName in result.Keys.ToList())
            {
                if (string.Equals(fileName, "BladeCommonNames.csv", StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(fileName, "BladeRareNames.csv", StringComparison.OrdinalIgnoreCase))
                    continue;

                string path = Path.Combine(existingDirectory, fileName);
                if (!File.Exists(path)) continue;
                ProjectCsvDocument existing = ProjectDataWriter.ReadCsv(path);
                ProjectCsvDocument generated = result[fileName];
                List<string> columns = new List<string>(generated.Columns);
                Dictionary<string, IReadOnlyList<string>> existingRows = new Dictionary<string, IReadOnlyList<string>>(StringComparer.Ordinal);
                foreach (IReadOnlyList<string> row in existing.Rows)
                    if (row.Count > 0) existingRows[row[0]] = row;

                List<IReadOnlyList<string>> rows = new List<IReadOnlyList<string>>();
                foreach (IReadOnlyList<string> generatedRow in generated.Rows)
                {
                    List<string> row = generatedRow.ToList();
                    IReadOnlyList<string> oldRow;
                    if (existingRows.TryGetValue(generatedRow[0], out oldRow))
                    {
                        for (int index = 0; index < columns.Count; index++)
                        {
                            string column = columns[index];
                            if (column.EndsWith("_EN", StringComparison.Ordinal) ||
                                column.EndsWith("_CN", StringComparison.Ordinal) ||
                                column.EndsWith("_JA", StringComparison.Ordinal) ||
                                column.EndsWith("_KO", StringComparison.Ordinal) ||
                                column.EndsWith("_ZH_HANS", StringComparison.Ordinal) ||
                                column.EndsWith("_ZH_HANT", StringComparison.Ordinal) ||
                                column.EndsWith("_FR", StringComparison.Ordinal) ||
                                column.EndsWith("_DE", StringComparison.Ordinal) ||
                                column.EndsWith("_IT", StringComparison.Ordinal) ||
                                column.EndsWith("_ES", StringComparison.Ordinal)) continue;
                            int oldIndex = existing.Columns.ToList().IndexOf(column);
                            if (oldIndex >= 0 && oldIndex < oldRow.Count) row[index] = oldRow[oldIndex];
                        }
                    }
                    int cn = columns.IndexOf("Name_CN");
                    int hans = columns.IndexOf("Name_ZH_HANS");
                    if (cn >= 0 && hans >= 0) row[cn] = row[hans];
                    rows.Add(row);
                }
                result[fileName] = new ProjectCsvDocument(generated.Preamble, columns, rows, generated.Postamble);
            }
        }

        private static void MergeLocalizedMessageTable(Dictionary<string, ProjectCsvDocument> result, IReadOnlyDictionary<string, string> directories, string existingDirectory, string sourceTable, string targetCsv)
        {
            string existingPath = Path.Combine(existingDirectory, targetCsv);
            ProjectCsvDocument document = File.Exists(existingPath) ? ProjectDataWriter.ReadCsv(existingPath) : result[targetCsv];
            string[] codes = { "en", "ja", "ko", "zh-Hans", "zh-Hant", "fr", "de", "it", "es" };
            string[] suffixes = { "EN", "JA", "KO", "ZH_HANS", "ZH_HANT", "FR", "DE", "IT", "ES" };
            List<string> columns = new List<string> { "ID", "Name_EN" };
            foreach (string suffix in suffixes.Skip(1)) columns.Add("Name_" + suffix);
            Dictionary<string, List<string>> rows = new Dictionary<string, List<string>>(StringComparer.Ordinal);
            foreach (IReadOnlyList<string> sourceRow in document.Rows)
                rows[sourceRow[0]] = NormalizeRow(document.Columns, sourceRow, columns).ToList();
            for (int languageIndex = 0; languageIndex < codes.Length; languageIndex++)
            {
                string directory;
                if (!directories.TryGetValue(codes[languageIndex], out directory)) continue;
                string path = Directory.GetFiles(directory, sourceTable + ".json", SearchOption.AllDirectories).FirstOrDefault();
                if (path == null) continue;
                BdatTable table = BdatJsonReader.ReadTable(path);
                int targetIndex = columns.IndexOf("Name_" + suffixes[languageIndex]);
                foreach (BdatRow sourceRow in table.Rows)
                {
                    string id = sourceRow.Id.ToString(CultureInfo.InvariantCulture);
                    List<string> target;
                    if (!rows.TryGetValue(id, out target)) continue;
                    JsonElement value;
                    if (sourceRow.Fields.TryGetValue("name", out value) && value.ValueKind == JsonValueKind.String)
                        target[targetIndex] = value.GetString();
                }
            }
            int cn = columns.IndexOf("Name_CN");
            int hans = columns.IndexOf("Name_ZH_HANS");
            if (cn >= 0 && hans >= 0) foreach (List<string> row in rows.Values) row[cn] = row[hans];
            result[targetCsv] = new ProjectCsvDocument(document.Preamble, columns, rows.Values.OrderBy(row => row[0], StringComparer.Ordinal).ToList(), document.Postamble);
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

        internal static BdatTable ReadExtractedTableForAnalysis(string root, string tableName)
        {
            return ReadExtractedTable(root, tableName);
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

        private static IReadOnlyList<string> NormalizeRow(IReadOnlyList<string> sourceColumns, IReadOnlyList<string> sourceRow, IReadOnlyList<string> targetColumns)
        {
            List<string> row = new List<string>();
            foreach (string column in targetColumns)
            {
                int index = -1;
                for (int i = 0; i < sourceColumns.Count; i++)
                    if (sourceColumns[i] == column) { index = i; break; }
                row.Add(index >= 0 && index < sourceRow.Count ? sourceRow[index] : string.Empty);
            }
            return row;
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
