// Copyright (c) 2017 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using DataLayer.EfClasses;
using Microsoft.EntityFrameworkCore;

namespace Test.Chapter06Listings
{
    public class Chapter06DbContext : DbContext
    {
        public DbSet<MyEntityClass> MyEntities { get; set; }

        public Chapter06DbContext(
            DbContextOptions<Chapter06DbContext> options)
            : base(options)
        { }

        protected override void OnModelCreating
            (ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MyEntityClass>()
                .ToTable("MyTable");

            modelBuilder.Entity<MyEntityClass>()
                .Property(p => p.InDatabaseProp)
                .HasColumnName("GenericInDatabaseProp") //#A
                .ForSqlServerHasColumnName("SqlServerInDatabaseProp") //#B
                .ForSqliteHasColumnName("SqliteInDatabaseProp"); //#C
        /********************************************************************
        #A This would be the column name if a For... command didn't override it
        #B This defines the column name for the sql server database provider
        #C This defines the column name for the sqlite database provider
        * *******************************************************************/

                modelBuilder.Entity<MyEntityClass>()
                    .Property<DateTime>("UpdatedOn"); //#A
        /*************************************************************************
        #A I use the Property<T> method to define the shadow property type
         * ********************************************************************/

            modelBuilder.Entity<MyEntityClass>()
                .Ignore(b => b.LocalString); //#A

            modelBuilder.Ignore<ExcludeClass>(); //#B
        }
    }
    /**Exclude section********************************************
    #A The Ignore method is used to exclude the property LocalString in the entity class, MyEntityClass, from being added to the database
    #B A different Ignore method can exclude a class such that if you have a property in an entity class of the Ignored type then that property is not added to the database
    * *********************************************/
}