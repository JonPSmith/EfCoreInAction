// Copyright (c) 2016 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System.Linq;
using DataLayer.EfClasses;
using ServiceLayer.BookServices.QueryObjects;
using test.EfHelpers;
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
            var inMemDb = new SqliteInMemory();
            var books = EfTestData.CreateDummyBooks();
            books[5].AddPromotion(1.5m, "Test to check order by works");
        

            using (var db = inMemDb.GetContextWithSetup())
            {
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