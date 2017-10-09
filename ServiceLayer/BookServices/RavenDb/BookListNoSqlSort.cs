// Copyright (c) 2016 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using DataNoSql;

namespace ServiceLayer.BookServices.RavenDb
{
    public enum OrderNoSqlByOptions
    {
        [Display(Name = "sort by...")]
        SimpleOrder = 0,
        [Display(Name = "Votes ↑")]
        ByVotes,
        [Display(Name = "Publication Date ↑")]
        ByPublicationDate,
        [Display(Name = "Price ↓")]
        ByPriceLowestFirst,
        [Display(Name = "Price ↑")]
        ByPriceHigestFirst
    }

    public static class BookListNoSqlSort
    {

        public static IQueryable<BookListNoSql> OrderBooksBy
            (this IQueryable<BookListNoSql> books,
            OrderNoSqlByOptions orderByOptions)
        {
            switch (orderByOptions)
            {
                case OrderNoSqlByOptions.SimpleOrder:
                    return books.OrderByDescending(      
                        x => x.Id);                   
                case OrderNoSqlByOptions.ByVotes:         
                    return books.OrderByDescending(x =>   
                        x.ReviewsAverageVotes);           
                case OrderNoSqlByOptions.ByPublicationDate:  
                    return books.OrderByDescending(   
                        x => x.PublishedOn);          
                case OrderNoSqlByOptions.ByPriceLowestFirst: 
                    return books.OrderBy(x => x.ActualPrice);
                case OrderNoSqlByOptions.ByPriceHigestFirst: 
                    return books.OrderByDescending(       
                        x => x.ActualPrice);              
                default:
                    throw new ArgumentOutOfRangeException(
                        nameof(orderByOptions), orderByOptions, null);
            }
        }
    }

}