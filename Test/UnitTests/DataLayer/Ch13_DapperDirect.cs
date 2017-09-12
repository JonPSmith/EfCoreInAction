// Copyright (c) 2017 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System.Data.SqlClient;
using System.Linq;
using Dapper;
using DataLayer.EfClasses;
using DataLayer.EfCode;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Logging;
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
                var books = context.Database.GetDbConnection().Query<Book>("SELECT * FROM Books").ToList();

                //VERIFY
                books.Count.ShouldEqual(4);
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
    }
}