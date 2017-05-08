// Copyright (c) 2017 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using test.EfHelpers;
using test.Helpers;
using Test.Chapter08Listings.EfClasses;
using Test.Chapter08Listings.EfCode;
using Xunit;
using Xunit.Abstractions;
using Xunit.Extensions.AssertExtensions;

namespace test.UnitTests.DataLayer
{
    public class Ch08_Concurrency
    {
        private readonly ITestOutputHelper _output;

        private readonly DbContextOptions<ConcurrencyDbContext> _options;

        public Ch08_Concurrency(ITestOutputHelper output)
        {
            _output = output;

            var connection = this.GetUniqueDatabaseConnectionString();
            var optionsBuilder =
                new DbContextOptionsBuilder<ConcurrencyDbContext>();

            optionsBuilder.UseSqlServer(connection);
            _options = optionsBuilder.Options;
            using (var context = new ConcurrencyDbContext(_options))
            {
                context.Database.EnsureCreated();
                if (!context.Books.Any())
                {
                    context.Books.Add(new ConcurrecyBook
                    {
                        Title = "Default Book",
                        PublishedOn = new DateTime(2015,1,1),
                        Author = new ConcurrencyAuthor { Name = "Default Author" }
                    });
                    context.SaveChanges();
                }
            }
        }

        [Fact]
        public void CreateConcurrencyDataAllOk()
        {
            //SETUP
            using (var context = new ConcurrencyDbContext(_options))
            {
                var numBooks = context.Books.Count();

                //ATTEMPT
                context.Books.Add(new ConcurrecyBook
                {
                    Title = "Unit Test",
                    PublishedOn = new DateTime(2014, 1, 1),
                    Author = new ConcurrencyAuthor { Name = "Unit Test"}
                });
                context.SaveChanges();

                //VERIFY
                context.Books.Count().ShouldEqual(numBooks + 1);
            }
        }

        [Fact]
        public void UpdateBookTitleOk()
        {
            //SETUP
            using (var context = new ConcurrencyDbContext(_options))
            {
                var logIt = new LogDbContext(context);
                var firstBookId = context.Books.First().ConcurrecyBookId;

                //ATTEMPT
                var firstBook = context.Books.First(k => k.ConcurrecyBookId == firstBookId);
                var sqlTitle = Guid.NewGuid().ToString();
                var newDate = DateTime.Now.AddDays(100);
                context.Database.ExecuteSqlCommand(
                    "UPDATE dbo.Books SET Title = @p0 WHERE ConcurrecyBookId = @p1", 
                    sqlTitle, firstBookId);
                firstBook.PublishedOn = newDate;
                context.SaveChanges();

                //VERIFY
                context.Entry(firstBook).Reload();
                firstBook.Title.ShouldEqual(sqlTitle);
                firstBook.PublishedOn.ShouldEqual(newDate);

                //foreach (var log in logIt.Logs)
                //{
                //    _output.WriteLine(log);
                //}
                ////to get the logs you need to fail see https://github.com/aspnet/Tooling/issues/541
                //Assert.True(false, "failed the test so that the logs show");
            }
        }

        [Fact]
        public void ThrowExceptionRowDeletedOk()
        {
            //SETUP
            using (var context = new ConcurrencyDbContext(_options))
            {
                //ATTEMPT
                var logIt = new LogDbContext(context);
                var firstBook = context.Books.First();

                context.Database.ExecuteSqlCommand(
                    "DELETE dbo.Books WHERE ConcurrecyBookId = @p0", 
                    firstBook.ConcurrecyBookId);
                firstBook.Title = Guid.NewGuid().ToString();

                var ex = Assert.Throws<DbUpdateConcurrencyException>(() => context.SaveChanges());

                //VERIFY
                ex.Message.StartsWith("Database operation expected to affect 1 row(s) but actually affected 0 row(s). Data may have been modified or deleted since entities were loaded. ")
                    .ShouldBeTrue();
            }
        }

        [Fact]
        public void ThrowExceptionOnPublishedDateChangedOk()
        {
            //SETUP
            using (var context = new ConcurrencyDbContext(_options))
            {
                //ATTEMPT
                var logIt = new LogDbContext(context);
                var firstBook = context.Books.First(); //#A

                context.Database.ExecuteSqlCommand(
                    "UPDATE dbo.Books SET PublishedOn = GETDATE()" + //#B
                    " WHERE ConcurrecyBookId = @p0",                //#B
                    firstBook.ConcurrecyBookId);                    //#B
                firstBook.Title = Guid.NewGuid().ToString(); //#C
                //context.SaveChanges(); //#D
                /******************************************
                #A I load the first book in the database as a tracked entity
                #B I simulate another thread/application changing the PublishedOn column of the same book
                #C I change the title in the book to cause EF Core to do an update to the book
                #D This SaveChanges will throw an DbUpdateConcurrencyException
                 * ***************************************/

                var ex = Assert.Throws<DbUpdateConcurrencyException>(() => context.SaveChanges());

                //VERIFY
                ex.Message.StartsWith("Database operation expected to affect 1 row(s) but actually affected 0 row(s). Data may have been modified or deleted since entities were loaded. ")
                    .ShouldBeTrue();
            }
        }

