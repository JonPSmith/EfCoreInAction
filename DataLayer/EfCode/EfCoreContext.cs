// Copyright (c) 2016 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DataLayer.EfClasses;
using DataLayer.EfCode.Configurations;
using Microsoft.EntityFrameworkCore;
using DataLayer.EfCode.Configurations;
using DataLayer.NoSql;
using DataLayer.SqlCode;
using Microsoft.Extensions.Primitives;

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

        public override int SaveChanges()
        {
            //I need to remember the changes, but not process them yet, as new entries BookId's are not set until after SaveChanges
            var copyOfChanged = ChangeTracker.Entries(); 
            var result = base.SaveChanges();
            var updater = new NoSqlUpdater(this);
            var bookChanged = BookChanges.FindChangedBooks(copyOfChanged);
            updater.UpdateNoSql(bookChanged);
            return result;
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            //I need to remember the changes, but not process them yet, as new entries BookId's are not set until after SaveChanges
            var copyOfChanged = ChangeTracker.Entries();
            var result = await base.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            var updater = new NoSqlUpdater(this);
            var bookChanged = BookChanges.FindChangedBooks(copyOfChanged);
            updater.UpdateNoSql(bookChanged);
            return result;
        }



        protected override void
            OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new BookConfig());      
            modelBuilder.ApplyConfiguration(new BookAuthorConfig());
            modelBuilder.ApplyConfiguration(new PriceOfferConfig());
            modelBuilder.ApplyConfiguration(new LineItemConfig());  

            modelBuilder.RegisterUdfDefintions();
        }
    }
}

/******************************************************************************
* NOTES ON MIGRATION:
* 
* see https://docs.microsoft.com/en-us/ef/core/get-started/aspnetcore/new-db
* 
* Add to EfCoreInAction the following NuGet libraries
* 1. "Microsoft.EntityFrameworkCore.Tools"  AND MOVE TO tools part of project
*    Note: You can move the Microsoft.EntityFrameworkCore.Tools pckage to the tools part of project. 
* 
* 2. Using Package Manager Console commands
* The steps are:
* a) Add a second param to the AddDbContext command in startup which is
*    b => b.MigrationsAssembly("DataLayer")
* b) Use the PMC command
*    Add-Migration Chapter02 -Project DataLayer -StartupProject EfCoreInAction
* c) Use PMC command
*    Update-database -Project DataLayer -StartupProject EfCoreInAction
*    
* If you want to start afreash then:
* a) Delete the current database
* b) Delete all the class in the Migration directory
* c) follow the steps to add a migration
******************************************************************************/
