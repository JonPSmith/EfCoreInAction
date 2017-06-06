// Copyright (c) 2016 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System.Linq;
using ServiceLayer.BookServices;
using Test.Chapter10Listings.EfClasses;

namespace Test.Chapter10Listings.QueryObjects
{
    public static class BookDddListDtoSelect
    {
        public static IQueryable<BookListDto>   
            MapBookDddToDto(this IQueryable<BookDdd> books)
        {
            return books.Select(p => new BookListDto
            {
                BookId = p.BookId,                      
                Title = p.Title,                        
                Price = p.Price,                        
                PublishedOn = p.PublishedOn,            
                ActualPrice = p.Promotion == null       
                        ? p.Price                       
                        : p.Promotion.NewPrice,         
                PromotionPromotionalText =              
                        p.Promotion == null             
                          ? null                        
                          : p.Promotion.PromotionalText,
                AuthorsOrdered = string.Join(", ",      
                        p.AuthorsLink                   
                        .OrderBy(q => q.Order)          
                        .Select(q => q.Author.Name)),   
                ReviewsCount = p.Reviews.Count(),       
                ReviewsAverageVotes =                   
                        !p.Reviews.Any()                
                        ? null                          
                        : (decimal?)p.Reviews           
                            .Select(q => q.NumStars).Average()
            });
        }
        /*********************************************************
        #A This method takes in IQueryable<Book> and returns IQueryable<BookListDto>
        #B These are simple copies of existing columns in the Books table
        #C This calculates the selling price, which is the normal price, or the promotion price if that relationship exists 
        #D The PromotionalText depends on whether a PriceOffer exists for this book
        #E This obtains an array of Authors' names, in the right order. We are using a Client vs. Server evaluation as we want the author's names combined into one string
        #F We need to calculate how many reviews there are
        #G We cannot calculate the average of zero reviews, so we need to check the count first
        * *******************************************************/

    }
}