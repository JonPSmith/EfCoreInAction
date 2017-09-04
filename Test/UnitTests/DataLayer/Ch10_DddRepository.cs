// Copyright (c) 2016 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using test.EfHelpers;
using Test.Chapter10Listings.EfClasses;
using Test.Chapter10Listings.EfCode;
using Test.Chapter10Listings.QueryObjects;
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
                
                var dddRepro = new BookDddRepository(context);

                //ATTEMPT
                var book = new BookDdd("My Book", null, new DateTime(2000, 1, 1), "Publisher", 123, null,
                    new List<AuthorDdd> {new AuthorDdd {Name = "Person"}});
                dddRepro.AddBook(book);
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
                var dddRepro = new BookDddRepository(context);
                var author = new AuthorDdd {Name = "Existing"};
                context.Add(author);
                context.SaveChanges();

                //ATTEMPT
                var book = new BookDdd("My Book", null, new DateTime(2000, 1, 1), "Publisher", 123, null,
                    new List<AuthorDdd> { author });
                dddRepro.AddBook(book);
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
        public void BookDddUpdatePublishedOn()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<Chapter10DbContext>();
            int bookId;
            using (var context = new Chapter10DbContext(options))
            {
                context.Database.EnsureCreated();
                var dddRepro = new BookDddRepository(context);

                var book = new BookDdd("My Book", null, new DateTime(2000, 1, 1), "Publisher", 123, null,
                    new List<AuthorDdd> { new AuthorDdd { Name = "Person" } });
                dddRepro.AddBook(book);
                book.AddReview(context, 5, "Great book", "Person");
                context.SaveChanges();
                bookId = book.BookId;
            }
            using (var context = new Chapter10DbContext(options))
            {
                var newDate = new DateTime(2010,1,1);

                //ATTEMPT
                var dddRepro = new BookDddRepository(context);
                var book = dddRepro.FindBook(bookId);
                book.ChangePubDate(newDate);
                context.SaveChanges();

                //VERIFY
                var rereadBook = context.Books.AsNoTracking().Single(x => x.BookId == bookId);
                rereadBook.PublishedOn.ShouldEqual(new DateTime(2010, 1, 1));

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
                var dddRepro = new BookDddRepository(context);

                var book = new BookDdd("My Book", null, new DateTime(2000, 1, 1), "Publisher", 123, null,
                    new List<AuthorDdd> { new AuthorDdd { Name = "Person" } });
                dddRepro.AddBook(book);
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
                
                var dddRepro = new BookDddRepository(context);

                var book = new BookDdd("My Book", null, new DateTime(2000, 1, 1), "Publisher", 123, null,
                    new List<AuthorDdd> { new AuthorDdd { Name = "Person" } });
                dddRepro.AddBook(book);
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
                var dddRepro = new BookDddRepository(context);

                var book = new BookDdd("My Book", null, new DateTime(2000, 1, 1), "Publisher", 123, null,
                    new List<AuthorDdd> { new AuthorDdd { Name = "Person" } });
                dddRepro.AddBook(book);
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
                var dddRepro = new BookDddRepository(context);

                //ATTEMPT
                var book = new BookDdd("My Book", null, new DateTime(2000, 1, 1), "Publisher", 123, null,
                    new List<AuthorDdd> { new AuthorDdd { Name = "Person" } });
                dddRepro.AddBook(book);
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

        [Fact]
        public void GetBookList()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<Chapter10DbContext>();
            using (var context = new Chapter10DbContext(options))
            {
                context.Database.EnsureCreated();
                
                var dddRepro = new BookDddRepository(context);

                var book1 = new BookDdd("Book1", null, new DateTime(2001, 1, 1), "Publisher", 100, null,
                    new List<AuthorDdd> { new AuthorDdd { Name = "Author1" } });
                dddRepro.AddBook(book1);
                var book2 = new BookDdd("Book2", null, new DateTime(2002, 1, 1), "Publisher", 200, null,
                    new List<AuthorDdd> { new AuthorDdd { Name = "Author2" } });
                dddRepro.AddBook(book2);
                var book3 = new BookDdd("Book3", null, new DateTime(2002, 1, 1), "Publisher", 300, null,
                    new List<AuthorDdd> { new AuthorDdd { Name = "Author3" } });
                dddRepro.AddBook(book3);
                book3.AddReview(context, 5, "Great", "Reviewer");
                context.SaveChanges();

                //ATTEMPT
                var dtos = dddRepro.GetBookList(new DddSortFilterPageOptions()).ToList();

                //VERIFY
                dtos.Count.ShouldEqual(3);
                dtos.All(x => x.Title.StartsWith("Book")).ShouldBeTrue();
                dtos.OrderBy(x => x.Title).Select(x => x.Price).ShouldEqual(new List<decimal>{100, 200, 300});
            }

        }
    }
}
