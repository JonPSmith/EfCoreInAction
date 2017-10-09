// Copyright (c) 2016 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.Linq;
using DataNoSql;
using Raven.Client;
using test.Helpers;
using Xunit;
using Xunit.Abstractions;
using Xunit.Extensions.AssertExtensions;

namespace test.UnitTests.DataLayer
{
    public class Ch14_RavenDb
    {
        private readonly ITestOutputHelper _output;

        private static readonly Lazy<IDocumentStore> LazyStore = new Lazy<IDocumentStore>(() =>
        {
            //var ravenDbTestConnection = AppSettings.GetConfiguration().GetConnectionString("RavenDb-Test");
            var ravenDbTestConnection = AppSettings.GetConfiguration()["RavenDb-Unit-Test"];
            if (string.IsNullOrEmpty( ravenDbTestConnection ))
                throw new InvalidOperationException("You need a RavenDb database host to run these tests." +
                                                    " You can get a free RavenDb database at http://www.ravenhq.com/");
            var storeFactory = new RavenStore(ravenDbTestConnection);
            return storeFactory.Store;
        });

        private IDocumentStore Store => LazyStore.Value;

        public Ch14_RavenDb(ITestOutputHelper output)
        {
            _output = output;
            if (Store.NumEntriesInDb() <= 0)
            {
                Store.SeedDummyBooks();
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
            var count = Store.NumEntriesInDb();

            //VERIFY
            count.ShouldEqual(10);
        }

        [Fact]
        public void TestQuery()
        {
            //SETUP
            using (var session = Store.OpenSession())
            {
                //ATTEMPT
                var data = session.Query<BookListNoSql>().ToList();

                //VERIFY
                data.Count.ShouldEqual(10);
            }
        }

        [Fact]
        public void TestQueryWithFilter()
        {
            //SETUP
            using (var session = Store.OpenSession())
            {
                //ATTEMPT
                var data = session .Load<BookListNoSql>(BookListNoSql.ConvertIdToNoSqlId(1));

                //VERIFY
                data.ShouldNotBeNull();
                data.Title.ShouldEqual("Book0000 Title");
            }
        }

        [Fact]
        public void TestQueryWithIndexSort()
        {
            //SETUP
            using (var session = Store.OpenSession())
            {
                //ATTEMPT
                var data = session.Query<BookListNoSql>().OrderByDescending(x => x.Id)
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
            using (var session = Store.OpenSession())
            {
                //ATTEMPT
                var data = session.Query<BookListNoSql>().OrderByDescending(x => x.Id)
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
            using (var session = Store.OpenSession())
            {
                //ATTEMPT
                var data = session.Query<BookListNoSql>().OrderByDescending(x => x.ActualPrice)
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
            using (var session = Store.OpenSession())
            {
                //ATTEMPT
                var data = session.Query<BookListNoSql>().OrderBy(x => x.ActualPrice)
                    .Where(x => x.ReviewsAverageVotes > 2.75)
                    .ToList();

                //VERIFY
                data.Count.ShouldEqual(2);
                data.Select(x => x.Id).ShouldEqual(new []{BookListNoSql.ConvertIdToNoSqlId(6), BookListNoSql.ConvertIdToNoSqlId(10)});
            }
        }
    }


}