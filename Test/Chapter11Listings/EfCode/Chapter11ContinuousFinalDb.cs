// Copyright (c) 2017 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using Microsoft.EntityFrameworkCore;
using Test.Chapter11Listings.EfClasses;

namespace Test.Chapter11Listings.EfCode
{
    public class Chapter11ContinuousFinalDb : DbContext
    {
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Addresses> Addresses { get; set; }

        public Chapter11ContinuousFinalDb(
            DbContextOptions<Chapter11ContinuousFinalDb> options)      
            : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Customer>()
                .HasOne(p => p.AddressData)
                .WithOne()
                .HasForeignKey<Addresses>(p => p.CustFK);
        }
    }
}