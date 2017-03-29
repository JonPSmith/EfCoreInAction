// Copyright (c) 2017 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using Microsoft.EntityFrameworkCore;

namespace Test.Chapter06Listings
{
    public class ExcludeDbContext : DbContext
    {
        public DbSet<MyEntityClass> MyEntities { get; set; }

        protected override void OnModelCreating
            (ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MyEntityClass>()
                .Property(p => p.InDatabaseProp)
                .ForSqlServerHasColumnName("SqlServerInDatabaseProp")
                .ForSqliteHasColumnName("SqliteInDatabaseProp");

            modelBuilder.Entity<MyEntityClass>()
                .Ignore(b => b.LocalString); //#A

            modelBuilder.Ignore<ExcludeClass>(); //#B
        }
    }
    /**********************************************
    #A The Ignore method is used to exclude the property LocalString in the entity class, MyEntityClass, from being added to the database
    #B A different Ignore method can exclude a class such that if you have a property in an entity class of the Ignored type then that property is not added to the database
    * *********************************************/
}