// Copyright (c) 2017 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
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
                var tagged = BookChangeInfo.FindBookChanges(context.ChangeTracker.Entries());
                var changes = BookChanges.FindChangedBooks(tagged);

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
                var changes = BookChanges.FindChangedBooks(BookChangeInfo.FindBookChanges(context.ChangeTracker.Entries()));

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
                var changes = BookChanges.FindChangedBooks(BookChangeInfo.FindBookChanges(context.ChangeTracker.Entries()));

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
                var changes = BookChanges.FindChangedBooks(BookChangeInfo.FindBookChanges(context.ChangeTracker.Entries()));

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
                var changes = BookChanges.FindChangedBooks(BookChangeInfo.FindBookChanges(context.ChangeTracker.Entries()));

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
                var changes = BookChanges.FindChangedBooks(BookChangeInfo.FindBookChanges(context.ChangeTracker.Entries()));

                //VERIFY
                changes.Count.ShouldEqual(1);
                changes.First().BookId.ShouldEqual(bookTracked.BookId);
                changes.First().State.ShouldEqual(EntityState.Added);
            }
        }

        [Fact]
        public void TestCreateNewBook()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureCreated();
                var book = EfTestData.CreateDummyBookOneAuthor();

                //ATTEMPT
                context.Books.Add(book);
                var preChanges = BookChangeInfo.FindBookChanges(context.ChangeTracker.Entries());
                context.SaveChanges();
                var changes = BookChanges.FindChangedBooks(preChanges);

                //VERIFY
                changes.Count.ShouldEqual(1);
                changes.First().BookId.ShouldEqual(1);
                changes.First().State.ShouldEqual(EntityState.Added);
            }
        }

        [Fact]
        public void TestDeleteBook()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureCreated();
                context.SeedDatabaseFourBooks();
            }
            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureCreated();
                var book = EfTestData.CreateDummyBookOneAuthor();

                //ATTEMPT
                context.Remove(context.Books.First());
                var preChanges = BookChangeInfo.FindBookChanges(context.ChangeTracker.Entries());
                context.SaveChanges();
                var changes = BookChanges.FindChangedBooks(preChanges);

                //VERIFY
                changes.Count.ShouldEqual(1);
                changes.First().BookId.ShouldEqual(1);
                changes.First().State.ShouldEqual(EntityState.Deleted);
            }
        }

        [Fact]
        public void TestDeleteBookAlreadySoftDeleted()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureCreated();
                context.SeedDatabaseFourBooks();
                context.Books.First().SoftDeleted = true;
                context.SaveChanges();
            }
            using (var context = new EfCoreContext(options))
            {
                //ATTEMPT
                context.Remove(context.Books.IgnoreQueryFilters().First());
                var preChanges = BookChangeInfo.FindBookChanges(context.ChangeTracker.Entries());
                context.SaveChanges();
                var changes = BookChanges.FindChangedBooks(preChanges);

                //VERIFY
                changes.Count.ShouldEqual(0);
            }
        }

        [Fact]
        public void TestChangeBookAlreadySoftDeleted()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureCreated();
                context.SeedDatabaseFourBooks();
                context.Books.First().SoftDeleted = true;
                context.SaveChanges();
            }
            using (var context = new EfCoreContext(options))
            {
                //ATTEMPT
                context.Books.IgnoreQueryFilters().First().Title = "New Title";
                var preChanges = BookChangeInfo.FindBookChanges(context.ChangeTracker.Entries());
                context.SaveChanges();
                var changes = BookChanges.FindChangedBooks(preChanges);

                //VERIFY
                changes.Count.ShouldEqual(0);
            }
        }

        [Fact]
        public void ExampleCreateNewBookWithReview()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureCreated();
                var logIt = new LogDbContext(context);

                //ATTEMPT
                var review = new Review {NumStars = 5};
                var book = new Book {Title = "New book"};
                book.Reviews = new List<Review> {review};

                review.BookId.ShouldEqual(0);
                context.Add(book);
                review.BookId.ShouldBeInRange(int.MinValue, -1);
                context.Entry(book).State.ShouldEqual(EntityState.Added);
                context.Entry(review).State.ShouldEqual(EntityState.Added);
                context.SaveChanges();
                context.Entry(book).State.ShouldEqual(EntityState.Unchanged);
                review.BookId.Equals(book.BookId);

                foreach (var log in logIt.Logs)
                {
                    _output.WriteLine(log);
                }
            }
        }

        [Fact]
        public void ExampleAddReviewToExistingBook()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureCreated();
                var book = new Book
                {
                    Title = "New book"
                };
                context.Add(book);
                context.SaveChanges();
                context.Entry(book).State.ShouldEqual(EntityState.Unchanged);

                //ATTEMPT
                var review = new Review { NumStars = 5 };
                book.Reviews = new List<Review> {review};
                review.BookId.ShouldEqual(0);
                context.ChangeTracker.DetectChanges();
                context.Entry(review).State.ShouldEqual(EntityState.Added);
                review.BookId.Equals(book.BookId);
                context.SaveChanges();
                context.Entry(book).State.ShouldEqual(EntityState.Unchanged);
                review.BookId.Equals(book.BookId);
            }
        }
    }
}