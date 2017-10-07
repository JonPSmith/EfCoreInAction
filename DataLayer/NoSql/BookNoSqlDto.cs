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
    public class BookNoSqlDto
    {
        private const string IdStart = "booklist/";

        //Need this to make the EF Core LINQ code efficient at finding the item
        private int _bookId;

        private List<string> _authors;

        /// <summary>
        /// This ensures that the Id is set to the correct format
        /// </summary>
        /// <param name="bookId"></param>
        internal BookNoSqlDto(int bookId)
        {
            _bookId = bookId;
            Id = ConvertIdToNoSqlId(bookId);
        }

        internal BookNoSqlDto()
        {
        }

        //For RavenDb I make the Id into a string.
        //Note: to allow orderby it needs to be in format D10, i.e. has leading zeros
        public string Id { get; set; }       
        
        //This returns the RavenId as an int
        public int StringIdAsInt()
        {
            return int.Parse(Id.Substring(IdStart.Length));
        }

        public string Title { get; set; }
        public DateTime PublishedOn { get; set; } 
        public decimal Price { get; set; }        
        public decimal ActualPrice { get; set; }             
        public string PromotionPromotionalText { get; set; }
        public string AuthorsOrdered { get; set; }
        public int ReviewsCount { get; set; }      
        public double? ReviewsAverageVotes { get; set; }

        public static string ConvertIdToNoSqlId(int bookId)
        {
            return IdStart + bookId.ToString("D10");
        }

        public static BookNoSqlDto ProjectBook(IQueryable<Book> books, int bookId)
        {
            var dto = books.Select(p => new BookNoSqlDto
            {
                _bookId = p.BookId,
                _authors = p.AuthorsLink
                    .OrderBy(q => q.Order)
                    .Select(q => q.Author.Name).ToList(),
                Id = ConvertIdToNoSqlId(bookId),                      
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
                ReviewsCount = p.Reviews.Count,        
                ReviewsAverageVotes =  p.Reviews.Select(y => (double?)y.NumStars).Average() 

            }).Single(x => x._bookId == bookId);

            dto.AuthorsOrdered = string.Join(", ", dto._authors);

            return dto;
        }
    }
}