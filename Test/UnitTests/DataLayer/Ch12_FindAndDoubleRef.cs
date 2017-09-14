// Copyright (c) 2016 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using DataLayer.EfClasses;
using DataLayer.EfCode;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Logging;
using test.Attributes;
using test.EfHelpers;
using Xunit;
using Xunit.Abstractions;

namespace test.UnitTests.DataLayer
{
    public class Ch12_FindAndDoubleRef
    {
        private readonly ITestOutputHelper _output;

        public Ch12_FindAndDoubleRef(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void AddAndAddRangeCompare()
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
                logger.LogInformation("Before Book1 -----------------------");
                var book1 = context.Books.First();
                logger.LogInformation("Before Book2 -----------------------");
                var book2 = context.Find<Book>(book1.BookId);
                logger.LogInformation("Before Book3 -----------------------");
                var book3 = context.Books.First();
                logger.LogInformation("After Books3 -----------------------");

                //VERIFY
                foreach (var log in logIt.Logs)
                {
                    _output.WriteLine(log);
                }
            }

        }

        [Fact]
        public void UseSelectCalculatedPropertyInOrderBy()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();

            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureCreated();
                context.SeedDatabaseFourBooks();
                var logIt = new LogDbContext(context);

                //ATTEMPT
                var books = context.Books.Select(b => new
                    {
                        b.BookId,
                        ReviewCount = b.Reviews.Count()
                    }).OrderBy(x => x.ReviewCount)
                    .ToList();

                //VERIFY
                foreach (var log in logIt.Logs)
                {
                    _output.WriteLine(log);
                }
            }
        }

        [Fact]
        public void UseSelectNormalPropertyInOrderBy()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();

            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureCreated();
                context.SeedDatabaseFourBooks();
                var logIt = new LogDbContext(context);

                //ATTEMPT
                var books = context.Books.Select(b => new
                    {
                        b.BookId,
                        ReviewCount = b.Reviews.Count()
                    }).OrderBy(x => x.BookId)
                    .ToList();

                //VERIFY
                foreach (var log in logIt.Logs)
                {
                    _output.WriteLine(log);
                }
            }
        }


    }
}