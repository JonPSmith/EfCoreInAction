// Copyright (c) 2017 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using test.EfHelpers;
using test.Helpers;
using Test.Chapter07Listings.EfClasses;
using Test.Chapter07Listings.EFCode;
using Xunit;
using Xunit.Extensions.AssertExtensions;

namespace test.UnitTests.DataLayer
{
    public class Ch07_TablePerHierarchy
    {
        [Fact]
        public void TestBookBaseOk()
        {
            //SETUP
            using (var context = new Chapter07DbContext(
                SqliteInMemory.CreateOptions<Chapter07DbContext>()))
            {
                var logs = new List<string>();
                SqliteInMemory.SetupLogging(context, logs);

                context.Database.EnsureCreated();

                //ATTEMPT
                var book = new BookBase
                {
                    Title = "Unit Test"
                };
                book.SetOrgPrice(123);
                context.Add(book);
                context.SaveChanges();

                //VERIFY
                context.Books.Count().ShouldEqual(1);
                context.Books.First().ShouldBeType<BookBase>();
                book.HasPromotion.ShouldEqual(false);
                book.GetPrice().ShouldEqual(123);
            }
        }

        [Fact]
        public void TestBookPromoteOk()
        {
            //SETUP
            using (var context = new Chapter07DbContext(
                SqliteInMemory.CreateOptions<Chapter07DbContext>()))
            {
                var logs = new List<string>();
                SqliteInMemory.SetupLogging(context, logs);

                context.Database.EnsureCreated();

                //ATTEMPT
                var book = new BookPromote
                {
                    Title = "Unit Test"
                };
                book.SetOrgPrice(123);
                book.DiscountPrice = 10;
                context.Add(book);
                context.SaveChanges();

                //VERIFY
                context.Books.Count().ShouldEqual(1);
                context.Books.First().ShouldBeType<BookPromote>();
                book.HasPromotion.ShouldEqual(true);
                book.GetPrice().ShouldEqual(10);
            }
        }

        [Fact]
        public void TestChangeBookTypeOk()
        {
            //SETUP
            //SETUP
            var connection = this.GetUniqueDatabaseConnectionString();
            var optionsBuilder =
                new DbContextOptionsBuilder<Chapter07DbContext>();

            optionsBuilder.UseSqlServer(connection);
            int bookid;
            //ATTEMPT
            using (var context = new Chapter07DbContext(optionsBuilder.Options))
            {
                context.Database.EnsureCreated();
                var book = new BookBase
                {
                    Title = "Unit Test"
                };
                book.SetOrgPrice(123);
                context.Add(book);
                context.SaveChanges();
                bookid = book.BookBaseId;
            }
            using (var context = new Chapter07DbContext(optionsBuilder.Options))
            {
                //ATTEMPT
                var book = context.Books.AsNoTracking().Single(x => x.BookBaseId == bookid);
                var bookP = new BookPromote(book)
                {
                    DiscountPrice = 21,
                    PromotionMessage = "Cheap today!"
                };
                context.Update(bookP);
                context.SaveChanges();

                //VERIFY
                context.Books.Single(x => x.BookBaseId == bookid).ShouldBeType<BookPromote>(); 
                bookP.HasPromotion.ShouldEqual(true);
                bookP.GetPrice().ShouldEqual(21);
            }
        }
    }
}