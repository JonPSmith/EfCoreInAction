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
                //There is a bug in EF Core 2.0.0 on this client vs. server query - see https://github.com/aspnet/EntityFrameworkCore/issues/9519
                AuthorNames = p.AuthorsLink.Select(c => c.Author.Name).ToList(),
                //AuthorsOrdered = string.Join(", ",        //#E
                //        p.AuthorsLink                         //#E
                //        .OrderBy(q => q.Order)                //#E
                //        .Select(q => q.Author.Name)),         //#E
                ReviewsCount = p.Reviews.Count(),       
                ReviewsAverageVotes = p.Reviews           
                            .Select(q => (double?)q.NumStars).Average()
            });
        }
        /*********************************************************
        #A This method takes in IQueryable<Book> and returns IQueryable<BookListDto>
        #B These are simple copies of existing columns in the Books table
        #C This calculates the selling price, which is the normal price, or the promotion price if that relationship exists 
        #D The PromotionalText depends on whether a PriceOffer exists for this book
        #E This obtains an array of Authors' names, in the right order. We are using a Client vs. Server evaluation as we want the author's names combined into one string
        #F We need to calculate how many reviews there are
        #G There is a bug in EF Core 2.0.0 that means I need to check for empty collection. 
        * *******************************************************/

    }
}