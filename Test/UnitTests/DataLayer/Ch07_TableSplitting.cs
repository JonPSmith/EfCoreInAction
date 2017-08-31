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
    public class Ch07_TableSplitting
    {
        private readonly ITestOutputHelper _output;

        public Ch07_TableSplitting(ITestOutputHelper output)
        {
            _output = output;
        }

        //-----------------------------------------------
        //private helper method
        private static void AddBookSummaryWithDetails(SplitOwnDbContext context)
        {
            var entity = new BookSummary
            {
                Title = "Title",
                AuthorsString = "AuthorA, AuthorB",
                Details = new BookDetail
                {
                    Description = "Description",
                    Price = 10
                }
            };
            context.Add(entity);
            context.SaveChanges();
        }
        //---------------------------------------------------

        [Fact]
        public void TestCreateBookSummaryWithDetailOk()
        {
            //SETUP
            using (var context = new SplitOwnDbContext(SqliteInMemory.CreateOptions<SplitOwnDbContext>()))
            {
                var logIt = new LogDbContext(context);
                context.Database.EnsureCreated();

                //ATTEMPT
                AddBookSummaryWithDetails(context);

                //VERIFY
                context.BookSummaries.Count().ShouldEqual(1);
                context.BookSummaries.Single().BookSummaryId.ShouldNotEqual(0);
                foreach (var log in logIt.Logs)
                {
                    _output.WriteLine(log);
                }
            }
        }

        [Fact]
        public void TestCreateBookSummaryWithoutDetailsBad()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<SplitOwnDbContext>();
            using (var context = new SplitOwnDbContext(options))
            {
                context.Database.EnsureCreated();

                //ATTEMPT
                var entity = new BookSummary
                {
                    Title = "Title",
                    AuthorsString = "AuthorA, AuthorB"
                };
                context.Add(entity);
                var ex = Assert.Throws<InvalidOperationException>(() => context.SaveChanges());

                //VERIFY
                ex.Message.StartsWith("The entity of 'BookSummary' is sharing the table 'BookSummaryAndDetail' with 'BookDetail', but there is no entity of this type with the same key value ")
                    .ShouldBeTrue();
            }
        }

        [Fact]
        public void TestReadBookSummaryOnlyOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<SplitOwnDbContext>();
            using (var context = new SplitOwnDbContext(options))
            {
                context.Database.EnsureCreated();
                AddBookSummaryWithDetails(context);
            }
            using (var context = new SplitOwnDbContext(options))
            {
                //ATTEMPT
                var entity = context.BookSummaries.First();

                //VERIFY
                entity.Details.ShouldBeNull();
            }
        }

        [Fact]
        public void TestReadBookSummaryAndDetailOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<SplitOwnDbContext>();
            using (var context = new SplitOwnDbContext(options))
            {
                context.Database.EnsureCreated();
                AddBookSummaryWithDetails(context);
            }
            using (var context = new SplitOwnDbContext(options))
            {
                //ATTEMPT
                var entity = context.BookSummaries.Include(p => p.Details).First();

                //VERIFY
                entity.Details.ShouldNotBeNull();
            }
        }

        [Fact]
        public void TestUpdateBookSummaryOnlyOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<SplitOwnDbContext>();
            using (var context = new SplitOwnDbContext(options))
            {
                context.Database.EnsureCreated();
                AddBookSummaryWithDetails(context);
            }
            using (var context = new SplitOwnDbContext(options))
            {
                //ATTEMPT
                var entity = context.BookSummaries.First();
                entity.Title = "New Title";
                context.SaveChanges();

                //VERIFY
                context.BookSummaries.First().Title.ShouldEqual("New Title");
            }
        }

        [Fact]
        public void TestUpdateDetailsOnlyOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<SplitOwnDbContext>();
            using (var context = new SplitOwnDbContext(options))
            {
                context.Database.EnsureCreated();
                AddBookSummaryWithDetails(context);
            }
            using (var context = new SplitOwnDbContext(options))
            {
                //ATTEMPT
                var entity = context.Set<BookDetail>().First();
                entity.Price = 1000;
                context.SaveChanges();

                //VERIFY
                context.Set<BookDetail>().First().Price.ShouldEqual(1000);
            }
        }
    }
}