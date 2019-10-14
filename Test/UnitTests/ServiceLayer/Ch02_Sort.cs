// Copyright (c) 2016 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System.Linq;
using DataLayer.EfClasses;
using DataLayer.EfCode;
using Microsoft.EntityFrameworkCore;
using ServiceLayer.BookServices.QueryObjects;
using test.EfHelpers;
using test.Helpers;
using Xunit;
using Xunit.Extensions.AssertExtensions;

namespace test.UnitTests.ServiceLayer
{
    public class Ch02_Sort
    {

        [Fact]
        public void CheckSortOnPrice()
        {
            //SETUP
            var connectionString = this.GetUniqueDatabaseConnectionString();
            var optionsBuilder = new DbContextOptionsBuilder<EfCoreContext>();
            optionsBuilder.UseSqlServer(connectionString);

            using (var db = new EfCoreContext(optionsBuilder.Options))
            {
                db.Database.EnsureCreated();

                var books = EfTestData.CreateDummyBooks(setBookId:false);
                books[5].Promotion = new PriceOffer
                {
                    NewPrice = 1.5m,
                    PromotionalText = "Test to check order by works"
                };
                db.Books.AddRange(books);
                db.SaveChanges();

                //ATTEMPT
                var sorted = db.Books.MapBookToDto().OrderBooksBy(OrderByOptions.ByPriceHigestFirst).ToList();

                //VERIFY
                sorted[8].ActualPrice.ShouldEqual(1.5m);
            }
        }
    }
}