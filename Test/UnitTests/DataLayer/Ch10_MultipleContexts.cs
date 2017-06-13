// Copyright (c) 2016 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using DataLayer.EfClasses;
using DataLayer.EfCode;
using Microsoft.EntityFrameworkCore;
using test.EfHelpers;
using test.Helpers;
using Test.Chapter10Listings.EfCode;
using Xunit;
using Xunit.Abstractions;
using Xunit.Extensions.AssertExtensions;

namespace test.UnitTests.DataLayer
{
    public class Ch10_MultipleContexts
    {
        private readonly ITestOutputHelper _output;

        private readonly string _connection;

        public Ch10_MultipleContexts(ITestOutputHelper output)
        {
            _output = output;

            _connection = this.GetUniqueDatabaseConnectionString();
            var optionsBuilder =
                new DbContextOptionsBuilder<EfCoreContext>();

            optionsBuilder.UseSqlServer(_connection);
            var options = optionsBuilder.Options;

            using (var context = new EfCoreContext(options))
            {
                if (context.Database.EnsureCreated())
                {
                    var books = EfTestData.CreateFourBooks();
                    var order = new Order
                    {
                        LineItems = new List<LineItem>
                        {
                            new LineItem
                            {
                                LineNum = 0,
                                ChosenBook = books.First(),
                                BookPrice = books.First().Price,
                                NumBooks = 1
                            }
                        },
                        CustomerName = Guid.NewGuid()
                    };
                    context.AddRange(books);
                    context.Add(order);
                    context.SaveChanges();
                }
            }
        }

        [Fact]
        public void GetTableNamesBookContextOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<BookContext>();
            using (var context = new BookContext(options))
            {
                //ATTEMPT
                var tableNames = context.GetTableNamesInOrderForWipe();

                //VERIFY
                tableNames
                    .ShouldEqual(new[] { "BookAuthor", "PriceOffers", "Review", "Authors", "Books"});
            }
        }

        [Fact]
        public void GetTableNamesOrderContextOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<OrderContext>();
            using (var context = new OrderContext(options))
            {
                //ATTEMPT
                var tableNames = context.GetTableNamesInOrderForWipe();

                //VERIFY
                tableNames
                    .ShouldEqual(new[] { "LineItem", "Orders", "Books" });
            }
        }

        [Fact]
        public void ReadBooksOk()
        {
            //SETUP
            var optionsBuilder =
                new DbContextOptionsBuilder<BookContext>();
            optionsBuilder.UseSqlServer(_connection);
            using (var context = new BookContext(optionsBuilder.Options))
            {
                //ATTEMPT
                var books = context.Books.ToList();

                //VERIFY
                books.Count.ShouldEqual(4);
            }
        }

        [Fact]
        public void ReadOrdersOk()
        {
            //SETUP
            var optionsBuilder =
                new DbContextOptionsBuilder<OrderContext>();
            optionsBuilder.UseSqlServer(_connection);
            using (var context = new OrderContext(optionsBuilder.Options))
            {
                //ATTEMPT
                var orders = context.Orders.ToList();

                //VERIFY
                orders.Count.ShouldEqual(1);
            }
        }


    }
}
