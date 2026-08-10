using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace XC2BdatImporter
{
    internal static class MappingSelfTests
    {
        internal static void Run()
        {
            Dictionary<string, JsonElement> first = new Dictionary<string, JsonElement>(StringComparer.Ordinal)
            {
                { "Name", JsonDocument.Parse("2").RootElement.Clone() }
            };
            Dictionary<string, JsonElement> second = new Dictionary<string, JsonElement>(StringComparer.Ordinal)
            {
                { "Name", JsonDocument.Parse("1").RootElement.Clone() }
            };
            BdatTable common = new BdatTable(
                "common.json",
                "CHR_Dr",
                new[] { new BdatColumn("Name", 2) },
                new[] { new BdatRow(10, first), new BdatRow(11, second) });

            BdatTable localized = new BdatTable(
                "localized.json",
                "chr_dr_ms",
                new[] { new BdatColumn("name", 7) },
                new[]
                {
                    new BdatRow(1, new Dictionary<string, JsonElement>(StringComparer.Ordinal)
                    {
                        { "name", JsonDocument.Parse("\"Team[ML:Space]\"").RootElement.Clone() }
                    }),
                    new BdatRow(2, new Dictionary<string, JsonElement>(StringComparer.Ordinal)
                    {
                        { "name", JsonDocument.Parse("\"Rapid Attack\"").RootElement.Clone() }
                    })
                });

            IReadOnlyDictionary<ulong, string> index = NameJoiner.BuildNameIndex(localized, "$id", "name");
            AssertEqual("Team", NameJoiner.ResolveName(common.Rows[1], "Name", index), "name index lookup");
            AssertEqual("Rapid Attack", NameJoiner.ResolveName(common.Rows[0], "Name", index), "name index lookup 2");

            BdatTable duplicate = new BdatTable(
                "duplicate.json",
                "dup",
                new[] { new BdatColumn("name", 7) },
                new[]
                {
                    new BdatRow(1, new Dictionary<string, JsonElement>(StringComparer.Ordinal)
                    {
                        { "name", JsonDocument.Parse("\"One\"").RootElement.Clone() }
                    }),
                    new BdatRow(1, new Dictionary<string, JsonElement>(StringComparer.Ordinal)
                    {
                        { "name", JsonDocument.Parse("\"Two\"").RootElement.Clone() }
                    })
                });
            AssertThrows<NameJoinException>(() => NameJoiner.BuildNameIndex(duplicate, "$id", "name"), "duplicate localized index");

            Dictionary<string, JsonElement> missingFields = new Dictionary<string, JsonElement>(StringComparer.Ordinal)
            {
                { "Name", JsonDocument.Parse("9").RootElement.Clone() }
            };
            AssertThrows<NameJoinException>(
                () => NameJoiner.ResolveName(new BdatRow(9, missingFields), "Name", index),
                "missing localized index");

            BdatTable unknownMarker = new BdatTable(
                "marker.json",
                "marker",
                new[] { new BdatColumn("name", 7) },
                new[]
                {
                    new BdatRow(1, new Dictionary<string, JsonElement>(StringComparer.Ordinal)
                    {
                        { "name", JsonDocument.Parse("\"Bad[ML:Color]\"").RootElement.Clone() }
                    })
                });
            AssertThrows<NameJoinException>(() => NameJoiner.BuildNameIndex(unknownMarker, "$id", "name"), "unknown localization marker");

            BdatTable emptyName = new BdatTable(
                "empty.json",
                "empty",
                new[] { new BdatColumn("name", 7) },
                new[]
                {
                    new BdatRow(1, new Dictionary<string, JsonElement>(StringComparer.Ordinal)
                    {
                        { "name", JsonDocument.Parse("\" [ML:Space] \"").RootElement.Clone() }
                    })
                });
            AssertThrows<NameJoinException>(() => NameJoiner.BuildNameIndex(emptyName, "$id", "name"), "empty normalized name");

            BdatTable keyedByField = new BdatTable(
                "field-key.json",
                "field-key",
                new[] { new BdatColumn("Name", 2), new BdatColumn("name", 7) },
                new[]
                {
                    new BdatRow(20, new Dictionary<string, JsonElement>(StringComparer.Ordinal)
                    {
                        { "Name", JsonDocument.Parse("7").RootElement.Clone() },
                        { "name", JsonDocument.Parse("\"Field Key\"").RootElement.Clone() }
                    })
                });
            IReadOnlyDictionary<ulong, string> fieldIndex = NameJoiner.BuildNameIndex(keyedByField, "Name", "name");
            AssertEqual("Field Key", fieldIndex[7], "localized field key");

            MappingConfig config = MappingConfig.Load(System.IO.Path.Combine(AppContext.BaseDirectory, "mappings.json"));
            if (config.Mappings.Count < 20)
            {
                throw new InvalidOperationException("mapping config should cover the project data tables");
            }

            AssertHasMapping(config, "Drivers");
            AssertHasMapping(config, "DriverArts");
            AssertHasMapping(config, "DriverSkills");
            AssertHasMapping(config, "Weapons");
            AssertHasMapping(config, "Items_POPPI_SKILL_RAM.csv");
            AssertHasMapping(config, "BladeTrustRanks");
            AssertEqual(10000, config.GetMapping("Items_CORE_CHIPS.csv").IdOffset, "core chip offset");
            AssertEqual(5000, config.GetMapping("Weapons").IdOffset, "weapon offset");
            AssertEqual(25000, config.GetMapping("Items", "Items_KEY_ITEMS.csv").IdOffset, "key item offset");
            AssertEqual(60000, config.GetMapping("Items", "Items_POPPI_SKILL_RAM.csv").IdOffset, "Poppi skill RAM offset");
            AssertEqual("source.id", config.GetMapping("Items", "Items_CORE_CHIPS.csv").IdExpression, "core chip source ID");
            AssertEqual("source.id", config.GetMapping("Weapons").IdExpression, "weapon source ID");
            AssertEqual(6, config.GetMapping("Drivers").PreserveExistingIds.Count, "ambiguous driver IDs are preserved");
            AssertEqual(1, config.GetMapping("WeaponTypes").PreserveExistingIds.Count, "ambiguous weapon type ID is preserved");
            AssertEqual(4, config.GetMapping("FavCategoriesIra").PreserveExistingIds.Count, "Torna favorite IDs are preserved");
            AssertEqual("BTL_Buff", config.GetMapping("BladeArts").SourceTable, "blade arts source table");
            AssertEqual("btl_buff_ms", config.GetMapping("BladeArts").LocalizedTable, "blade arts localized table");
            AssertEqual("BTL_Arts_Bl", config.GetMapping("BladeSpecials").SourceTable, "blade specials source table");
            AssertEqual("BTL_Arts_BlSp", config.GetMapping("BladeSpecialsLv4").SourceTable, "lv4 specials source table");
            AssertEqual("itm_orb", config.GetMapping("Items", "Items_POPPI_SPECIALS_ENHANCING_RAM.csv").LocalizedTable, "Poppi special RAM localized table");
            AssertEqual("itm_orb", config.GetMapping("Items", "Items_POPPI_SKILL_RAM.csv").LocalizedTable, "Poppi skill RAM localized table");
            if (config.GetMappings("Items_KEY_ITEMS.csv").Count != 2)
            {
                throw new InvalidOperationException("key item mappings must merge base and Torna rows");
            }

            AssertEqual(5001L, config.GetMapping("Weapons").ComputeTargetId(5001), "weapon target ID preserves BDAT ID");
            AssertEqual(10001L, config.GetMapping("Items", "Items_CORE_CHIPS.csv").ComputeTargetId(10001), "core chip target ID preserves BDAT ID");
            AssertEqual(58001L, config.GetMapping("Items", "Items_POPPI_SPECIALS_ENHANCING_RAM.csv").ComputeTargetId(58001), "Poppi special target ID preserves BDAT ID");
            AssertEqual(0L, config.GetMapping("Elements").ComputeTargetId(1), "element target ID subtracts one");
            AssertEqual(12L, config.GetMapping("FavCategories").ComputeTargetId(1), "favorite category target ID adds eleven");
            AssertEqual(10001L, MappingGenerator.ComputeWeaponCoreChipId(5001), "first weapon core chip");
            AssertEqual(10060L, MappingGenerator.ComputeWeaponCoreChipId(5060), "last weapon core chip");
            AssertEqual(10001L, MappingGenerator.ComputeWeaponCoreChipId(5061), "weapon core chip wraps per type");

            BdatTable chipTable = new BdatTable(
                "chips.json",
                "ITM_PcWpnChip",
                new[] { new BdatColumn("CreateWpn1", 1), new BdatColumn("CreateWpn33", 1) },
                new[]
                {
                    new BdatRow(10001, new Dictionary<string, JsonElement>(StringComparer.Ordinal)
                    {
                        { "CreateWpn1", JsonDocument.Parse("5001").RootElement.Clone() },
                        { "CreateWpn33", JsonDocument.Parse("5990").RootElement.Clone() }
                    })
                });
            IReadOnlyDictionary<long, long> coreChipIndex = MappingGenerator.BuildWeaponCoreChipIndex(chipTable);
            AssertEqual(10001L, MappingGenerator.ResolveWeaponCoreChipId(5001, coreChipIndex), "weapon core chip lookup");
            AssertEqual(10001L, MappingGenerator.ResolveWeaponCoreChipId(5990, coreChipIndex), "sparse weapon core chip lookup");
            AssertEqual(0L, MappingGenerator.ResolveWeaponCoreChipId(5961, coreChipIndex), "unmapped weapon core chip lookup");

            BdatRow weaponType = new BdatRow(
                17,
                new Dictionary<string, JsonElement>(StringComparer.Ordinal)
                {
                    { "Motion", JsonDocument.Parse("1").RootElement.Clone() }
                });
            AssertEqual("1", MappingGenerator.ComputeWeaponTypeBaseId(weaponType, 17), "weapon type motion base ID");

            string invalidIdExpressionPath = System.IO.Path.Combine(
                TemporaryExtractionDirectory.Create(), "invalid-mapping.json");
            try
            {
                System.IO.File.WriteAllText(
                    invalidIdExpressionPath,
                    "[{\"sourceFile\":\"common.bdat\",\"sourceTable\":\"X\",\"localizedFile\":\"x.bdat\",\"localizedTable\":\"x\",\"targetCsv\":\"X.csv\",\"targetTable\":\"X\",\"keyColumn\":\"Name\",\"localizedKeyColumn\":\"$id\",\"idExpression\":\"row.id\"}]");
                AssertThrows<InvalidDataException>(
                    () => MappingConfig.Load(invalidIdExpressionPath),
                    "invalid ID expression");
            }
            finally
            {
                TemporaryExtractionDirectory.DeleteExact(System.IO.Path.GetDirectoryName(invalidIdExpressionPath));
            }

            BdatTable rapidAttack = new BdatTable(
                "rapid.json",
                "btl_skill_dr_name",
                new[] { new BdatColumn("name", 7) },
                new[]
                {
                    new BdatRow(2, new Dictionary<string, JsonElement>(StringComparer.Ordinal)
                    {
                        { "name", JsonDocument.Parse("\"Rapid Attack III\"").RootElement.Clone() }
                    }),
                    new BdatRow(24, new Dictionary<string, JsonElement>(StringComparer.Ordinal)
                    {
                        { "name", JsonDocument.Parse("\"Rapid Attack II\"").RootElement.Clone() }
                    })
                });
            IReadOnlyDictionary<ulong, string> rapidIndex = NameJoiner.BuildNameIndex(rapidAttack, "$id", "name");
            AssertEqual("Rapid Attack III", rapidIndex[2], "rapid attack III index");
            AssertEqual("Rapid Attack II", rapidIndex[24], "rapid attack II index");

            BdatTable initiative = new BdatTable(
                "initiative.json",
                "btl_skill_dr_name",
                new[] { new BdatColumn("name", 7) },
                new[]
                {
                    new BdatRow(123, new Dictionary<string, JsonElement>(StringComparer.Ordinal)
                    {
                        { "name", JsonDocument.Parse("\"Initiative I\"").RootElement.Clone() }
                    }),
                    new BdatRow(132, new Dictionary<string, JsonElement>(StringComparer.Ordinal)
                    {
                        { "name", JsonDocument.Parse("\"Initiative II\"").RootElement.Clone() }
                    })
                });
            IReadOnlyDictionary<ulong, string> initiativeIndex = NameJoiner.BuildNameIndex(initiative, "$id", "name");
            AssertEqual("Initiative I", initiativeIndex[123], "initiative I index");
            AssertEqual("Initiative II", initiativeIndex[132], "initiative II index");

            BdatTable normalizedBuffs = new BdatTable(
                "buff.json",
                "btl_buff_ms",
                new[] { new BdatColumn("name", 7) },
                new[]
                {
                    new BdatRow(1, new Dictionary<string, JsonElement>(StringComparer.Ordinal)
                    {
                        { "name", JsonDocument.Parse("\"Critical Up\"").RootElement.Clone() }
                    }),
                    new BdatRow(19, new Dictionary<string, JsonElement>(StringComparer.Ordinal)
                    {
                        { "name", JsonDocument.Parse("\"Invincible\"").RootElement.Clone() }
                    })
                });
            IReadOnlyDictionary<ulong, string> buffIndex = NameJoiner.BuildNameIndex(normalizedBuffs, "$id", "name");
            AssertEqual("Critical Up", buffIndex[1], "blade arts source index");
            AssertEqual("Invincible", buffIndex[19], "blade arts sparse source index");

            BdatTable nonSequentialSpecials = new BdatTable(
                "specials.json",
                "btl_arts_blsp_ms",
                new[] { new BdatColumn("name", 7) },
                new[]
                {
                    new BdatRow(33, new Dictionary<string, JsonElement>(StringComparer.Ordinal)
                    {
                        { "name", JsonDocument.Parse("\"Special 24\"").RootElement.Clone() }
                    }),
                    new BdatRow(123, new Dictionary<string, JsonElement>(StringComparer.Ordinal)
                    {
                        { "name", JsonDocument.Parse("\"Special 150\"").RootElement.Clone() }
                    })
                });
            IReadOnlyDictionary<ulong, string> specialIndex = NameJoiner.BuildNameIndex(nonSequentialSpecials, "$id", "name");
            AssertEqual("Special 150", specialIndex[123], "non-sequential special index");
        }

        private static void AssertHasMapping(MappingConfig config, string targetTable)
        {
            if (config.GetMapping(targetTable) == null)
            {
                throw new InvalidOperationException("missing mapping: " + targetTable);
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
}
