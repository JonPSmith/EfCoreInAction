// Copyright (c) 2016 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System.Linq;
using DataLayer.EfCode;
using Microsoft.EntityFrameworkCore;
using ServiceLayer.AdminServices.Concrete;
using test.EfHelpers;
using Xunit;
using Xunit.Abstractions;
using Xunit.Extensions.AssertExtensions;

namespace Test.UnitTests.ServiceLayer
{
    public class Ch13_BookAddRemoveReviews
    {
        private readonly ITestOutputHelper _output;

        public Ch13_BookAddRemoveReviews(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void TestAddReviewNoExisting()
        {
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureCreated();
                context.SeedDatabaseFourBooks();
                var service = new AddReviewService(context);
                const int bookId = 1;

                //ATTEMPT
                service.AddReviewToBook(bookId, 5, null, null);
                context.SaveChangesWithReviewCheck();

                //VERIFY
                var book = context.Books.Include(x => x.Reviews).Single(x => x.BookId == bookId);
                book.ReviewsCount.ShouldEqual(1);
                book.Reviews.Count().ShouldEqual(1);
                book.AverageVotes.ShouldEqual(5);
            }
        }

        [Fact]
        public void TestAddReviewExisting()
        {
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureCreated();
                context.SeedDatabaseFourBooks();
                var service = new AddReviewService(context);
                const int bookId = 4;

                //ATTEMPT
                service.AddReviewToBook(bookId, 1, null, null);
                context.SaveChangesWithReviewCheck();

                //VERIFY
                var book = context.Books.Include(x => x.Reviews).Single(x => x.BookId == bookId);
                book.ReviewsCount.ShouldEqual(3);
                book.Reviews.Count().ShouldEqual(3);
                ((double)book.AverageVotes).ToString("F3").ShouldEqual("3.667");
            }
        }
    }
}