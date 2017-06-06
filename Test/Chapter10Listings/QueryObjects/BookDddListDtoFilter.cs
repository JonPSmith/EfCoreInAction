// Copyright (c) 2016 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using ServiceLayer.BookServices.QueryObjects;
using Test.Chapter10Listings.EfClasses;

namespace Test.Chapter10Listings.QueryObjects
{
    public enum BooksDddFilterBy
    {
        [Display(Name = "All")]
        NoFilter = 0,
        [Display(Name = "By Votes...")]
        ByVotes,
        [Display(Name = "By Year published...")]
        ByPublicationYear
    }

    public static class BookDddListDtoFilter
    {
        public const string AllBooksNotPublishedString = "Coming Soon";

        public static IQueryable<BookDdd> FilterBooksBy(
            this IQueryable<BookDdd> books,
            BooksDddFilterBy filterBy, string filterValue)         
        {
            if (string.IsNullOrEmpty(filterValue))              
                return books;                                   

            switch (filterBy)
            {
                case BooksDddFilterBy.NoFilter:                    
                    return books;                               
                case BooksDddFilterBy.ByVotes:
                    var filterVote = int.Parse(filterValue);     
                    return books.Where(                        
                        x => x.Reviews.Any()                  
                          && x.Reviews.Average(y => y.NumStars) > filterVote);
                case BooksDddFilterBy.ByPublicationYear:             
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