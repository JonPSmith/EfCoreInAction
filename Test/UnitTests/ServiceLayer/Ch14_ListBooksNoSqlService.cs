// Copyright (c) 2016 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using DataNoSql;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ServiceLayer.BookServices.Concrete;
using ServiceLayer.BookServices.RavenDb;
using test.Helpers;
using test.Mocks;
using Xunit;
using Xunit.Abstractions;
using Xunit.Extensions.AssertExtensions;

namespace Test.UnitTests.ServiceLayer
{
    public class Ch14_ListBooksNoSqlService
    {
        private readonly ITestOutputHelper _output;
        private static List<string> _logList = new List<string>();
        private static ILogger _logger = new StandInLogger(_logList);

        private static readonly Lazy<RavenStore> LazyStoreFactory = new Lazy<RavenStore>(() =>
        {
            var ravenDbTestConnection = AppSettings.GetConfiguration().GetConnectionString("RavenDb-Test");
            if (string.IsNullOrEmpty( ravenDbTestConnection ))
                throw new InvalidOperationException("You need a connection string in the test's appsetting.json file.");
            var storeFactory = new RavenStore(ravenDbTestConnection, _logger);
            return storeFactory;
        });

        private RavenStore StoreFactory => LazyStoreFactory.Value;

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
            var logs = new List<string>();
            var logger = new StandInLogger(logs);
            var service = new ListBooksNoSqlService(StoreFactory.CreateNoSqlAccessor().BookListQuery());
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
            var logs = new List<string>();
            var logger = new StandInLogger(logs);
            var service = new ListBooksNoSqlService(StoreFactory.CreateNoSqlAccessor().BookListQuery());
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
            var logs = new List<string>();
            var logger = new StandInLogger(logs);
            var service = new ListBooksNoSqlService(StoreFactory.CreateNoSqlAccessor().BookListQuery());
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
            var logs = new List<string>();
            var logger = new StandInLogger(logs);
            var service = new ListBooksNoSqlService(StoreFactory.CreateNoSqlAccessor().BookListQuery());
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