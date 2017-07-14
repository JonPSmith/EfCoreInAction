// Copyright (c) 2017 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Text.RegularExpressions;
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
        public static int ExecuteRowCount(this string connectionString, string tableName, string whereClause = "")
        {
            using (var myConn = new SqlConnection(connectionString))
            {
                var command = "SELECT COUNT(*) FROM " + tableName + " " + whereClause;
                var myCommand = new SqlCommand(command, myConn);
                myConn.Open();
                return (int) myCommand.ExecuteScalar();
            }
        }

        /// <summary>
        /// Wipes out the existing database and creates a new, empty one
        /// </summary>
        /// <param name="databaseConnectionString">a actual connection string</param>
        /// <param name="timeout">Defines a timeout for connection and the SQL DELETE/CREATE database commands</param>
        public static void WipeCreateDatabase(this string databaseConnectionString, int timeout = 30)
        {
            var builder = new SqlConnectionStringBuilder(databaseConnectionString);
            var databaseName = builder.InitialCatalog;
            builder.InitialCatalog = "";          //remove database, as create database won't work with it
            builder.ConnectTimeout = timeout;

            var nonDbConnectionString = builder.ToString();
            if (nonDbConnectionString.ExecuteRowCount("sys.databases", String.Format("WHERE [Name] = '{0}'", databaseName)) == 1)
            {
                DeleteDatabase(databaseName);
            }
            nonDbConnectionString.ExecuteNonQuery("CREATE DATABASE [" + databaseName + "]", timeout);
        }

        public static void ExecuteScriptFileInTransaction(this string connectionString, string filePath)
        {
            var scriptContent = File.ReadAllText(filePath);
            Regex regex = new Regex("^GO", RegexOptions.IgnoreCase | RegexOptions.Multiline);
            string[] lines = regex.Split(scriptContent);

            using (var sqlConnection = new SqlConnection(connectionString))
            {
                sqlConnection.Open();
                SqlTransaction transaction = sqlConnection.BeginTransaction();
                using (SqlCommand cmd = sqlConnection.CreateCommand())
                {
                    cmd.Connection = sqlConnection;
                    cmd.Transaction = transaction;

                    foreach (string line in lines)
                    {
                        if (line.Length > 0)
                        {
                            cmd.CommandText = line;
                            cmd.CommandType = CommandType.Text;

                            try
                            {
                                cmd.ExecuteNonQuery();
                            }
                            catch (SqlException)
                            {
                                transaction.Rollback();
                                throw;
                            }
                        }
                    }
                }
                transaction.Commit();
            }
        }


        //-------------------------------------------------------------------
        //private methods

        private static void DeleteDatabase(string databaseName)
        {
            if (LocalHostConnectionString.ExecuteRowCount("sys.databases", $"WHERE [Name] = '{databaseName}'") == 1)
                LocalHostConnectionString.ExecuteNonQuery("DROP DATABASE [" + databaseName + "]");
            if (LocalHostConnectionString.ExecuteRowCount("sys.databases", $"WHERE [Name] = '{databaseName}'") == 1)
                //it failed
                throw new InvalidOperationException($"Failed to deleted {databaseName}. Did you have SSMS open or something?");         
        }

        private static int ExecuteNonQuery(this string connection, string command, int commandTimeout = 10)
        {
            using (var myConn = new SqlConnection(connection))
            {
                var myCommand = new SqlCommand(command, myConn) {CommandTimeout = commandTimeout};
                myConn.Open();
                return myCommand.ExecuteNonQuery();
            }
        }
    }
}