// Copyright (c) 2017 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.ComponentModel.DataAnnotations;

namespace Test.Chapter14Listings.EfClasses
{
    public class ConcurrencyAuthorMySql
    {
        public int ConcurrencyAuthorMySqlId { get; set; }

        public string Name { get; set; }

        [Timestamp]
        public DateTime ChangeCheck { get; set; }
    }
}