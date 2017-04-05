// Copyright (c) 2016 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using DataLayer.EfClasses;
using DataLayer.EfCode;
using Microsoft.EntityFrameworkCore;
using test.EfHelpers;
using Xunit;
using Xunit.Extensions.AssertExtensions;

namespace test.UnitTests.DataLayer
{
    public class Ch03_Create
    {
        [Fact]
        public void TestCreateBookWithNewAuthor()
        {
            //SETUP
            var inMemDb = new SqliteInMemory();

            using (var context = inMemDb.GetContextWithSetup())
            {
                var book = new Book                     //#A
                {                                       //#A
                    Title = "Test Book",                //#A
                    PublishedOn = DateTime.Today        //#A
                };                                      //#A
                book.AuthorsLink = new List<BookAuthor> //#B
                {                                       //#B
                    new BookAuthor                      //#B
                    {                                   //#B
                        Book = book,                    //#B
                        Author = new Author             //#B
                        {                               //#B
                            Name = "Test Author"        //#B
                        }                               //#B
                    }                                   //#B
                };                                      //#B

                //ATTEMPT
                context.Add(book);                      //#C
                context.SaveChanges();                  //#D
                /******************************************************
                #A This creates the book with the title "Test Book"
                #B This adds a single author called "Test Author" using the linking table, BookAuthor
                #C It uses the .Add method to add the book to the application's DbContext property, Books
                #D It calls the SaveChanges() method from the application's DbContext to update the database
                 * *****************************************************/

                //VERIFY
                context.Books.Count().ShouldEqual(1);   //#E
                context.Authors.Count().ShouldEqual(1); //#F
            }
        }

        [Fact]
        public void TestCreateBookWithExistingAuthorSavedToDatabase()
        {
            //SETUP
            var inMemDb = new SqliteInMemory();

            using (var context = inMemDb.GetContextWithSetup())
            {
                var oneBook = 
                    EfTestData.CreateDummyBookOneAuthor();//#A
                context.Add(oneBook);               //#A
                context.SaveChanges();                    //#A

                var book = new Book                     //#B
                {                                       //#B
                    Title = "Test Book",                //#B
                    PublishedOn = DateTime.Today        //#B
                };                                      //#B
                book.AuthorsLink = new List<BookAuthor> //#C
                {                                       //#C
                    new BookAuthor                      //#C
                    {                                   //#C
                        Book = book,                    //#C
                        Author = oneBook.AuthorsLink    //#C
                             .First().Author            //#C
                    }                                   //#C
                };                                      //#C

                //ATTEMPT
                context.Add(book);                //#D
                context.SaveChanges();                  //#D
                /************************************************************
                #A This method creates dummy books for testing. I create one dummy book with one Author and add it to the empty database
                #B This creates a book in the same way as the previous exmaple
                #C This adds a AuthorBook linking entry, but it now uses the Author from the first book
                #D This is the same process: add the new book to the DbContext Books property and call SaveChanges
                 * *********************************************************/

                //VERIFY
                context.Books.Count().ShouldEqual(2);   //#E
                context.Authors.Count().ShouldEqual(1); //#F
            }
        }

        [Fact]
        public void TestCreateBookWithExistingAuthorAddedButNotSavedToDatabase()
        {
            //SETUP
            var inMemDb = new SqliteInMemory();

            using (var context = inMemDb.GetContextWithSetup())
            {
                var oneBook =
                    EfTestData.CreateDummyBookOneAuthor();
                context.Add(oneBook);               

                var book = new Book                    
                {                                      
                    Title = "Test Book",               
                    PublishedOn = DateTime.Today       
                };                                     
                book.AuthorsLink = new List<BookAuthor>
                {                                      
                    new BookAuthor                     
                    {                                  
                        Book = book,                   
                        Author = oneBook.AuthorsLink   
                             .First().Author           
                    }                                  
                };                                     

                //ATTEMPT
                context.Add(book);               
                context.SaveChanges();                 

                //VERIFY
                context.Books.Count().ShouldEqual(2);
                context.Authors.Count().ShouldEqual(1);
            }
        }

        [Fact]
        public void TestCreateBookWithExistingAuthorAttached()
        {
            //SETUP
            var options =
                this.NewMethodUniqueDatabaseSeeded4Books();

            Author disconnectedAuthor;
            using (var context = new EfCoreContext(options))
            {
                //ATTEMPT
                disconnectedAuthor = context.Authors.First();
            }

            using (var context = new EfCoreContext(options))
            {
                var book = new Book
                {
                    Title = "Test Book",
                    PublishedOn = DateTime.Today
                };
                context.Authors.Attach(disconnectedAuthor);
                book.AuthorsLink = new List<BookAuthor>
                {
                    new BookAuthor
                    {
                        Book = book,
                        Author = disconnectedAuthor
                    }
                };

                //ATTEMPT
                context.Add(book);
                context.SaveChanges();

                //VERIFY
                context.Books.Count().ShouldEqual(5);
                context.Authors.Count().ShouldEqual(3);
            }
        }

        [Fact]
        public void TestCreateBookAddTwice()
        {
            //SETUP
            var inMemDb = new SqliteInMemory();

            using (var context = inMemDb.GetContextWithSetup())
            {
                var oneBook =
                    EfTestData.CreateDummyBookOneAuthor();

                //ATTEMPT
                context.Add(oneBook);
                context.Add(oneBook);
                context.SaveChanges();

                //VERIFY
                context.Books.Count().ShouldEqual(1);
            }
        }

        [Fact]
        public void TestCreateBookWriteTwiceBad()
        {
            //SETUP
            var inMemDb = new SqliteInMemory();

            using (var context = inMemDb.GetContextWithSetup())
            {
                var oneBook =
                    EfTestData.CreateDummyBookOneAuthor();

                //ATTEMPT
                context.Add(oneBook);
                context.SaveChanges();
                var state1 = context.Entry(oneBook).State;
                context.Add(oneBook);
                var state2 = context.Entry(oneBook).State;
                var ex = Assert.Throws<DbUpdateException>( () => context.SaveChanges());

                //VERIFY
                ex.Message.ShouldEqual("An error occurred while updating the entries. See the inner exception for details.");
                state1.ShouldEqual(EntityState.Unchanged);
                state2.ShouldEqual(EntityState.Added);
            }
        }
    }
}