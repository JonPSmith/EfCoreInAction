// Copyright (c) 2016 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DataLayer.EfClasses;
using DataLayer.EfCode;
using Microsoft.EntityFrameworkCore;
using test.EfHelpers;
using test.Helpers;
using Test.Chapter12Listings.EfClasses;
using Test.Chapter12Listings.EfCode;
using Xunit;
using Xunit.Abstractions;
using Xunit.Extensions.AssertExtensions;

namespace test.UnitTests.DataLayer
{
    public class Ch12_MultipleActiveResultSets
    {
        private readonly ITestOutputHelper _output;

        public Ch12_MultipleActiveResultSets(ITestOutputHelper output)
        {
            _output = output;

            var connection = this.GetUniqueDatabaseConnectionString();
            var optionsBuilder =
                new DbContextOptionsBuilder<Chapter12DbContext>();

            optionsBuilder.UseSqlServer(connection);
            var options = optionsBuilder.Options;

            using (var context = new Chapter12DbContext(options))
            {
                if (context.Database.EnsureCreated())
                {
                    //only add the UDF if its a new database (as drop will fail as column uses it)
                    var scriptFilePath = Path.Combine(TestFileHelpers.GetSolutionDirectory(),
                        @"Test\Chapter12Listings\EfCode\AddUserDefinedFunction.sql");
                    connection.ExecuteScriptFileInTransaction(scriptFilePath);
                }

                if (!context.Books.Any())
                {
                    var entities = new List<Ch12Book>
                    {
                        new Ch12Book
                        {
                            Title = "Book - no votes"
                        },
                        new Ch12Book
                        {
                            Title = "Book - one vote",
                            Reviews= new List<Ch12Review>
                            {
                                new Ch12Review { NumStars = 5}
                            }
                        },
                        new Ch12Book
                        {
                            Title = "Book - two votes",
                            Reviews = new List<Ch12Review>
                            {
                                new Ch12Review { NumStars = 2},
                                new Ch12Review { NumStars = 4},
                            }
                        },
                    };
                    context.AddRange(entities);
                    context.SaveChanges();
                }
            }
        }

        [Fact]
        public void SqlServerRemoveMultipleActiveResultSetsThenDualRead()
        {
            //SETUP
            var connection = this.GetUniqueDatabaseConnectionString()
                .Replace(";MultipleActiveResultSets=True", "");
            
            var optionsBuilder =
                new DbContextOptionsBuilder<Chapter12DbContext>();
            optionsBuilder.UseSqlServer(connection);

            using (var context = new Chapter12DbContext(optionsBuilder.Options))
            {
                var logIt = new LogDbContext(context);
                //ATTEMPT
                foreach (var book in context.Books)
                {
                    var ex = Assert.Throws<InvalidOperationException>(() => context.Set<Ch12Review>().Where(x => x.Ch12BookId == book.Ch12BookId).ToList());

                    //VERIFY
                    ex.Message.ShouldEqual("There is already an open DataReader associated with this Command which must be closed first.");
                }
                foreach (var log in logIt.Logs)
                {
                    _output.WriteLine(log);
                }
            }
        }

        [Fact]
        public void SqlServerRemoveMultipleActiveResultSetsSingleRead()
        {
            //SETUP
            var connection = this.GetUniqueDatabaseConnectionString()
                .Replace(";MultipleActiveResultSets=True", "");

            var optionsBuilder =
                new DbContextOptionsBuilder<Chapter12DbContext>();
            optionsBuilder.UseSqlServer(connection);

            using (var context = new Chapter12DbContext(optionsBuilder.Options))
            {
                //ATTEMPT
                foreach (var book in context.Books.ToList())
                {
                    var reviews = context.Set<Ch12Review>().Where(x => x.Ch12BookId == book.Ch12BookId).ToList();
                }
            }
        }

        [Fact]
        public void SqlServerDualReadOk()
        {
            //SETUP
            var connection = this.GetUniqueDatabaseConnectionString()
                .Replace(";MultipleActiveResultSets=True", "");

            var optionsBuilder =
                new DbContextOptionsBuilder<Chapter12DbContext>();
            optionsBuilder.UseSqlServer(connection);

            using (var context = new Chapter12DbContext(optionsBuilder.Options))
            {
                var logIt = new LogDbContext(context);

                //ATTEMPT
                foreach (var book in context.Books.IgnoreQueryFilters())
                {
                    var reviews = context.Set<Ch12Review>().Where(x => x.Ch12BookId == book.Ch12BookId).ToList();
                }
                foreach (var log in logIt.Logs)
                {
                    _output.WriteLine(log);
                }
            }
        }
    }
}