using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Test.Chapter09Listings.EfClasses;
using Test.Chapter09Listings.EfCode.Configuration;
using Test.Chapter09Listings.WipeDbClasses;

namespace Test.Chapter09Listings.EfCode
{
    public class Chapter09DbContext : DbContext
    {
        public DbSet<MyEntity> MyEntities { get; set; }
        public DbSet<OneEntity> OneEntities { get; set; }
        public DbSet<ManyEntity> ManyEntities { get; set; }
        public DbSet<NotifyEntity> Notify { get; set; }
        public DbSet<Notify2Entity> Notify2 { get; set; }
        public DbSet<GuidKeyEntity> GuidKeyEntities { get; set; }
        public DbSet<AutoWhenEntity> LoggedEntities { get; set; }

        public Chapter09DbContext(
            DbContextOptions<Chapter09DbContext> options)
            : base(options)
        { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MyEntity>().Configure();
            modelBuilder.Entity<NotifyEntity>().Configure();
            modelBuilder.Entity<Notify2Entity>().Configure();
        }

        public override int SaveChanges() //#A
        {
            HandleWhen(); //#B
            return base.SaveChanges();
        }

        private void HandleWhen() //#C
        {
            foreach (var entity in ChangeTracker.Entries() //#D
                .Where(e =>
                        e.State == EntityState.Added ||  //#E
                        e.State == EntityState.Modified))//#E
            {
                var tracked = entity.Entity as IWhen; //#F
                tracked?.SetWhen( //#G
                    entity.State == EntityState.Added); //#H
            }
        }
    }
    /*******************************************************
    #A I override SaveChanges so that I can add my method before calling the base.SaveChanges method. Note: In this example I only override one of the four versions of SaveChanges
    #B I call my method, HandleWhen, before I call the base.SaveChanges to do the save
    #C This is my method for finding and handling entity classes that inherited the IWhen interface
    #D I look at each entity that the ChangeTracker says has changed in some way
    #E I am only interested in entities that have been added or Modified
    #F I cast this entity to the IWhen interface. The result will be null if it does not inherit from the IWhen interface
    #G I now call the method that the IWhen interface provides for setting the IWhen properties
    #H This tells the method whether it was an Add or Update
        * * *****************************************************/
}
