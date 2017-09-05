// Copyright (c) 2017 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System.Linq;
using Microsoft.EntityFrameworkCore;
using test.EfHelpers;
using test.Helpers;
using Test.Chapter08Listings.EfCode;
using Xunit;
using Xunit.Abstractions;
using Xunit.Extensions.AssertExtensions;

namespace test.UnitTests.DataLayer
{
    public class Ch08_ScalarFunctionMapping
    {
        private readonly ITestOutputHelper _output;

        private readonly DbContextOptions<Chapter08EfCoreContext> _options;

        public Ch08_ScalarFunctionMapping(ITestOutputHelper output)
        {
            _output = output;

            var connection = this.GetUniqueDatabaseConnectionString();
            var optionsBuilder =
                new DbContextOptionsBuilder<Chapter08EfCoreContext>();

            optionsBuilder.UseSqlServer(connection);
            _options = optionsBuilder.Options;
            using (var context = new Chapter08EfCoreContext(_options))
            {
                if (context.Database.EnsureCreated())
                {
                   //new database, so seed it with function and books
                    context.AddUdfToDatabase();

                    context.AddRange(EfTestData.CreateDummyBooks(setBookId: false));
                    context.SaveChanges();
                }
            }
        }

        private class Dto
        {
            public int BookId { get; set; }
            public string Title { get; set; }
            public double? AveVotes { get; set; }
        }

        [Fact]
        public void TestUdfWorksOk()
        {
            //SETUP
            using (var context = new Chapter08EfCoreContext(_options))
            {
                var logIt = new LogDbContext(context);

                //ATTEMPT
                var bookAndVotes = context.Books.Select(x => new Dto
                {
                    BookId = x.BookId,
                    Title = x.Title,
                    AveVotes = MyUdfMethods.AverageVotes(x.BookId)
                }).ToList();

                //VERIFY
                foreach (var log in logIt.Logs)
                {
                    _output.WriteLine(log);
                }
            }
        }

        [Fact]
        public void TestUdfAverageIsCorrectOk()
        {
            //SETUP
            using (var context = new Chapter08EfCoreContext(_options))
            {
                //ATTEMPT
                var bookAndVotes = context.Books.Select(x => new Dto
                {
                    BookId = x.BookId,
                    Title = x.Title,
                    AveVotes = MyUdfMethods.AverageVotes(x.BookId)
                }).ToList();

                //VERIFY
                var softAve = context.Books.Include(x => x.Reviews).OrderBy(p => p.BookId)
                    .Select(x => x.Reviews.Any() ? (double?)x.Reviews.Average(y => y.NumStars) : null).ToList();
                bookAndVotes.OrderBy(p => p.BookId).Select(x => x.AveVotes).ShouldEqual(softAve);
            }
        }

        [Fact]
        public void TestFilterUsingUdfOk()
        {
            //SETUP
            using (var context = new Chapter08EfCoreContext(_options))
            {
                var logIt = new LogDbContext(context);

                //ATTEMPT
                var books = context.Books
                    .Where(x => 
                        MyUdfMethods.AverageVotes(x.BookId) >= 2.5)
                .ToList();

                //VERIFY
                books.Count.ShouldEqual(6);
                foreach (var log in logIt.Logs)
                {
                    _output.WriteLine(log);
                }
            }
        }
    }
}