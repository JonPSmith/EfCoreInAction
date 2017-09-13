// Copyright (c) 2017 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using Dapper;
using DataLayer.EfClasses;
using DataLayer.EfCode;
using Microsoft.EntityFrameworkCore;
using ServiceLayer.BookServices;
using test.Attributes;
using test.EfHelpers;
using test.Helpers;
using Xunit;
using Xunit.Abstractions;
using Xunit.Extensions.AssertExtensions;

namespace test.UnitTests.DataLayer
{
    public class Ch13_DapperDirect
    {
        private readonly string _connectionString;
        private readonly ITestOutputHelper _output;

        public Ch13_DapperDirect(ITestOutputHelper output)
        {
            _output = output;
            _connectionString = this.GetUniqueDatabaseConnectionString();
            var optionsBuilder =
                new DbContextOptionsBuilder<EfCoreContext>();

            optionsBuilder.UseSqlServer(_connectionString);
            var options = optionsBuilder.Options;
            using (var context = new EfCoreContext(options))
            {
                if (context.Database.EnsureCreated())
                    context.SeedDatabaseFourBooks();
            }
        }

        [Fact]
        public void DapperReadBooks()
        {
            //SETUP 

            using (var con = new SqlConnection(_connectionString))
            {
                //ATTEMPT
                var books = con.Query<Book>("SELECT * FROM Books").ToList();

                //VERIFY
                books.Count.ShouldEqual(4);
            }
        }

        [Fact]
        public void DapperReadBooksEfCoreSqlServer()
        {
            //SETUP 
            var optionsBuilder =
                new DbContextOptionsBuilder<EfCoreContext>();

            optionsBuilder.UseSqlServer(_connectionString);
            var options = optionsBuilder.Options;
            using (var context = new EfCoreContext(options))
            {
                //ATTEMPT
                var books = context.Database //#A
                    .GetDbConnection() //#B
                    .Query<Book>("SELECT * FROM Books"); //#C

                /*********************************************
                #A I can use the application's DbContext to run the Dapper query
                #B I need to get a DbConnection for EF Core, as that is what Dapper needs to access the database
                #C Here is the Dapper call. It will execute the SQL code provide as the first parameter and Dapper will then map the results to the type I supplied, in this case Book. 
                 * * *********************************************/

                //VERIFY
                books.Count().ShouldEqual(4);
            }
        }

        [Fact]
        public void DapperReadBooksEfCoreSqlite()
        {
            //SETUP 
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();

            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureCreated();
                context.SeedDatabaseFourBooks();

                //ATTEMPT
                var books = context.Database.GetDbConnection().Query<Book>("SELECT * FROM Books").ToList();

                //VERIFY
                books.Count.ShouldEqual(4);
            }
        }

        [RunnableInDebugOnly]
        public void DapperEfCorePerformance()
        {
            //SETUP 
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();

            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureCreated();
                context.SeedDatabaseDummyBooks(500);

                //ATTEMPT
                RunTest(context, 1, "First access, EFCore:", (c, i) => c.Books.AsNoTracking().Single(x => x.BookId == i+1));
                RunTest(context, 1, "Second access, EFCore", (c, i) => c.Books.AsNoTracking().Single(x => x.BookId == i + 1));
                RunTest(context, 500, "Multi access, EFCore", (c, i) => c.Books.AsNoTracking().Single(x => x.BookId == i + 1));

                RunTest(context, 1, "First access, Dapper:", (c, i) => c.Database.GetDbConnection()
                    .Query<Book>("SELECT * FROM Books WHERE BookId = @i", new {i = i+1}).First());
                RunTest(context, 1, "First access, Dapper:", (c, i) => c.Database.GetDbConnection()
                    .Query<Book>("SELECT * FROM Books WHERE BookId = @i", new { i = i + 1 }).First());
                RunTest(context, 500, "First access, Dapper:", (c, i) => c.Database.GetDbConnection()
                    .Query<Book>("SELECT * FROM Books WHERE BookId = @i", new { i = i + 1 }).First());

                //VERIFY
            }
        }

        private void RunTest(EfCoreContext context, int numCyclesToRun, string testType, Action<EfCoreContext, int> actionToRun)
        {
            var timer = new Stopwatch();
            timer.Start();
            for (int i = 0; i < numCyclesToRun; i++)
            {
                actionToRun(context, i);
            }
            timer.Stop();
            _output.WriteLine("Ran {0}: total time = {1} ms ({2:f2} ms per action)", testType,
                timer.ElapsedMilliseconds,
                timer.ElapsedMilliseconds / ((double)numCyclesToRun));
        }
    }
}