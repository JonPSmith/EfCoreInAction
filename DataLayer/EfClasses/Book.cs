// Copyright (c) 2016 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;

namespace DataLayer.EfClasses
{
    public class Book                                   //#A
    {
        public int BookId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime PublishedOn { get; set; }
        public string Publisher { get; set; }
        public decimal Price { get; set; }
        /// <summary>
        /// Holds the url to get the image of the book
        /// </summary>
        public string ImageUrl { get; set; }

        //-----------------------------------------------
        //relationships

        public PriceOffer Promotion { get; set; }        //#B 
        public ICollection<Review> Reviews { get; set; } //#C
        public ICollection<BookAuthor> 
            AuthorsLink { get; set; }                    //#D
    }
    /****************************************************#
    #A The Book class contains the main book information
    #B This is the link to the optional PriceOffer
    #C There can be zero to many Reviews of the book
    #D This provides a link to the Many-to-Many linking table that links to the Authors of this book
     * **************************************************/
}