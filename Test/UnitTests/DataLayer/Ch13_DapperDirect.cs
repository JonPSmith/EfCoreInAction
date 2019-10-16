// Copyright (c) 2017 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using Dapper;
using DataLayer.EfClasses;
using DataLayer.EfCode;
using Microsoft.Data.SqlClient;
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

    }
}