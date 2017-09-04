// Copyright (c) 2017 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using DataLayer.EfClasses;

namespace Test.Chapter12Listings.EfClasses
{
    public class Ch12Book
    {
        public int Ch12BookId { get; set; }

        [Required]
        [MaxLength(256)]
        public string Title { get; set; }
        public decimal Price { get; set; }

        //----------------------------------------------
        //Computed columns

        public double? AverageVotes { get; set; }
        public decimal ActualPrice { get; set; }

        //-----------------------------------------------
        //relationships

        public ICollection<Ch12Review> Reviews { get; set; }
        public Ch12PriceOffer Promotion { get; set; }
    }
}