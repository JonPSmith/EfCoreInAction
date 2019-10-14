// Copyright (c) 2017 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using test.EfHelpers;
using test.Helpers;
using Xunit;
using Xunit.Abstractions;
using Xunit.Extensions.AssertExtensions;

namespace test.UnitTests.Tests
{
    public class Ch02_SqlDatabaseHelpers
    {
        private readonly ITestOutputHelper _output;

        public Ch02_SqlDatabaseHelpers(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void TestGetAllMatchingDatabasesOk()
        {
            //SETUP
            var config = AppSettings.GetConfiguration();
            var connection = config.GetConnectionString(AppSettings.ConnectionStringName);
            var orgDbName = new SqlConnectionStringBuilder(connection).InitialCatalog;

            //ATTEMPT
            var databaseNames = orgDbName.GetAllMatchingDatabases();

            //VERIFY
            databaseNames.Count.ShouldBeInRange(1,1000);
            databaseNames.ForEach(x => x.StartsWith(orgDbName).ShouldBeTrue());
            _output.WriteLine("This found {0} databases.", databaseNames.Count);
        }
    }
}