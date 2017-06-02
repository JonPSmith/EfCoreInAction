// // Copyright (c) 2017 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// // Licensed under MIT licence. See License.txt in the project root for license information.

using Microsoft.EntityFrameworkCore;
using Test.Chapter10Listings.EfClasses;
using Test.Chapter10Listings.EfCode.Configuration;


namespace Test.Chapter10Listings.EfCode
{
    public class Chapter10DbContext : DbContext
    {
        public DbSet<BookDdd> Books { get; set; }

        public Chapter10DbContext(
            DbContextOptions<Chapter10DbContext> options)
            : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //set primary keys as not the same as class name
            modelBuilder.Entity<PriceOfferDdd>()
                .HasKey(k => k.PriceOfferId);
            modelBuilder.Entity<ReviewDdd>()
                .HasKey(k => k.ReviewId);
            modelBuilder.Entity<AuthorDdd>()
                .HasKey(k => k.AuthorId);
            modelBuilder.Entity<BookAuthorDdd>()
                .HasKey(k => new {k.BookId, k.AuthorId});

            modelBuilder.Entity<MyUnique>().Configure();
            modelBuilder.Entity<BookDdd>().Configure();
        }

    }
}
