// Copyright (c) 2016 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
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
    public class Ch03_OneToOneUpdate
    {
        private readonly ITestOutputHelper _output;

        public Ch03_OneToOneUpdate(ITestOutputHelper output)
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
                var book = context.Books                  //#A
                    .Include(p => p.Promotion)            //#A
                    .First(p => p.Promotion == null);     //#A

                book.Promotion = new PriceOffer           //#B
                {                                         //#B
                    NewPrice = book.Price / 2,            //#B
                    PromotionalText = "Half price today!" //#B
                };                                        //#B
                context.SaveChanges();                    //#C                  
                /**********************************************************
                #A This finds the first book that does not have an existing promotion
                #B I add a new PriceOffer to this book
                #C The SaveChanges method calls DetectChanges, which find that the Promotion property has changed, so it adds that entry to the PricerOffers table
                * *******************************************************/

                //VERIFY
                var bookAgain = context.Books 
                    .Include(p => p.Promotion)                    
                    .Single(p => p.BookId == book.BookId);
                bookAgain.Promotion.ShouldNotBeNull();
                bookAgain.Promotion.PromotionalText.ShouldEqual("Half price today!");     
            }
        }

        [Fact]
        public void TestConnectedUpdateIncludeOk()
        {
            //SETUP
            var inMemDb = new SqliteInMemory();

            using (var context = inMemDb.GetContextWithSetup())
            {
                context.SeedDatabaseFourBooks();

                //ATTEMPT
                var book = context.Books             
                    .Include(p => p.Promotion)       
                    .First(p => p.Promotion != null);

                book.Promotion = new PriceOffer          
                {                                        
                    NewPrice = book.Price / 2,           
                    PromotionalText = "Half price today!"
                };                                       
                context.SaveChanges();                                    

                //VERIFY
                context.PriceOffers.Count().ShouldEqual(1); //there is only one promotion in the four book data
                context.PriceOffers.First().PromotionalText.ShouldEqual("Half price today!");
            }
        }

        [Fact]
        public void TestConnectedUpdateNoIncludeBad()
        {
            //SETUP
            var options =
                this.ClassUniqueDatabaseSeeded4Books();

            int bookId;
            using (var context = new EfCoreContext(options))
            {
                //ATTEMPT
                bookId = context.Books.ToList().Last().BookId; //Last has a price promotion
            }

            using (var context = new EfCoreContext(options))
            {
                var book = context.Books
                    .Single(p => p.BookId == bookId);
                book.Promotion = new PriceOffer
                {
                    NewPrice = book.Price / 2,
                    PromotionalText = "Half price today!"
                };
                var ex = Assert.ThrowsAny<DbUpdateException>(() => context.SaveChanges());

                //VERIFY
                ex.InnerException.Message.StartsWith(
                    "Cannot insert duplicate key row in object 'dbo.PriceOffers' with unique index 'IX_PriceOffers_BookId'.")
                    .ShouldBeTrue();
            }
        }

        [Fact]
        public void TestDeleteExistingRelationship()
        {
            //SETUP
            var inMemDb = new SqliteInMemory();

            using (var context = inMemDb.GetContextWithSetup())
            {
                context.SeedDatabaseFourBooks();

                var book = context.Books
                    .Include(p => p.Promotion)
                    .First(p => p.Promotion != null);

                //ATTEMPT
                book.Promotion = null;
                context.SaveChanges();

                //VERIFY
                var bookAgain = context.Books
                    .Include(p => p.Promotion)
                    .Single(p => p.BookId == book.BookId);
                bookAgain.Promotion.ShouldBeNull();
                context.PriceOffers.Count().ShouldEqual(0);
            }
        }

        //---------------------------------------------------------
        // Create/delete of PriceOffer via its own table

        [Fact]
        public void TestCreatePriceOffer()
        {
            //SETUP
            var inMemDb = new SqliteInMemory();

            using (var context = inMemDb.GetContextWithSetup())
            {
                context.SeedDatabaseFourBooks();

                var orgPriceOffers = context.PriceOffers.Count();
                var book = context.Books
                    .First(p => p.Promotion == null);     //#A

                //ATTEMPT
                context.PriceOffers.Add(                  //#B
                new PriceOffer                            //#C
                {                                         //#C
                    BookId = book.BookId,                 //#C
                    NewPrice = book.Price / 2,            //#C
                    PromotionalText = "Half price today!" //#C
                });                                       //#C
                context.SaveChanges();                    //#D
                /******************************************************
                #A Here I find the book that I want to add the new PriceOffer to. It must NOT have an existing PriceOffer
                #B I add the new PriceOffer to the PriceOffers table
                #C This defines the PriceOffer. Note that you MUST include the BookId (previously EF Core filled that in)
                #D SaveChanges adds the PriceOffer to the PriceOffers table
                 * *****************************************************/

                //VERIFY
                var bookAgain = context.Books
                    .Include(p => p.Promotion)
                    .Single(p => p.BookId == book.BookId);
                bookAgain.Promotion.ShouldNotBeNull();
                context.PriceOffers.Count().ShouldEqual(orgPriceOffers+1);
            }
        }

        [Fact]
        public void TestDeletePriceOffer()
        {
            //SETUP
            var inMemDb = new SqliteInMemory();

            using (var context = inMemDb.GetContextWithSetup())
            {
                context.SeedDatabaseFourBooks();

                var book = context.Books
                    .First(p => p.Promotion != null);

                //ATTEMPT
                context.PriceOffers.Remove(context.PriceOffers.Find(book.Promotion.PriceOfferId));
                context.SaveChanges();

                //VERIFY
                var bookAgain = context.Books
                    .Include(p => p.Promotion)
                    .Single(p => p.BookId == book.BookId);
                bookAgain.Promotion.ShouldBeNull();
                context.PriceOffers.Count().ShouldEqual(0);
            }
        }

        //-----------------------------------------------------------

        [Fact]
        public void TestDisconnectedUpdateNoExistingRelationshipOk()
        {
            //SETUP
            var inMemDb = new SqliteInMemory();

            using (var context = inMemDb.GetContextWithSetup())
            {
                context.SeedDatabaseFourBooks();

                //ATTEMPT
                var bookId = 1;                      //#A
                var newPrice = 12;                   //#A
                var newText = "Half price today!";   //#A

                var book = context.Books             //#B
                    .Include(p => p.Promotion)       //#B
                    .Single(p => p.BookId == bookId);//#B
                book.Promotion = new PriceOffer      //#C
                {                                    //#C
                    NewPrice = newPrice,             //#C
                    PromotionalText = newText        //#C
                };                                   //#C
                context.SaveChanges();               //#D

                /*********************************************************
                #A This simulates receiving the the data passed back after the disconnect. In a browser this would come back from an HTML form 
                #B This code loads the book that the new promotion should be applied to
                #C This forms the PriceOffer to be added to the Book. Note that I don't need to set the BookId - EF Core does that
                #D The SaveChanges method calls DetectChanges, which find that the Promotion property has changed, so it adds that entry to the PricerOffers table
                 * *******************************************************/
                //VERIFY
                var bookAgain = context.Books
                    .Include(p => p.Promotion)
                    .Single(p => p.BookId == book.BookId);
                bookAgain.Promotion.ShouldNotBeNull();
                bookAgain.Promotion.PromotionalText.ShouldEqual(newText);
            }
        }
    }
}