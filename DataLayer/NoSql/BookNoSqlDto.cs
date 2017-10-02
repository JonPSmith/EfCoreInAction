// Copyright (c) 2016 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using DataLayer.EfClasses;

[assembly: InternalsVisibleTo("Test")]
namespace DataLayer.NoSql
{
    internal class BookNoSqlDto
    {
        public string Id { get; set; }           
        public string Title { get; set; }
        public DateTime PublishedOn { get; set; } 
        public decimal Price { get; set; }        
        public decimal ActualPrice { get; set; }             
        public string PromotionPromotionalText { get; set; }
        public string AuthorsOrdered => string.Join(", ", AuthorNames);
        internal ICollection<string> AuthorNames { get; set; }
        public int ReviewsCount { get; set; }      
        public double? ReviewsAverageVotes { get; set; }

        public static string ConvertIdToRavenId(int bookId)
        {
            return bookId.ToString("D10");
        }

        public static BookNoSqlDto SelectBook(IQueryable<Book> books, int bookId)
        {
            return books.Select(p => new BookNoSqlDto
            {
                Id = ConvertIdToRavenId(bookId),                      
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
                ReviewsAverageVotes =  p.Reviews.Select(y => (double?)y.NumStars).Average() 

            }).Single(x => x.Id == ConvertIdToRavenId(bookId));
        }
    }
}