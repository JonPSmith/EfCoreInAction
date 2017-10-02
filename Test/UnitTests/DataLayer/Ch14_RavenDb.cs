// Copyright (c) 2016 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System.Linq;
using DataLayer.NoSql;
using Raven.Client;
using Raven.Client.Documents;
using Raven.Client.Documents.Indexes;
using Raven.Client.Documents.Session;
using test.Helpers;
using Xunit;
using Xunit.Abstractions;
using Xunit.Extensions.AssertExtensions;

namespace test.UnitTests.DataLayer
{
    public class Ch14_RavenDb
    {
        private readonly ITestOutputHelper _output;
        private readonly IDocumentStore _store;

        private const string DatabaseName = "EfCoreInAction-Ch14-RavenDb";

        public Ch14_RavenDb(ITestOutputHelper output)
        {
            _output = output;
            _store = new DocumentStore
            {
                Urls = new []{ RavenDbHelpers.RavenDbTestServerUrl},
                Database = DatabaseName
            }.Initialize();
            new BookById().Execute(_store);
            if (_store.NumEntriesInDb() <= 0)
            {
                _store.SeedDummyBooks();
            }
        }

        [Fact]
        public void TestAccessDatabase()
        {
            //SETUP

            //ATTEMPT
            var count = _store.NumEntriesInDb();

            //VERIFY
            count.ShouldEqual(10);
        }

        [Fact]
        public void TestQuery()
        {
            //SETUP
            using (var session = _store.OpenSession())
            {
                //ATTEMPT
                var data = session.Query<BookNoSqlDto>().ToList();

                //VERIFY
                data.Count.ShouldEqual(10);
            }
        }

        private class BookById : AbstractIndexCreationTask<BookNoSqlDto>
        {
            public BookById()
            {
                Map = books => from book in books
                               select new {book.Id};
                Indexes.Add(x => x.Id, FieldIndexing.Exact);
            }
        }

        [Fact]
        public void TestQueryWithFilter()
        {
            //SETUP
            using (var session = _store.OpenSession())
            {
                //ATTEMPT
                var data = session.Advanced.DocumentQuery<BookNoSqlDto, BookById>()
                    .WhereEquals(x => x.Id, 1.ToString("D10"))
                    .ToList();

                //VERIFY
                data.Count.ShouldEqual(1);
            }
        }

        [Fact]
        public void TestQueryWithSort()
        {
            //SETUP
            using (var session = _store.OpenSession())
            {
                //ATTEMPT
                var data = session.Query<BookNoSqlDto>().OrderByDescending(x => x.Id)
                    .ToList();

                //VERIFY
                var i = 10;
                data.ForEach(x => x.Id.ShouldEqual((i--).ToString("D10")));
            }
        }
    }


}