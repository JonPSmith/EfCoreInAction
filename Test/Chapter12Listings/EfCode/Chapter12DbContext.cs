// Copyright (c) 2017 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using Microsoft.EntityFrameworkCore;
using Test.Chapter12Listings.EfClasses;

namespace Test.Chapter12Listings.EfCode
{
    public class Chapter12DbContext : DbContext
    {
        public DbSet<IndexClass> IndexClasses { get; set; }
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
                .HasComputedColumnSql(
                    "dbo.udf_AverageVotes([FixSubOptimalSqlId])");   
        }
    }
}