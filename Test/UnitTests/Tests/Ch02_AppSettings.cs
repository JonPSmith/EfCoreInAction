// Copyright (c) 2016 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using test.Helpers;
using Xunit;
using Xunit.Extensions.AssertExtensions;

namespace test.UnitTests.Tests
{
    public class Ch02_AppSettings
    {
        [Fact]
        public void GetConfigurationOk()
        {
            //SETUP

            //ATTEMPT
            var config = AppSettings.GetConfiguration();

            //VERIFY
            config.GetConnectionString(AppSettings.ConnectionStringName)
                .ShouldEqual("Server=(localdb)\\mssqllocaldb;Database=Test.EfCoreInActionDb;Trusted_Connection=True;MultipleActiveResultSets=true");
        }

        [Fact]
        public void GetTestConnectionStringOk()
        {
            //SETUP
            var config = AppSettings.GetConfiguration();
            var orgDbName = new SqlConnectionStringBuilder(config.GetConnectionString(AppSettings.ConnectionStringName)).InitialCatalog;

            //ATTEMPT
            var con = this.GetUniqueDatabaseConnectionString();

            //VERIFY
            var newDatabaseName = new SqlConnectionStringBuilder(con).InitialCatalog;
            Assert.StartsWith($"{orgDbName}.", newDatabaseName);
            Assert.EndsWith($".{typeof(Ch02_AppSettings).Name}", newDatabaseName);
        }


        [Fact]
        public void GetTestConnectionStringWithExtraMethodNameOk()
        {
            //SETUP
            var config = AppSettings.GetConfiguration();
            var orgDbName = new SqlConnectionStringBuilder(config.GetConnectionString(AppSettings.ConnectionStringName)).InitialCatalog;

            //ATTEMPT
            var con = this.GetUniqueDatabaseConnectionString("ExtraMethodName");

            //VERIFY
            var newDatabaseName = new SqlConnectionStringBuilder(con).InitialCatalog;
            Assert.StartsWith($"{orgDbName}.", newDatabaseName);
            Assert.EndsWith($".{typeof(Ch02_AppSettings).Name}.ExtraMethodName", newDatabaseName);
        }
    }
}