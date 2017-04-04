// Copyright (c) 2016 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DataLayer.EfClasses
{
    public class Book                        
    {
        public const int WebUrlMaxlength = 512; //#A

        public int BookId { get; set; }

        [Required] //#B
        [MaxLength(256)] //#C
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime PublishedOn { get; set; }
        [MaxLength(64)] //#D
        public string Publisher { get; set; }
        public decimal Price { get; set; }

        [MaxLength(WebUrlMaxlength)] //#E
        public string ImageUrl { get; set; }

        //-----------------------------------------------
        //relationships

        public PriceOffer Promotion { get; set; }         
        public ICollection<Review> Reviews { get; set; } 
        public ICollection<BookAuthor> 
            AuthorsLink { get; set; }                    
    }
    /****************************************************#
    #A I normally use constants for MaxLength attributes because I might need to repeat the MaxLength in DTO (Data Trasfer Object)
    #B This tells EF Core that the string is non-nullable.
    #C This defines the the size of the string column in the database
    #D This also defines the size of the string column in the database
    #E Again, I define the size of the string column in the database, but I use a constant rather than a number, which I think is a better practice
     * **************************************************/
}