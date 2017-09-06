// Copyright (c) 2017 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System.Linq;
using DataLayer.EfCode;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Logging;
using test.EfHelpers;
using Xunit;
using Xunit.Abstractions;
using Xunit.Extensions.AssertExtensions;

namespace test.UnitTests.DataLayer
{
    public class Ch13_LookAtManyToManyQuery
    {
        private readonly ITestOutputHelper _output;

        public Ch13_LookAtManyToManyQuery(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void ManyToManySubQueryWithoutToList()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();

            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureCreated();
                context.SeedDatabaseFourBooks();
                var logIt = new LogDbContext(context);
                var logger = context.GetService<ILoggerFactory>().CreateLogger("TEST");

                //ATTEMPT
                var bookWithAuthors = context.Books
                    .Select(b => new
                    {
                        b.BookId,
                        AuthorNames = b.AuthorsLink.Select(c => c.Author.Name)
                    }).ToList();

                logger.LogInformation("Main query finished.");

                //VERIFY
                bookWithAuthors.First().AuthorNames.Count().ShouldEqual(1);
                bookWithAuthors.First().AuthorNames.First().ShouldEqual("Martin Fowler");
                logIt.Logs.Count.ShouldEqual(4);
                foreach (var log in logIt.Logs)
                {
                    _output.WriteLine(log);
                }
            }
        }


        [Fact]
        public void ManyToManySubQueryWithToList()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();

            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureCreated();
                context.SeedDatabaseFourBooks();
                var logIt = new LogDbContext(context);
                var logger = context.GetService<ILoggerFactory>().CreateLogger("TEST");

                //ATTEMPT
                var bookWithAuthors = context.Books
                    .Select(b => new
                    {
                        b.BookId,
                        AuthorNames = b.AuthorsLink.Select(c => c.Author.Name).ToList()
                    }).ToList();
                logger.LogInformation("Main query finished.");

                //VERIFY
                bookWithAuthors.First().AuthorNames.Count().ShouldEqual(1);
                bookWithAuthors.First().AuthorNames.First().ShouldEqual("Martin Fowler");
                logIt.Logs.Count.ShouldEqual(6);
                foreach (var log in logIt.Logs)
                {
                    _output.WriteLine(log);
                }
            }
        }

    }
}