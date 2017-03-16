// Copyright (c) 2017 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.Linq;
using ServiceLayer.CheckoutServices.Concrete;
using ServiceLayer.OrderServices.Concrete;
using test.EfHelpers;
using test.Mocks;
using Xunit;
using Xunit.Extensions.AssertExtensions;

namespace test.UnitTests.ServiceLayer
{
    public class Ch04_PlaceOrderServiceTransact
    {
        [Fact]
        public void TestPlaceOrderOk()
        {
            //SETUP
            var inMemDb = new SqliteInMemory();

            using (var context = inMemDb.GetContextWithSetup())
            {
                context.SeedDatabaseFourBooks();

                var mockCookieRequests = new MockHttpCookieAccess(CheckoutCookie.CheckoutCookieName, $"{Guid.NewGuid()},1,2");
                var service = new PlaceOrderServiceTransact(mockCookieRequests.CookiesIn, mockCookieRequests.CookiesOut, context);

                //ATTEMPT
                var orderId = service.PlaceOrder(true);
                context.SaveChanges();

                //VERIFY
                orderId.ShouldNotEqual(0);
                service.Errors.Count.ShouldEqual(0);
                context.Orders.Count().ShouldEqual(1);
                context.Orders.First().OrderId.ShouldEqual(orderId);
            }
        }

        [Fact]
        public void TestPlaceOrderBad()
        {
            //SETUP
            var inMemDb = new SqliteInMemory();

            using (var context = inMemDb.GetContextWithSetup())
            {
                context.SeedDatabaseFourBooks();

                var mockCookieRequests = new MockHttpCookieAccess(CheckoutCookie.CheckoutCookieName, $"{Guid.NewGuid()},1,2");
                var service = new PlaceOrderServiceTransact(mockCookieRequests.CookiesIn, mockCookieRequests.CookiesOut, context);

                //ATTEMPT
                var orderId = service.PlaceOrder(false);
                context.SaveChanges();

                //VERIFY
                orderId.ShouldEqual(0);
                service.Errors.Count.ShouldEqual(1);
                service.Errors.First().ErrorMessage.ShouldEqual("You must accept the T&Cs to place an order.");
            }
        }

    }
}