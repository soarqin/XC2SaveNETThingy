using System;
using System.IO;
using System.Text;

namespace XC2BdatImporter
{
    internal static class VerifySelfTests
    {
        internal static void Run()
        {
            string directory = TemporaryExtractionDirectory.Create();
            try
            {
                string ddl = Path.Combine(directory, "Demo.sql");
                string csv = Path.Combine(directory, "Demo.csv");
                string database = Path.Combine(directory, "Demo.db");
                File.WriteAllText(ddl, "DROP TABLE IF EXISTS [Demo];\nCREATE TABLE [Demo] ([ID] INTEGER PRIMARY KEY, [Name] text NOT NULL);\n", Encoding.UTF8);
                File.WriteAllText(csv, "ID\tName\r\n1\tOne\r\n", Encoding.Unicode);
                DatabaseBuilder.Build(ddl, directory, database);
                VerificationResult ok = Verifier.Verify(database, directory, 0);
                if (!ok.Passed) throw new InvalidOperationException("Expected valid database: " + string.Join("; ", ok.Errors));

                File.WriteAllText(csv, "ID\tName\r\n1\tOne\r\n2\tTwo\r\n", Encoding.Unicode);
                VerificationResult mismatch = Verifier.Verify(database, directory, 0);
                if (mismatch.Passed) throw new InvalidOperationException("Expected CSV/SQLite mismatch to fail.");
            }
            finally
            {
                TemporaryExtractionDirectory.DeleteExact(directory);
            }
        }
    }
}
