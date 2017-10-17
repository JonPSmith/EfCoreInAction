// Copyright (c) 2016 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using DataNoSql;
using Microsoft.Extensions.Configuration;
using Raven.Client;
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
        private static List<string> _logList;

        private static readonly Lazy<IRavenStore> LazyStore = new Lazy<IRavenStore>(() =>
        {
            var ravenDbTestConnection = AppSettings.GetConfiguration().GetConnectionString("RavenDb-Test");
            if (string.IsNullOrEmpty( ravenDbTestConnection ))
                throw new InvalidOperationException("You need a connection string in the test's appsetting.json file.");
            var storeFactory = new RavenStore(ravenDbTestConnection);
            return storeFactory;
        });

        private IRavenStore StoreFactory => LazyStore.Value;

        public Ch14_INoSqlQuery(ITestOutputHelper output)
        {
            _output = output;
            if (StoreFactory.Store.NumEntriesInDb() <= 0)
            {
                StoreFactory.Store.SeedDummyBooks();
            }
        }


        [Fact]
        public void TestCreateUseCreateNoSqlAccessor()
        {
            //SETUP
            var logs = new List<string>();
            var logger = new StandInLogger(logs);

            //ATTEMPT
            using (var context = StoreFactory.CreateNoSqlAccessor(logger))
            {
                context.Command = "Count";
                var count = context.BookListQuery().Count();

                //VERIFY
                count.ShouldEqual(10);
            }
            logs.Count.ShouldEqual(1);
            logs.First().StartsWith("Information: Raven Command. Execute time =").ShouldBeTrue();
            logs.First().EndsWith("\nCount").ShouldBeTrue();
        }

        
    }
}