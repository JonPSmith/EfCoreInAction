// Copyright (c) 2017 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.Linq;
using DataLayer.EfCode;
using ServiceLayer.CheckoutServices.Concrete;
using test.EfHelpers;
using test.Mocks;
using Xunit;
using Xunit.Extensions.AssertExtensions;

namespace test.UnitTests.ServiceLayer
{
    public class Ch04_CheckoutListService
    {
        [Fact]
        public void TestCheckoutListOneBookDatabaseOneBook()
        {
            //SETUP
            var options = EfInMemory.CreateNewContextOptions();
            using (var context = new EfCoreContext(options))
            {
                context.SeedDatabaseDummyBooks(1);
            }
            //two line items: BookId:1 NumBooks:1
            var mockCookieRequests = new MockHttpCookieAccess(CheckoutCookie.CheckoutCookieName, $"{Guid.NewGuid()},1,1");

            //ATTEMPT
            using (var context = new EfCoreContext(options))
            {
                var service = new CheckoutListService(context, mockCookieRequests.CookiesIn);
                var list = service.GetCheckoutList();
                
                //VERIFY
                list.Count.ShouldEqual(1);
                list.First().BookId.ShouldEqual(1);
                list.First().BookPrice.ShouldEqual(1);
            }
        }

        [Fact]
        public void TestCheckoutListTwoBooksSqLite()
        {
            //SETUP
            var inMemDb = new SqliteInMemory();


            using (var context = inMemDb.GetContextWithSetup())
            {
                context.SeedDatabaseDummyBooks(10);

                //two line items: BookId:1 NumBooks:2, BookId:2 NumBooks:3
                var mockCookieRequests = new MockHttpCookieAccess(CheckoutCookie.CheckoutCookieName, $"{Guid.NewGuid()},1,2,2,3");

                //ATTEMPT

                var service = new CheckoutListService(context, mockCookieRequests.CookiesIn);
                var list = service.GetCheckoutList();

                //VERIFY
                for (int i = 0; i < list.Count(); i++)
                {
                    list[i].BookId.ShouldEqual(i + 1);
                    list[i].NumBooks.ShouldEqual((short)(i + 2));
                    list[i].BookPrice.ShouldEqual((i + 1));
                }
            }
        }

        [Fact]
        public void TestCheckoutListBookWithPromotion()
        {
            //SETUP
            var inMemDb = new SqliteInMemory();

            using (var context = inMemDb.GetContextWithSetup())
            {
                context.SeedDatabaseFourBooks();

                //I select the last book, which has a promotion
                var mockCookieRequests = new MockHttpCookieAccess(CheckoutCookie.CheckoutCookieName, $"{Guid.NewGuid()},4,1");

                //ATTEMPT

                var service = new CheckoutListService(context, mockCookieRequests.CookiesIn);
                var list = service.GetCheckoutList();

                //VERIFY
                list.Count.ShouldEqual(1);
                list.First().BookPrice.ShouldEqual(219);
            }
        }
    }
}