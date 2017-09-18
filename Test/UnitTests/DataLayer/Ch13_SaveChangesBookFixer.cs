// Copyright (c) 2016 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System.Linq;
using DataLayer.EfClasses;
using DataLayer.EfCode;
using Microsoft.EntityFrameworkCore;
using test.EfHelpers;
using Xunit;
using Xunit.Abstractions;
using Xunit.Extensions.AssertExtensions;

namespace Test.UnitTests.DataLayer
{
    public class Ch13_SaveChangesBookFixer
    {
        private readonly ITestOutputHelper _output;

        public Ch13_SaveChangesBookFixer(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void TestBookAddReviewOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureCreated();
                context.SeedDatabaseFourBooks();

                //ATTEMPT
                var logIt = new LogDbContext(context);

                var book = context.Books.First();
                book.AddReview(context, 5, null, null);
                context.SaveChanges();

                //VERIFY
                context.Set<Review>().Count().ShouldEqual(3);
                //foreach (var log in logIt.Logs)
                //{
                //    _output.WriteLine(log);
                //}
            }
        }

        [Fact]
        public void TestBookThrowsConcurrencyProblemIfCachedReviewValuesChangesOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureCreated();
                context.SeedDatabaseFourBooks();

                //ATTEMPT
                var logIt = new LogDbContext(context);

                var book = context.Books.First();
                book.AddReview(context, 3, null, null);
                context.Database.ExecuteSqlCommand(      
                    "UPDATE Books SET ReviewsCount = 10" +
                    $" WHERE BookId = {book.BookId}");
                var ex = Assert.Throws<DbUpdateConcurrencyException>(() => context.SaveChanges());

                //VERIFY
                ex.Message.StartsWith("Database operation expected to affect 1 row(s) but actually affected 0 row(s). Data may have been modified or deleted since entities were loaded. ")
                    .ShouldBeTrue();
                //foreach (var log in logIt.Logs)
                //{
                //    _output.WriteLine(log);
                //}
                ////to get the logs you need to fail see https://github.com/aspnet/Tooling/issues/541
                //Assert.True(false, "failed the test so that the logs show");
            }
        }

        [Theory]
        [InlineData(1, 1)]
        [InlineData(4, 3)]
        public void TestBookFixesProblemIfCachedReviewValuesChangesOk(int bookId, int expectedReviewCount)
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureCreated();
                context.SeedDatabaseFourBooks();

                //ATTEMPT
                var logIt = new LogDbContext(context);

                var book = context.Books.Single(x => x.BookId == bookId);
                book.AddReview(context, 3, null, null);
                context.Database.ExecuteSqlCommand(
                    "UPDATE Books SET ReviewsCount = 10" +
                    $" WHERE BookId = {bookId}");
                context.SaveChangesWithReviewCheck();

                //VERIFY
                var updatedBook = context.Books.Single(x => x.BookId == bookId);
                updatedBook.ReviewsCount.ShouldEqual(expectedReviewCount);
            }
        }

        [Theory]
        [InlineData(1, 2)]
        [InlineData(4, 4)]
        public void TestBookFixesProblemAddedReviewOk(int bookId, int expectedReviewCount)
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureCreated();
                context.SeedDatabaseFourBooks();

                //ATTEMPT
                var logIt = new LogDbContext(context);

                var book = context.Books.Single(x => x.BookId == bookId);
                book.AddReview(context, 1, null, null);
                context.Database.ExecuteSqlCommand(
                    $"INSERT INTO Review (BookId, NumStars) VALUES ({bookId}, 5)");
                context.Database.ExecuteSqlCommand(
                    "UPDATE Books SET ReviewsCount = 99" +
                    $" WHERE BookId = {bookId}");
                context.SaveChangesWithReviewCheck();

                //VERIFY
                var updatedBook = context.Books.Single(x => x.BookId == bookId);
                updatedBook.ReviewsCount.ShouldEqual(expectedReviewCount);
            }
        }

        [Fact]
        public void TestBookFixesProblemDeletedDatabaseReviewOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureCreated();
                context.SeedDatabaseFourBooks();
                var bookId = 4;

                //ATTEMPT
                var logIt = new LogDbContext(context);

                var book = context.Books.Include(x => x.Reviews).Single(x => x.BookId == bookId);
                book.AddReview(context, 3, null, null);
                context.Database.ExecuteSqlCommand(
                    $"DELETE FROM Review WHERE ReviewId = {book.Reviews.First().ReviewId}");
                context.Database.ExecuteSqlCommand(
                    "UPDATE Books SET ReviewsCount = 99" +
                    $" WHERE BookId = {bookId}");
                context.SaveChangesWithReviewCheck();

                //VERIFY
                var updatedBook = context.Books.SingleOrDefault(x => x.BookId == bookId);
                updatedBook.ReviewsCount.ShouldEqual(2);
            }
        }

        [Fact]
        public void TestBookFixesProblemDeletedBookReviewOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureCreated();
                context.SeedDatabaseFourBooks();
                var bookId = 4;

                //ATTEMPT
                var logIt = new LogDbContext(context);

                var book = context.Books.Include(x => x.Reviews).Single(x => x.BookId == bookId);
                book.RemoveReview(context, book.Reviews.First());
                context.Database.ExecuteSqlCommand(
                    "UPDATE Books SET ReviewsCount = 99" +
                    $" WHERE BookId = {bookId}");
                context.SaveChangesWithReviewCheck();

                //VERIFY
                var updatedBook = context.Books.SingleOrDefault(x => x.BookId == bookId);
                updatedBook.ReviewsCount.ShouldEqual(2);
            }
        }

        [Fact]
        public void TestBookFixesProblemIfOriginalBookWasDeletedOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureCreated();
                context.SeedDatabaseFourBooks();
                var bookId = 4;

                //ATTEMPT
                var logIt = new LogDbContext(context);

                var book = context.Books.Single(x => x.BookId == bookId);
                book.AddReview(context, 1, null, null);
                context.Database.ExecuteSqlCommand(
                    $"DELETE FROM Books WHERE BookId = {bookId}");
                context.SaveChangesWithReviewCheck();

                //VERIFY
                var updatedBook = context.Books.SingleOrDefault(x => x.BookId == bookId);
                updatedBook.ShouldBeNull();
            }
        }

    }
}