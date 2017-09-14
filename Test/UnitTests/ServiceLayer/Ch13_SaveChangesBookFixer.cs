// Copyright (c) 2016 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.IO;
using System.Linq;
using DataLayer.EfClasses;
using DataLayer.EfCode;
using DataLayer.SqlCode;
using Microsoft.EntityFrameworkCore;
using ServiceLayer.BookServices;
using ServiceLayer.BookServices.QueryObjects;
using test.EfHelpers;
using test.Helpers;
using Xunit;
using Xunit.Abstractions;
using Xunit.Extensions.AssertExtensions;

namespace Test.UnitTests.ServiceLayer
{
    public class Ch13_SaveChangesBookFixer
    {
        private readonly ITestOutputHelper _output;

        public Ch13_SaveChangesBookFixer(ITestOutputHelper output)
        {
            _output = output;
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
                book.AddReview(new Review{ NumStars = 5});
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

    }
}