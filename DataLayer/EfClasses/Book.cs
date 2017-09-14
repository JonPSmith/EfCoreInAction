// Copyright (c) 2016 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Test")]

namespace DataLayer.EfClasses
{
    public class Book
    {
        public const int PromotionalTextLength = 200;

        private readonly List<Review> _reviews = new List<Review>();
        private readonly List<BookAuthor> _bookAuthors = new List<BookAuthor>();

        public int BookId { get; set; }

        [Required] //#A
        [MaxLength(256)] //#B
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime PublishedOn { get; set; }
        [MaxLength(64)] //#B
        public string Publisher { get; set; }
        public decimal ActualPrice { get; private set; }
        public decimal OrgPrice { get; private set; }
        [MaxLength(PromotionalTextLength)]
        public string PromotionalText { get; private set; }
        public bool HasPromotion => PromotionalText != null;
        [MaxLength(512)] //#B
        public string ImageUrl { get; set; }
        public bool SoftDeleted { get; set; }

        //The pre-calculated values
        public int ReviewsCount { get; private set; }
        public double? AverageVotes { get; private set; }
        public string AuthorsString { get; private set; }

        //-----------------------------------------------
        //relationships

        public IEnumerable<Review> Reviews => _reviews.ToList();
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
        public Book(string title, string description, DateTime publishedOn, string publisher, decimal orgPrice, string imageUrl, 
            ICollection<Author> authors, string authorsString = null)
        {
            Title = title;
            Description = description;
            PublishedOn = publishedOn;
            Publisher = publisher;
            ActualPrice = orgPrice;
            OrgPrice = orgPrice;
            ImageUrl = imageUrl;

            _bookAuthors = authors.Select(a => new BookAuthor {Book = this, Author = a}).ToList();
            AuthorsString = authorsString ?? string.Join(", ", authors.Select(a => a.Name));
        }

        //------------------------------------------
        //Action methods

        public void AddReview(Review review) //#D
        {
            _reviews.Add(review);
            AverageVotes = _reviews.Average(x => x.NumStars);
            ReviewsCount = _reviews.Count;
        }

        public void RemoveReview(Review review) //#G
        {
            _reviews.Remove(review);
            AverageVotes = _reviews.Any()
                ? _reviews.Average(x => x.NumStars)
                : (double?)null;
            ReviewsCount = _reviews.Count;
        }

        /// <summary>
        /// This sets up a promotion
        /// It assumes OrgPrice was set 
        /// </summary>
        /// <param name="newPrice"></param>
        /// <param name="promotionalText"></param>
        /// <returns>string conatining error, or null if no error</returns>
        public string AddPromotion(decimal newPrice, string promotionalText)
        {
            if (promotionalText == null)
                return "You must provide some text to go with the promotion";
            ActualPrice = newPrice;
            PromotionalText = promotionalText;
            return null;
        }

        public void RemovePromotion()
        {
            ActualPrice = OrgPrice;
            PromotionalText = null;
        }
    }

}