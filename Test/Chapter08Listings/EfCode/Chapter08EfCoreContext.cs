// Copyright (c) 2016 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using DataLayer.EfClasses;
using DataLayer.EfCode;
using DataLayer.EfCode.Configurations;
using Microsoft.EntityFrameworkCore;

namespace Test.Chapter08Listings.EfCode
{
    public class Chapter08EfCoreContext : DbContext
    {
        public DbSet<Book> Books { get; set; }              
        public DbSet<Author> Authors { get; set; }          
        public DbSet<Order> Orders { get; set; }            

        public Chapter08EfCoreContext(                             
            DbContextOptions<Chapter08EfCoreContext> options)      
            : base(options) {}

        protected override void
            OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new BookConfig());
            modelBuilder.ApplyConfiguration(new BookAuthorConfig());
            modelBuilder.ApplyConfiguration(new LineItemConfig());

            modelBuilder.HasDbFunction(
                () => MyUdfMethods.AverageVotes(default(int)));
            //.HasSchema("dbo"); - you don't need to set the schema if its the default
        }

    }
}

