// Copyright (c) 2016 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
    public class Ch12_ImproveSqlViaUdf
    {
        private readonly ITestOutputHelper _output;

        private readonly DbContextOptions<Chapter12DbContext> _options;

        public Ch12_ImproveSqlViaUdf(ITestOutputHelper output)
        {
            _output = output;

            var connection = this.GetUniqueDatabaseConnectionString();
            var optionsBuilder =
                new DbContextOptionsBuilder<Chapter12DbContext>();

            optionsBuilder.UseSqlServer(connection);
            _options = optionsBuilder.Options;

            using (var context = new Chapter12DbContext(_options))
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
                            Title = "Book - no promotion",
                            Price = 10
                        },
                        new Ch12Book
                        {
                            Title = "Book - with promotion",
                            Price = 10,
                            Promotion = new Ch12PriceOffer
                            {
                                NewPrice = 5
                            }
                        }
                    };
                    context.AddRange(entities);
                    context.SaveChanges();
                }
            }
        }

        [Fact]
        public void UseUdfToCreateOptimalSqlOk()
        {
            //SETUP

            using (var context = new Chapter12DbContext(_options))
            {
                var logIt = new LogDbContext(context);
                //ATTEMPT
                var entities = context.Books.Include(p => p.Promotion).ToList();

                //VERIFY
                entities.Count.ShouldEqual(2);
                entities.Single(x => x.Promotion == null).ActualPrice.ShouldEqual(10);
                entities.Single(x => x.Promotion != null).ActualPrice.ShouldEqual(5);
                foreach (var log in logIt.Logs)
                {
                    _output.WriteLine(log);
                }
            }
        }


    }
}