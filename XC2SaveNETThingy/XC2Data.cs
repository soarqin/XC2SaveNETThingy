using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XC2SaveNETThingy
{
    public static class XC2Data
    {
        public enum LANG
        {
            EN,
            CN
        }

        public static readonly Dictionary<LANG, string> LANG_STR = new Dictionary<LANG, string>()
        {
            { LANG.EN, "EN" },
            //{ LANG.CN, "CN" }
        };

        private static SQLiteConnection dbConn = new SQLiteConnection(
            @"Data Source=" + RuntimePaths.DatabaseFile + @"; Read Only=True;");
        private static SQLiteDataAdapter dbAdapter;
        private static DataSet ds;
        private static readonly Dictionary<string, DataSet> dataSetCache = new Dictionary<string, DataSet>();
        private static readonly object dataSetCacheLock = new object();
        private static bool? itemStatusTableAvailable;

        private static bool HasItemStatusTable()
        {
            if (itemStatusTableAvailable.HasValue)
                return itemStatusTableAvailable.Value;

            try
            {
                DataTable tables = GetDataSet(
                    "SELECT name FROM sqlite_master WHERE type = 'table' AND name = 'ItemStatus'").Tables[0];
                itemStatusTableAvailable = tables.Rows.Count != 0;
            }
            catch
            {
                // Keep older databases usable while the optional metadata table is absent.
                itemStatusTableAvailable = false;
            }

            return itemStatusTableAvailable.Value;
        }

        private static string ItemReservedSelectExpression()
        {
            return HasItemStatusTable()
                ? "CASE WHEN EXISTS (SELECT 1 FROM ItemStatus WHERE ItemID = Items.ID AND Status = 'Reserved') THEN 1 ELSE 0 END"
                : "0";
        }

        private static string ItemReservedMarkerExpression()
        {
            return HasItemStatusTable()
                ? "CASE WHEN EXISTS (SELECT 1 FROM ItemStatus WHERE ItemID = Items.ID AND Status = 'Reserved') THEN ' [Reserved]' ELSE '' END"
                : "''";
        }

        private static string ItemNameExpression(LANG l, string idExpression = null)
        {
            string expression = "Name_" + LANG_STR[l] + " || " + ItemReservedMarkerExpression();
            if (idExpression != null)
                expression += " || ' [' || (" + idExpression + ") || ']'";
            return expression;
        }

        public static DataTable Elements(LANG l = LANG.EN)
        {
            string query =
                "SELECT ID, " +
                "Name_" + LANG_STR[l] + " AS Name " +
                "FROM Elements";

            return GetDataSet(query).Tables[0];
        }

        public static DataTable Drivers(LANG l = LANG.EN)
        {
            string query =
                "SELECT ID, " +
                "Name_" + LANG_STR[l] + " AS Name " +
                "FROM Drivers";

            return GetDataSet(query).Tables[0];
        }

        public static DataTable DriversIra(LANG l = LANG.EN)
        {
            string query =
                "SELECT ID, " +
                "Name_" + LANG_STR[l] + " AS Name " +
                "FROM Drivers " +
                "WHERE ID >= 17";

            return GetDataSet(query).Tables[0];
        }

        public static bool ContainsDriver(UInt16 id)
        {
            DataTable dr = GetDataSet("SELECT ID FROM Drivers").Tables[0];
            List<UInt16> IDs = new List<UInt16>();
            foreach (DataRow r in dr.Rows)
                IDs.Add(Convert.ToUInt16(r[0]));

            return IDs.Contains(id);
        }

        public static DataTable DriverBladeSetStatus(LANG l = LANG.EN)
        {
            string query =
                "SELECT ID, " +
                "Desc_" + LANG_STR[l] + " AS Desc " +
                "FROM DriverBladeSetStatus";

            return GetDataSet(query).Tables[0];
        }

        public static DataTable DriverArts(Byte args_bits, LANG l = LANG.EN)
        {
            string viewDriverArts = 
                "SELECT ID, " +
                "(SELECT CASE(ID) " +
                "WHEN 0 THEN(Name_" + LANG_STR[l] + ") " +
                "ELSE(Name_" + LANG_STR[l] + " || ' [' || (SELECT Name_" + LANG_STR[l] + " FROM WeaponTypes WHERE ID = WeaponTypeID) || '] (' || (SELECT Name_" + LANG_STR[l] + " FROM Drivers WHERE ID = DriverID) || ')') " +
                "END) AS FriendlyName " +
                "FROM DriverArts";

            string query = "";

            if ((args_bits & 1) == 1) // get (none) placeholder art
            {
                query +=
                    viewDriverArts + " " +
                    "WHERE ID = 0";
            }

            if ((args_bits & 2) >> 1 == 1) // get 1.5.1 and lower arts
            {
                if (query != "")
                    query += " UNION ";
                query +=
                    viewDriverArts + " " +
                    "WHERE ID >= 1 AND ID < 582";
            }

            if ((args_bits & 4) >> 2 == 1) // get Torna DLC arts
            {
                if (query != "")
                    query += " UNION ";
                query +=
                    viewDriverArts + " " +
                    "WHERE ID >= 582 AND ID < 672";
            }

            if ((args_bits & 8) >> 3 == 1) // get Jin (Challenge Mode) arts
            {
                if (query != "")
                    query += " UNION ";
                query +=
                    viewDriverArts + " " +
                    "WHERE ID >= 672 AND ID < 678";
            }

            if ((args_bits & 16) >> 4 == 1) // get Torna DLC Talent arts
            {
                if (query != "")
                    query += " UNION ";
                query +=
                    viewDriverArts + " " +
                    "WHERE ID >= 678 AND ID < 687";
            }

            if ((args_bits & 32) >> 5 == 1) // get Dual Swords/Uchigatana arts
            {
                if (query != "")
                    query += " UNION ";
                query +=
                    viewDriverArts + " " +
                    "WHERE ID >= 687";
            }

            return GetDataSet(query).Tables[0];
        }

        public static DataTable DriverSkills(LANG l = LANG.EN)
        {
            string query =
                "SELECT ID, " +
                "Name_" + LANG_STR[l] + " || ' [' || ID || ']' AS Name " +
                "FROM DriverSkills";

            return GetDataSet(query).Tables[0];
        }

        public static DataTable WeaponTypes(Byte args_bits, LANG l = LANG.EN)
        {
            string query = "";

            query +=
                "SELECT ID, " +
                "Name_" + LANG_STR[l] + " AS Name " +
                "FROM WeaponTypes WHERE ID = 0";

            if ((args_bits & 1) == 1) // get pre-1.5.0 weapons
            {
                if (query != "")
                    query += " UNION ";
                query +=
                    "SELECT ID, " +
                    "Name_" + LANG_STR[l] + " AS Name " +
                    "FROM WeaponTypes WHERE ID >= 1 AND ID < 27";
            }

            if ((args_bits & 2) >> 1 == 1) // get Torna DLC unused Aegis Sword
            {
                if (query != "")
                    query += " UNION ";
                query +=
                    "SELECT ID, " +
                    "Name_" + LANG_STR[l] + " AS Name " +
                    "FROM WeaponTypes WHERE ID = 27";
            }

            if ((args_bits & 4) >> 2 == 1) // get Torna DLC Weapons
            {
                if (query != "")
                    query += " UNION ";
                query +=
                    "SELECT ID, " +
                    "Name_" + LANG_STR[l] + " AS Name " +
                    "FROM WeaponTypes WHERE ID >= 28 AND ID < 33";
            }

            if ((args_bits & 8) >> 3 == 1) // get 1.5.0+ (non-Torna) DLC Weapons
            {
                if (query != "")
                    query += " UNION ";
                query +=
                    "SELECT ID, " +
                    "Name_" + LANG_STR[l] + " AS Name " +
                    "FROM WeaponTypes WHERE ID >= 33";
            }

            return GetDataSet(query).Tables[0];
        }

        public static DataTable Weapons(LANG l = LANG.EN)
        {
            string query =
                "SELECT ID, " +
                "(SELECT CASE(ID) " + 
                "WHEN 0 THEN(Name_" + LANG_STR[l] + ") " +
                "ELSE(Name_" + LANG_STR[l] + " || ' [' || (SELECT Name_" + LANG_STR[l] + " FROM Items WHERE ID = CoreChipID) || ']') END) AS FriendlyName " +
                "FROM Weapons";

            return GetDataSet(query).Tables[0];
        }

        public static DataTable BladeCommonNames(LANG l = LANG.EN)
        {
            string query =
                "SELECT ID, " +
                "Name_" + LANG_STR[l] + " AS Name " +
                "FROM BladeCommonNames";

            return GetDataSet(query).Tables[0];
        }

        public static DataTable BladeRareNames(LANG l = LANG.EN)
        {
            string query =
                "SELECT ID, " +
                "Name_" + LANG_STR[l] + " AS Name " +
                "FROM BladeRareNames";

            return GetDataSet(query).Tables[0];
        }

        public static DataTable BladeTrustRanks(LANG l = LANG.EN)
        {
            return GetDataSet("SELECT * FROM BladeTrustRanks").Tables[0];
        }

        public static DataTable BladeSpecials(LANG l = LANG.EN)
        {
            string query =
                "SELECT ID, " +
                "Name_" + LANG_STR[l] + " || ' [' || ID || ']' AS Name " +
                "FROM BladeSpecials";
            return GetDataSet(query).Tables[0];
        }

        public static DataTable BladeSpecialsLv4(LANG l = LANG.EN)
        {
            string query =
                "SELECT ID, " +
                "Name_" + LANG_STR[l] + " || ' [' || ID || ']' AS Name " +
                "FROM BladeSpecialsLv4";
            return GetDataSet(query).Tables[0];
        }

        public static DataTable BladeArts(LANG l = LANG.EN)
        {
            string query =
                "SELECT ID, " +
                "Name_" + LANG_STR[l] + " || ' [' || ID || ']' AS Name " +
                "FROM BladeArts";
            return GetDataSet(query).Tables[0];
        }

        public static DataTable BladeSkills(LANG l = LANG.EN)
        {
            string query =
                "SELECT ID, " +
                "Name_" + LANG_STR[l] + " || ' [' || ID || ']' AS Name " +
                "FROM BladeSkills";
            return GetDataSet(query).Tables[0];
        }

        public static DataTable BladeFieldSkills(LANG l = LANG.EN)
        {
            string query =
                "SELECT ID, " +
                "Name_" + LANG_STR[l] + " || ' [' || ID || ']' AS Name " +
                "FROM BladeFieldSkills";
            return GetDataSet(query).Tables[0];
        }

        public static DataTable PoppiPowerLevels()
        {
            return GetDataSet("SELECT * FROM PoppiPowerLevels").Tables[0];
        }

        public static DataTable Items(LANG l = LANG.EN)
        {
            string query =
                "SELECT ID, " +
                ItemNameExpression(l, "ID") + " AS Name, " +
                ItemReservedSelectExpression() + " AS Reserved " +
                "FROM Items";

            return GetDataSet(query).Tables[0];
        }

        public static DataTable ItemTypes(LANG l = LANG.EN)
        {
            string query =
                "SELECT ID, " +
                "Name_" + LANG_STR[l] + " AS Name " +
                "FROM ItemTypes";

            return GetDataSet(query).Tables[0];
        }

        public static DataTable FavCategories(LANG l = LANG.EN)
        {
            string query =
                "SELECT ID, " +
                "Name_" + LANG_STR[l] + " AS Name " +
                "FROM FavCategories";

            return GetDataSet(query).Tables[0];
        }

        public static DataTable FavCategoriesIra(LANG l = LANG.EN)
        {
            string query =
                "SELECT ID, " +
                "Name_" + LANG_STR[l] + " AS Name " +
                "FROM FavCategoriesIra";

            return GetDataSet(query).Tables[0];
        }

        public static DataTable GetCoreChips(LANG l = LANG.EN)
        {
            string query =
                "SELECT ID, " + ItemNameExpression(l) + " AS Name, " + ItemReservedSelectExpression() + " AS Reserved FROM Items WHERE ItemTypeID = 0 " +
                "UNION ALL " +
                "SELECT REPLACE(ID, ID, ID -10000) AS ID, " +
                ItemNameExpression(l, "ID - 10000") + " AS Name, " + ItemReservedSelectExpression() + " AS Reserved " +
                "FROM Items WHERE ItemTypeID = 1";

            return GetDataSet(query).Tables[0];
        }

        public static DataTable GetAccessories(LANG l = LANG.EN)
        {
            string query =
                "SELECT ID, " + ItemNameExpression(l) + " AS Name, " + ItemReservedSelectExpression() + " AS Reserved FROM Items WHERE ItemTypeID = 0 " +
                "UNION ALL " +
                "SELECT ID, " +
                ItemNameExpression(l, "ID") + " AS Name, " + ItemReservedSelectExpression() + " AS Reserved " +
                "FROM Items WHERE ItemTypeID = 2";

            return GetDataSet(query).Tables[0];
        }

        public static DataTable GetAuxCores(LANG l = LANG.EN)
        {
            string query =
                "SELECT ID, " + ItemNameExpression(l) + " AS Name, " + ItemReservedSelectExpression() + " AS Reserved FROM Items WHERE ItemTypeID = 0 " +
                "UNION ALL " +
                "SELECT REPLACE(ID, ID, ID - 17000) AS ID, " +
                ItemNameExpression(l, "ID - 17000") + " AS Name, " + ItemReservedSelectExpression() + " AS Reserved " +
                "FROM Items WHERE ItemTypeID = 3";

            return GetDataSet(query).Tables[0];
        }

        public static DataTable GetCylinders(LANG l = LANG.EN)
        {
            string query =
                "SELECT ID, " + ItemNameExpression(l) + " AS Name, " + ItemReservedSelectExpression() + " AS Reserved FROM Items WHERE ItemTypeID = 0 " +
                "UNION ALL " +
                "SELECT REPLACE(ID, ID, ID - 20000) AS ID, " +
                ItemNameExpression(l, "ID - 20000") + " AS Name, " + ItemReservedSelectExpression() + " AS Reserved " +
                "FROM Items WHERE ItemTypeID = 5";

            return GetDataSet(query).Tables[0];
        }

        public static DataTable GetKeyItems(LANG l = LANG.EN)
        {
            string query =
                "SELECT ID, " + ItemNameExpression(l) + " AS Name, " + ItemReservedSelectExpression() + " AS Reserved FROM Items WHERE ItemTypeID = 0 " +
                "UNION ALL " +
                "SELECT REPLACE(ID, ID, ID - 25000) AS ID, " +
                ItemNameExpression(l, "ID - 25000") + " AS Name, " + ItemReservedSelectExpression() + " AS Reserved " +
                "FROM Items WHERE ItemTypeID = 6";

            return GetDataSet(query).Tables[0];
        }

        public static DataTable GetCollectibles(LANG l = LANG.EN)
        {
            string query =
                "SELECT ID, " + ItemNameExpression(l) + " AS Name, " + ItemReservedSelectExpression() + " AS Reserved FROM Items WHERE ItemTypeID = 0 " +
                "UNION ALL " +
                "SELECT REPLACE(ID, ID, ID - 30000) AS ID, " +
                ItemNameExpression(l, "ID - 30000") + " AS Name, " + ItemReservedSelectExpression() + " AS Reserved " +
                "FROM Items WHERE ItemTypeID = 7";

            return GetDataSet(query).Tables[0];
        }

        public static DataTable GetTreasure(LANG l = LANG.EN)
        {
            string query =
                "SELECT ID, " + ItemNameExpression(l) + " AS Name, " + ItemReservedSelectExpression() + " AS Reserved FROM Items WHERE ItemTypeID = 0 " +
                "UNION ALL " +
                "SELECT REPLACE(ID, ID, ID - 35000) AS ID, " +
                ItemNameExpression(l, "ID - 35000") + " AS Name, " + ItemReservedSelectExpression() + " AS Reserved " +
                "FROM Items WHERE ItemTypeID = 8";

            return GetDataSet(query).Tables[0];
        }

        public static DataTable GetUnrefinedAuxCores(LANG l = LANG.EN)
        {
            string query =
                "SELECT ID, " + ItemNameExpression(l) + " AS Name, " + ItemReservedSelectExpression() + " AS Reserved FROM Items WHERE ItemTypeID = 0 " +
                "UNION ALL " +
                "SELECT REPLACE(ID, ID, ID - 15000) AS ID, " +
                ItemNameExpression(l, "ID - 15000") + " AS Name, " + ItemReservedSelectExpression() + " AS Reserved " +
                "FROM Items WHERE ItemTypeID = 9";

            return GetDataSet(query).Tables[0];
        }

        public static DataTable GetPouchItems(LANG l = LANG.EN)
        {
            string query =
                "SELECT ID, " + ItemNameExpression(l) + " AS Name, " + ItemReservedSelectExpression() + " AS Reserved FROM Items WHERE ItemTypeID = 0 " +
                "UNION ALL " +
                "SELECT REPLACE(ID, ID, ID - 40000) AS ID, " +
                ItemNameExpression(l, "ID - 40000") + " AS Name, " + ItemReservedSelectExpression() + " AS Reserved " +
                "FROM Items WHERE ItemTypeID = 10";

            return GetDataSet(query).Tables[0];
        }

        public static DataTable GetCoreCrystals(LANG l = LANG.EN)
        {
            string query =
                "SELECT ID, " + ItemNameExpression(l) + " AS Name, " + ItemReservedSelectExpression() + " AS Reserved FROM Items WHERE ItemTypeID = 0 " +
                "UNION ALL " +
                "SELECT REPLACE(ID, ID, ID - 45000) AS ID, " +
                ItemNameExpression(l, "ID - 45000") + " AS Name, " + ItemReservedSelectExpression() + " AS Reserved " +
                "FROM Items WHERE ItemTypeID = 11";

            return GetDataSet(query).Tables[0];
        }

        public static DataTable GetBoosters(LANG l = LANG.EN)
        {
            string query =
                "SELECT ID, " + ItemNameExpression(l) + " AS Name, " + ItemReservedSelectExpression() + " AS Reserved FROM Items WHERE ItemTypeID = 0 " +
                "UNION ALL " +
                "SELECT REPLACE(ID, ID, ID - 50000) AS ID, " +
                ItemNameExpression(l, "ID - 50000") + " AS Name, " + ItemReservedSelectExpression() + " AS Reserved " +
                "FROM Items WHERE ItemTypeID = 12";

            return GetDataSet(query).Tables[0];
        }

        public static DataTable GetInfoItems(LANG l = LANG.EN)
        {
            string query =
                "SELECT ID, " + ItemNameExpression(l) + " AS Name, " + ItemReservedSelectExpression() + " AS Reserved FROM Items WHERE ItemTypeID = 0 " +
                "UNION ALL " +
                "SELECT REPLACE(ID, ID, ID - 26000) AS ID, " +
                ItemNameExpression(l, "ID - 26000") + " AS Name, " + ItemReservedSelectExpression() + " AS Reserved " +
                "FROM Items WHERE ItemTypeID = 14";

            return GetDataSet(query).Tables[0];
        }

        public static DataTable GetPoppiRoleCPU(LANG l = LANG.EN)
        {
            string query =
                "SELECT ID, " + ItemNameExpression(l) + " AS Name, " + ItemReservedSelectExpression() + " AS Reserved FROM Items WHERE ItemTypeID = 0 " +
                "UNION ALL " +
                "SELECT REPLACE(ID, ID, ID - 56000) AS ID, " +
                ItemNameExpression(l, "ID - 56000") + " AS Name, " + ItemReservedSelectExpression() + " AS Reserved " +
                "FROM Items WHERE ItemTypeID = 16";

            return GetDataSet(query).Tables[0];
        }

        public static DataTable GetPoppiElementCores(LANG l = LANG.EN)
        {
            string query =
                "SELECT ID, " + ItemNameExpression(l) + " AS Name, " + ItemReservedSelectExpression() + " AS Reserved FROM Items WHERE ItemTypeID = 0 " +
                "UNION ALL " +
                "SELECT REPLACE(ID, ID, ID - 57000) AS ID, " +
                ItemNameExpression(l, "ID - 57000") + " AS Name, " + ItemReservedSelectExpression() + " AS Reserved " +
                "FROM Items WHERE ItemTypeID = 17";

            return GetDataSet(query).Tables[0];
        }

        public static DataTable GetPoppiSpecialsEnhancingRAM(LANG l = LANG.EN)
        {
            string query =
                "SELECT ID, " + ItemNameExpression(l) + " AS Name, " + ItemReservedSelectExpression() + " AS Reserved FROM Items WHERE ItemTypeID = 0 " +
                "UNION ALL " +
                "SELECT REPLACE(ID, ID, ID - 58000) AS ID, " +
                ItemNameExpression(l, "ID - 58000") + " AS Name, " + ItemReservedSelectExpression() + " AS Reserved " +
                "FROM Items WHERE ItemTypeID = 18";

            return GetDataSet(query).Tables[0];
        }

        public static DataTable GetPoppiArtsCards(LANG l = LANG.EN)
        {
            string query =
                "SELECT ID, " + ItemNameExpression(l) + " AS Name, " + ItemReservedSelectExpression() + " AS Reserved FROM Items WHERE ItemTypeID = 0 " +
                "UNION ALL " +
                "SELECT REPLACE(ID, ID, ID - 59000) AS ID, " +
                ItemNameExpression(l, "ID - 59000") + " AS Name, " + ItemReservedSelectExpression() + " AS Reserved " +
                "FROM Items WHERE ItemTypeID = 19";

            return GetDataSet(query).Tables[0];
        }

        public static DataTable GetPoppiSkillRAM(LANG l = LANG.EN)
        {
            string query =
                "SELECT ID, " + ItemNameExpression(l) + " AS Name, " + ItemReservedSelectExpression() + " AS Reserved FROM Items WHERE ItemTypeID = 0 " +
                "UNION ALL " +
                "SELECT REPLACE(ID, ID, ID - 60000) AS ID, " +
                ItemNameExpression(l, "ID - 60000") + " AS Name, " + ItemReservedSelectExpression() + " AS Reserved " +
                "FROM Items WHERE ItemTypeID = 20";

            return GetDataSet(query).Tables[0];
        }

        public static Dictionary<int, string> Flags1Bit(LANG l = LANG.EN)
        {
            string query =
                "SELECT ID, Name_" + LANG_STR[l] + " AS Name FROM Flags1Bit";

            DataTable dt = GetDataSet(query).Tables[0];
            Dictionary<int, string> dict = new Dictionary<int, string>();

            foreach (DataRow r in dt.Rows)
                dict.Add(Convert.ToInt32(r[0]), r[1].ToString());

            return dict;
        }

        public static Dictionary<int, string> Flags8Bit(LANG l = LANG.EN)
        {
            string query =
                "SELECT ID, Name_" + LANG_STR[l] + " AS Name FROM Flags8Bit";

            DataTable dt = GetDataSet(query).Tables[0];
            Dictionary<int, string> dict = new Dictionary<int, string>();

            foreach (DataRow r in dt.Rows)
                dict.Add(Convert.ToInt32(r[0]), r[1].ToString());

            return dict;
        }

        public static Dictionary<int, string> Flags16Bit(LANG l = LANG.EN)
        {
            string query =
                "SELECT ID, Name_" + LANG_STR[l] + " AS Name FROM Flags16Bit";

            DataTable dt = GetDataSet(query).Tables[0];
            Dictionary<int, string> dict = new Dictionary<int, string>();

            foreach (DataRow r in dt.Rows)
                dict.Add(Convert.ToInt32(r[0]), r[1].ToString());

            return dict;
        }

        public static Dictionary<int, string> Flags32Bit(LANG l = LANG.EN)
        {
            string query =
                "SELECT ID, Name_" + LANG_STR[l] + " AS Name FROM Flags32Bit";

            DataTable dt = GetDataSet(query).Tables[0];
            Dictionary<int, string> dict = new Dictionary<int, string>();

            foreach (DataRow r in dt.Rows)
                dict.Add(Convert.ToInt32(r[0]), r[1].ToString());

            return dict;
        }

        public static Dictionary<string, string> GetFrmAboutText(LANG l = LANG.EN)
        {
            string query =
                "SELECT ControlName, Text_" + LANG_STR[l] + " AS Text FROM FrmAboutText";

            DataTable dt = GetDataSet(query).Tables[0];
            Dictionary<string, string> dict = new Dictionary<string, string>();

            foreach (DataRow r in dt.Rows)
                dict.Add(r[0].ToString(), r[1].ToString());

            return dict;
        }

        public static Dictionary<string, string> GetFrmFAQText(LANG l = LANG.EN)
        {
            string query =
                "SELECT ControlName, Text_" + LANG_STR[l] + " AS Text FROM FrmFAQText";

            DataTable dt = GetDataSet(query).Tables[0];
            Dictionary<string, string> dict = new Dictionary<string, string>();

            foreach (DataRow r in dt.Rows)
                dict.Add(r[0].ToString(), r[1].ToString());

            return dict;
        }

        public static Dictionary<string, string> GetFrmMainText(LANG l = LANG.EN)
        {
            string query =
                "SELECT ControlName, Text_" + LANG_STR[l] + " AS Text FROM FrmMainText";

            DataTable dt = GetDataSet(query).Tables[0];
            Dictionary<string, string> dict = new Dictionary<string, string>();

            foreach (DataRow r in dt.Rows)
                dict.Add(r[0].ToString(), r[1].ToString());

            return dict;
        }

        public static Dictionary<string, string> GetFrmMainInternalText(LANG l = LANG.EN)
        {
            string query =
                "SELECT Name, Text_" + LANG_STR[l] + " AS Text FROM FrmMainInternalText";

            DataTable dt = GetDataSet(query).Tables[0];
            Dictionary<string, string> dict = new Dictionary<string, string>();

            foreach (DataRow r in dt.Rows)
                dict.Add(r[0].ToString(), r[1].ToString());

            return dict;
        }

        /// <summary>
        /// Method for getting data from database and binding to DataSet
        /// </summary>
        /// <param name="query">SQL query to send to the database</param>
        public static DataSet GetDataSet(string query)
        {
            lock (dataSetCacheLock)
            {
                if (dataSetCache.TryGetValue(query, out DataSet cachedDataSet))
                    return cachedDataSet;

                ds = new DataSet();
                dbAdapter = new SQLiteDataAdapter(query, dbConn);

                dbConn.Open();
                try
                {
                    dbAdapter.Fill(ds);
                }
                finally
                {
                    dbConn.Close();
                }

                dataSetCache.Add(query, ds);
                return ds;
            }
        }
    }
}
