// Copyright (c) 2017 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System.ComponentModel.DataAnnotations;

namespace Test.Chapter12Listings.EfClasses
{
    public class Ch12PriceOffer
    {
        public const int PromotionalTextLength = 200;

        public int Ch12PriceOfferId { get; set; }
        public decimal NewPrice { get; set; }
        [MaxLength(PromotionalTextLength)]
        public string PromotionalText { get; set; }

        //-----------------------------------------------
        //Relationships

        public int BookId { get; set; }
    }
}