using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Test.Chapter09Listings.EfClasses;

namespace Test.Chapter09Listings.EfCode
{
    public class Chapter09DbContext : DbContext
    {
        public DbSet<TrackedEntity> Tracked { get; set; }
        public DbSet<NotifyEntity> Notify { get; set; }

        public Chapter09DbContext(
            DbContextOptions<Chapter09DbContext> options)
            : base(options)
        { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<NotifyEntity>()
                .HasChangeTrackingStrategy(ChangeTrackingStrategy.ChangedNotifications);

            //modelBuilder.Entity<NotifyEntity>()
            //    .Metadata
            //    .FindNavigation(nameof(NotifyEntity.Collection))
            //    .SetPropertyAccessMode(PropertyAccessMode.Field);
        }
    }
}
