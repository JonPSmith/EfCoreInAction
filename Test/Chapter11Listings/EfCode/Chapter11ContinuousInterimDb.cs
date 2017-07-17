// Copyright (c) 2017 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using Microsoft.EntityFrameworkCore;
using Test.Chapter11Listings.EfClasses;

namespace Test.Chapter11Listings.EfCode
{
    public class Chapter11ContinuousInterimDb : DbContext
    {
        public DbSet<CustomerAndAddresses> CustomerAndAddresses { get; set; }

        public Chapter11ContinuousInterimDb(
            DbContextOptions<Chapter11ContinuousInterimDb> options)      
            : base(options) { }
    }
}