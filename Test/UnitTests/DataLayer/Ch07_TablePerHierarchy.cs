// Copyright (c) 2017 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using test.Attributes;
using test.EfHelpers;
using test.Helpers;
using Test.Chapter07Listings.EfClasses;
using Test.Chapter07Listings.EFCode;
using Xunit;
using Xunit.Extensions.AssertExtensions;

namespace test.UnitTests.DataLayer
{
    public class Ch07_TablePerHierarchy
    {

        [Fact]
        public void TestCreateTphPaymentCashOk()
        {
            //SETUP
            using (var context = new Chapter07DbContext(SqliteInMemory.CreateOptions<Chapter07DbContext>()))
            {
                var logs = new List<string>();
                SqliteInMemory.SetupLogging(context, logs);

                context.Database.EnsureCreated();

                //ATTEMPT
                var payment = new PaymentCash()
                {
                    Amount = 123
                };
                context.Add(payment);
                context.SaveChanges();

                //VERIFY
                context.Payments.Count().ShouldEqual(1);
                var cash = context.Payments.First() as PaymentCash;
                cash.ShouldNotBeNull();
            }
        }

        [Fact]
        public void TestReadBackDifferentPaymentsOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<Chapter07DbContext>();
            using (var context = new Chapter07DbContext(options))
            {
                var logs = new List<string>();
                SqliteInMemory.SetupLogging(context, logs);

                context.Database.EnsureCreated();

                //ATTEMPT
                context.Add(new PaymentCash());
                context.Add(new PaymentCard());
                context.Add(new PaymentCash());
                context.Add(new PaymentCash());
                context.Add(new PaymentCard());
                context.SaveChanges();
            }
            using (var context = new Chapter07DbContext(options))
            {
                //VERIFY
                context.Payments.OfType<PaymentCash>().Count().ShouldEqual(3);
                context.Payments.OfType<PaymentCard>().Count().ShouldEqual(2);
            }
        }

        [Fact]
        public void TestCreateSoldItTphPaymentCashOk()
        {
            //SETUP
            using (var context = new Chapter07DbContext(SqliteInMemory.CreateOptions<Chapter07DbContext>()))
            {
                var logs = new List<string>();
                SqliteInMemory.SetupLogging(context, logs);

                context.Database.EnsureCreated();

                //ATTEMPT
                var sold = new SoldIt()
                {
                    WhatSold = "A hat",
                    Payment = new PaymentCash {  Amount = 12}
                };
                context.Add(sold);
                context.SaveChanges();

                //VERIFY
                context.Payments.Count().ShouldEqual(1);
                var cash = context.Payments.First() as PaymentCash;
                cash.ShouldNotBeNull();
            }
        }

        [Fact]
        public void TestReadBackSoldItOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<Chapter07DbContext>();
            using (var context = new Chapter07DbContext(options))
            {
                var logs = new List<string>();
                SqliteInMemory.SetupLogging(context, logs);

                context.Database.EnsureCreated();

                //ATTEMPT
                var sold = new SoldIt()
                {
                    WhatSold = "A hat",
                    Payment = new PaymentCard { Amount = 12, ReceiptCode = "1234"}
                };
                context.Add(sold);
                context.SaveChanges();
            }
            using (var context = new Chapter07DbContext(options))
            {
                //VERIFY
                var sold = context.SoldThings.Include(x => x.Payment).Single(p => p.PaymentId == 1);
                sold.Payment.PType.ShouldEqual(PTypes.Card);
                sold.Payment.ShouldBeType<PaymentCard>();
                var card = sold.Payment as PaymentCard;
                card.ShouldNotBeNull();
                card.ReceiptCode.ShouldEqual("1234");
            }
        }

        [RunnableInDebugOnly]
        public void TestCreateTphPaymentCashSqlOk()
        {
            //SETUP
            var connection = this.GetUniqueDatabaseConnectionString();
            var optionsBuilder =
                new DbContextOptionsBuilder<Chapter07DbContext>();

            optionsBuilder.UseSqlServer(connection);
            using (var context = new Chapter07DbContext(optionsBuilder.Options))
            {
                context.Database.EnsureDeleted();
                var logIt = new LogDbContext(context);
                context.Database.EnsureCreated();

                var payment = new PaymentCash()
                {
                    Amount = 123
                };
                context.Add(payment);
                context.SaveChanges();

                //VERIFY
                context.Payments.Count().ShouldEqual(1);
            }
        }
    }
}