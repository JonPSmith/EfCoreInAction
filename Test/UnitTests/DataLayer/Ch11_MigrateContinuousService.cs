// Copyright (c) 2016 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using test.EfHelpers;
using test.Helpers;
using Test.Chapter11Listings.EfCode;
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

        //NOTE: I thought I could run these on the same database, but I got a timeout.
        //I *think* the problem is an open connection, but I couldn't find it.

        [Fact]
        public void Test01CreateInitialDatabase()
        {
            Test01CreateInitialDatabase(this.GetUniqueDatabaseConnectionString(nameof(Test01CreateInitialDatabase)));
        }

        [Fact]
        public void Test02FirstMigrationCreateTablesAndStoredProc()
        {
            Test02FirstMigrationCreateTablesAndStoredProc(this.GetUniqueDatabaseConnectionString(nameof(Test02FirstMigrationCreateTablesAndStoredProc)));
        }

        [Fact]
        public void Test02AInterimCodeReadData()
        {
            //SETUP
            var connection = this.GetUniqueDatabaseConnectionString(nameof(Test02AInterimCodeReadData));
            Test02FirstMigrationCreateTablesAndStoredProc(connection);

            var optionsBuilder =
                new DbContextOptionsBuilder<Chapter11ContinuousInterimDb>();
            optionsBuilder.UseSqlServer(connection);
            using (var context = new Chapter11ContinuousInterimDb(optionsBuilder.Options))
            {
                //ATTEMPT
                var orgData = context.CustomerAndAddresses.ToList();

                //VERIFY         
                orgData.Select(x => x.Name).ShouldEqual(new []{ "John", "Jane", "Mid-migrate name" });
            }
        }

        [Fact]
        public void Test02BInterimCodeWriteData()
        {
            //SETUP
            var connection = this.GetUniqueDatabaseConnectionString(nameof(Test02BInterimCodeWriteData));
            Test02FirstMigrationCreateTablesAndStoredProc(connection);

            var optionsBuilder =
                new DbContextOptionsBuilder<Chapter11ContinuousInterimDb>();
            optionsBuilder.UseSqlServer(connection);
            using (var context = new Chapter11ContinuousInterimDb(optionsBuilder.Options))
            {
                //ATTEMPT
                var entity = context.CustomerAndAddresses.FromSql(
                    "EXECUTE InterimCustomerAndAddressUpdate {0}, {1}",
                    "EF mid-migate name", "EF mid-migrate address").Single();

                //VERIFY
                entity.Id.ShouldNotEqual(0);
                var orgData = context.CustomerAndAddresses.ToList();
                orgData.Select(x => x.Name).ShouldEqual(new[] { "John", "Jane", "Mid-migrate name", "EF mid-migate name" });
            }
        }

        [Fact]
        public void Test03SecondMigrationCopyData()
        {
            Test03SecondMigrationCopyData(this.GetUniqueDatabaseConnectionString(nameof(Test03SecondMigrationCopyData)));
        }

        [Fact]
        public void Test03AFinalCodeReadData()
        {
            //SETUP
            var connection = this.GetUniqueDatabaseConnectionString(nameof(Test03AFinalCodeReadData));
            Test03SecondMigrationCopyData(connection);

            var optionsBuilder =
                new DbContextOptionsBuilder<Chapter11ContinuousFinalDb>();
            optionsBuilder.UseSqlServer(connection);
            using (var context = new Chapter11ContinuousFinalDb(optionsBuilder.Options))
            {
                //ATTEMPT
                var orgNames = context.Customers.Select(x => x.Name).ToList();

                //VERIFY         
                orgNames.ShouldEqual(new List<string> { "Mid-migrate name", "John", "Jane" });
            }
        }

        [Fact]
        public void Test04FinalMigrationTidyUp()
        {
            Test04FinalMigrationTidyUp(this.GetUniqueDatabaseConnectionString(nameof(Test04FinalMigrationTidyUp)));
        }

        private void Test01CreateInitialDatabase(string connectionString)
        {
            //SETUP
            connectionString.WipeCreateDatabase();
            var filePath = GetChapter11ScriptFilePath("Script00*.sql");

            //ATTEMPT
            connectionString.ExecuteScriptFileInTransaction(filePath);

            //VERIFY
            connectionString.ExecuteRowCount("sys.databases", $"WHERE [Name] = '{GetDatabaseName(connectionString)}'").ShouldEqual(1);
            connectionString.ExecuteRowCount("INFORMATION_SCHEMA.TABLES",
                $"WHERE TABLE_TYPE = 'BASE TABLE' AND TABLE_CATALOG='{GetDatabaseName(connectionString)}'").ShouldEqual(1);
        }


        private void Test02FirstMigrationCreateTablesAndStoredProc(string connectionString)
        {
            //SETUP
            Test01CreateInitialDatabase(connectionString);
            var filePath = GetChapter11ScriptFilePath("Script01*.sql");

            //ATTEMPT
            connectionString.ExecuteScriptFileInTransaction(filePath);

            //VERIFY
            connectionString.ExecuteRowCount("INFORMATION_SCHEMA.TABLES",
                $"WHERE TABLE_TYPE = 'BASE TABLE' AND TABLE_CATALOG='{GetDatabaseName(connectionString)}'").ShouldEqual(3);
        }

        private void Test03SecondMigrationCopyData(string connectionString)
        {
            //SETUP
            Test02FirstMigrationCreateTablesAndStoredProc(connectionString);
            var filePath = GetChapter11ScriptFilePath("Script02*.sql");

            //ATTEMPT
            connectionString.ExecuteScriptFileInTransaction(filePath);

            //VERIFY
            connectionString.ExecuteRowCount("[dbo].[Addresses]").ShouldEqual(3);
        }

        private void Test04FinalMigrationTidyUp(string connectionString)
        {
            //SETUP
            Test03SecondMigrationCopyData(connectionString);
            var filePath = GetChapter11ScriptFilePath("Script03*.sql");

            //ATTEMPT
            connectionString.ExecuteScriptFileInTransaction(filePath);

            //VERIFY
            connectionString.ExecuteRowCount("INFORMATION_SCHEMA.TABLES",
                $"WHERE TABLE_TYPE = 'BASE TABLE' AND TABLE_CATALOG='{GetDatabaseName(connectionString)}'").ShouldEqual(2);
        }
    }
}
