// Copyright (c) 2016 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.Data.SqlClient;
using System.IO;
using System.Threading;
using test.Attributes;
using test.EfHelpers;
using test.Helpers;
using Xunit;
using Xunit.Abstractions;
using Xunit.Extensions.AssertExtensions;

namespace test.UnitTests.DataLayer
{
    public class Ch11_MigrateContinuousService
    {
        private readonly string _connectionString;
        private readonly string _databaseName;

        private readonly ITestOutputHelper _output;

        public Ch11_MigrateContinuousService(ITestOutputHelper output)
        {
            _output = output;
            _connectionString = this.GetUniqueDatabaseConnectionString();
            var builder = new SqlConnectionStringBuilder(_connectionString);
            _databaseName = builder.InitialCatalog;

            _connectionString.WipeCreateDatabase();
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

        [Fact]
        public void Test01CreateInitialDatabase()
        {
            //SETUP
            var filePath = GetChapter11ScriptFilePath("Script01*.sql");

            //ATTEMPT
            _connectionString.ExecuteScriptFileInTransaction(filePath);

            //VERIFY
            _connectionString.ExecuteRowCount("sys.databases", $"WHERE [Name] = '{_databaseName}'").ShouldEqual(1);
            _connectionString.ExecuteRowCount("INFORMATION_SCHEMA.TABLES",
                $"WHERE TABLE_TYPE = 'BASE TABLE' AND TABLE_CATALOG='{_databaseName}'").ShouldEqual(1);

            Thread.Sleep(100);      //
        }

        [Fact]
        public void Test02FirstMigrationCreateAddressesTable()
        {
            //SETUP
            Test01CreateInitialDatabase(2);
            var filePath = GetChapter11ScriptFilePath("Script02*.sql");

            //ATTEMPT
            _connectionString.ExecuteScriptFileInTransaction(filePath);

            //VERIFY
            _connectionString.ExecuteRowCount("INFORMATION_SCHEMA.TABLES",
                $"WHERE TABLE_TYPE = 'BASE TABLE' AND TABLE_CATALOG='{_databaseName}'").ShouldEqual(2);

            Thread.Sleep(100);      //
        }

        [Fact]
        public void Test03SecondMigrationCopyData()
        {
            //SETUP
            Test02FirstMigrationCreateAddressesTable();
            var filePath = GetChapter11ScriptFilePath("Script03*.sql");

            //ATTEMPT
            _connectionString.ExecuteScriptFileInTransaction(filePath);

            //VERIFY
            _connectionString.ExecuteRowCount("INFORMATION_SCHEMA.TABLES",
                $"WHERE TABLE_TYPE = 'BASE TABLE' AND TABLE_CATALOG='{_databaseName}'").ShouldEqual(2);

            Thread.Sleep(100);      //
        }
    }
}
