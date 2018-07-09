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

        public static DataTable DriverStatus()
        {
            return GetDataSet("SELECT * FROM DriverStatus").Tables[0];
        }

        public static DataTable DriverBladeSetStatus()
        {
            return GetDataSet("SELECT * FROM DriverBladeSetStatus").Tables[0];
        }

        public static DataTable DriverArts()
        {
            return GetDataSet("SELECT * FROM viewDriverArts").Tables[0];
        }

        public static DataTable Weapons()
        {
            return GetDataSet("SELECT * FROM Weapons").Tables[0];
        }

        public static DataTable DriverSkills()
        {
            return GetDataSet("SELECT * FROM DriverSkills").Tables[0];
        }

        public static DataTable ItemTypes()
        {
            return GetDataSet("SELECT * FROM ItemTypes").Tables[0];
        }

        public static DataTable GetCoreChips()
        {
            string query =
                "SELECT ID, Name FROM Items WHERE ItemTypeID = 0 " +
                "UNION ALL " +
                "SELECT REPLACE(ID, ID, ID -10000) AS ID, " +
                "REPLACE(ID, ID, ID - 10000) || ': ' || NAME AS Name " +
                "FROM Items WHERE ItemTypeID = 1";

            return GetDataSet(query).Tables[0];
        }

        public static DataTable GetCollectibles()
        {
            string query =
                "SELECT ID, Name FROM Items WHERE ItemTypeID = 0 " +
                "UNION ALL " +
                "SELECT REPLACE(ID, ID, ID -30000) AS ID, " +
                "REPLACE(ID, ID, ID - 30000) || ': ' || NAME AS Name " +
                "FROM Items WHERE ItemTypeID = 7";

            return GetDataSet(query).Tables[0];
        }

        public static DataTable GetCoreCrystals()
        {
            string query =
                "SELECT ID, Name FROM Items WHERE ItemTypeID = 0 " +
                "UNION ALL " +
                "SELECT REPLACE(ID, ID, ID -45000) AS ID, " +
                "REPLACE(ID, ID, ID - 45000) || ': ' || NAME AS Name " +
                "FROM Items WHERE ItemTypeID = 11";

            return GetDataSet(query).Tables[0];
        }

        public static DataTable GetBoosters()
        {
            string query =
                "SELECT ID, Name FROM Items WHERE ItemTypeID = 0 " +
                "UNION ALL " +
                "SELECT REPLACE(ID, ID, ID -50000) AS ID, " +
                "REPLACE(ID, ID, ID - 50000) || ': ' || NAME AS Name " +
                "FROM Items WHERE ItemTypeID = 12";

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
