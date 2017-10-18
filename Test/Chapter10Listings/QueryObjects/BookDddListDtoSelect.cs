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
                            .Select(q => (decimal?)q.NumStars).Average()
            });
        }

    }
}