// Copyright (c) 2017 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using DataLayer.EfClasses;
using DataLayer.EfCode;
using Microsoft.EntityFrameworkCore;
using test.EfHelpers;
using test.Helpers;
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

        private readonly DbContextOptions<EfCoreContext> _options;

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
                const int rankFilterBy = 5;
                var books = context.Books //#A
                    .FromSql( //#B
                        "EXECUTE dbo.FilterOnReviewRank " + //#C
                        $"@RankFilter = {rankFilterBy}") //#D
                    .ToList();

                /***********************************************************
                #A I start the query in the normal way, with the DbSet<T> I want to read
                #B The FromSql method then allows me to insert a SQL command. This MUST return all the columns of the entity type T, that the DbSet<T> property has - in this case the Book entity class
                #C Here I execute a stored procedure that I added to the database outside of the normal EF Core database creation system
                #D I use C#7's string interpolation feature to provide the parameter. EF Core intercepts the string interpolation and turns it into a SQL parameter with checks against common SQL injection mistakes/security issues
                 * ********************************************************/

                //VERIFY
                books.Count.ShouldEqual(1);
                books.First().Title.ShouldEqual("Quantum Networking");
            }
        }

        [Fact]
        public void TestFromSqlWithIncludeOk()
        {
            //SETUP
            using (var context = new EfCoreContext(_options))
            {
                //ATTEMPT
                const int rankFilterBy = 5;
                var books = context.Books
                    .FromSql(
                       @"SELECT * FROM Books b WHERE 
                        (SELECT AVG(CAST([NumStars] AS float)) 
                           FROM dbo.Review AS r " + //#A
                           $"WHERE b.BookId = r.BookId) >= {rankFilterBy}") //#B
                    .Include(r => r.Reviews) //#B
                    .ToList();

                /**************************************************************
                #A In this case I write some SQL to calculate the average votes and I then use that result in a outer WHERE test
                #B I make the last line of the command into a string interpolation and add the parameter by name
                #B The Include method works with the FromSql because I am not executing a store procedure
                 * ****************************************************************/

                //VERIFY
                books.Count.ShouldEqual(1);
                books.First().Title.ShouldEqual("Quantum Networking");
                books.First().Reviews.Count.ShouldEqual(2);
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
                var conn = context.Database.GetDbConnection(); //#A
                try
                {
                    conn.Open(); //#B
                    using (var command = conn.CreateCommand())//#C
                    {
                        string query = "SELECT BookId, Title, " + //#D
                            "dbo.udf_AverageVotes(BookId) AS AverageVotes " //#D
                            + "FROM Books"; //#D
                        command.CommandText = query; //#E

                        using (DbDataReader reader = command.ExecuteReader()) //F
                        {
                            while (reader.Read()) //#G
                            {
                                var row = new RawSqlDto
                                {
                                    BookId = reader.GetInt32(0), //#H
                                    Title = reader.GetString(1), //#H
                                    AverageVotes = reader.GetDecimal(2) //#H
                                };
                                bookDtos.Add(row);
                            }
                        }
                    }
                }
                finally
                {
                    conn.Close(); //#I
                }
                /****************************************************************
                #A I ask EF Core for a DbConnection, which the low-level SqlClient library can use 
                #B I need to open the connection before I use it
                #C I create a DbCommand on that connection
                #D This library transfers SQL directly to the database server, hence all the database accesses have to be defined in SQL
                #E I assign my command to the DbCommand instance
                #F The ExecuteReader method sends the SQL command to the database server and then creates a reader to read the data that the server will return
                #G This tries to reaad the next row and returns true if it was successful
                #H I have to hand-code the conversion and copying of the data from the reader into my class
                #I When the read has finished I need to close the connection to the database server
                 * ******************************************************************/


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
                var bookId = context.Books.
                    Single(x => x.Title == "Quantum Networking").BookId;
                var uniqueString = Guid.NewGuid().ToString();

                //ATTEMPT
                var rowsAffected = context.Database //#A
                    .ExecuteSqlCommand( //#B
                        "UPDATE Books " + //#C
                        "SET Description = {0} " +
                        "WHERE BookId = {1}",
                        uniqueString, bookId); //#D
                /*********************************************************
                #A The ExecuteSqlCommand can be found in the context.Database property
                #B The ExecuteSqlCommand will execute the SQL and return an integer, which in this case is the number of rows updated
                #D I provide two parameters which referred to in the command
                 * **********************************************************/

                //VERIFY
                rowsAffected.ShouldEqual(1);
                context.Books.AsNoTracking().Single(x => x.BookId == bookId).Description.ShouldEqual(uniqueString);
            }
        }

        [Fact]
        public void TestReloadOk()
        {
            //SETUP
            using (var context = new EfCoreContext(_options))
            {
                var entity = context.Books. //#A
                    Single(x => x.Title == "Quantum Networking");
                var uniqueString = Guid.NewGuid().ToString();

                context.Database.ExecuteSqlCommand( //#B
                        "UPDATE Books " + 
                        "SET Description = {0} " +
                        "WHERE BookId = {1}",
                        uniqueString, entity.BookId); 

                //ATTEMPT
                context.Entry(entity).Reload(); //#C

                /*************************************************
                #A I load a Book entity in the normal way
                #B I now use ExecuteSqlCommand to change the Descrinptionb column of that same Book entity. After this command has finished the Book entity EF Core load is out of date
                #C By callin the Reload method EF Core will reread that entity to make sure the local copy is up to date.
                 * **************************************************/
                //VERIFY
                entity.Description.ShouldEqual(uniqueString);
            }
        }

        [Fact]
        public void TestReloadWithChangeOk()
        {
            //SETUP
            using (var context = new EfCoreContext(_options))
            {
                var entity = context.Books.
                    Single(x => x.Title == "Quantum Networking");
                var uniqueString = Guid.NewGuid().ToString();

                var rowsAffected = context.Database 
                    .ExecuteSqlCommand( 
                        "UPDATE Books " + 
                        "SET Description = {0} " +
                        "WHERE BookId = {1}",
                        uniqueString, entity.BookId); 

                //ATTEMPT
                entity.Title = "Changed it";
                context.Entry(entity).Reload();

                //VERIFY
                entity.Description.ShouldEqual(uniqueString);
                entity.Title.ShouldEqual("Quantum Networking");
            }
        }

        [Fact]
        public void TestReloadWithNavigationalChangeOk()
        {
            //SETUP
            using (var context = new EfCoreContext(_options))
            {
                var entity = context.Books
                    .Include(p => p.Reviews)
                    .Single(x => x.Title == "Quantum Networking");
                var uniqueString = Guid.NewGuid().ToString();

                var rowsAffected = context.Database
                    .ExecuteSqlCommand(
                        "UPDATE Books " +
                        "SET Description = {0} " +
                        "WHERE BookId = {1}",
                        uniqueString, entity.BookId);

                //ATTEMPT
                entity.Reviews.Add(new Review{ NumStars = 99});
                context.Entry(entity).Reload();

                //VERIFY
                entity.Description.ShouldEqual(uniqueString);
                entity.Reviews.Count.ShouldEqual(3);
            }
        }

        [Fact]
        public void TestGetDatabaseValuesOk()
        {
            //SETUP
            using (var context = new EfCoreContext(_options))
            {
                var entity = context.Books.
                    Single(x => x.Title == "Quantum Networking");
                var uniqueString = Guid.NewGuid().ToString();

                var rowsAffected = context.Database
                    .ExecuteSqlCommand(
                        "UPDATE Books " +
                        "SET Description = {0} " +
                        "WHERE BookId = {1}",
                        uniqueString, entity.BookId);

                //ATTEMPT
                var values = context.Entry(entity).GetDatabaseValues();
                var book = (Book)values.ToObject();

                //VERIFY
                book.Description.ShouldEqual(uniqueString);
            }
        }
    }
}