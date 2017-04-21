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
        public void TestChangePaymentTypeOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<Chapter07DbContext>();
            using (var context = new Chapter07DbContext(options))
            {
                var logs = new List<string>();
                SqliteInMemory.SetupLogging(context, logs);

                context.Database.EnsureCreated();

                //ATTEMPT
                context.Add(new PaymentCard{Amount =  12, ReceiptCode = "1234"});
                context.SaveChanges();
            }
            using (var context = new Chapter07DbContext(options))
            {
                //You MUST read it untracked because of issue #7340
                var untracked = context.Payments.AsNoTracking().Single();
                //Then you need to copy ALL the information to the new TPH type, especially its primary key.
                var changed = new PaymentCash
                {
                    PaymentId = untracked.PaymentId,
                    Amount = untracked.Amount,
                    //You MUST explictly set the discriminator
                    //NOTE: this only works because the PaymentConfig code contains the following Fluent API command below - see EF Core issue #7510
                    //entity.Property(p => p.PType).Metadata.IsReadOnlyAfterSave = false;
                    PType = PTypes.Cash //You MUST explictly set the discriminator
                };
                context.Update(changed);
                context.SaveChanges();
            }
            //VERITY
            using (var context = new Chapter07DbContext(options))
            {
                var payment = context.Payments.Single();
                payment.ShouldBeType<PaymentCash>();
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