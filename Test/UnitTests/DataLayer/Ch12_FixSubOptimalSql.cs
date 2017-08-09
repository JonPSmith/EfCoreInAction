// Copyright (c) 2016 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Console;
using test.EfHelpers;
using test.Helpers;
using Test.Chapter12Listings.EfClasses;
using Test.Chapter12Listings.EfCode;
using Xunit;
using Xunit.Abstractions;
using Xunit.Extensions.AssertExtensions;

namespace test.UnitTests.DataLayer
{
    public class Ch12_FixSubOptimalSql
    {
        private readonly ITestOutputHelper _output;

        private readonly DbContextOptions<Chapter12DbContext> _options;

        public Ch12_FixSubOptimalSql(ITestOutputHelper output)
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

                if (!context.FixSubOptimalSqls.Any())
                {
                    var entities = new List<FixSubOptimalSql>
                    {
                        new FixSubOptimalSql
                        {
                            Name = "Book - no votes"
                        },
                        new FixSubOptimalSql
                        {
                            Name = "Book - one vote",
                            Reviews = new List<Ch12Review>
                            {
                                new Ch12Review { NumStars = 5}
                            }
                        },
                        new FixSubOptimalSql
                        {
                            Name = "Book - two votes",
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
        public void UseUdfToCreateOptimalSqlOk()
        {
            //SETUP

            using (var context = new Chapter12DbContext(_options))
            {
                var logIt = new LogDbContext(context);
                //ATTEMPT
                var entities = context.FixSubOptimalSqls.ToList();

                //VERIFY
                entities.Count.ShouldEqual(3);
                entities.Select(x => x.AverageVotes).ShouldEqual(new []{null, (decimal?)5, (decimal?)3 });
                foreach (var log in logIt.Logs)
                {
                    _output.WriteLine(log);
                }
            }
        }
    }
}