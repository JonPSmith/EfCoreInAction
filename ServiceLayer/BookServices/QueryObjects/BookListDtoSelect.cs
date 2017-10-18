// Copyright (c) 2016 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System.Linq;
using DataLayer.EfClasses;
using DataLayer.SqlCode;

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
                AuthorsOrdered = UdfDefinitions.AuthorsStringUdf(p.BookId),
                //There is a bug in EF Core 2.0.0 on this client vs. server query - see https://github.com/aspnet/EntityFrameworkCore/issues/9519
                //AuthorsOrdered = string.Join(", ",    
                //        p.AuthorsLink                 
                //        .OrderBy(q => q.Order)        
                //        .Select(q => q.Author.Name)), 
                ReviewsCount = p.Reviews.Count,         
                ReviewsAverageVotes =                   
                    p.Reviews.Select(y =>               
                        (decimal?)y.NumStars).Average()          

            });
        }
 

    }
}