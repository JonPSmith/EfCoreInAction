// Copyright (c) 2016 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DataLayer.EfClasses;
using DataLayer.EfCode.Configurations;
using Microsoft.EntityFrameworkCore;
using DataLayer.EfCode.Configurations;
using DataLayer.NoSql;
using DataLayer.SqlCode;
using DataNoSql;
using Microsoft.Extensions.Logging;

namespace DataLayer.EfCode
{
    public class EfCoreContext : DbContext
    {
        private readonly INoSqlUpdater _updater;

        public DbSet<Book> Books { get; set; }              //#A
        public DbSet<Author> Authors { get; set; }          //#A
        public DbSet<PriceOffer> PriceOffers { get; set; }  //#A
        public DbSet<Order> Orders { get; set; }            //#A

        public EfCoreContext(                             
            DbContextOptions<EfCoreContext> options, IUpdateCreator updateCreator = null)      
            : base(options)
        {
            _updater = updateCreator?.CreateSqlUpdater();
        }

        public override int SaveChanges() //#A
        {
            //I need to remember the changes, but not process them yet, as new entries BookId's are not set until after SaveChanges
            var detectedChanges = BookChangeInfo      //#B
                .FindBookChanges(ChangeTracker.Entries());//#B

            var result = base.SaveChanges(); //#C

            var booksChanged = BookChange          //#D
                .FindChangedBooks(detectedChanges); //#D
            var updater = new ApplyChangeToNoSql //#E
                (this, _updater);                //#E
            updater.UpdateNoSql(booksChanged);   //#F
            return result;
        }
        /********************************************************
        #A I must override all the SaveChanges methods (sync and async) to make sure I capture all updates to the database
        #B This stage is all about detecting the State of all the tracked entities before they get cleared by the call to the base SaveChanges
        #C Now I can call the base SaveChanges, as I have all the state information
        #D There may have been multiple changes to a single Book. This stage combines them so that I only send one update to the NoSQL database
        #E My Data Layer oversees the projection of the SQL database into the form that the NoSQL database needs it in. The NoSQL provides an updater method, via the constuctor, which will do the update
        #F Now I apply apply any updates to the NoSQL database
         * ******************************************************/

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            //I need to remember the changes, but not process them yet, as new entries BookId's are not set until after SaveChanges

            var detectedChanges = BookChangeInfo.FindBookChanges(ChangeTracker.Entries());
            int result;

            result = await base.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

            var booksChanged = BookChange.FindChangedBooks(detectedChanges);
            var updater = new ApplyChangeToNoSql(this, _updater);
            updater.UpdateNoSql(booksChanged);
            return result;
        }

        protected override void
            OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new BookConfig());       //#B
            modelBuilder.ApplyConfiguration(new BookAuthorConfig()); //#B
            modelBuilder.ApplyConfiguration(new PriceOfferConfig()); //#B
            modelBuilder.ApplyConfiguration(new LineItemConfig());   //#B

            modelBuilder.RegisterUdfDefintions();
        }
        /*****************************************************************
        #A We only define three of the five tables in the database: Books, Authors and PriceOffers. The other two tables, Review and BookAuthor are found via navigational links from the other tables
        #B I have moved the Fluent API configuration of various entity classes to separate configration classes that implement the IEntityTypeConfiguration<T> interface
         * ****************************************************************/
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
