// Copyright (c) 2016 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System.Linq;
using ServiceLayer.BookServices;
using ServiceLayer.BookServices.Concrete;
using ServiceLayer.BookServices.QueryObjects;
using test.EfHelpers;
using Xunit;
using Xunit.Extensions.AssertExtensions;

namespace test.UnitTests.ServiceLayer
{
    public class Ch02_ListBooksService
    {
        [Theory]
        [InlineData(OrderByOptions.SimpleOrder)]
        [InlineData(OrderByOptions.ByPublicationDate)]
        public void OrderBooksBy(OrderByOptions orderByOptions)
        {
            //SETUP
            var inMemDb = new SqliteInMemory();

            const int numBooks = 5;
            using (var db = inMemDb.GetContextWithSetup())
            {
                db.SeedDatabaseDummyBooks(numBooks);

                //ATTEMPT
                var service = new ListBooksService(db);
                var listOptions = new SortFilterPageOptions() {OrderByOptions = orderByOptions};
                var dtos = service.SortFilterPage(listOptions).ToList();

                //VERIFY
                dtos.Count.ShouldEqual(numBooks);
            }
        }

        [Theory]
        [InlineData(5)]
        [InlineData(10)]
        public void PageBooks(int pageSize)
        {
            //SETUP
            var inMemDb = new SqliteInMemory();
            const int numBooks = 12;
            using (var db = inMemDb.GetContextWithSetup())
            {
                db.SeedDatabaseDummyBooks(numBooks);

                //ATTEMPT
                var service = new ListBooksService(db);
                var listOptions = new SortFilterPageOptions() { PageSize = pageSize};
                var dtos = service.SortFilterPage(listOptions).ToList();

                //VERIFY
                dtos.Count.ShouldEqual(pageSize);
            }
        }
    }
}