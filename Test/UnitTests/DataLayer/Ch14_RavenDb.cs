// Copyright (c) 2016 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.Linq;
using DataLayer.NoSql;
using Raven.Client.Documents;
using Raven.Client.Documents.Indexes;
using test.Helpers;
using Xunit;
using Xunit.Abstractions;
using Xunit.Extensions.AssertExtensions;

namespace test.UnitTests.DataLayer
{
    public class Ch14_RavenDb
    {
        private readonly ITestOutputHelper _output;

        private const string DatabaseName = "EfCoreInAction-Ch14-RavenDb";

        private static readonly Lazy<IDocumentStore> theDocStore = new Lazy<IDocumentStore>(() =>
        {
            var store = new DocumentStore
            {
                Urls = new[] { RavenDbHelpers.RavenDbTestServerUrl },
                Database = DatabaseName
            }.Initialize();

            new BookById().Execute(store);
            new BookByActualPrice().Execute(store);
            new BookByVotes().Execute(store);
            if (store.NumEntriesInDb() <= 0)
            {
                store.SeedDummyBooks();
            }
            return store;
        });

        private IDocumentStore Store
        {
            get { return theDocStore.Value; }
        }

        public Ch14_RavenDb(ITestOutputHelper output)
        {
            _output = output;
            
        }

        private class BookById : AbstractIndexCreationTask<BookNoSqlDto>
        {
            public BookById()
            {
                Map = books => from book in books
                    select new { book.Id };
                Indexes.Add(x => x.Id, FieldIndexing.Exact);
            }
        }

        private class BookByActualPrice : AbstractIndexCreationTask<BookNoSqlDto>
        {
            public BookByActualPrice()
            {
                Map = books => from book in books
                    select new { book.ActualPrice };
                Indexes.Add(x => x.Id, FieldIndexing.Default);
            }
        }

        private class BookByVotes : AbstractIndexCreationTask<BookNoSqlDto>
        {
            public BookByVotes()
            {
                Map = books => from book in books
                    select new { book.ReviewsAverageVotes };
                Indexes.Add(x => x.Id, FieldIndexing.Default);
            }
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
                var data = session.Query<BookNoSqlDto>()
                    .Where(x => x.Id == BookNoSqlDto.ConvertIdToRavenId(1))
                    .ToList();

                //VERIFY
                data.Count.ShouldEqual(1);
                data.First().Title.ShouldEqual("Book0000 Title");
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
                data.ForEach(x => x.Id.ShouldEqual(BookNoSqlDto.ConvertIdToRavenId(i--)));
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
                data.Select(x => x.Id).ShouldEqual(new []{BookNoSqlDto.ConvertIdToRavenId(6), BookNoSqlDto.ConvertIdToRavenId(10)});
            }
        }
    }


}