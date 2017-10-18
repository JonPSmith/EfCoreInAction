// Copyright (c) 2016 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using DataNoSql;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using test.Helpers;
using test.Mocks;
using Xunit;
using Xunit.Abstractions;
using Xunit.Extensions.AssertExtensions;

namespace test.UnitTests.DataLayer
{
    public class Ch14_INoSqlQuery
    {
        private readonly ITestOutputHelper _output;
        private static List<string> _logList = new List<string>();
        private static ILogger _logger = new StandInLogger(_logList);

        private static readonly Lazy<RavenStore> LazyStore = new Lazy<RavenStore>(() =>
        {
            var ravenDbTestConnection = AppSettings.GetConfiguration().GetConnectionString("RavenDb-Test");
            if (string.IsNullOrEmpty( ravenDbTestConnection ))
                throw new InvalidOperationException("You need a connection string in the test's appsetting.json file.");
            var storeFactory = new RavenStore(ravenDbTestConnection, _logger);
            return storeFactory;
        });

        private RavenStore StoreFactory => LazyStore.Value;

        public Ch14_INoSqlQuery(ITestOutputHelper output)
        {
            _output = output;
            if (StoreFactory.NumEntriesInDb() <= 0)
            {
                StoreFactory.SeedDummyBooks();
            }
        }


        [Fact]
        public void TestCreateUseCreateNoSqlAccessor()
        {
            //SETUP
            _logList.Clear();

            //ATTEMPT
            using (var context = StoreFactory.CreateNoSqlAccessor())
            {
                context.Command = "Count";
                var count = context.BookListQuery().Count();

                //VERIFY
                count.ShouldEqual(10);
            }
            _logList.Count.ShouldEqual(1);
            _logList.First().StartsWith("Information: Raven Command. Execute time =").ShouldBeTrue();
            _logList.First().EndsWith("\nCount").ShouldBeTrue();
        }

        
    }
}