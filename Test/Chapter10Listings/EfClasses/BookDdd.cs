// // Copyright (c) 2017 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// // Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.EntityFrameworkCore;

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
            Title = title ?? throw new 
                ArgumentNullException(nameof(title)); //#F
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
    #F Using a constuctor to create the BookDdd allows me to add a few system checks
    #G The called does have to worry about setting up the BookAuthorDdd linking table as I do it inside the constructor.
        * *****************************************************/

        public void AddBook(DbContext context)
        {
            context.Add(this);
        }

        public void AddReview(DbContext context, int numStars, string comment, string voterName)
        {
            if (numStars < 0 || numStars > 5)
                throw new ArgumentException("NumStars must be between 0 and 5");
            var review = new ReviewDdd
            {
                NumStars = numStars,
                Comment = comment,
                VoterName = voterName
            };

            if (BookId != default(int))
                context.Entry(this).Collection(c => c.Reviews).Load();

            _reviews.Add(review);
        }

        public void ChangePubDate(DateTime newDate)
        {
            PublishedOn = newDate;
        }

        public void AddUpdatePromotion(DbContext context, decimal newPrice, string promotionalText)
        {
            if (BookId != default(int))
                context.Entry(this).Reference(r => r.Promotion).Load();
            if (Promotion == null)
            {
                Promotion = new PriceOfferDdd
                {
                    NewPrice = newPrice,
                    PromotionalText = promotionalText
                };
            }
            else
            {
                Promotion.NewPrice = newPrice; 
                Promotion.PromotionalText = promotionalText;
            }
        }

        public void RemovePromotion(DbContext context)
        {
            if (BookId == default(int))
                throw new InvalidOperationException("You cannot remove a Promotion from a new book.");
            var promotion = context.Set<PriceOfferDdd>().FirstOrDefault(x => x.BookId == BookId);
            if (promotion != null)
                context.Remove(promotion);
        }
    }
}