        [Fact]
        public void ThrowExceptionOnAuthorChangedOk()
        {
            //SETUP
            using (var context = new ConcurrencyDbContext(_options))
            {
                //ATTEMPT
                var logIt = new LogDbContext(context);

                var firstAuthor = context.Authors.First(); //#A
                context.Database.ExecuteSqlCommand(      //#B
                    "UPDATE dbo.Authors SET Name = @p0"+ //#B
                    " WHERE ConcurrencyAuthorId = @p1",  //#B
                    firstAuthor.Name,                    //#B
                    firstAuthor.ConcurrencyAuthorId);    //#B
                firstAuthor.Name = "Concurrecy Name"; //#C
                //context.SaveChanges(); //#D
                /******************************************
                #A I load the first author in the database as a tracked entity
                #B I simulate another thread/application updating the entity. In fact nothing is changed, but the timestamp
                #C I change something in the author to cause EF Core to do an update to the book
                #D This SaveChanges will throw an DbUpdateConcurrencyException
                 * ***************************************/

                var ex = Assert.Throws<DbUpdateConcurrencyException>(() => context.SaveChanges());

                //VERIFY
                ex.Message.StartsWith("Database operation expected to affect 1 row(s) but actually affected 0 row(s). Data may have been modified or deleted since entities were loaded. ")
                    .ShouldBeTrue();
                //foreach (var log in logIt.Logs)
                //{
                //    _output.WriteLine(log);
                //}
                ////to get the logs you need to fail see https://github.com/aspnet/Tooling/issues/541
                //Assert.True(false, "failed the test so that the logs show");
            }
        }

        [Fact]
        public void HandleExceptionOnPublishedDateChangedOk()
        {
            //SETUP
            using (var context = new ConcurrencyDbContext(_options))
            {
                //ATTEMPT
                var firstBook = context.Books.First(); //#A

                context.Database.ExecuteSqlCommand(
                    "UPDATE dbo.Books SET PublishedOn = GETDATE()" +  //#B
                    " WHERE ConcurrecyBookId = @p0",                  //#B
                    firstBook.ConcurrecyBookId);                      //#B
                firstBook.Title = Guid.NewGuid().ToString(); //#C
                try
                {
                    context.SaveChanges(); //#D
                }
                catch (DbUpdateConcurrencyException ex) //#E
                {
                    foreach (var entry in ex.Entries) //#F
                    {
                        if (!HandleBookConcurrency(context, entry)) //#G
                        {
                            throw new NotSupportedException(  //#H
                                "Don't know how to handle concurrency conflicts for " +
                                entry.Metadata.Name);
                        }
                    }
                    context.SaveChanges(); //#I;
                }
                /***********************************************************
                #A I load the first book in the database as a tracked entity
                #B I simulate another thread/application changing the PublishedOn column of the same book
                #C I change the title in the book to cause EF Core to do an update to the book
                #D This SaveChanges will throw an DbUpdateConcurrencyException
                #E I catch the DbUpdateConcurrencyException and put in my code to handle it
                #F There may be multiple entities that have concurrency issues, so we need to look at each in turn
                #G I call my HandleBookConcurrency method, which returns true if it could handle the concurrency on this entity
                #H If my method couldn't handle it then I have to throw an exception
                #I If I got to here then the concurrecy issue has been handled, so we try the SaveChanges again
                 * **********************************************************/
            }

            //VERIFY
            using (var context = new ConcurrencyDbContext(_options))
            {
                var rereadBook = context.Books.First();
                rereadBook.PublishedOn.ShouldEqual(new DateTime(2050, 5, 5));
            }
        }

        private static bool HandleBookConcurrency( //#A
            ConcurrencyDbContext context, 
            EntityEntry entry)
        {
            var book = entry.Entity 
                as ConcurrecyBook; //#B
            if (book == null)      //#B
                return false;      //#B

            var databaseEntity =                   //#C
                context.Books.AsNoTracking()       //#D
                    .Single(p => p.ConcurrecyBookId
                        == book.ConcurrecyBookId);
            var version2Entity = context.Entry(databaseEntity); //#E

            foreach (var property in entry.Metadata.GetProperties()) //#F
            {
                var version1_original = entry               //#G
                    .Property(property.Name).OriginalValue; //#G
                var version2_someoneElse = version2Entity  //#H
                    .Property(property.Name).CurrentValue; //#H
                var version3_whatIWanted = entry          //#I
                    .Property(property.Name).CurrentValue;//#I

                // TODO: Logic to decide which value should be written to database
                if (property.Name ==                           //#J
                    nameof(ConcurrecyBook.PublishedOn))        //#J
                {                                              //#J
                    entry.Property(property.Name).CurrentValue //#J
                        = new DateTime(2050, 5, 5);            //#J
                }                                              //#J

                // Update original values so that the concurrecy 
                entry.Property(property.Name).OriginalValue =            //#K
                    version2Entity.Property(property.Name).CurrentValue; //#K
            }
            return true; //#L
        }
        /***********************************************************
        #A My method takes in the application DbContext and the ChangeTracking entry from the exception's Entities property
        #B This method only handles a ConcurrecyBook, so it returns false if the entity isn't of that type
        #C I want to get the data that someone else wrote into the database after my read. 
        #D This entity MUST be read as NoTracking otherwise it will interfere with the same entity we are trying to write
        #E I get the TEntity version of the entity, which has all the tracking information
        #F In this case I show going through all of the properties in the book entity. I need to do this to reset the Original values so that the exception does not happen again
        #G This holds the version of the property at the time when I did the tracked read of the book
        #H This holds the version of the property as written to the database by someone else
        #I This holds the version of the property that I wanted to set it to in my update
        #J This is where you should put your code to fix the concurrency issue. I set the PublishedOn property to a specific value so I can check it in my unit test
        #K Here I set the OriginalValue to the value that someone else set it to. This handles both the case where you use concurrency tokens or a timestamp
        #L I return true to say I handled this concurrency issue
         * ********************************************************/
    }
}