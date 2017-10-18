// Copyright (c) 2016 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System.Linq;
using DataLayer.EfCode;
using ServiceLayer.BookServices;
using ServiceLayer.BookServices.Concrete;
using ServiceLayer.BookServices.QueryObjects;
using test.EfHelpers;
using Xunit;
using Xunit.Extensions.AssertExtensions;

namespace test.UnitTests.ServiceLayer
{
    public class Ch14_ListBooksServiceMySql
    {

        public Ch14_ListBooksServiceMySql()
        {
            var options = this.MySqlOptionsWithCorrectConnection();
            using (var db = new EfCoreContext(options))
            {
                db.Database.EnsureCreated();
                if (!db.Books.Any())
                    db.SeedDatabaseDummyBooks();
            }
        }

        [Theory]
        [InlineData(OrderByOptions.SimpleOrder)]
        [InlineData(OrderByOptions.ByPublicationDate)]
        public void OrderBooksBy(OrderByOptions orderByOptions)
        {
            //SETUP
            var options = this.MySqlOptionsWithCorrectConnection();
            using (var db = new EfCoreContext(options))
            {
                var numBooks = db.Books.Count();

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
            var options = this.MySqlOptionsWithCorrectConnection();
            using (var db = new EfCoreContext(options))
            {
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