// Copyright (c) 2016 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore;

[assembly: InternalsVisibleTo("Test")]

namespace DataLayer.EfClasses
{
    public class Book
    {
        public const int PromotionalTextLength = 200;

        private readonly List<Review> _reviews = new List<Review>(); //#A
        private readonly List<BookAuthor> _bookAuthors = new List<BookAuthor>();

        public int BookId { get; set; }

        [Required] //#A
        [MaxLength(256)] //#B
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime PublishedOn { get; set; }
        [MaxLength(64)] //#B
        public string Publisher { get; set; }
        public decimal OrgPrice { get; private set; } //#A
        public decimal ActualPrice { get; private set; } //#A
        [MaxLength(PromotionalTextLength)]
        public string PromotionalText { get; private set; } //#A
        public bool HasPromotion => PromotionalText != null;
        [MaxLength(512)] //#B
        public string ImageUrl { get; set; }
        public bool SoftDeleted { get; set; }

        //The pre-calculated values
        [ConcurrencyCheck] //#B
        public int ReviewsCount { get; private set; } //#C
        [ConcurrencyCheck] //#B
        public double? AverageVotes { get; private set; } //#C
        public string AuthorsString { get; private set; }

        //-----------------------------------------------
        //relationships

        public IEnumerable<Review> Reviews => _reviews.ToList(); //#D
        public IEnumerable<BookAuthor> AuthorsLink => _bookAuthors.ToList();

        //------------------------------------------------------
        //Ctors

        //This ctor is needed for EF Core
        internal Book()
        {
        }

        /// <summary>
        /// This is used for creating a new Book
        /// </summary>
        /// <param name="title"></param>
        /// <param name="description"></param>
        /// <param name="publishedOn"></param>
        /// <param name="publisher"></param>
        /// <param name="orgPrice"></param>
        /// <param name="imageUrl"></param>
        /// <param name="authors"></param>
        /// <param name="authorsString"></param>
        public Book(string title, string description, DateTime publishedOn, //#B
            string publisher, decimal orgPrice, string imageUrl, 
            ICollection<Author> authors, string authorsString = null)
        {
            Title = title;
            Description = description;
            PublishedOn = publishedOn;
            Publisher = publisher;
            OrgPrice = orgPrice; //#C
            ActualPrice = OrgPrice; //#D
            ImageUrl = imageUrl;

            ReviewsCount = 0;
            AverageVotes = null;

            byte order = 0;
            _bookAuthors = authors.Select(a => new BookAuthor(this, a, order++)).ToList();
            AuthorsString = authorsString ?? string.Join(", ", authors.Select(a => a.Name));
        }

        //------------------------------------------
        //Action methods

        /// <summary>
        /// This sets up a promotion
        /// It assumes OrgPrice was set 
        /// </summary>
        /// <param name="newPrice"></param>
        /// <param name="promotionalText"></param>
        /// <returns>string conatining error, or null if no error</returns>
        public string AddPromotion(decimal newPrice, //#E
            string promotionalText)                  //#E
        {
            if (promotionalText == null) //#F
                return 
            "You must provide some text to go with the promotion";

            ActualPrice = newPrice;  //#G
            PromotionalText = promotionalText; //#H
            return null; //#I
        }

        public void RemovePromotion() //#J
        {
            ActualPrice = OrgPrice; //#K
            PromotionalText = null; //#L
        }
        /**ActualPrice part*******************************************************
        #A The properties that control the price all have a private setter so that only the Book entity can change their values
        #B The only public way to create a Book entity is now via this constructor
        #C I set the OrgPrice with the recommended retail price of the book
        #D I also set the ActualPrice to the OrgPrice because a new book starts off without any promotion
        #E This method adds a price promotion. It returns null if successful, or an error message if there was an error
        #F I used the fact that the PromotionalText property is not null to tell me if there is a promotion on this book. Therefore it mustn’t be null when adding a promotion
        #G I replace the current ActualPrice with the new, promotional price
        #H I also set the PromotionalText property, which then tells the rest of the system that a propmotion is in place
        #I I return null to say that it was successful
        #J This method removes a price promotion 
        #K This sets the book's ActualPrice to the recommended retail price held in the OrgPrice property
        #L It also nulls the PromotionalText, which tells the rest of the system that there isn't a price promotion on this book
         * ****************************************************************/

        public void AddReview(DbContext context, //#E
            int numStars, string comment, 
            string voterName) 
        {
            context.Entry(this)
                .Collection(c => c.Reviews).Load(); //#F
            AddReviewWhenYouKnowReviewCollectionIsLoaded(numStars, comment, voterName);
        }

        public void AddReviewWhenYouKnowReviewCollectionIsLoaded(int numStars, string comment, string voterName)
        {
            var review = new 
                Review(numStars, comment, voterName); //#G
            _reviews.Add(review); //#G
            AverageVotes = _reviews.Average(x => x.NumStars); //#H
            ReviewsCount = _reviews.Count;                    //#H
        }

        public void RemoveReview(DbContext context, //#I
            Review review)                          //#I
        {
            context.Entry(this) //#J
                .Collection(c => c.Reviews).Load(); //#J

            _reviews.Remove(review); //#K
            AverageVotes = _reviews.Any()
                ? _reviews.Average(x => x.NumStars) //#L
                : (double?)null; //#M
            ReviewsCount = _reviews.Count; //#N
        }
        /**Reviews*******************************************
        #A I add a backing field, which is a list. I then tell EF Core to use this for all reads and writes
        #B I add a [ConcurrencyCheck] attribute to this property - see section on handling concurrent updates of reviews
        #C This holds a pre-calculated average of the reviews and the number of reviews for this book. Note that it is read-only so that it cannot be changed outside this class
        #D This returns a copy of the reviews that were loaded. By taking a copy it means no one can alter the list by casting the IEnumerable<T> to List<T>
        #E I add a method to allow a new Review to be added to the _reviews collection
        #F I make sure the backing field, _reviews, has the reviews for this book loaded
        #G I create a new review using the data give and then add the new review to the backing field _reviews. This will update the database on the call to SaveChanges
        #H I then recalculate the average votes and number of reviews for this book
        #I I add a method to remove a review from the _reviews collection
        #J I make sure the backing field, _reviews, has the reviews for this book loaded
        #K I remove the review from the list. This will update the database on the call to SaveChanges
        #L If there are any reviews I recalculate the average votes for the book
        #M If there are no reviews I set the value to null
        #N I recalculate the number of reviews in this book
        * *********************************************************/

    }

}