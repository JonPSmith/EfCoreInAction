// Copyright (c) 2017 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.ComponentModel.DataAnnotations;
using DataLayer.EfClasses;

namespace Test.Chapter12Listings.EfClasses
{
    public class Ch12Book
    {
        public int BookId { get; set; }

        [Required]
        [MaxLength(256)]
        public string Title { get; set; }
        public decimal Price { get; set; }

        //----------------------------------------------
        //Computed column

        public decimal ActualPrice { get; set; }

        //-----------------------------------------------
        //relationships

        public Ch12PriceOffer Promotion { get; set; }
    }
}