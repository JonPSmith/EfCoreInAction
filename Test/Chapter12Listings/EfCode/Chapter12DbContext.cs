// Copyright (c) 2017 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using Microsoft.EntityFrameworkCore;
using Test.Chapter12Listings.EfClasses;

namespace Test.Chapter12Listings.EfCode
{
    public class Chapter12DbContext : DbContext
    {
        public DbSet<IndexClass> IndexClasses { get; set; }
        public DbSet<Ch12Book> Books { get; set; }
        public DbSet<Ch12PriceOffer> PriceOffers { get; set; }
        public DbSet<FixSubOptimalSql> FixSubOptimalSqls { get; set; }

        public Chapter12DbContext(
            DbContextOptions<Chapter12DbContext> options)      
            : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<IndexClass>()
                .HasIndex(p => p.WithIndex);

            modelBuilder.Entity<FixSubOptimalSql>()
                .Property(p => p.AverageVotes)
                //The computed column is set up by a script
                .ValueGeneratedOnAddOrUpdate();

            modelBuilder.Entity<Ch12Book>()
                .HasKey(p => p.BookId);

            modelBuilder.Entity<Ch12Book>()
                .Property(p => p.ActualPrice)
                //The computed column is set up by a script
                .ValueGeneratedOnAddOrUpdate();

            modelBuilder.Entity<Ch12Book>()
                .HasOne(r => r.Promotion)
                .WithOne()
                .HasForeignKey<Ch12PriceOffer>(k => k.BookId);
        }
    }
}