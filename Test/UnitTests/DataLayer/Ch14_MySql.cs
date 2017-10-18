// Copyright (c) 2016 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using DataLayer.EfClasses;
using DataLayer.EfCode;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;
using test.Attributes;
using test.EfHelpers;
using test.Helpers;
using Xunit;
using Xunit.Abstractions;
using Xunit.Extensions.AssertExtensions;

namespace test.UnitTests.DataLayer
{
    public class Ch14_MySql
    {
        private readonly ITestOutputHelper _output;

        private readonly string _connection;

        public Ch14_MySql(ITestOutputHelper output)
        {
            _output = output;
            _connection = AppSettings.GetConfiguration().GetConnectionString("MySqlDatabaseUnitTest");
        }

        [RunnableInDebugOnly]
        public void TestMySqlDatabaseExists()
        {
            //SETUP
            var optionsBuilder =
                new DbContextOptionsBuilder<EfCoreContext>();
            optionsBuilder.UseMySql(_connection, mysqlOptions => mysqlOptions.MaxBatchSize(1)); //Needed to overcome https://github.com/PomeloFoundation/Pomelo.EntityFrameworkCore.MySql/issues/397

            //ATTEMPT
            using (var context = new EfCoreContext(optionsBuilder.Options))
            {
                var exists = (context.GetService<IDatabaseCreator>() as RelationalDatabaseCreator).Exists();
                
                //VERIFY
                exists.ShouldBeTrue();
            }
        }

        [Fact]
        public void TestMySqlDatabaseCreate()
        {
            //SETUP
            var optionsBuilder =
                new DbContextOptionsBuilder<EfCoreContext>();
            optionsBuilder.UseMySql(_connection, mysqlOptions => mysqlOptions.MaxBatchSize(1));

            //ATTEMPT
            using (var context = new EfCoreContext(optionsBuilder.Options))
            {
                var logIt = new LogDbContext(context);
                context.Database.EnsureCreated();

                //VERIFY
                foreach (var log in logIt.Logs)
                {
                    _output.WriteLine(log);
                }
            }
        }

        [RunnableInDebugOnly]
        public void TestMySqlDatabaseDeleteAndCreate()
        {
            //SETUP
            var optionsBuilder =
                new DbContextOptionsBuilder<EfCoreContext>();
            optionsBuilder.UseMySql(_connection, mysqlOptions => mysqlOptions.MaxBatchSize(1));

            //ATTEMPT
            using (var context = new EfCoreContext(optionsBuilder.Options))
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();

                //VERIFY
                var exists = (context.GetService<IDatabaseCreator>() as RelationalDatabaseCreator).Exists();
                exists.ShouldBeTrue();
            }
        }

        [Fact]
        public void TestMySqlAddBook()
        {
            //SETUP
            var optionsBuilder =
                new DbContextOptionsBuilder<EfCoreContext>();
            optionsBuilder.UseMySql(_connection, mysqlOptions => mysqlOptions.MaxBatchSize(1));
            optionsBuilder.EnableSensitiveDataLogging();

            using (var context = new EfCoreContext(optionsBuilder.Options))
            {
                context.Database.EnsureCreated();
                var logIt = new LogDbContext(context);
                var existingBooks = context.Books.Count();

                //ATTEMPT
                context.Add(new Book {Title = "Hello"});
                context.SaveChanges();

                //VERIFY
                context.Books.Count().ShouldEqual(existingBooks + 1);
            }
        }

        [Fact]
        public void TestMySqlAddBookWithReview()
        {
            //SETUP
            var optionsBuilder =
                new DbContextOptionsBuilder<EfCoreContext>();
            optionsBuilder.UseMySql(_connection, mysqlOptions => mysqlOptions.MaxBatchSize(1));
            optionsBuilder.EnableSensitiveDataLogging();

            using (var context = new EfCoreContext(optionsBuilder.Options))
            {
                context.Database.EnsureCreated();
                var logIt = new LogDbContext(context);
                var existingBooks = context.Books.Count();

                //ATTEMPT
                context.Add(new Book
                {
                    Title = "Hello" ,
                    Reviews = new List<Review> { new Review {  NumStars = 5} }
                });
                context.SaveChanges();

                //VERIFY
                context.Books.Count().ShouldEqual(existingBooks + 1);
            }
        }

        [Fact]
        public void TestMySqlAddBookWithTwoReview()
        {
            //SETUP
            var optionsBuilder =
                new DbContextOptionsBuilder<EfCoreContext>();
            optionsBuilder.UseMySql(_connection, mysqlOptions => mysqlOptions.MaxBatchSize(1));
            optionsBuilder.EnableSensitiveDataLogging();

            using (var context = new EfCoreContext(optionsBuilder.Options))
            {
                context.Database.EnsureCreated();
                var logIt = new LogDbContext(context);
                var existingBooks = context.Books.Count();

                //ATTEMPT
                context.Add(new Book
                {
                    Title = "Hello",
                    Reviews = new List<Review> { new Review { NumStars = 5 }, new Review{ NumStars = 1}}
                });
                context.SaveChanges();

                //VERIFY
                context.Books.Count().ShouldEqual(existingBooks + 1);
            }
        }

        [Fact]
        public void TestMySqlWipeDatabase()
        {
            //SETUP
            var optionsBuilder =
                new DbContextOptionsBuilder<EfCoreContext>();
            optionsBuilder.UseMySql(_connection, mysqlOptions => mysqlOptions.MaxBatchSize(1));
            optionsBuilder.EnableSensitiveDataLogging();

            using (var context = new EfCoreContext(optionsBuilder.Options))
            {
                context.Database.EnsureCreated();

                //ATTEMPT
                context.WipeAllDataFromDatabase(true);
;
                //VERIFY
                context.Books.Count().ShouldEqual(0);
            }
        }

        [Fact]
        public void TestMySqlCreateBooks()
        {
            //SETUP
            var optionsBuilder =
                new DbContextOptionsBuilder<EfCoreContext>();
            optionsBuilder.UseMySql(_connection, 
                mysqlOptions => mysqlOptions.MaxBatchSize(1)); //Needed to overcome https://github.com/PomeloFoundation/Pomelo.EntityFrameworkCore.MySql/issues/397
            optionsBuilder.EnableSensitiveDataLogging();

            using (var context = new EfCoreContext(optionsBuilder.Options))
            {
                context.Database.EnsureCreated();
                var logIt = new LogDbContext(context);
                var existingBooks = context.Books.Count();

                //ATTEMPT
                context.SeedDatabaseFourBooks();

                //VERIFY
                context.Books.Count().ShouldEqual(existingBooks + 4);
            }
        }

    }


}