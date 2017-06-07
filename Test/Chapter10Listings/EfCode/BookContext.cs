// // Copyright (c) 2017 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// // Licensed under MIT licence. See License.txt in the project root for license information.

using DataLayer.EfClasses;
using DataLayer.EfCode.Configurations;
using Microsoft.EntityFrameworkCore;

namespace Test.Chapter10Listings.EfCode
{
    public class BookContext : DbContext
    {
        public DbSet<Book> Books { get; set; }
        public DbSet<Author> Authors { get; set; }
        public DbSet<PriceOffer> PriceOffers { get; set; }

        public BookContext(
            DbContextOptions<BookContext> options)      
            : base(options) { }

        protected override void
            OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Book>().Configure();
            modelBuilder.Entity<BookAuthor>().Configure();
            modelBuilder.Entity<PriceOffer>().Configure();
        }
    }
}