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
        public string AuthorsString { get; set; }
        public int ReviewsCount { get; set; }   
        public double? AverageVotes { get; set; }   
        }
}