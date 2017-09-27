#region licence
// =====================================================
// EfCoreExample - Example code to go with book
// Filename: MockPlaceOrderDbAccess.cs
// Date Created: 2016/08/15
// 
// Under the MIT License (MIT)
// 
// Written by Jon Smith : GitHub JonPSmith, www.thereformedprogrammer.net
// =====================================================
#endregion

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using BizDbAccess.Orders;
using DataLayer.EfClasses;
using test.EfHelpers;

namespace test.Mocks
{
    public class MockPlaceOrderDbAccess 
        : IPlaceOrderDbAccess //#A
    {
        public ImmutableList<Book> DummyBooks   //#B
            { get; private set; }               //#B


        public Order AddedOrder { get; private set; } //#C

        /// <summary>
        /// 
        /// </summary>
        /// <param name="createLastInFuture">If true then the last book will be in the future</param>
        /// <param name="promotionPriceForFirstBook">if number it adds a promotion to the first book</param>
        public MockPlaceOrderDbAccess(              //#D
            bool createLastInFuture = false,        //#E
            int? promotionPriceForFirstBook = null) //#F
        {
            var numBooks = createLastInFuture             //#G
                ? DateTime.UtcNow.Year -                  //#G
                    EfTestData.DummyBookStartDate.Year + 2//#G 
                : 10;                                     //#G
            var books = EfTestData.CreateDummyBooks   //#H
                (numBooks, createLastInFuture);       //#H
            if (promotionPriceForFirstBook != null)
                books.First().Promotion = new PriceOffer       //#I
                {                                              //#I
                    NewPrice = (int)promotionPriceForFirstBook,//#I
                    PromotionalText = "Unit Test"              //#I
                };                                             //#I
            DummyBooks = books.ToImmutableList();
        }


        /// <summary>
        /// This finds any books that fits the BookIds given to it
        /// and includes any promoptions
        /// </summary>
        /// <param name="bookIds"></param>
        /// <returns>A dictionary with the BookId as the key, and the Book as the value</returns>
        public IDictionary<int, Book> 
            FindBooksByIdsWithPriceOffers //#J
            (IEnumerable<int> bookIds)
        {
            return DummyBooks.AsQueryable()             //#K
                .Where(x => bookIds.Contains(x.BookId)) //#K
                .ToDictionary(key => key.BookId);       //#K
        }

        public void Add(Order newOrder)//#L
        {                              //#L
            AddedOrder = newOrder;     //#L
        }                              //#L
    }
    /***************************************************************
    #A My Mock MockPlaceOrderDbAccess implements the IPlaceOrderDbAccess, which allows it to replace the normal PlaceOrderDbAccess class
    #B This holds the dummy books that the mock uses. Can be useful if the test wants to compare the output with the dummy database
    #C This will contain the Order built by the PlaceOrderAction's method
    #D In this case I set up my mock via its constructor
    #E This parameter allows me to check that it won't accept an order for a book that hasn't yet been published
    #F This parameter allows me to add a PriceOffer to the first book so that I can check the correct price is recorded on the order
    #G This works out how to create enough books such that the last one isn't published yet
    #H I now create some dummy books using one of my unit test data generators
    #I This adds a PriceOffer to the first book, if required
    #J This method is called to get the books that the input selected. It uses the DummyBooks generated in the constructor
    #K This is very similar code to the original, but in this case it reads from the DummyBooks, not the database
    #L This method is called by the PlaceOrderAction's method to write the Order to the database. In this case I just capture it so that the unit test can inspect it
        * ************************************************************/
}