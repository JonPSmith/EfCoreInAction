using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Test.Chapter10Listings.EfClasses;
using Test.Chapter10Listings.EfCode.Configuration;


namespace Test.Chapter10Listings.EfCode
{
    public class Chapter10DbContext : DbContext
    {

        public Chapter10DbContext(
            DbContextOptions<Chapter10DbContext> options)
            : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MyUnique>().Configure();
        }

    }
}
