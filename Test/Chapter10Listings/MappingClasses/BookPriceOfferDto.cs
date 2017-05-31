// // Copyright (c) 2017 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// // Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.ComponentModel.DataAnnotations;

namespace Test.Chapter10Listings.MappingClasses
{
    public class BookPriceOfferDto //#A
    {
        public int BookId { get; set; } //#B
        [Required]
        [MaxLength(256)]
        public string Title { get; set; } //#B
        public decimal Price { get; set; } //#B

        public decimal? //#C
            PromotionNewPrice { get; set; } //#D
    }
    /**********************************************************
    #A This is a simple DTO to show how AutoMapper maps using the DTO's property names and types
    #B These are properties in the Book entity - they are mapped because of their name
    #C I must set the type to nullable, as if there is no PriceOffer linked to the book then it will be set to null
    #D The name of the property is made up of the name of the navigational property, Promotion, followed by the name of the property I want to map in that class, i.e. NewPrice
     * ***********************************************************/
}