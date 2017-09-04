// // Copyright (c) 2017 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// // Licensed under MIT licence. See License.txt in the project root for license information.

using DataLayer.EfClasses;
using DataLayer.EfCode.Configurations;
using Microsoft.EntityFrameworkCore;

namespace Test.Chapter10Listings.EfCode
{
    public class BookContext : DbContext
    {
        public DbSet<Book> Books { get; set; } //#A
        public DbSet<Author> Authors { get; set; } //#A
        public DbSet<PriceOffer> PriceOffers { get; set; } //#A

        public BookContext(
            DbContextOptions<BookContext> options)      
            : base(options) { }

        protected override void
            OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new BookConfig());       //#B
            modelBuilder.ApplyConfiguration(new BookAuthorConfig()); //#B
            modelBuilder.ApplyConfiguration(new PriceOfferConfig()); //#B
        }
        /*****************************************************************
        #A We only define three of the five tables in the database: Books, Authors and PriceOffers. The other two tables, Review and BookAuthor are found via navigational links from the other tables
        #B I have moved the Fluent API configuration of various entity classes to separate configration classes that implement the IEntityTypeConfiguration<T> interface
         * ****************************************************************/
    }
}