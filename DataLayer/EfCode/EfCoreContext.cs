// Copyright (c) 2016 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using DataLayer.EfClasses;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;

namespace DataLayer.EfCode
{
    public class EfCoreContext : DbContext
    {
        public DbSet<Book> Books { get; set; }            
        public DbSet<Author> Authors { get; set; }        
        public DbSet<PriceOffer> PriceOffers { get; set; }
        public DbSet<Order> Orders { get; set; } 

        public EfCoreContext(                             
            DbContextOptions<EfCoreContext> options)      
            : base(options) {}

        protected override void
            OnModelCreating(ModelBuilder modelBuilder)    
        {                                                 
            modelBuilder.Entity<Book>() //#A
                .Property(x => x.PublishedOn)
                .HasColumnType("date");

            modelBuilder.Entity<Book>()
                .Property(p => p.Price) //#B
                .HasColumnType("decimal(9,2)");

            modelBuilder.Entity<Book>() //#C
                .Property(x => x.ImageUrl)
                .IsUnicode(false);

            modelBuilder.Entity<BookAuthor>()          
                .HasKey(x => new {x.BookId, x.AuthorId});

            modelBuilder.Entity<LineItem>()        
                .HasOne(p => p.ChosenBook)         
                .WithMany()                        
                .OnDelete(DeleteBehavior.Restrict);
        }                                                 
    }
    /*********************************************************
    #A The convention-based mapping for .NET DateTime is SQL datetime2. This command changes the SQL column type to date, which only holds the date, not time
    #B I set a smaller precision and scale of (9,2) for the price instead of the default (18,2)
    #C The convention-based mapping for .NET string is SQL nvarchar (16 bit Unicode). This command changes the SQL column type to varchar (8 bit ASCII)
    * ******************************************************/
}

/******************************************************************************
* NOTES ON MIGRATION:
* 
* see https://docs.microsoft.com/en-us/ef/core/get-started/aspnetcore/new-db
* 
* Add to EfCoreInAction the following NuGet libraries
* 1. "Microsoft.EntityFrameworkCore.Tools"  AND MOVE TO tools part of project
* 2. "Microsoft.EntityFrameworkCore.Design"
* 
* 1. Using Package Manager Console commands
* The steps are:
* a) Add a second param to the AddDbContext command in startup which is
*    b => b.MigrationsAssembly("DataLayer")
* b) Use the PMC command
*    Add-Migration Chapter02 -Project DataLayer -StartupProject EfCoreInAction
* c) Use PMC command
*    Update-database -Project DataLayer -StartupProject EFCoreExample
*    
* If you want to start afreash then:
* a) Delete the current database
* b) Delete all the class in the Migration directory
* c) follow the steps to add a migration
******************************************************************************/
