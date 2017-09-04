// // Copyright (c) 2017 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// // Licensed under MIT licence. See License.txt in the project root for license information.

using DataLayer.EfClasses;
using DataLayer.EfCode.Configurations;
using Microsoft.EntityFrameworkCore;

namespace Test.Chapter10Listings.EfCode
{
    public class OrderContext : DbContext
    {
        public DbSet<Book> Books { get; set; } //#A
        public DbSet<Order> Orders { get; set; }
        //public DbSet<AddressO> Addresses { get; set; }

        public OrderContext(
            DbContextOptions<OrderContext> options)      
            : base(options) { }

        protected override void
            OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new BookConfig());
            modelBuilder.ApplyConfiguration( new LineItemConfig());

            modelBuilder.Ignore<Review>();    //#B
            modelBuilder.Ignore<PriceOffer>();//#B
            modelBuilder.Ignore<Author>();    //#B
            modelBuilder.Ignore<BookAuthor>();//#B
        }
    }
    /*********************************************************
    #A I included the DbSet<Book> property to make sure the name of the table was set. I could have used the Fluent API modelBuilder.Entity<Book>().ToTable("Books")
    #B I used the Fluent API Ignore<T> method to stop these entities/tables from being included in the 
     * ********************************************************/
}