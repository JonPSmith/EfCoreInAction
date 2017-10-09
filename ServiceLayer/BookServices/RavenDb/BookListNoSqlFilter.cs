// Copyright (c) 2016 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using DataNoSql;

namespace ServiceLayer.BookServices.RavenDb
{
    public enum BooksNoSqlFilterBy
    {
        [Display(Name = "All")]
        NoFilter = 0,
        [Display(Name = "By Votes...")]
        ByVotes,
        [Display(Name = "By Year published...")]
        ByPublicationYear
    }

    public static class BookListNoSqlFilter
    {
        public const string AllBooksNotPublishedString = "Coming Soon";

        public static IQueryable<BookListNoSql> FilterBooksBy(
            this IQueryable<BookListNoSql> books, 
            BooksNoSqlFilterBy filterBy, string filterValue)  
        {
            if (string.IsNullOrEmpty(filterValue)) 
                return books;                                 

            switch (filterBy)
            {
                case BooksNoSqlFilterBy.NoFilter:             
                    return books;                             
                case BooksNoSqlFilterBy.ByVotes:
                    var filterVote = int.Parse(filterValue);  
                    return books.Where(x =>                   
                          x.ReviewsAverageVotes > filterVote);
                case BooksNoSqlFilterBy.ByPublicationYear:             
                    if (filterValue == AllBooksNotPublishedString)
                        return books.Where(                       
                            x => x.PublishedOn > DateTime.UtcNow);

                    var filterYear = int.Parse(filterValue);      
                    return books.Where(                           
                        x => x.PublishedOn.Year == filterYear     
                          && x.PublishedOn <= DateTime.UtcNow);   
                default:
                    throw new ArgumentOutOfRangeException
                        (nameof(filterBy), filterBy, null);
            }
        }

    }
}