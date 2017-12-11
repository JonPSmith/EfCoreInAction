// // Copyright (c) 2017 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// // Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Test.Chapter10Listings.EfCode;

namespace Test.Chapter10Listings.EfClasses
{
    public class BookDdd
    {
        private readonly List<ReviewDdd> _reviews         //#A
            = new List<ReviewDdd>();                      //#A
        private readonly List<BookAuthorDdd> _authorsLink //#A
            = new List<BookAuthorDdd>();                  //#A

        public int BookId { get; private set; }           //#B
        public string Title { get; private set; }         //#B
        public string Description { get; private set; }   //#B
        public DateTime PublishedOn { get; private set; } //#B
        public string Publisher { get; private set; }     //#B
        public decimal Price { get; private set; }        //#B
        public string ImageUrl { get; private set; }      //#B

        //-----------------------------------------------
        //relationships

        public PriceOfferDdd Promotion { get; private set; } //#B
        public IEnumerable<ReviewDdd> Reviews          //#C
            => _reviews.ToList();                      //#C
        public IEnumerable<BookAuthorDdd> AuthorsLink  //#C
            => _authorsLink.ToList();                  //#C

        //-----------------------------------------------
        //ctors

        internal BookDdd() { } //#D

        public BookDdd(string title, string description, //#E
            DateTime publishedOn, string publisher, 
            decimal price, string imageUrl,
            IReadOnlyList<AuthorDdd> authors)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new 
               ArgumentNullException(nameof(title)); //#F
            Title = title;
            Description = description;
            PublishedOn = publishedOn;
            Publisher = publisher;
            Price = price;
            ImageUrl = imageUrl;

            if (authors == null || authors.Count < 1) //#F
                throw new ArgumentException(
                    "You must have at least one Author for a book",
                    nameof(authors));
            _authorsLink = authors.Select(a => //#G
                new BookAuthorDdd              //#G
                {                              //#G
                    Book = this,               //#G
                    Author = a                 //#G
                }).ToList();                   //#G
        }
        /*******************************************************
        #A I use backing fields to hold the two collection navigational properties
        #B All the properties now have private setters so that you can only set properties via the DDD repository methods
        #C The collection navigational properties are now IEnumerable<T>, which does not have the Add and Remove methods, so you can only change them via the DDD repository methods
        #D I create an internal, no parameter constructor for EF Core to use. This stops code in other assemblies from being able to create a BookDdd other than via the parameterised constructor
        #E The developer uses this contructor to create the BookDdd. This takes all the parameters it needs to create a book, inlcuding the Author(s)
        #F Using a constructor to create the BookDdd allows me to add a few system checks, such as the book title not being empty
        #G The called does have to worry about setting up the BookAuthorDdd linking table as I do it inside the constructor.
        * ****************************************************/

        public void ChangePubDate(DateTime newDate) //#A
        {
            PublishedOn = newDate;
        }

        public void AddReview(DbContext context,        //#B
            int numStars, string comment, string voterName) //#B
        {
            var review = new ReviewDdd //#C
            {
                NumStars = numStars,
                Comment = comment,
                VoterName = voterName
            };

            context.Entry(this) //#D
                .Collection(c => c.Reviews).Load(); //#D

            _reviews.Add(review); //#E
        }

        public void AddUpdatePromotion(DbContext context, //#F
            decimal newPrice, string promotionalText)
        {
            context.Entry(this) //#G
                .Reference(r => r.Promotion).Load(); //#G
            if (Promotion == null) //#H
            {
                Promotion = new PriceOfferDdd
                {
                    NewPrice = newPrice,
                    PromotionalText = promotionalText
                };
            }
            else //#I
            {
                Promotion.NewPrice = newPrice;
                Promotion.PromotionalText = promotionalText;
            }
        }

        public void RemovePromotion(DbContext context) //#J
        {
            var promotion = context.Set<PriceOfferDdd>() //#K
                .SingleOrDefault(x => x.BookId == BookId); //#K
            if (promotion != null) //#L
                context.Remove(promotion); //#L
        }
    }
    /***************************************************************
    #A A simple method to update the book's PubishedOn date
    #B This adds a ReviewDdd to the book, using the parameters passed in
    #C I create a ReviewDdd using the parameters passed in. Note that the parameter-less constructor of the ReviewDdd has an access modifier of internal, so only code in this assembly can create the ReviewDdd entity
    #D I explicitly load the Reviews collection so that I can add to it (see chapter 2 for more on explicit loading)
    #E I now add the review to the backing field collection
    #F This method will add or update the PriceOfferDdd entity to go with this book
    #G I try to load the PriceOfferDdd entity. As this is an optional one-to-one relationship it can be null
    #H There is no existing Promotion, so I add a new one
    #I There was an existing Promotion, so I update it
    #J This method remove any promotion from the current book.
    #K I load the PriceOfferDdd linked to this book, which could be null
    #L If there was a promotion then I delete it
     * *************************************************************/
}