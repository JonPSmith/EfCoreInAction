// Copyright (c) 2017 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.Linq;
using DataLayer.EfClasses;
using DataLayer.EfCode;
using DataLayer.NoSql;
using Microsoft.EntityFrameworkCore;
using test.EfHelpers;
using Xunit;
using Xunit.Abstractions;
using Xunit.Extensions.AssertExtensions;

namespace test.UnitTests.DataLayer
{
    public class Ch14_BookChangeDetect
    {
        private readonly ITestOutputHelper _output;
        private readonly DbContextOptions<EfCoreContext> _options;

        public Ch14_BookChangeDetect(ITestOutputHelper output)
        {
            _output = output;
            _options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(_options))
            {
                context.Database.EnsureCreated();
                context.SeedDatabaseFourBooks();
            }
        }

        [Fact]
        public void TestFindChangedBooksOneBook()
        {
            //SETUP
            using (var context = new EfCoreContext(_options))
            {
                var bookTracked = context.Books.First();

                //ATTEMPT
                bookTracked.Title = Guid.NewGuid().ToString();
                var changes = BookChanges.FindChangedBooks(context.ChangeTracker.Entries());

                //VERIFY
                changes.Count.ShouldEqual(1);
                changes.First().BookId.ShouldEqual(bookTracked.BookId);
                changes.First().State.ShouldEqual(EntityState.Modified);
            }
        }

        [Fact]
        public void TestFindChangedBooksOneReview()
        {
            //SETUP
            using (var context = new EfCoreContext(_options))
            {
                var bookTracked = context.Books.Include(x => x.Reviews).Last();

                //ATTEMPT
                bookTracked.Reviews.First().NumStars = 0;
                var changes = BookChanges.FindChangedBooks(context.ChangeTracker.Entries());

                //VERIFY
                changes.Count.ShouldEqual(1);
                changes.First().BookId.ShouldEqual(bookTracked.BookId);
                changes.First().State.ShouldEqual(EntityState.Modified);
            }
        }

        [Fact]
        public void TestFindChangedBooksOnePromotion()
        {
            //SETUP
            using (var context = new EfCoreContext(_options))
            {
                var bookTracked = context.Books.First();

                //ATTEMPT
                context.Add(new PriceOffer { BookId = bookTracked.BookId, PromotionalText = "Unit Test", NewPrice = 1 });
                var changes = BookChanges.FindChangedBooks(context.ChangeTracker.Entries());

                //VERIFY
                changes.Count.ShouldEqual(1);
                changes.First().BookId.ShouldEqual(bookTracked.BookId);
                changes.First().State.ShouldEqual(EntityState.Modified);
            }
        }

        [Fact]
        public void TestFindChangedBooksBookDeleted()
        {
            //SETUP
            using (var context = new EfCoreContext(_options))
            {
                var bookTracked = context.Books.First();

                //ATTEMPT
                context.Remove(bookTracked);
                var changes = BookChanges.FindChangedBooks(context.ChangeTracker.Entries());

                //VERIFY
                changes.Count.ShouldEqual(1);
                changes.First().BookId.ShouldEqual(bookTracked.BookId);
                changes.First().State.ShouldEqual(EntityState.Deleted);
            }
        }

        [Fact]
        public void TestFindChangedBooksBookSoftDeleted()
        {
            //SETUP
            using (var context = new EfCoreContext(_options))
            {
                var bookTracked = context.Books.First();

                //ATTEMPT
                bookTracked.SoftDeleted = true;
                var changes = BookChanges.FindChangedBooks(context.ChangeTracker.Entries());

                //VERIFY
                changes.Count.ShouldEqual(1);
                changes.First().BookId.ShouldEqual(bookTracked.BookId);
                changes.First().State.ShouldEqual(EntityState.Deleted);
            }
        }

        [Fact]
        public void TestFindChangedBooksBookUnSoftDeleted()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureCreated();
                context.SeedDatabaseFourBooks();

                var bookTracked = context.Books.First();
                bookTracked.SoftDeleted = true;
                context.SaveChanges();
            }
            using (var context = new EfCoreContext(options))
            {
                var bookTracked = context.Books.IgnoreQueryFilters().First();

                //ATTEMPT
                bookTracked.SoftDeleted = false;
                var changes = BookChanges.FindChangedBooks(context.ChangeTracker.Entries());

                //VERIFY
                changes.Count.ShouldEqual(1);
                changes.First().BookId.ShouldEqual(bookTracked.BookId);
                changes.First().State.ShouldEqual(EntityState.Added);
            }
        }
    }
}