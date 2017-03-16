// Copyright (c) 2017 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using test.Helpers;

namespace test.EfHelpers
{
    public static class SqlDatabaseHelpers
    {
        //Note that there is no initial catalog
        private const string LocalHostConnectionString = "Server=(localdb)\\mssqllocaldb;Initial Catalog=;Trusted_Connection=True;";

        public static int DeleteAllUnitTestBranchDatabases()
        {
            var config = AppSettings.GetConfiguration();
            var builder = new SqlConnectionStringBuilder(config.GetConnectionString(AppSettings.ConnectionStringName));
            var orgDbName = builder.InitialCatalog;

            var gitBranchName = AppSettings.GetGitBranchName();

            var databaseNamesToDelete = GetAllMatchingDatabases($"{orgDbName}.{gitBranchName}");
            foreach (var databaseName in databaseNamesToDelete)
            {
                DeleteDatabase(databaseName);
            }
            return databaseNamesToDelete.Count;
        }

        public static List<string> GetAllMatchingDatabases(this string startsWith, 
            string connectionString = LocalHostConnectionString)
        {
            var result = new List<string>();

            using (var myConn = new SqlConnection(connectionString))
            {
                var command = $"SELECT name FROM master.dbo.sysdatabases WHERE name LIKE '{startsWith}%'";
                var myCommand = new SqlCommand(command, myConn);
                myConn.Open();
                using (var reader = myCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        result.Add(reader.GetString(0));
                    }
                }
            }
            return result;
        }

        //-------------------------------------------------------------------
        //private methods

        private static void DeleteDatabase(string databaseName)
        {
            LocalHostConnectionString.ExecuteNonQuery("DROP DATABASE [" + databaseName + "]");
            if (LocalHostConnectionString.ExecuteRowCount("sys.databases", $"WHERE [Name] = '{databaseName}'") == 1)
                //it failed
                throw new InvalidOperationException($"Failed to deleted {databaseName}. Did you have SSMS open or something?");         
        }

        private static int ExecuteNonQuery(this string connection, string command)
        {
            using (var myConn = new SqlConnection(connection))
            {
                var myCommand = new SqlCommand(command, myConn);
                myConn.Open();
                return myCommand.ExecuteNonQuery();
            }
        }

        private static int ExecuteRowCount(this string connectionString, string tableName, string whereClause = "")
        {
            using (var myConn = new SqlConnection(connectionString))
            {
                var command = "SELECT COUNT(*) FROM " + tableName + " " + whereClause;
                var myCommand = new SqlCommand(command, myConn);
                myConn.Open();
                return (int) myCommand.ExecuteScalar();
            }
        }

    }
}