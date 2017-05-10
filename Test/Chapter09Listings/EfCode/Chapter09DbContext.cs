using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Test.Chapter09Listings.EfClasses;
using Test.Chapter09Listings.EfCode.Configuration;

namespace Test.Chapter09Listings.EfCode
{
    public class Chapter09DbContext : DbContext
    {
        public DbSet<MyEntity> MyEntities { get; set; }
        public DbSet<OneEntity> OneEntities { get; set; }
        public DbSet<NotifyEntity> Notify { get; set; }
        public DbSet<GuidKeyEntity> GuidKeyEntities { get; set; }

        public Chapter09DbContext(
            DbContextOptions<Chapter09DbContext> options)
            : base(options)
        { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<NotifyEntity>().Configure();
            modelBuilder.Entity<MyEntity>().Configure();
        }
    }
}
