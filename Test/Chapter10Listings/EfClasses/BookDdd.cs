// // Copyright (c) 2017 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// // Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Test.Chapter10Listings.EfClasses
{
    public class BookDdd
    {
        public int BookId { get; set; }

        [Required] //#A
        [MaxLength(256)] //#B
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime PublishedOn { get; set; }
        [MaxLength(64)] //#B
        public string Publisher { get; set; }
        public decimal Price { get; set; }

        [MaxLength(512)] //#B
        public string ImageUrl { get; set; }


        //-----------------------------------------------
        //relationships

        public PriceOfferDdd Promotion { get; private set; }
        public ICollection<ReviewDdd> Reviews { get; private set; }
        public ICollection<BookAuthorDdd> AuthorsLink { get; private set; }
    }
}