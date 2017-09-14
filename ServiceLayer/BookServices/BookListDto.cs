// Copyright (c) 2016 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.Collections;
using System.Collections.Generic;

namespace ServiceLayer.BookServices
{
    public class BookListDto
    {
        public int BookId { get; set; }   
        public string Title { get; set; }
        public DateTime PublishedOn { get; set; }  
        public decimal ActualPrice { get; set; } 
        public decimal OrgPrice { get; set; }   
        public string PromotionalText { get; set; }
        public bool HasPromotion => PromotionalText != null;
        public ICollection<string> AuthorNames { get; set; }
        //There is a bug in EF Core 2.0.0 on this client vs. server query - see https://github.com/aspnet/EntityFrameworkCore/issues/9519
        //public string AuthorsOrdered => string.Join(", ", AuthorNames);
        public string AuthorsOrdered { get; set; }  //#F

        public int ReviewsCount { get; set; }      //#G
        public double? 
            ReviewsAverageVotes { get; set; }      //#H
    /******************************************************
    #A I need the Primary Key if the customer clicks the entry to buy the book
    #B While the publish date isn't shown we will want to sort by it, so we have to include it
    #C This is the normal Price
    #D This is the selling price - either the normal price, or the promotional.NewPrice if present
    #E The promotional text to show if there is a new price
    #F An array of the authors' names in the right order
    #G The number of people who reviewed the book
    #H The average of all the Votes - null if no votes
     * ***************************************************/

        
    }
}