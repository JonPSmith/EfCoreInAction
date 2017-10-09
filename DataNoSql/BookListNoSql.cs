// Copyright (c) 2016 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Test")]
namespace DataNoSql
{
    public class BookListNoSql
    {
        private const string IdStart = "booklist/";

        public BookListNoSql() { }

        //For RavenDb I make the Id into a string.
        //Note: to allow orderby it needs to be in format D10, i.e. has leading zeros
        public string Id { get; set; }       
        
        //This returns the RavenId as an int
        public int GetIdAsInt()
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
    }
}