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
        private static SQLiteConnection dbConn = new SQLiteConnection(@"Data Source=db\XC2Data.db");
        private static SQLiteDataAdapter dbAdapter;
        private static SQLiteCommand dbCmd;
        private static DataSet ds;

        public static DataTable Elements()
        {
            return GetDataSet("SELECT * FROM Elements").Tables[0];
        }

        public static DataTable Drivers()
        {
            return GetDataSet("SELECT * FROM Drivers").Tables[0];
        }

        public static bool ContainsDriver(UInt16 id)
        {
            DataTable dr = GetDataSet("SELECT ID FROM Drivers").Tables[0];
            List<UInt16> IDs = new List<UInt16>();
            foreach (DataRow r in dr.Rows)
                IDs.Add(Convert.ToUInt16(r[0]));

            return IDs.Contains(id);
        }

        public static DataTable DriverBladeSetStatus()
        {
            return GetDataSet("SELECT * FROM DriverBladeSetStatus").Tables[0];
        }

        public static DataTable DriverArts(Byte args_bits)
        {
            //return GetDataSet("SELECT * FROM viewDriverArts").Tables[0];

            string query = "";

            query += "SELECT * FROM viewDriverArts WHERE ID = 0";

            if ((args_bits & 1) == 1) // get 1.5.1 and lower arts
            {
                if (query != "")
                    query += " UNION ";
                query += "SELECT * FROM viewDriverArts WHERE ID >= 1 AND ID < 582";
            }

            if ((args_bits & 2) >> 1 == 1) // get higher than 1.5.1 arts
            {
                if (query != "")
                    query += " UNION ";
                query += "SELECT * FROM viewDriverArts WHERE ID >= 582 AND ID < 743";
            }

            return GetDataSet(query).Tables[0];
        }

        public static DataTable DriverSkills()
        {
            return GetDataSet("SELECT * FROM DriverSkills").Tables[0];
        }

        public static DataTable Weapons(Byte args_bits)
        {
            string query = "";

            query += "SELECT * FROM Weapons WHERE ID = 0";

            if ((args_bits & 1) == 1) // get pre-1.5.0 weapons
            {
                if (query != "")
                    query += " UNION ";
                query += "SELECT * FROM Weapons WHERE ID >= 1 AND ID < 27";
            }

            if ((args_bits & 2) >> 1 == 1) // get 1.5.0 unknown weapons
            {
                if (query != "")
                    query += " UNION ";
                query += "SELECT * FROM Weapons WHERE ID >= 27 AND ID < 33";
            }

            if ((args_bits & 4) >> 2 == 1) // get 1.5.0 monado + knives
            {
                if (query != "")
                    query += " UNION ";
                query += "SELECT * FROM Weapons WHERE ID >= 33 AND ID < 35";
            }

            return GetDataSet(query).Tables[0];
        }

        public static DataTable BladeWeaponNames()
        {
            return GetDataSet("SELECT * from viewWeaponNames").Tables[0];
        }

        public static DataTable BladeCommonNames()
        {
            return GetDataSet("SELECT * FROM BladeCommonNames").Tables[0];
        }

        public static DataTable BladeRareNames()
        {
            return GetDataSet("SELECT * FROM BladeRareNames").Tables[0];
        }

        public static DataTable BladeTrustRanks()
        {
            return GetDataSet("SELECT * FROM BladeTrustRanks").Tables[0];
        }

        public static DataTable BladeSpecials()
        {
            string query =
                "SELECT ID, " +
                "Name || ' [' || ID || ']' AS Name " +
                "FROM BladeSpecials";
            return GetDataSet(query).Tables[0];
        }

        public static DataTable BladeArts()
        {
            string query =
                "SELECT ID, " +
                "Name || ' [' || ID || ']' AS Name " +
                "FROM BladeArts";
            return GetDataSet(query).Tables[0];
        }

        public static DataTable BladeSkills()
        {
            string query =
                "SELECT ID, " +
                "Name || ' [' || ID || ']' AS Name " +
                "FROM BladeSkills";
            return GetDataSet(query).Tables[0];
        }

        public static DataTable BladeFieldSkills()
        {
            string query =
                "SELECT ID, " +
                "Name || ' [' || ID || ']' AS Name " +
                "FROM BladeFieldSkills";
            return GetDataSet(query).Tables[0];
        }

        public static DataTable PoppiPowerLevels()
        {
            return GetDataSet("SELECT * FROM PoppiPowerLevels").Tables[0];
        }

        public static DataTable Items()
        {
            string query =
                "SELECT ID, " +
                "Name || ' [' || ID || ']' AS Name " +
                "FROM Items";

            return GetDataSet(query).Tables[0];
        }

        public static DataTable ItemTypes()
        {
            return GetDataSet("SELECT * FROM ItemTypes").Tables[0];
        }

        public static DataTable FavCategories()
        {
            return GetDataSet("SELECT * FROM FavCategories").Tables[0];
        }

        public static DataTable GetCoreChips()
        {
            string query =
                "SELECT ID, Name FROM Items WHERE ItemTypeID = 0 " +
                "UNION ALL " +
                "SELECT REPLACE(ID, ID, ID -10000) AS ID, " +
                "NAME || ' [' || REPLACE(ID, ID, ID - 10000) || ']' AS Name " +
                "FROM Items WHERE ItemTypeID = 1";

            return GetDataSet(query).Tables[0];
        }

        public static DataTable GetAccessories()
        {
            string query =
                "SELECT ID, Name FROM Items WHERE ItemTypeID = 0 " +
                "UNION ALL " +
                "SELECT ID, " +
                "NAME || ' [' || ID || ']' AS Name " +
                "FROM Items WHERE ItemTypeID = 2";

            return GetDataSet(query).Tables[0];
        }

        public static DataTable GetAuxCores()
        {
            string query =
                "SELECT ID, Name FROM Items WHERE ItemTypeID = 0 " +
                "UNION ALL " +
                "SELECT REPLACE(ID, ID, ID - 17000) AS ID, " +
                "NAME || ' [' || REPLACE(ID, ID, ID - 17000) || ']' AS Name " +
                "FROM Items WHERE ItemTypeID = 3";

            return GetDataSet(query).Tables[0];
        }

        public static DataTable GetCylinders()
        {
            string query =
                "SELECT ID, Name FROM Items WHERE ItemTypeID = 0 " +
                "UNION ALL " +
                "SELECT REPLACE(ID, ID, ID - 20000) AS ID, " +
                "NAME || ' [' || REPLACE(ID, ID, ID - 20000) || ']' AS Name " +
                "FROM Items WHERE ItemTypeID = 5";

            return GetDataSet(query).Tables[0];
        }

        public static DataTable GetKeyItems()
        {
            string query =
                "SELECT ID, Name FROM Items WHERE ItemTypeID = 0 " +
                "UNION ALL " +
                "SELECT REPLACE(ID, ID, ID - 25000) AS ID, " +
                "NAME || ' [' || REPLACE(ID, ID, ID - 25000) || ']' AS Name " +
                "FROM Items WHERE ItemTypeID = 6";

            return GetDataSet(query).Tables[0];
        }

        public static DataTable GetCollectibles()
        {
            string query =
                "SELECT ID, Name FROM Items WHERE ItemTypeID = 0 " +
                "UNION ALL " +
                "SELECT REPLACE(ID, ID, ID - 30000) AS ID, " +
                "NAME || ' [' || REPLACE(ID, ID, ID - 30000) || ']' AS Name " +
                "FROM Items WHERE ItemTypeID = 7";

            return GetDataSet(query).Tables[0];
        }

        public static DataTable GetTreasure()
        {
            string query =
                "SELECT ID, Name FROM Items WHERE ItemTypeID = 0 " +
                "UNION ALL " +
                "SELECT REPLACE(ID, ID, ID - 35000) AS ID, " +
                "NAME || ' [' || REPLACE(ID, ID, ID - 35000) || ']' AS Name " +
                "FROM Items WHERE ItemTypeID = 8";

            return GetDataSet(query).Tables[0];
        }

        public static DataTable GetUnrefinedAuxCores()
        {
            string query =
                "SELECT ID, Name FROM Items WHERE ItemTypeID = 0 " +
                "UNION ALL " +
                "SELECT REPLACE(ID, ID, ID - 15000) AS ID, " +
                "NAME || ' [' || REPLACE(ID, ID, ID - 15000) || ']' AS Name " +
                "FROM Items WHERE ItemTypeID = 9";

            return GetDataSet(query).Tables[0];
        }

        public static DataTable GetPouchItems()
        {
            string query =
                "SELECT ID, Name FROM Items WHERE ItemTypeID = 0 " +
                "UNION ALL " +
                "SELECT REPLACE(ID, ID, ID - 40000) AS ID, " +
                "NAME || ' [' || REPLACE(ID, ID, ID - 40000) || ']' AS Name " +
                "FROM Items WHERE ItemTypeID = 10";

            return GetDataSet(query).Tables[0];
        }

        public static DataTable GetCoreCrystals()
        {
            string query =
                "SELECT ID, Name FROM Items WHERE ItemTypeID = 0 " +
                "UNION ALL " +
                "SELECT REPLACE(ID, ID, ID - 45000) AS ID, " +
                "NAME || ' [' || REPLACE(ID, ID, ID - 45000) || ']' AS Name " +
                "FROM Items WHERE ItemTypeID = 11";

            return GetDataSet(query).Tables[0];
        }

        public static DataTable GetBoosters()
        {
            string query =
                "SELECT ID, Name FROM Items WHERE ItemTypeID = 0 " +
                "UNION ALL " +
                "SELECT REPLACE(ID, ID, ID - 50000) AS ID, " +
                "NAME || ' [' || REPLACE(ID, ID, ID - 50000) || ']' AS Name " +
                "FROM Items WHERE ItemTypeID = 12";

            return GetDataSet(query).Tables[0];
        }

        public static DataTable GetInfoItems()
        {
            string query =
                "SELECT ID, Name FROM Items WHERE ItemTypeID = 0 " +
                "UNION ALL " +
                "SELECT REPLACE(ID, ID, ID - 26000) AS ID, " +
                "NAME || ' [' || REPLACE(ID, ID, ID - 26000) || ']' AS Name " +
                "FROM Items WHERE ItemTypeID = 14";

            return GetDataSet(query).Tables[0];
        }

        public static DataTable GetPoppiRoleCPU()
        {
            string query =
                "SELECT ID, Name FROM Items WHERE ItemTypeID = 0 " +
                "UNION ALL " +
                "SELECT REPLACE(ID, ID, ID - 56000) AS ID, " +
                "NAME || ' [' || REPLACE(ID, ID, ID - 56000) || ']' AS Name " +
                "FROM Items WHERE ItemTypeID = 16";

            return GetDataSet(query).Tables[0];
        }

        public static DataTable GetPoppiElementCores()
        {
            string query =
                "SELECT ID, Name FROM Items WHERE ItemTypeID = 0 " +
                "UNION ALL " +
                "SELECT REPLACE(ID, ID, ID - 57000) AS ID, " +
                "NAME || ' [' || REPLACE(ID, ID, ID - 57000) || ']' AS Name " +
                "FROM Items WHERE ItemTypeID = 17";

            return GetDataSet(query).Tables[0];
        }

        public static DataTable GetPoppiSpecialsEnhancingRAM()
        {
            string query =
                "SELECT ID, Name FROM Items WHERE ItemTypeID = 0 " +
                "UNION ALL " +
                "SELECT REPLACE(ID, ID, ID - 58000) AS ID, " +
                "NAME || ' [' || REPLACE(ID, ID, ID - 58000) || ']' AS Name " +
                "FROM Items WHERE ItemTypeID = 18";

            return GetDataSet(query).Tables[0];
        }

        public static DataTable GetPoppiArtsCards()
        {
            string query =
                "SELECT ID, Name FROM Items WHERE ItemTypeID = 0 " +
                "UNION ALL " +
                "SELECT REPLACE(ID, ID, ID - 59000) AS ID, " +
                "NAME || ' [' || REPLACE(ID, ID, ID - 59000) || ']' AS Name " +
                "FROM Items WHERE ItemTypeID = 19";

            return GetDataSet(query).Tables[0];
        }

        public static DataTable GetPoppiSkillRAM()
        {
            string query =
                "SELECT ID, Name FROM Items WHERE ItemTypeID = 0 " +
                "UNION ALL " +
                "SELECT REPLACE(ID, ID, ID - 60000) AS ID, " +
                "NAME || ' [' || REPLACE(ID, ID, ID - 60000) || ']' AS Name " +
                "FROM Items WHERE ItemTypeID = 20";

            return GetDataSet(query).Tables[0];
        }

        /// <summary>
        /// Method for getting data from database and binding to DataSet
        /// </summary>
        /// <param name="query">SQL query to send to the database</param>
        public static DataSet GetDataSet(string query)
        {
            ds = new DataSet();                             // Make new DataSet
            dbAdapter = new SQLiteDataAdapter(query, dbConn);  // Make new DbAdapter using supplied query

            dbConn.Open();                                  // Open connection to database
            dbAdapter.Fill(ds);                             // Fill the DataSet with obtained data
            dbConn.Close();                                 // Close connection to database

            return ds;
        }

        /// <summary>
        /// Method for executing commands on database
        /// </summary>
        /// <param name="query">SQL query to send to the database</param>
        public static void SendNonQuery(string query)
        {
            dbCmd = new SQLiteCommand(query, dbConn);        // Make new dbCmd using supplied query

            dbConn.Open();                                  // Open connection to database
            dbCmd.ExecuteNonQuery();                        // Send the query to database
            System.Threading.Thread.Sleep(500);             // A quick sleep to ensure database is finished before closing connection
            dbConn.Close();                                 // Close connection to database
        }

        public static bool Contains(this DataTable dt, object value, string columnName)
        {
            List<object> elements = dt.AsEnumerable().Select(r => r.Field<object>(columnName)).ToList();

            return elements.Contains(value);
        }
    }
}
