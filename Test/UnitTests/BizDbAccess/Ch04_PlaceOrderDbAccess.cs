// Copyright (c) 2017 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using BizDbAccess.Orders;
using test.EfHelpers;
using Xunit;
using Xunit.Extensions.AssertExtensions;

namespace test.UnitTests.BizDbAccess
{
    public class Ch04_PlaceOrderDbAccess
    {
        [Fact]
        public void TestCheckoutListTwoBooksSqLite()
        {
            //SETUP
            var inMemDb = new SqliteInMemory();

            using (var context = inMemDb.GetContextWithSetup())
            {
                context.SeedDatabaseFourBooks();
                context.SaveChanges();
                var dbAccess = new PlaceOrderDbAccess(context);

                //ATTEMPT
                var booksDict = dbAccess.FindBooksByIdsWithPriceOffers(new []{1, 4});

                //VERIFY
                booksDict.Count.ShouldEqual(2);
                booksDict[1].HasPromotion.ShouldBeFalse();
                booksDict[4].HasPromotion.ShouldBeTrue();
            }
        }

        
    }
}