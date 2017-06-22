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
using Xunit.Abstractions;
using Xunit.Extensions.AssertExtensions;

namespace test.UnitTests.DataLayer
{
    public class Ch03_OneToManyUpdate
    {
        private readonly ITestOutputHelper _output;

        public Ch03_OneToManyUpdate(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void TestConnectedUpdateNoExistingRelationshipOk()
        {
            //SETUP
            var inMemDb = new SqliteInMemory();

            using (var context = inMemDb.GetContextWithSetup())
            {
                context.SeedDatabaseFourBooks();

                //ATTEMPT
                //NOTE: I know that the first book does not have a review!
                var book = context.Books            //#A
                    .Include(p => p.Reviews)        //#A
                    .First();                       //#A

                book.Reviews.Add(new Review         //#B
                {                                   //#B
                    VoterName = "Unit Test",        //#B
                    NumStars = 5,                   //#B
                    Comment = "Great book!"         //#B
                });                                 //#B
                context.SaveChanges();              //#C           
                /**********************************************************
                #A This finds the first book and loads it with any reviews it might have
                #B I add a new review to this book
                #C The SaveChanges method calls DetectChanges, which finds that the Reviews property has changed, and from there finds the new Review, which it adds to the Review table
                * *******************************************************/

                //VERIFY
                var bookAgain = context.Books 
                    .Include(p => p.Reviews)                    
                    .Single(p => p.BookId == book.BookId);
                bookAgain.Reviews.ShouldNotBeNull();
                bookAgain.Reviews.Count.ShouldEqual(1);   
            }
        }

        [Fact]
        public void TestConnectedUpdateExistingRelationship()
        {
            //SETUP
            var inMemDb = new SqliteInMemory();

            using (var context = inMemDb.GetContextWithSetup())
            {
                context.SeedDatabaseFourBooks();

                var book = context.Books
                    .Include(p => p.Reviews)
                    .First(p => p.Reviews.Any());

                var orgReviews = book.Reviews.Count;

                //ATTEMPT
                book.Reviews.Add(new Review
                {
                    VoterName = "Unit Test",
                    NumStars = 5,
                });
                context.SaveChanges();

                //VERIFY
                var bookAgain = context.Books
                    .Include(p => p.Reviews)
                    .Single(p => p.BookId == book.BookId);
                bookAgain.Reviews.ShouldNotBeNull();
                bookAgain.Reviews.Count.ShouldEqual(orgReviews+1);
            }
        }

        [Fact]
        public void TestConnectedUpdateNoIncludeOk()
        {
            //SETUP
            var options =
                this.ClassUniqueDatabaseSeeded4Books();

            int bookId;
            int orgReviews;
            using (var context = new EfCoreContext(options))
            {
                //ATTEMPT
                var bookWithReviews = context.Books.Include(x => x.Reviews).First(x => x.Reviews.Count > 0);
                bookId = bookWithReviews.BookId;
                orgReviews = bookWithReviews.Reviews.Count;
            }

            using (var context = new EfCoreContext(options))
            {
                var book = context.Books
                    .Single(p => p.BookId == bookId);
                book.Reviews = new List<Review>
                {
                    new Review
                    {
                        VoterName = "Unit Test",
                        NumStars = 5,
                    }
                };
                context.SaveChanges();

                //VERIFY
                var bookAgain = context.Books
                    .Include(p => p.Reviews)
                    .Single(p => p.BookId == book.BookId);
                bookAgain.Reviews.ShouldNotBeNull();
                bookAgain.Reviews.Count.ShouldEqual(orgReviews + 1);
            }
        }

        [Fact]
        public void TestReplaceReviewsLoggedOk()
        {
            //SETUP
            var options =
                this.NewMethodUniqueDatabaseSeeded4Books( );

            int twoReviewBookId;
            using (var context = new EfCoreContext(options))
            {
                //ATTEMPT
                twoReviewBookId = context.Books.ToList().Last().BookId; //Last has reviews

            }

            using (var context = new EfCoreContext(options))
            {

                var logIt = new LogDbContext(context);

                var book = context.Books
                    .Include(p => p.Reviews)                  //#A
                    .Single(p => p.BookId == twoReviewBookId);//#B

                book.Reviews = new List<Review>               //#C 
                {                                             //#C
                    new Review                                //#C
                    {                                         //#C
                        VoterName = "Unit Test",              //#C
                        NumStars = 5,                         //#C
                    }                                         //#C
                };                                            //#C
                context.SaveChanges();                        //#D
                /*******************************************************
                #A This include is important, otherwise EF Core won't know about the old reviews 
                #B This book I am loading has two review
                #C I completely replace the whole collection
                #D SaveChanges, via DetectChanges knows that a) the old collection should be deleted, b) the new collection should be written to the database
                 * ******************************************************/
                //VERIFY
                var bookAgain = context.Books
                    .Include(p => p.Reviews)
                    .Single(p => p.BookId == book.BookId);
                bookAgain.Reviews.ShouldNotBeNull();
                bookAgain.Reviews.Count.ShouldEqual(1);
                context.Set<Review>().Count().ShouldEqual(1);

                foreach (var log in logIt.Logs)
                {
                    _output.WriteLine(log);
                }
            }
        }


        private class ChangeReviewDto
        {
            public int ReviewId { get; set; }
            public int NewBookId { get; set; }
        }

        [Fact]
        public void TestChangeReviewViaForeignKeyOk()
        {
            //SETUP
            var options =
                this.NewMethodUniqueDatabaseSeeded4Books();

            ChangeReviewDto dto;
            using (var context = new EfCoreContext(options))
            {
                var book = context.Books.First();
                var review = context.Set<Review>().First();
                dto = new ChangeReviewDto
                {
                    ReviewId = review.ReviewId,
                    NewBookId = book.BookId
                };
            }

            using (var context = new EfCoreContext(options))
            {
                //ATTEMPT
                var reviewToChange = context     //#A
                    .Find<Review>(dto.ReviewId); //#A
                reviewToChange.BookId = dto.NewBookId; //#B
                context.SaveChanges();                 //#C
                /*****************************************************
                #A I find the review that I want to move using the primary key returned from the browser
                #C I then change the foreign key in the review to point to the book it should be linked to
                #D Finally I call SaveChanges which finds the foreign key in the Review changed, so it updates that column in the database
                * **************************************************/

                //VERIFY
                var bookAgain = context.Books
                    .Include(p => p.Reviews)
                    .First();
                bookAgain.Reviews.ShouldNotBeNull();
                bookAgain.Reviews.Count.ShouldEqual(1);
            }
        }
    }
}