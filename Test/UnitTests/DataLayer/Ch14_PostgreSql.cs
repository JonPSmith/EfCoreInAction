// Copyright (c) 2017 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using DataLayer.EfClasses;
using DataLayer.EfCode;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using test.Attributes;
using test.EfHelpers;
using test.Helpers;
using Xunit;
using Xunit.Abstractions;
using Xunit.Extensions.AssertExtensions;

namespace Test.UnitTests.DataLayer
{
    public class Ch14_PostgreSql
    {
        private readonly ITestOutputHelper _output;


        public Ch14_PostgreSql(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void TestPostgreSqlDatabaseExists()
        {
            //SETUP
            var connection = AppSettings.GetConfiguration()["PostgreSQL-Azure-Test"];
            var optionsBuilder =
                new DbContextOptionsBuilder<EfCoreContext>();
            optionsBuilder.UseNpgsql(connection);

            //ATTEMPT
            using (var context = new EfCoreContext(optionsBuilder.Options))
            {
                var exists = (context.GetService<IDatabaseCreator>() as RelationalDatabaseCreator).Exists();

                //VERIFY
                exists.ShouldBeTrue();
            }
        }

        [Fact]
        public void TestDatabaseCreate()
        {
            //SETUP
            var connection = AppSettings.GetConfiguration()["PostgreSQL-Azure-Test"];
            var optionsBuilder =
                new DbContextOptionsBuilder<EfCoreContext>();
            optionsBuilder.UseNpgsql(connection);

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

        [Fact]
        public void TestPostgreSqlAddBook()
        {
            //SETUP
            var connection = AppSettings.GetConfiguration()["PostgreSQL-Azure-Test"];
            var optionsBuilder =
                new DbContextOptionsBuilder<EfCoreContext>();
            optionsBuilder.UseNpgsql(connection, b => b.MigrationsAssembly("DataLayer"));
            optionsBuilder.EnableSensitiveDataLogging();

            using (var context = new EfCoreContext(optionsBuilder.Options))
            {
                context.Database.EnsureCreated();
                var logIt = new LogDbContext(context);
                var existingBooks = context.Books.Count();

                //ATTEMPT
                context.Add(new Book { Title = "Hello" });
                context.SaveChanges();

                //VERIFY
                context.Books.Count().ShouldEqual(existingBooks + 1);
            }
        }

        [Fact]
        public void TestPostgreSqlAddBookWithReview()
        {
            //SETUP
            var connection = AppSettings.GetConfiguration()["PostgreSQL-Azure-Test"];
            var optionsBuilder =
                new DbContextOptionsBuilder<EfCoreContext>();
            optionsBuilder.UseNpgsql(connection, b => b.MigrationsAssembly("DataLayer"));
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
                    Reviews = new List<Review> { new Review { NumStars = 5 } }
                });
                context.SaveChanges();

                //VERIFY
                context.Books.Count().ShouldEqual(existingBooks + 1);
            }
        }

        [Fact]
        public void TestPostgreSqlAddBookWithTwoReview()
        {
            //SETUP
            var connection = AppSettings.GetConfiguration()["PostgreSQL-Azure-Test"];
            var optionsBuilder =
                new DbContextOptionsBuilder<EfCoreContext>();
            optionsBuilder.UseNpgsql(connection, b => b.MigrationsAssembly("DataLayer"));
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
                    Reviews = new List<Review> { new Review { NumStars = 5 }, new Review { NumStars = 1 } }
                });
                context.SaveChanges();

                //VERIFY
                context.Books.Count().ShouldEqual(existingBooks + 1);
            }
        }

        [Fact]
        public void TestPostgreSqlCreateBooks()
        {
            //SETUP
            var connection = AppSettings.GetConfiguration()["PostgreSQL-Azure-Test"];
            var optionsBuilder =
                new DbContextOptionsBuilder<EfCoreContext>();
            optionsBuilder.UseNpgsql(connection);
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

        [RunnableInDebugOnly]
        public void TestPostgreSqlMigrate()
        {
            //SETUP
            var connection = AppSettings.GetConfiguration()["PostgreSQL-Azure-Test"];
            var optionsBuilder =
                new DbContextOptionsBuilder<EfCoreContext>();
            optionsBuilder.UseNpgsql(connection);
            optionsBuilder.EnableSensitiveDataLogging();

            using (var context = new EfCoreContext(optionsBuilder.Options))
            {
                //context.Database.EnsureDeleted();

                //ATTEMPT
                context.Database.Migrate();

                //VERIFY

            }
        }
    }
}