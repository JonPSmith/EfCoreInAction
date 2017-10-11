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


        public Ch14_MySql(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void TestMySqlDatabaseExists()
        {
            //SETUP
            var connection = AppSettings.GetConfiguration()["MySql-Test"];
            var optionsBuilder =
                new DbContextOptionsBuilder<EfCoreContext>();
            optionsBuilder.UseMySql(connection);

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
            var connection = AppSettings.GetConfiguration()["MySql-Test"];
            var optionsBuilder =
                new DbContextOptionsBuilder<EfCoreContext>();
            optionsBuilder.UseMySql(connection, b => b.MigrationsAssembly("DataLayer"));

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
        public void TestMySqlAddBook()
        {
            //SETUP
            var connection = AppSettings.GetConfiguration()["MySql-Test"];
            var optionsBuilder =
                new DbContextOptionsBuilder<EfCoreContext>();
            optionsBuilder.UseMySql(connection, b => b.MigrationsAssembly("DataLayer"));
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
            var connection = AppSettings.GetConfiguration()["MySql-Test"];
            var optionsBuilder =
                new DbContextOptionsBuilder<EfCoreContext>();
            optionsBuilder.UseMySql(connection, b => b.MigrationsAssembly("DataLayer"));
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
            var connection = AppSettings.GetConfiguration()["MySql-Test"];
            var optionsBuilder =
                new DbContextOptionsBuilder<EfCoreContext>();
            optionsBuilder.UseMySql(connection, b => b.MigrationsAssembly("DataLayer"));
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
            var connection = AppSettings.GetConfiguration()["MySql-Test"];
            var optionsBuilder =
                new DbContextOptionsBuilder<EfCoreContext>();
            optionsBuilder.UseMySql(connection, b => b.MigrationsAssembly("DataLayer"));
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
            var connection = AppSettings.GetConfiguration()["MySql-Test"];
            var optionsBuilder =
                new DbContextOptionsBuilder<EfCoreContext>();
            optionsBuilder.UseMySql(connection, mysqlOptions => mysqlOptions.MaxBatchSize(1));
            optionsBuilder.EnableSensitiveDataLogging();

            using (var context = new EfCoreContext(optionsBuilder.Options))
            {
                context.Database.EnsureCreated();
                var logIt = new LogDbContext(context);
                var existingBooks = context.Books.Count();

                //ATTEMPT
                context.SeedDatabaseFourBooks();
                //var martinFowler = new Author
                //{
                //    Name = "Martin Fowler"
                //};

                //var books = new List<Book>();

                //var book2 = new Book
                //{
                //    Title = "Patterns of Enterprise Application Architecture",
                //    Description = "Written in direct response to the stiff challenges",
                //    PublishedOn = new DateTime(2002, 11, 15),
                //    Price = 53
                //};
                //book2.AuthorsLink = new List<BookAuthor> { new BookAuthor { Author = martinFowler, Book = book2 } };
                //books.Add(book2);

                //var book3 = new Book
                //{
                //    Title = "Domain-Driven Design",
                //    Description = "Linking business needs to software design",
                //    PublishedOn = new DateTime(2003, 8, 30),
                //    Price = 56
                //};
                //book3.AuthorsLink = new List<BookAuthor> { new BookAuthor { Author = new Author { Name = "Eric Evans" }, Book = book3 } };
                //books.Add(book3);

                //var book4 = new Book
                //{
                //    Title = "Quantum Networking",
                //    Description = "Entangled quantum networking provides faster-than-light data communications",
                //    PublishedOn = new DateTime(2057, 1, 1),
                //    Price = 220
                //};
                //book4.AuthorsLink = new List<BookAuthor> { new BookAuthor { Author = new Author { Name = "Future Person" }, Book = book4 } };
                //book4.Reviews = new List<Review>
                //    {
                //        new Review { VoterName = "Jon P Smith", NumStars = 5, Comment = "I look forward to reading this book, if I am still alive!"},
                //        new Review { VoterName = "Albert Einstein", NumStars = 5, Comment = "I write this book if I was still alive!"}
                //    };
                //book4.Promotion = new PriceOffer { NewPrice = 219, PromotionalText = "Save $1 if you order 40 years ahead!" };
                //books.Add(book4);
                //context.AddRange(books);
                //context.SaveChanges();

                //VERIFY
                context.Books.Count().ShouldEqual(existingBooks + 4);
            }
        }

    }


}