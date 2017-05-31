// // Copyright (c) 2017 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// // Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.ComponentModel.DataAnnotations;

namespace Test.Chapter10Listings.MappingClasses
{
    public class BookPriceOfferDto
    {
        public int BookId { get; set; }
        [Required]
        [MaxLength(256)]
        public string Title { get; set; }
        public decimal Price { get; set; }

        public decimal? PromotionNewPrice { get; set; }
    }
}