// Copyright (c) 2017 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using DataLayer.EfCode;
using Microsoft.EntityFrameworkCore;
using test.EfHelpers;
using test.Helpers;
using Test.Chapter08Listings.EfClasses;
using Test.Chapter09Listings.Dtos;
using Test.Chapter09Listings.EfCode;
using Xunit;
using Xunit.Abstractions;
using Xunit.Extensions.AssertExtensions;

namespace test.UnitTests.DataLayer
{
    public class Ch09_RawSqlCommands
    {
        private readonly ITestOutputHelper _output;

        private DbContextOptions<EfCoreContext> _options;

        public Ch09_RawSqlCommands(ITestOutputHelper output)
        {
            _output = output;

            var connection = this.GetUniqueDatabaseConnectionString();
            var optionsBuilder =
                new DbContextOptionsBuilder<EfCoreContext>();

            optionsBuilder.UseSqlServer(connection);
            _options = optionsBuilder.Options;

            using (var context = new EfCoreContext(_options))
            {
                if (context.Database.EnsureCreated())
                {
                    context.AddUpdateSqlProcs();
                    context.SeedDatabaseFourBooks();
                }
            }
        }

        [Fact]
        public void TestCheckProcExistsOk()
        {
            //SETUP
            using (var context = new EfCoreContext(_options))
            {
                //ATTEMPT
                context.AddUpdateSqlProcs();

                //VERIFY
                context.EnsureSqlProcsSet().ShouldBeTrue();
            }
        }

        [Fact]
        public void TestFromSqlOk()
        {
            //SETUP
            using (var context = new EfCoreContext(_options))
            {
                //ATTEMPT
                var books = context.Books
                    .FromSql($"EXECUTE dbo.{RawSqlHelpers.FilterOnReviewRank} @RankFilter = 1")
                    .ToList();

                //VERIFY
                books.Count.ShouldEqual(1);
                books.First().Title.ShouldEqual("Quantum Networking");
            }
        }

        [Fact]
        public void TestSqlToNonEntityClassOk()
        {
            //SETUP
            using (var context = new EfCoreContext(_options))
            {
                //ATTEMPT
                var bookDtos = new List<RawSqlDto>();
                var conn = context.Database.GetDbConnection();
                try
                {
                    conn.Open();
                    using (var command = conn.CreateCommand())
                    {
                        string query = "SELECT BookId, Title, "
                                       + "dbo.udf_AverageVotes(BookId) AS AverageVotes "
                                       + "FROM Books";
                        command.CommandText = query;
                        DbDataReader reader = command.ExecuteReader();
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                var row = new RawSqlDto
                                {
                                    BookId = reader.GetInt32(0), 
                                    Title = reader.GetString(1),
                                    AverageVotes = reader.GetDecimal(2)
                                };
                                bookDtos.Add(row);
                            }
                        }
                        reader.Dispose();
                    }
                }
                finally
                {
                    conn.Close();
                }

                //VERIFY
                bookDtos.Count.ShouldEqual(4);
                bookDtos.First().AverageVotes.ShouldEqual(-1);
                bookDtos.Last().AverageVotes.ShouldEqual(5);
            }
        }

        [Fact]
        public void TestExecuteSqlCommandOk()
        {
            //SETUP
            using (var context = new EfCoreContext(_options))
            {
                var quantumBookId = context.Books.
                    Single(x => x.Title == "Quantum Networking").BookId;
                var uniqueString = Guid.NewGuid().ToString();

                //ATTEMPT
                var rowsAffected = context.Database
                    .ExecuteSqlCommand(
                        "UPDATE Books SET Description = {0} WHERE BookId = {1}",
                        uniqueString, quantumBookId);

                //VERIFY
                rowsAffected.ShouldEqual(1);
                context.Books.AsNoTracking().Single(x => x.BookId == quantumBookId).Description.ShouldEqual(uniqueString);
            }
        }


    }
}