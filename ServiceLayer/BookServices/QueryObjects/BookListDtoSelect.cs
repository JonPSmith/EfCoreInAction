// Copyright (c) 2016 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System.Linq;
using DataLayer.EfClasses;

namespace ServiceLayer.BookServices.QueryObjects
{
    public static class BookListDtoSelect
    {
        public static IQueryable<BookListDto>            
            MapBookToDto(this IQueryable<Book> books)    
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
                AuthorNames = p.AuthorsLink
                        .OrderBy(q => q.Order)
                        .Select(q => q.Author.Name).ToList(),
                ReviewsCount = p.Reviews.Count,         
                ReviewsAverageVotes =                   
                    p.Reviews.Select(y =>               
                        (decimal?)y.NumStars).Average()          

            });
        }
 

    }
}