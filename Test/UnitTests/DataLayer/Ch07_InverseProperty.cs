// Copyright (c) 2017 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using test.EfHelpers;
using Test.Chapter07Listings.EfClasses;
using Test.Chapter07Listings.EFCode;
using Xunit;
using Xunit.Extensions.AssertExtensions;

namespace test.UnitTests.DataLayer
{
    public class Ch07_InverseProperty
    {
        [Fact]
        public void TestLibraryBookLibarianOnlyOk()
        {
            //SETUP
            using (var context = new Chapter07DbContext(
                SqliteInMemory.CreateOptions<Chapter07DbContext>()))
            {
                var logs = new List<string>();
                SqliteInMemory.SetupLogging(context, logs);
                context.Database.EnsureCreated();

                //ATTEMPT
                var librarian = new Person {Name = "Librarian", UserId = "libarian@somewhere.com"};
                var book = new LibraryBook
                {
                    Title = "Entity Framework in Action",
                    Librarian = librarian
                };
                context.Add(book);
                context.SaveChanges();

                //VERIFY
                context.LibraryBooks.Count().ShouldEqual(1);
                context.People.Count().ShouldEqual(1);
            }
        }

        [Fact]
        public void TestLibraryBookLibarianAndOnLoadOk()
        {
            //SETUP
            using (var context = new Chapter07DbContext(
                SqliteInMemory.CreateOptions<Chapter07DbContext>()))
            {
                context.Database.EnsureCreated();

                //ATTEMPT
                var librarian = new Person { Name = "Librarian", UserId = "librarian@somewhere.com" };
                var reader = new Person { Name = "OnLoanTo", UserId = "reader@somewhere.com" };
                var book = new LibraryBook
                {
                    Title = "Entity Framework in Action",
                    Librarian = librarian,
                    OnLoanTo = reader
                };
                context.Add(book);
                context.SaveChanges();

                //VERIFY
                context.LibraryBooks.Count().ShouldEqual(1);
                context.People.Count().ShouldEqual(2);
            }
        }

        [Fact]
        public void TestLibraryBookNoLibarianBad()
        {
            //SETUP
            using (var context = new Chapter07DbContext(
                SqliteInMemory.CreateOptions<Chapter07DbContext>()))
            {
                var logs = new List<string>();
                SqliteInMemory.SetupLogging(context, logs);
                context.Database.EnsureCreated();

                //ATTEMPT
                var book = new LibraryBook
                {
                    Title = "Entity Framework in Action"
                };
                context.Add(book);
                //context.SaveChanges();
                var ex = Assert.Throws<DbUpdateException>(() => context.SaveChanges());

                //VERIFY
                //context.LibraryBooks.Count().ShouldEqual(1);
                ex.InnerException.Message.ShouldEqual("xx");
            }
        }
    }
}