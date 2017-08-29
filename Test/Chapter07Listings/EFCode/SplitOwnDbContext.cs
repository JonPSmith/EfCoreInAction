// Copyright (c) 2017 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using DataLayer.EfClasses;
using Microsoft.EntityFrameworkCore;
using Test.Chapter07Listings.EfClasses;
using Test.Chapter07Listings.EFCode.Configurations;
using Test.Chapter07Listings.SplitOwnClasses;

namespace Test.Chapter07Listings.EFCode
{
    public class SplitOwnDbContext : DbContext
    {
 
        public DbSet<BookSummary> BookSummaries { get; set; }

        public SplitOwnDbContext(
            DbContextOptions<SplitOwnDbContext> options)
            : base(options)
        { }

        protected override void OnModelCreating
            (ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new BookSummaryConfig());
            modelBuilder.ApplyConfiguration(new BookDetailConfig());
        }
    }
    /****************************************************

     * ******************************************************/
}