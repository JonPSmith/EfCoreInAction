// Copyright (c) 2017 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.ComponentModel.DataAnnotations;

namespace Test.Chapter07Listings.SplitOwnClasses
{
    public class BookDetail
    {
        public int BookId { get; set; }

        public string Description { get; set; }
        public DateTime PublishedOn { get; set; }
        [MaxLength(64)] //#B
        public string Publisher { get; set; }
        public decimal Price { get; set; }

        [MaxLength(512)] //#B
        public string ImageUrl { get; set; }
    }
}