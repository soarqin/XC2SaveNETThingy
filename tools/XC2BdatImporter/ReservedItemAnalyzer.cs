using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace XC2BdatImporter
{
    internal sealed class ReservedItemSignals
    {
        internal ReservedItemSignals(IReadOnlyList<string> reasonCodes)
        {
            ReasonCodes = reasonCodes ?? throw new ArgumentNullException(nameof(reasonCodes));
            ReasonCode = string.Join(";", reasonCodes);
        }

        internal IReadOnlyList<string> ReasonCodes { get; }
        internal string ReasonCode { get; }
        internal bool IsReserved { get { return ReasonCodes.Count != 0; } }
    }

    internal sealed class ReservedItemRecord
    {
        internal ReservedItemRecord(long itemId, string status, string reasonEn, string reasonCn, string evidence)
        {
            ItemId = itemId;
            Status = status;
            ReasonEn = reasonEn;
            ReasonCn = reasonCn;
            Evidence = evidence;
        }

        internal long ItemId { get; }
        internal string Status { get; }
        internal string ReasonEn { get; }
        internal string ReasonCn { get; }
        internal string Evidence { get; }
    }

    internal static class ReservedItemAnalyzer
    {
        private static readonly Regex ScriptPattern = new Regex(
            "[\\u3040-\\u30ff\\u3400-\\u4dbf\\u4e00-\\u9fff\\uac00-\\ud7af]",
            RegexOptions.Compiled | RegexOptions.CultureInvariant);
        private static readonly Regex UnknownPattern = new Regex(
            "^\\s*UNKNOWN(?:_|$)", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
        private static readonly Regex InternalEnglishPattern = new Regex(
            "\\b(?:dummy|unused|temporary|temp|delete(?:d)?|test|reserved|provisional|tbd)\\b|削除予定|内容未定|未使用|(?:^|[（(])仮(?:$|[）)])|\\bprov\\.?(?:$|\\s|[)])",
            RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
        private static readonly Regex ProvisionalPattern = new Regex(
            "(?:^|[\\s(（\\[【])仮(?:$|[\\s)）\\]】])|削除予定|内容未定|未使用|\\b(?:dummy|unused|temporary|temp|delete(?:d)?|test|reserved|provisional|tbd)\\b",
            RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
        private static readonly string[] SortColumns =
        {
            "sortJP", "sortGE", "sortFR", "sortSP", "sortIT", "sortGB", "sortCN", "sortTW", "sortKR"
        };

        internal static ReservedItemSignals Analyze(
            string english,
            string chinese,
            string japanese,
            string traditionalChinese,
            BdatRow sourceRow = null)
        {
            List<string> reasons = new List<string>();
            string en = english ?? string.Empty;
            if (string.IsNullOrWhiteSpace(en)) reasons.Add("empty-name");
            if (UnknownPattern.IsMatch(en)) reasons.Add("unknown-name");
            if (ScriptPattern.IsMatch(en)) reasons.Add("untranslated-name");
            if (InternalEnglishPattern.IsMatch(en)) reasons.Add("internal-marker");
            if (ProvisionalPattern.IsMatch(chinese ?? string.Empty) ||
                ProvisionalPattern.IsMatch(japanese ?? string.Empty) ||
                ProvisionalPattern.IsMatch(traditionalChinese ?? string.Empty))
            {
                reasons.Add("provisional-marker");
            }

            AddSourceSignals(sourceRow, reasons);

            return new ReservedItemSignals(reasons);
        }

        internal static IReadOnlyList<ReservedItemRecord> AnalyzeProjectItems(
            ProjectCsvDocument items,
            MappingConfig config,
            IReadOnlyDictionary<string, BdatTable> sourceTables,
            IReadOnlyDictionary<string, IReadOnlyDictionary<string, BdatTable>> localizedTables)
        {
            if (items == null) throw new ArgumentNullException(nameof(items));
            if (config == null) throw new ArgumentNullException(nameof(config));
            if (sourceTables == null) throw new ArgumentNullException(nameof(sourceTables));
            if (localizedTables == null) throw new ArgumentNullException(nameof(localizedTables));

            int idColumn = FindColumn(items.Columns, "ID");
            int typeColumn = FindColumn(items.Columns, "ItemTypeID");
            int nameColumn = FindColumn(items.Columns, "Name_EN");
            List<ReservedItemRecord> result = new List<ReservedItemRecord>();
            foreach (IReadOnlyList<string> row in items.Rows)
            {
                long itemId;
                int itemType;
                if (!long.TryParse(row[idColumn], NumberStyles.Integer, CultureInfo.InvariantCulture, out itemId) ||
                    !int.TryParse(row[typeColumn], NumberStyles.Integer, CultureInfo.InvariantCulture, out itemType))
                {
                    throw new InvalidDataException("Items.csv contains a non-numeric ID or ItemTypeID: " + string.Join(" | ", row));
                }

                string projectName = row[nameColumn] ?? string.Empty;
                if (itemId == 0 && string.Equals(projectName.Trim(), "(none)", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }
                string english = string.Empty;
                string chinese = string.Empty;
                string japanese = string.Empty;
                string traditionalChinese = string.Empty;
                BdatRow sourceRowForAnalysis = null;
                string mappingEvidence = "project Items.csv name=" + Compact(projectName);
                foreach (MappingDefinition mapping in config.GetMappings("Items"))
                {
                    if (!mapping.ItemTypeId.HasValue || mapping.ItemTypeId.Value != itemType)
                    {
                        continue;
                    }

                    long sourceId;
                    try { sourceId = mapping.ComputeSourceId(itemId); }
                    catch (InvalidDataException) { continue; }

                    BdatTable sourceTable;
                    if (!sourceTables.TryGetValue(mapping.SourceTable, out sourceTable)) continue;
                    BdatRow sourceRow = sourceTable.Rows.FirstOrDefault(candidate => candidate.Id == sourceId);
                    if (sourceRow == null) continue;
                    sourceRowForAnalysis = sourceRow;

                    english = ResolveRawName(sourceRow, mapping.KeyColumn, localizedTables, "gb", mapping.LocalizedTable, mapping.SourceKeyOffset);
                    chinese = ResolveRawName(sourceRow, mapping.KeyColumn, localizedTables, "cn", mapping.LocalizedTable, mapping.SourceKeyOffset);
                    japanese = ResolveRawName(sourceRow, mapping.KeyColumn, localizedTables, "jp", mapping.LocalizedTable, mapping.SourceKeyOffset);
                    traditionalChinese = ResolveRawName(sourceRow, mapping.KeyColumn, localizedTables, "tw", mapping.LocalizedTable, mapping.SourceKeyOffset);
                    mappingEvidence = mapping.SourceTable + " row=" + sourceId + "; " + mapping.LocalizedTable +
                        " gb=" + Compact(english) + "; cn=" + Compact(chinese) + "; jp=" + Compact(japanese) + "; tw=" + Compact(traditionalChinese);
                    break;
                }

                ReservedItemSignals signals = Analyze(english, chinese, japanese, traditionalChinese, sourceRowForAnalysis);
                List<string> codes = new List<string>(signals.ReasonCodes);
                if (UnknownPattern.IsMatch(projectName) && !codes.Contains("unknown-name")) codes.Add("unknown-name");
                if (string.IsNullOrWhiteSpace(english) && string.IsNullOrWhiteSpace(projectName) && !codes.Contains("empty-name")) codes.Add("empty-name");
                if (codes.Count == 0) continue;

                result.Add(new ReservedItemRecord(
                    itemId,
                    "Reserved",
                    BuildReason(codes, false),
                    BuildReason(codes, true),
                    mappingEvidence));
            }

            return result.OrderBy(record => record.ItemId).ToList();
        }

        private static void AddSourceSignals(BdatRow sourceRow, ICollection<string> reasons)
        {
            if (sourceRow == null)
            {
                return;
            }

            long namePointer;
            if (TryGetNumericField(sourceRow, "Name", out namePointer) && namePointer == 0)
            {
                AddReason(reasons, "source-name-missing");
            }

            bool allSortsZero;
            if (TryGetAllSortsZero(sourceRow, out allSortsZero))
            {
                if (allSortsZero)
                {
                    AddReason(reasons, "no-locale-sort");
                }
                else
                {
                    long englishSort;
                    if (TryGetNumericField(sourceRow, "sortGB", out englishSort) && englishSort == 0)
                    {
                        AddReason(reasons, "western-sort-missing");
                    }
                }
            }
        }

        private static bool TryGetAllSortsZero(BdatRow sourceRow, out bool allZero)
        {
            allZero = true;
            foreach (string column in SortColumns)
            {
                long value;
                if (!TryGetNumericField(sourceRow, column, out value))
                {
                    return false;
                }

                if (value != 0)
                {
                    allZero = false;
                }
            }

            return true;
        }

        private static bool TryGetNumericField(BdatRow row, string field, out long value)
        {
            value = 0;
            JsonElement element;
            if (!row.Fields.TryGetValue(field, out element) || element.ValueKind != JsonValueKind.Number)
            {
                return false;
            }

            return element.TryGetInt64(out value);
        }

        private static void AddReason(ICollection<string> reasons, string code)
        {
            if (!reasons.Contains(code))
            {
                reasons.Add(code);
            }
        }

        internal static IReadOnlyDictionary<string, BdatTable> BuildSourceTableIndex(IEnumerable<BdatTable> tables)
        {
            Dictionary<string, BdatTable> index = new Dictionary<string, BdatTable>(StringComparer.Ordinal);
            foreach (BdatTable table in tables ?? Array.Empty<BdatTable>()) index[table.TableName] = table;
            return index;
        }

        internal static IReadOnlyDictionary<string, IReadOnlyDictionary<string, BdatTable>> BuildLocalizedTableIndex(
            IEnumerable<KeyValuePair<string, BdatTable>> tables)
        {
            Dictionary<string, IReadOnlyDictionary<string, BdatTable>> result = new Dictionary<string, IReadOnlyDictionary<string, BdatTable>>(StringComparer.OrdinalIgnoreCase);
            foreach (KeyValuePair<string, BdatTable> pair in tables ?? Array.Empty<KeyValuePair<string, BdatTable>>())
            {
                Dictionary<string, BdatTable> languageTables;
                if (!result.TryGetValue(pair.Key, out IReadOnlyDictionary<string, BdatTable> existing))
                {
                    languageTables = new Dictionary<string, BdatTable>(StringComparer.Ordinal);
                    result[pair.Key] = languageTables;
                }
                else
                {
                    languageTables = new Dictionary<string, BdatTable>(existing, StringComparer.Ordinal);
                    result[pair.Key] = languageTables;
                }

                languageTables[pair.Value.TableName] = pair.Value;
            }

            return result;
        }

        internal static IReadOnlyDictionary<string, BdatTable> BuildLocalizedLanguageTables(IEnumerable<BdatTable> tables)
        {
            Dictionary<string, BdatTable> result = new Dictionary<string, BdatTable>(StringComparer.Ordinal);
            foreach (BdatTable table in tables ?? Array.Empty<BdatTable>()) result[table.TableName] = table;
            return result;
        }

        internal static void WriteStatusCsv(string path, IReadOnlyList<ReservedItemRecord> records)
        {
            List<IReadOnlyList<string>> rows = new List<IReadOnlyList<string>>();
            foreach (ReservedItemRecord record in records ?? Array.Empty<ReservedItemRecord>())
            {
                rows.Add(new[]
                {
                    record.ItemId.ToString(CultureInfo.InvariantCulture),
                    record.Status,
                    record.ReasonEn,
                    record.ReasonCn,
                    record.Evidence
                });
            }

            ProjectDataWriter.WriteCsv(
                path,
                new ProjectCsvDocument(
                    Array.Empty<string>(),
                    new[] { "ItemID", "Status", "Reason_EN", "Reason_CN", "Evidence" },
                    rows,
                    Array.Empty<string>()));
        }

        private static string ResolveRawName(
            BdatRow sourceRow,
            string sourceKeyColumn,
            IReadOnlyDictionary<string, IReadOnlyDictionary<string, BdatTable>> localizedTables,
            string language,
            string localizedTableName,
            int sourceKeyOffset)
        {
            IReadOnlyDictionary<string, BdatTable> languageTables;
            BdatTable table;
            if (!localizedTables.TryGetValue(language, out languageTables) || !languageTables.TryGetValue(localizedTableName, out table)) return string.Empty;
            ulong key;
            if (string.Equals(sourceKeyColumn, "$id", StringComparison.Ordinal))
            {
                long adjusted = checked(sourceRow.Id + sourceKeyOffset);
                if (adjusted < 0) return string.Empty;
                key = checked((ulong)adjusted);
            }
            else
            {
                JsonElement value;
                if (!sourceRow.Fields.TryGetValue(sourceKeyColumn, out value) || value.ValueKind != JsonValueKind.Number || !value.TryGetUInt64(out key)) return string.Empty;
                if (sourceKeyOffset != 0) key = checked((ulong)checked((long)key + sourceKeyOffset));
            }

            BdatRow localized = table.Rows.FirstOrDefault(row => checked((ulong)row.Id) == key);
            if (localized == null) return string.Empty;
            JsonElement name;
            return localized.Fields.TryGetValue("name", out name) && name.ValueKind == JsonValueKind.String
                ? (name.GetString() ?? string.Empty).Trim()
                : string.Empty;
        }

        private static string BuildReason(IReadOnlyList<string> codes, bool chinese)
        {
            List<string> parts = new List<string>();
            foreach (string code in codes)
            {
                if (chinese)
                {
                    if (code == "empty-name") parts.Add("英文名称为空");
                    else if (code == "unknown-name") parts.Add("UNKNOWN 占位名称");
                    else if (code == "untranslated-name") parts.Add("英文表仍保留日文、中文或韩文字符");
                    else if (code == "internal-marker") parts.Add("包含内部、测试或删除标记");
                    else if (code == "provisional-marker") parts.Add("其他语言保留临时占位标记");
                    else if (code == "source-name-missing") parts.Add("BDAT 未提供名称指针");
                    else if (code == "no-locale-sort") parts.Add("所有语言均没有物品箱排序索引");
                    else if (code == "western-sort-missing") parts.Add("欧美语言没有物品箱排序索引");
                }
                else
                {
                    if (code == "empty-name") parts.Add("English name is empty");
                    else if (code == "unknown-name") parts.Add("UNKNOWN placeholder name");
                    else if (code == "untranslated-name") parts.Add("English table retains Japanese, Chinese, or Korean characters");
                    else if (code == "internal-marker") parts.Add("Internal, test, or deletion marker");
                    else if (code == "provisional-marker") parts.Add("Other languages retain a provisional marker");
                    else if (code == "source-name-missing") parts.Add("BDAT has no name pointer");
                    else if (code == "no-locale-sort") parts.Add("No locale has an item-box sort index");
                    else if (code == "western-sort-missing") parts.Add("Western locales have no item-box sort index");
                }
            }

            return string.Join("; ", parts);
        }

        private static string Compact(string value)
        {
            return (value ?? string.Empty).Replace('\r', ' ').Replace('\n', ' ').Trim();
        }

        private static int FindColumn(IReadOnlyList<string> columns, string name)
        {
            for (int index = 0; index < columns.Count; index++)
            {
                if (string.Equals(columns[index], name, StringComparison.Ordinal)) return index;
            }

            throw new InvalidDataException("Items.csv is missing column '" + name + "'.");
        }
    }
}
