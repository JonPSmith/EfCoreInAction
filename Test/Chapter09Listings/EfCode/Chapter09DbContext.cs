using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Test.Chapter09Listings.EfClasses;
using Test.Chapter09Listings.EfCode.Configuration;

namespace Test.Chapter09Listings.EfCode
{
    public class Chapter09DbContext : DbContext
    {
        private readonly Func<string> _getUserName; //#A

        public DbSet<MyEntity> MyEntities { get; set; }
        public DbSet<OneEntity> OneEntities { get; set; }
        public DbSet<NotifyEntity> Notify { get; set; }
        public DbSet<Notify2Entity> Notify2 { get; set; }
        public DbSet<GuidKeyEntity> GuidKeyEntities { get; set; }
        public DbSet<LoggedEntity> LoggedEntities { get; set; }
        public DbSet<T2P2> WipeCheck { get; set; }

        public Chapter09DbContext(
            DbContextOptions<Chapter09DbContext> options, 
            Func<string> getUserName = null) //#B
            : base(options)
        {
            _getUserName = getUserName; //#C
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MyEntity>().Configure();
            modelBuilder.Entity<NotifyEntity>().Configure();
            modelBuilder.Entity<Notify2Entity>().Configure();
        }

        public override int SaveChanges() //#D
        {
            HandleWhenWho(); //#E
            return base.SaveChanges();
        }

        private void HandleWhenWho() //#F
        {
            foreach (var entity in ChangeTracker.Entries() //#G
                .Where(e =>
                        e.State == EntityState.Added ||  //#H
                        e.State == EntityState.Modified))//#H
            {
                var tracked = entity.Entity as IWhenWho; //#I
                tracked?.SetWhenWhere( //#J
                    _getUserName, //#K
                    entity.State == EntityState.Added); //#L
            }
        }
    }
    /*******************************************************
    #A This hold a method that can get the current username
    #B I provide a method to get the current username via the constructor. How you do this will depend on your application. In ASP.NET Core you would use dependency injection with something derived from IHttpContextAccessor.HttpContext.User.Identity.Name
    #C I store this method so it can be called later by my method
    #D I override SaveChanges so that I can add my method before calling the base.SaveChanges method. Note: In this example I only override one of the four versions of SaveChanges
    #E I call my method, HandleWhenWho, before I call the base.SaveChanges to do the save
    #F This is my method for finding and handling entity classes that inherited the IWhenWho interface
    #G I look at each entity that the ChangeTracker says has changed in some way
    #H I am only interested in entities that have been added or Modified
    #I I cast this entity to the IWhenWho interface. The result will be null if it does not inherit from the IWhenWho interface
    #J I now call the method that the IWhenWho interface provides for setting the four properties
    #K I pass in the method for getting the the current username, which was provided via the constructor
    #L This tells the method whether it was an Add or Update
        * * *****************************************************/
}
