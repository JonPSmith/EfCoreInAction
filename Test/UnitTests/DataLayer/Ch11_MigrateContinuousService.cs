// Copyright (c) 2016 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.Data.SqlClient;
using System.IO;
using test.EfHelpers;
using test.Helpers;
using Xunit;
using Xunit.Abstractions;
using Xunit.Extensions.AssertExtensions;

namespace test.UnitTests.DataLayer
{
    public class Ch11_MigrateContinuousService
    {
        private readonly ITestOutputHelper _output;

        public Ch11_MigrateContinuousService(ITestOutputHelper output)
        {
            _output = output;
        }

        private string GetChapter11ScriptFilePath(string searchPattern)
        {
            var directory = Path.Combine(TestFileHelpers.GetSolutionDirectory(),
                @"Test\Chapter11Listings\Scripts");
            var files = Directory.GetFiles(directory, searchPattern);
            if (files.Length != 1)
                throw new InvalidOperationException($"Could not find, or was ambiguous name - {searchPattern}");
            return files[0];
        }

        private string GetDatabaseName(string connectionString)
        {
            var builder = new SqlConnectionStringBuilder(connectionString);
            return builder.InitialCatalog;
        }

        [Fact]
        public void Test01CreateInitialDatabase()
        {
            Test01CreateInitialDatabase(this.GetUniqueDatabaseConnectionString(nameof(Test01CreateInitialDatabase)));
        }

        [Fact]
        public void Test02FirstMigrationCreateAddressesTable()
        {
            Test02FirstMigrationCreateAddressesTable(this.GetUniqueDatabaseConnectionString(nameof(Test02FirstMigrationCreateAddressesTable)));
        }

        [Fact]
        public void Test03SecondMigrationCopyData()
        {
            Test03SecondMigrationCopyData(this.GetUniqueDatabaseConnectionString(nameof(Test03SecondMigrationCopyData)));
        }

        private void Test01CreateInitialDatabase(string connectionString)
        {
            //SETUP
            connectionString.WipeCreateDatabase();
            var filePath = GetChapter11ScriptFilePath("Script01*.sql");

            //ATTEMPT
            connectionString.ExecuteScriptFileInTransaction(filePath);

            //VERIFY
            connectionString.ExecuteRowCount("sys.databases", $"WHERE [Name] = '{GetDatabaseName(connectionString)}'").ShouldEqual(1);
            connectionString.ExecuteRowCount("INFORMATION_SCHEMA.TABLES",
                $"WHERE TABLE_TYPE = 'BASE TABLE' AND TABLE_CATALOG='{GetDatabaseName(connectionString)}'").ShouldEqual(1);
        }


        private void Test02FirstMigrationCreateAddressesTable(string connectionString)
        {
            //SETUP
            Test01CreateInitialDatabase(connectionString);
            var filePath = GetChapter11ScriptFilePath("Script02*.sql");

            //ATTEMPT
            connectionString.ExecuteScriptFileInTransaction(filePath);

            //VERIFY
            connectionString.ExecuteRowCount("INFORMATION_SCHEMA.TABLES",
                $"WHERE TABLE_TYPE = 'BASE TABLE' AND TABLE_CATALOG='{GetDatabaseName(connectionString)}'").ShouldEqual(2);
        }


        private void Test03SecondMigrationCopyData(string connectionString)
        {
            //SETUP
            Test02FirstMigrationCreateAddressesTable(connectionString);
            var filePath = GetChapter11ScriptFilePath("Script03*.sql");

            //ATTEMPT
            connectionString.ExecuteScriptFileInTransaction(filePath);

            //VERIFY
            connectionString.ExecuteRowCount("INFORMATION_SCHEMA.TABLES",
                $"WHERE TABLE_TYPE = 'BASE TABLE' AND TABLE_CATALOG='{GetDatabaseName(connectionString)}'").ShouldEqual(2);
        }
    }
}
