// Copyright (c) 2017 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using Microsoft.EntityFrameworkCore;
using test.Chapter05Listings;

namespace Test.Chapter08Listings.EfCode
{
    public class ConcurrencyDbContext : DbContext
    {
        public DbSet<ConcurrecyBook> Books { get; set; }
        public DbSet<ConcurrencyAuthor> Authors { get; set; }

        public ConcurrencyDbContext(
            DbContextOptions<ConcurrencyDbContext> options)      
                : base(options) { }

        protected override void
            OnModelCreating(ModelBuilder modelBuilder) //#A
        {
            //modelBuilder.Entity<ConcurrecyBook>()//#B
            //    .Property(p => p.PublishedOn)    //#B
            //    .IsConcurrencyToken();           //#B

            //modelBuilder.Entity<ConcurrencyAuthor>()//#C
            //    .Property(p => p.RowVersion)         //#C
            //    .ValueGeneratedOnAddOrUpdate()      //#C
            //    .IsConcurrencyToken();              //#C
        }
    }
    /****************************************************
    #A The OnModelCreating method is where I place the configuration of the concurrecy detection
    #B I define the property PublishedOn as a concurrency token, which means EF Core checks it hasn't changed when write out an update
    #C I define an extra property called Timestamp, that will be changed every time the row is created/updated. EF Core checks it hasn't changed when write out an update
     * ************************************************/
}