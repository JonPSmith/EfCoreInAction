// Copyright (c) 2017 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.ComponentModel.DataAnnotations;
using Test.Chapter08Listings.EfClasses;

namespace Test.Chapter14Listings.EfClasses
{
    public class ConcurrencyBookMySql
    {
        public int ConcurrencyBookMySqlId { get; set; }
        public string Title { get; set; }
        [ConcurrencyCheck] //#A
        public DateTime PublishedOn { get; set; }

        public ConcurrencyAuthorMySql Author { get; set; }
    }
    /*********************************************
    #A This tells EF Core that PublishedOn property ia a concurrecy token, which means EF Core will check it hasn't changed when I update it
     * **********************************************/
}