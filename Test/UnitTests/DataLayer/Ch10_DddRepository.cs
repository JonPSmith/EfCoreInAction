// Copyright (c) 2016 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using test.EfHelpers;
using Test.Chapter10Listings.EfClasses;
using Test.Chapter10Listings.EfCode;
using Xunit;
using Xunit.Abstractions;
using Xunit.Extensions.AssertExtensions;

namespace test.UnitTests.DataLayer
{
    public class Ch10_DddRepository 
    {
        private readonly ITestOutputHelper _output;

        public Ch10_DddRepository(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void CreateBookDddWithNewAuthor()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<Chapter10DbContext>();
            using (var context = new Chapter10DbContext(options))
            {
                var logs = new List<string>();
                SqliteInMemory.SetupLogging(context, logs);
                context.Database.EnsureCreated();

                //ATTEMPT
                var book = new BookDdd("My Book", null, new DateTime(2000, 1, 1), "Publisher", 123, null,
                    new List<AuthorDdd> {new AuthorDdd {Name = "Person"}});
                book.AddBook(context);
                context.SaveChanges();

                //VERIFY
                book.BookId.ShouldNotEqual(0);
                book.AuthorsLink.Count().ShouldEqual(1);
            }
            using (var context = new Chapter10DbContext(options))
            {
                var book = context.Books
                    .Include(p => p.AuthorsLink).ThenInclude(p => p.Author)
                    .Single();

                book.AuthorsLink.Count().ShouldEqual(1);
                book.AuthorsLink.First().Author?.Name.ShouldEqual("Person");

            }
        }

        [Fact]
        public void CreateBookDddWithExistingAuthor()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<Chapter10DbContext>();
            using (var context = new Chapter10DbContext(options))
            {
                context.Database.EnsureCreated();
                var author = new AuthorDdd {Name = "Existing"};
                context.Add(author);
                context.SaveChanges();

                //ATTEMPT
                var book = new BookDdd("My Book", null, new DateTime(2000, 1, 1), "Publisher", 123, null,
                    new List<AuthorDdd> { author });
                book.AddBook(context);
                context.SaveChanges();

                //VERIFY
                book.BookId.ShouldNotEqual(0);
                book.AuthorsLink.Count().ShouldEqual(1);
            }
            using (var context = new Chapter10DbContext(options))
            {
                var book = context.Books
                    .Include(p => p.AuthorsLink).ThenInclude(p => p.Author)
                    .Single();

                context.Set<AuthorDdd>().Count().ShouldEqual(1);
                book.AuthorsLink.Count().ShouldEqual(1);
                book.AuthorsLink.First().Author?.Name.ShouldEqual("Existing");

            }
        }

        [Fact]
        public void BookDddAddPromotion()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<Chapter10DbContext>();
            using (var context = new Chapter10DbContext(options))
            {
                context.Database.EnsureCreated();

                var book = new BookDdd("My Book", null, new DateTime(2000, 1, 1), "Publisher", 123, null,
                    new List<AuthorDdd> { new AuthorDdd { Name = "Person" } });
                book.AddBook(context);
                context.SaveChanges();

                //ATTEMPT
                book.AddUpdatePromotion(context, 456, "Message");
                context.SaveChanges();
            }
            using (var context = new Chapter10DbContext(options))
            {
                //VERIFY
                var book = context.Books
                    .Include(p => p.Promotion)
                    .Single();
                book.Promotion.ShouldNotBeNull();
                book.Promotion.NewPrice.ShouldEqual(456);
            }
        }

        [Fact]
        public void BookDddUpdatePromotion()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<Chapter10DbContext>();
            using (var context = new Chapter10DbContext(options))
            {
                context.Database.EnsureCreated();

                var book = new BookDdd("My Book", null, new DateTime(2000, 1, 1), "Publisher", 123, null,
                    new List<AuthorDdd> { new AuthorDdd { Name = "Person" } });
                book.AddBook(context);
                context.SaveChanges();
                book.AddUpdatePromotion(context, 321, "Existing");
                context.SaveChanges();

                //ATTEMPT
                book.AddUpdatePromotion(context, 456, "Message");
                context.SaveChanges();
            }
            using (var context = new Chapter10DbContext(options))
            {
                //VERIFY
                var book = context.Books
                    .Include(p => p.Promotion)
                    .Single();
                book.Promotion.ShouldNotBeNull();
                book.Promotion.NewPrice.ShouldEqual(456);
            }
        }

        [Fact]
        public void BookDddRemovePromotion()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<Chapter10DbContext>();
            using (var context = new Chapter10DbContext(options))
            {
                context.Database.EnsureCreated();

                var book = new BookDdd("My Book", null, new DateTime(2000, 1, 1), "Publisher", 123, null,
                    new List<AuthorDdd> { new AuthorDdd { Name = "Person" } });
                book.AddBook(context);
                context.SaveChanges();
                book.AddUpdatePromotion(context, 321, "Existing");
                context.SaveChanges();

                //ATTEMPT
                book.RemovePromotion(context);
                context.SaveChanges();
            }
            using (var context = new Chapter10DbContext(options))
            {
                //VERIFY
                var book = context.Books
                    .Include(p => p.Promotion)
                    .Single();
                book.Promotion.ShouldBeNull();
            }
        }

        [Fact]
        public void BookDddAddReviewBeforeSave()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<Chapter10DbContext>();
            using (var context = new Chapter10DbContext(options))
            {
                context.Database.EnsureCreated();

                //ATTEMPT
                var book = new BookDdd("My Book", null, new DateTime(2000, 1, 1), "Publisher", 123, null,
                    new List<AuthorDdd> { new AuthorDdd { Name = "Person" } });
                book.AddBook(context);
                book.AddReview(context, 5, "Great book", "Person");
                context.SaveChanges();
            }
            using (var context = new Chapter10DbContext(options))
            {
                //VERIFY
                var book = context.Books
                    .Include(p => p.Reviews)
                    .Single();
                book.Reviews.Count().ShouldEqual(1);
                book.Reviews.First().NumStars.ShouldEqual(5);
            }
        }
    }
}
