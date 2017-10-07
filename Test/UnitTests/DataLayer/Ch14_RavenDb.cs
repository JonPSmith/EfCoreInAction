// Copyright (c) 2016 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.Linq;
using DataLayer.NoSql;
using Microsoft.Extensions.Configuration;
using Raven.Client;
using ServiceLayer.BookServices.RavenDb;
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
            var ravenDbTestConnection = AppSettings.GetConfiguration().GetConnectionString("RavenDb-Test");
            var storeFactory = new RavenStore(ravenDbTestConnection);
            return storeFactory.Store;
        });

        private IDocumentStore Store => LazyStore.Value;

        public Ch14_RavenDb(ITestOutputHelper output)
        {
            _output = output;
            
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
                var data = session.Query<BookNoSqlDto>().ToList();

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
                var data = session .Load<BookNoSqlDto>(BookNoSqlDto.ConvertIdToNoSqlId(1));

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
                var data = session.Query<BookNoSqlDto>().OrderByDescending(x => x.Id)
                    .ToList();

                //VERIFY
                var i = 10;
                data.ForEach(x => x.Id.ShouldEqual(BookNoSqlDto.ConvertIdToNoSqlId(i--)));
            }
        }

        [Fact]
        public void TestQueryWithIndexSortAndPage()
        {
            //SETUP
            using (var session = Store.OpenSession())
            {
                //ATTEMPT
                var data = session.Query<BookNoSqlDto>().OrderByDescending(x => x.Id)
                    .Skip(5)
                    .Take(2)
                    .ToList();

                //VERIFY
                data.Count.ShouldEqual(2);
                var i = 10 - 5;
                data.ForEach(x => x.Id.ShouldEqual(BookNoSqlDto.ConvertIdToNoSqlId(i--)));
            }
        }

        [Fact]
        public void TestQueryWithPriceSort()
        {
            //SETUP
            using (var session = Store.OpenSession())
            {
                //ATTEMPT
                var data = session.Query<BookNoSqlDto>().OrderByDescending(x => x.ActualPrice)
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
                var data = session.Query<BookNoSqlDto>().OrderBy(x => x.ActualPrice)
                    .Where(x => x.ReviewsAverageVotes > 2.75)
                    .ToList();

                //VERIFY
                data.Count.ShouldEqual(2);
                data.Select(x => x.Id).ShouldEqual(new []{BookNoSqlDto.ConvertIdToNoSqlId(6), BookNoSqlDto.ConvertIdToNoSqlId(10)});
            }
        }
    }


}