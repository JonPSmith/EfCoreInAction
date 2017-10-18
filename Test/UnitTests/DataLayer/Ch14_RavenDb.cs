// Copyright (c) 2016 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using DataNoSql;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Raven.Client;
using test.Helpers;
using test.Mocks;
using Xunit;
using Xunit.Abstractions;
using Xunit.Extensions.AssertExtensions;

namespace test.UnitTests.DataLayer
{
    public class Ch14_RavenDb
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

        public Ch14_RavenDb(ITestOutputHelper output)
        {
            _output = output;
            if (StoreFactory.NumEntriesInDb() <= 0)
            {
                StoreFactory.SeedDummyBooks();
            }
        }

        [Fact]
        public void CheckRavenDbHelpersCreateDummyBooks()
        {
            //SETUP

            //ATTEMPT
            var books = RavenDbHelpers.CreateDummyBooks().ToList();

            //VERIFY
            books.Count().ShouldEqual(10);
            books.First().AuthorsOrdered.ShouldEqual("Author0000, CommonAuthor");
        }

        [Fact]
        public void TestAccessDatabase()
        {
            //SETUP

            //ATTEMPT
            var count = StoreFactory.NumEntriesInDb();

            //VERIFY
            count.ShouldEqual(10);
        }

        [Fact]
        public void TestQuery()
        {
            //SETUP
            using (var context = StoreFactory.CreateNoSqlAccessor())
            {
                //ATTEMPT
                var data = context.BookListQuery().ToList();

                //VERIFY
                data.Count.ShouldEqual(10);
            }
        }


        [Fact]
        public void TestQueryWithIndexSort()
        {
            //SETUP
            using (var context = StoreFactory.CreateNoSqlAccessor())
            {
                //ATTEMPT
                var data = context.BookListQuery().OrderByDescending(x => x.Id)
                    .ToList();

                //VERIFY
                var i = 10;
                data.ForEach(x => x.Id.ShouldEqual(BookListNoSql.ConvertIdToNoSqlId(i--)));
            }
        }

        [Fact]
        public void TestQueryWithIndexSortAndPage()
        {
            //SETUP
            using (var context = StoreFactory.CreateNoSqlAccessor())
            {
                //ATTEMPT
                var data = context.BookListQuery().OrderByDescending(x => x.Id)
                    .Skip(5)
                    .Take(2)
                    .ToList();

                //VERIFY
                data.Count.ShouldEqual(2);
                var i = 10 - 5;
                data.ForEach(x => x.Id.ShouldEqual(BookListNoSql.ConvertIdToNoSqlId(i--)));
            }
        }

        [Fact]
        public void TestQueryWithPriceSort()
        {
            //SETUP
            using (var context = StoreFactory.CreateNoSqlAccessor())
            {
                //ATTEMPT
                var data = context.BookListQuery().OrderByDescending(x => x.ActualPrice)
                    .ToList();

                //VERIFY
                var i = 10;
                data.ForEach(x => x.ActualPrice.ShouldEqual(i--));
            }
        }

        [Fact]
        public void TestQuerySortAndFilter()
        {
            //SETUP
            using (var context = StoreFactory.CreateNoSqlAccessor())
            {
                //ATTEMPT
                var data = context.BookListQuery().OrderBy(x => x.ActualPrice)
                    .Where(x => x.ReviewsAverageVotes > 2.75)
                    .ToList();

                //VERIFY
                data.Count.ShouldEqual(2);
                data.Select(x => x.Id).ShouldEqual(new []{BookListNoSql.ConvertIdToNoSqlId(6), BookListNoSql.ConvertIdToNoSqlId(10)});
            }
        }
    }


}