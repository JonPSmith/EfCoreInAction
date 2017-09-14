// // Copyright (c) 2017 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// // Licensed under MIT licence. See License.txt in the project root for license information.

using System.Collections.Generic;

namespace Test.Chapter10Listings.MappingClasses
{
    public class BookDto //#A
    {
        public int BookId { get; set; } //#B
        public string Title { get; set; } //#B
        public decimal ActualPrice { get; set; } //#B

        public decimal //#C
            OrgPrice { get; set; } //#D
        public string
            PromotionalText { get; set; } //#D

        public ICollection<ReviewDto> 
            Reviews { get; set; } //#E
    }
    /**********************************************************
    #A This is a simple DTO to show how AutoMapper maps using the DTO's property names and types
    #B These are properties in the Book entity - they are mapped because of their name
    #C I must set the type to nullable, as if there is no PriceOffer linked to the book then it will be set to null
    #D These 'flatten' a relationship. The start of the name, Promotion, refers to the one-to-one relationship, while the end of the property name refers to the property in the relationship class, e.g. NewPrice
    #E This is an example of a nested DTO. This will map the Book's Reviews collection navigational property to a collection of ReviewDtos. The ReviewDto class only has the property NumVotes, which makes for an efficient copy
     * * ***********************************************************/
}