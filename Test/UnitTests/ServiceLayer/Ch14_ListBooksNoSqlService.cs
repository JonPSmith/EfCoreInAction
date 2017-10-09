// Copyright (c) 2016 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.Linq;
using DataNoSql;
using ServiceLayer.BookServices.Concrete;
using ServiceLayer.BookServices.RavenDb;
using test.Helpers;
using Xunit;
using Xunit.Abstractions;
using Xunit.Extensions.AssertExtensions;

namespace Test.UnitTests.ServiceLayer
{
    public class Ch14_ListBooksNoSqlService
    {
        private readonly ITestOutputHelper _output;

        private static readonly Lazy<IRavenStore> LazyStoreFactory = new Lazy<IRavenStore>(() =>
        {
            //var ravenDbTestConnection = AppSettings.GetConfiguration().GetConnectionString("RavenDb-Test");
            var ravenDbTestConnection = AppSettings.GetConfiguration()["RavenDb-Unit-Test"];
            if (string.IsNullOrEmpty( ravenDbTestConnection ))
                throw new InvalidOperationException("You need a RavenDb database host to run these tests." +
                                                    " You can get a free RavenDb database at http://www.ravenhq.com/");
            var storeFactory = new RavenStore(ravenDbTestConnection);
            return storeFactory;
        });

        private IRavenStore StoreFactory => LazyStoreFactory.Value;

        private int _numEntries;
        public Ch14_ListBooksNoSqlService(ITestOutputHelper output)
        {
            _output = output;
            _numEntries = StoreFactory.Store.NumEntriesInDb();
            if (_numEntries <= 0)
            {
                StoreFactory.Store.SeedDummyBooks();
                _numEntries = StoreFactory.Store.NumEntriesInDb();
            }
        }

        [Fact]
        public void TestDefaultSettings()
        {
            //SETUP
            var service = new ListBooksNoSqlService(StoreFactory);
            var options = new NoSqlSortFilterPageOptions();

            //ATTEMPT
            var books = service.SortFilterPage(options).ToList();

            //VERIFY
            books.Count.ShouldEqual(_numEntries);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        public void TestPagingFirstPage(int pageNum)
        {
            //SETUP
            var service = new ListBooksNoSqlService(StoreFactory);
            var options = new NoSqlSortFilterPageOptions();

            //ATTEMPT
            options.PageSize = 5;
            options.SetupRestOfDto(_numEntries); //need this otherwise page is set back to 1
            options.PageNum = pageNum;
            var books = service.SortFilterPage(options).ToList();

            //VERIFY
            books.Count.ShouldEqual(options.PageSize);
            books.First().GetIdAsInt().ShouldEqual(10 - ((pageNum - 1) * options.PageSize));
        }

        [Fact]
        public void TestSortByPriceLowestFirst()
        {
            //SETUP
            var service = new ListBooksNoSqlService(StoreFactory);
            var options = new NoSqlSortFilterPageOptions();

            //ATTEMPT
            options.OrderByOptions = OrderNoSqlByOptions.ByPriceLowestFirst;
            var books = service.SortFilterPage(options).ToList();

            //VERIFY
            books.Count.ShouldEqual(options.PageSize);
            var i = books.First().ActualPrice;
            books.ForEach(x =>
            {
                (x.ActualPrice >= i).ShouldBeTrue();
                i = x.ActualPrice;
            });
        }

        [Fact]
        public void TestFilterByVotes()
        {
            //SETUP
            var service = new ListBooksNoSqlService(StoreFactory);
            var options = new NoSqlSortFilterPageOptions();

            //ATTEMPT
            options.FilterBy = BooksNoSqlFilterBy.ByVotes;
            options.FilterValue = (3).ToString();
            var books = service.SortFilterPage(options).ToList();

            //VERIFY
            books.All(x => x.ReviewsAverageVotes > 3).ShouldBeTrue();

        }
    }
}