// Copyright (c) 2016 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Test.Chapter10Listings.EfClasses;

namespace Test.Chapter10Listings.QueryObjects
{
    public enum OrderDddByOptions
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

    public static class BookDddListDtoSort
    {

        public static IQueryable<BookDdd> OrderBooksBy
            (this IQueryable<BookDdd> books,
            OrderDddByOptions orderByOptions)
        {
            switch (orderByOptions)
            {
                case OrderDddByOptions.SimpleOrder:          
                    return books.OrderByDescending(       
                        x => x.BookId);                   
                case OrderDddByOptions.ByVotes:              
                    return books.OrderByDescending(x =>   
                        x.Reviews.Any()                
                             ? x.Reviews.Average(y => y.NumStars) : 0);
                case OrderDddByOptions.ByPublicationDate:    
                    return books.OrderByDescending(       
                        x => x.PublishedOn);              
                case OrderDddByOptions.ByPriceLowestFirst:   
                    return books.OrderBy(x => x.Promotion == null
                        ? x.Price : x.Promotion.NewPrice);
                case OrderDddByOptions.ByPriceHigestFirst:   
                    return books.OrderByDescending(x => x.Promotion == null
                        ? x.Price : x.Promotion.NewPrice);              
                default:
                    throw new ArgumentOutOfRangeException(
                        nameof(orderByOptions), orderByOptions, null);
            }
        }
    }

}