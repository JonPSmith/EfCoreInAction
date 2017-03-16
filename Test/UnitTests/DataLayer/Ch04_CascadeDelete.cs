// Copyright (c) 2017 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using DataLayer.EfClasses;
using DataLayer.EfCode;
using Microsoft.EntityFrameworkCore;
using test.EfHelpers;
using Xunit;
using Xunit.Extensions.AssertExtensions;

namespace test.UnitTests.DataLayer
{
    public class Ch04_CascadeDelete
    {
        [Fact]
        public void TestDeleteBookNoOrder()
        {
            //SETUP
            var inMemDb = new SqliteInMemory();

            using (var context = inMemDb.GetContextWithSetup())
            {
                context.SeedDatabaseFourBooks();
                //ATTEMPT

                context.Books.Remove(context.Books.First());
                context.SaveChanges();

                //VERIFY
                context.Books.Count().ShouldEqual(3);
            }
        }

        [Fact]
        public void TestDeleteBookInLineItemFails()
        {
            //SETUP
            var inMemDb = new SqliteInMemory();

            using (var context = inMemDb.GetContextWithSetup())
            {
                context.SeedDatabaseFourBooks();
                var userId = Guid.NewGuid();

                var order = new Order
                {
                    CustomerName = userId,
                    LineItems = new List<LineItem>
                    {
                        new LineItem
                        {
                            ChosenBook = context.Books.First(),
                            LineNum = 0,
                            BookPrice = 123,
                            NumBooks = 1
                        }
                    }
                };
                context.Orders.Add(order);
                context.SaveChanges();

                //ATTEMPT
                context.Books.Remove(context.Books.First());
                var ex = Assert.Throws<InvalidOperationException>(() => context.SaveChanges());

                //VERIFY
                ex.Message.ShouldEqual("The association between entity types 'Book' and 'LineItem' has been severed but the foreign key for this relationship cannot be set to null. If the dependent entity should be deleted, then setup the relationship to use cascade deletes.");
            }
        }

        [Fact]
        public void TestDeleteBookInLineItemFailsWithEfNotKowningAboutTheOrder()
        {
            //SETUP
            var options =
                this.NewMethodUniqueDatabaseSeeded4Books();

            using (var context = new EfCoreContext(options))
            {
                context.SeedDatabaseFourBooks();
                var userId = Guid.NewGuid();

                var order = new Order
                {
                    CustomerName = userId,
                    LineItems = new List<LineItem>
                    {
                        new LineItem
                        {
                            ChosenBook = context.Books.First(),
                            LineNum = 0,
                            BookPrice = 123,
                            NumBooks = 1
                        }
                    }
                };
                context.Orders.Add(order);
                context.SaveChanges();
            }

                //ATTEMPT
            using (var context = new EfCoreContext(options))
            {
                context.Books.Remove(context.Books.First());
                var ex = Assert.ThrowsAny<DbUpdateException>(() => context.SaveChanges());

                //VERIFY
                ex.InnerException.Message.StartsWith("The DELETE statement conflicted with the REFERENCE constraint \"FK_LineItem_Books_BookId\". ")
                    .ShouldBeTrue();
            }
        }
    }
}