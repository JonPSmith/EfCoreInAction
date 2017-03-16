// Copyright (c) 2016 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.Linq;
using DataLayer.EfClasses;
using DataLayer.EfCode;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using test.EfHelpers;
using Xunit;
using Xunit.Abstractions;
using Xunit.Extensions.AssertExtensions;

namespace test.UnitTests.DataLayer
{
    public class Ch03_CoreUpdate
    {
        private readonly ITestOutputHelper _output;

        public Ch03_CoreUpdate(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void UpdatePublicationDate()
        {
            //SETUP
            var inMemDb = new SqliteInMemory();

            using (var context = inMemDb.GetContextWithSetup())
            {
                context.SeedDatabaseFourBooks();

                //ATTEMPT
                var book = context.Books                          //#A
                    .Single(p => p.Title == "Quantum Networking");//#A
                book.PublishedOn = new DateTime(2058, 1, 1);      //#B     
                context.SaveChanges();                            //#C
                /**********************************************************
                #A This finds the specific book we want to update. In the case our special book on Quantum Networking
                #B Then it changes the expected publication date to year 2058 (it was 2057)
                #C It calls SaveChanges which includes running a method called DetectChanges. This spots that the PublishedOn property has been changed
                * *******************************************************/

                //VERIFY
                var bookAgain = context.Books                     //#E
                    .Single(p => p.Title == "Quantum Networking");//#E
                bookAgain.PublishedOn                             //#F
                    .ShouldEqual(new DateTime(2058, 1, 1));       //#F
            }
        }

        [Fact]
        public void UpdatePublicationDateWithLogging()
        {
            //SETUP
            var options =
                this.NewMethodUniqueDatabaseSeeded4Books();      //#A

            using (var context = new EfCoreContext(options))
            {
                //REMOVE!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                var logIt = new LogDbContext(context);

                //ATTEMPT
                var book = context.Books                          //#B
                    .Single(p => p.Title == "Quantum Networking");//#B
                book.PublishedOn = new DateTime(2058, 1, 1);      //#C     
                context.SaveChanges();                            //#D

                //VERIFY
                var bookAgain = context.Books                     //#E
                    .Single(p => p.Title == "Quantum Networking");//#E
                bookAgain.PublishedOn                             //#F
                    .ShouldEqual(new DateTime(2058, 1, 1));       //#F
                //REMOVE!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                foreach (var log in logIt.Logs)
                {
                    _output.WriteLine(log);
                }
            }
        }
        /**********************************************************
        #A This create a database and seeds it with four books, one of which is the book on Quantum Networking
        #B It finds the specific book we want to update. In the case our special book on Quantum Networking
        #C Then it changes the expected publication date to year 2058 (it was 2057)
        #D It calls SaveChanges which includes running a method called DetectChanges. This spots that the PublishedOn property has been changed
        #E This reloads the Quantum Networking book from the database
        #F This shows that the PublishedOn date is what we expect
        * *******************************************************/


        [Fact]
        public void UpdateAuthorWithLogging()
        {
            //SETUP
            var options =
                this.NewMethodUniqueDatabaseSeeded4Books();

            string json;
            using (var context = new EfCoreContext(options))    //#A
            {                                                   //#A
                var author = context.Books                      //#A
                    .Where(p => p.Title == "Quantum Networking")//#A
                    .Select(p => p.AuthorsLink.First().Author)  //#A
                    .Single();                                  //#A
                author.Name = "Future Person 2";                //#A
                json = JsonConvert.SerializeObject(author);     //#A
            }                                                   //#A

            using (var context = new EfCoreContext(options))
            {
                var logIt = new LogDbContext(context);  //REMOVE THIS

                //ATTEMPT
                var author = JsonConvert
                    .DeserializeObject<Author>(json);           //#B


                context.Authors.Update(author);                 //#C                               
                context.SaveChanges();                          //#D  
            /**********************************************************
            #A This simulates an external system returning a modified Author entity class as a JSON string
            #B This simulates receiving a JSON string from an external system and decoding it into an Author class
            #C I use the Update command, which replaces all the row data for the given primary key, in this case AuthorId
            * *******************************************************/

                //VERIFY
                var authorAgain = context.Books.Where(p => p.Title == "Quantum Networking")
                    .Select(p => p.AuthorsLink.First().Author)
                    .Single();
                authorAgain.Name.ShouldEqual("Future Person 2");
                context.Authors.Any(p =>
                    p.Name == "Future Person").ShouldBeFalse();

                foreach (var log in logIt.Logs)
                {
                    _output.WriteLine(log);
                }
            }
        }

        [Fact]
        public void UpdateAuthorBadId()
        {
            //SETUP
            var inMemDb = new SqliteInMemory();

            using (var context = inMemDb.GetContextWithSetup())
            {
                context.SeedDatabaseFourBooks();

                //ATTEMPT
                var author = new Author
                {
                    AuthorId = 999999,
                    Name = "Future Person 2"
                };
                context.Authors.Update(author);
                var ex = Assert.Throws<DbUpdateConcurrencyException>(() => context.SaveChanges());

                //VERIFY
                ex.Message.StartsWith("Database operation expected to affect 1 row(s) but actually affected 0 row(s).").ShouldBeTrue();
            }
        }
        /**********************************************************

        * *******************************************************/
    }
}