// Copyright (c) 2017 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using test.EfHelpers;
using Test.Chapter07Listings.EFCode;
using Test.Chapter07Listings.SplitOwnClasses;
using Xunit;
using Xunit.Abstractions;
using Xunit.Extensions.AssertExtensions;

namespace test.UnitTests.DataLayer
{
    public class Ch07_OwnedTypes
    {
        private readonly ITestOutputHelper _output;

        public Ch07_OwnedTypes(ITestOutputHelper output)
        {
            _output = output;
        }

        //-----------------------------------------------
        //private helper method
        private static void AddOrderWithAddresses(SplitOwnDbContext context)
        {
            var entity = new OrderInfo()
            {
                OrderNumber = "123",
                DeliveryAddress = new Address
                {
                    NumberAndStreet = "1, some street",
                    City = "Some city",
                    ZipPostCode = "W1A 1AA",
                    CountryCodeIso2 = "UK"
                },
                BillingAddress = new Address
                {
                    NumberAndStreet = "1, some street",
                    City = "Some city",
                    ZipPostCode = "1234-5678",
                    CountryCodeIso2 = "US"
                }
            };
            context.Add(entity);
            context.SaveChanges();
        }
        //---------------------------------------------------

        [Fact]
        public void TestCreateOrderWithAddressesOk()
        {
            //SETUP
            using (var context = new SplitOwnDbContext(SqliteInMemory.CreateOptions<SplitOwnDbContext>()))
            {
                var logIt = new LogDbContext(context);
                context.Database.EnsureCreated();

                //ATTEMPT
                AddOrderWithAddresses(context);

                //VERIFY
                context.Orders.Count().ShouldEqual(1);
                foreach (var log in logIt.Logs)
                {
                    _output.WriteLine(log);
                }
            }
        }

        [Fact]
        public void TestCreateOrderWithoutAddressesOk()
        {
            //SETUP
            using (var context = new SplitOwnDbContext(SqliteInMemory.CreateOptions<SplitOwnDbContext>()))
            {
                context.Database.EnsureCreated();

                //ATTEMPT
                var entity = new OrderInfo()
                {
                    OrderNumber = "123"
                };
                context.Add(entity);
                var ex = Assert.Throws<InvalidOperationException>(() => context.SaveChanges());

                //VERIFY
                ex.Message.StartsWith("The entity of 'OrderInfo' is sharing the table 'Orders' with 'OrderInfo.BillingAddress#Address', but there is no entity of this type with the same key value")
                    .ShouldBeTrue();
            }
        }

        [Fact]
        public void TestReadOrderNoIncludesOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<SplitOwnDbContext>();
            using (var context = new SplitOwnDbContext(options))
            {
                context.Database.EnsureCreated();
                AddOrderWithAddresses(context);
            }
            using (var context = new SplitOwnDbContext(options))
            {
                var logIt = new LogDbContext(context);
                //ATTEMPT
                var entity = context.Orders.First();

                //VERIFY
                entity.DeliveryAddress.ShouldNotBeNull();
                entity.BillingAddress.ShouldNotBeNull();
                foreach (var log in logIt.Logs)
                {
                    _output.WriteLine(log);
                }
            }
        }

        [Fact]
        public void TestUpdateOrderOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<SplitOwnDbContext>();
            using (var context = new SplitOwnDbContext(options))
            {
                context.Database.EnsureCreated();
                AddOrderWithAddresses(context);
            }
            using (var context = new SplitOwnDbContext(options))
            {
                //ATTEMPT
                var entity = context.Orders.First();
                entity.OrderNumber = "567";
                context.SaveChanges();

                //VERIFY
                context.Orders.First().OrderNumber.ShouldEqual("567");
            }
        }


    }
}