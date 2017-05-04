// Copyright (c) 2017 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using Microsoft.EntityFrameworkCore;
using Test.Chapter08Listings.EfClasses;
using Test.Chapter08Listings.EfCode.Configuration;

namespace Test.Chapter08Listings.EfCode
{
    public class Chapter08DbContext : DbContext
    {
        public DbSet<DefaultTest> Defaults { get; set; }
        public DbSet<Person> Persons { get; set; }

        public Chapter08DbContext(
            DbContextOptions<Chapter08DbContext> options)
            : base(options)
        { }

        protected override void
            OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DefaultTest>()
                .Configure(new OrderIdValueGenerator());
            modelBuilder.Entity<Person>().Configure();
        }
    }
}
