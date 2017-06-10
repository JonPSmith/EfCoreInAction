using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Test.Chapter09Listings.EfClasses;

namespace Test.Chapter09Listings.EfCode
{
    public class WipeDbContext : DbContext
    {
        public DbSet<T1P1> T1P1 { get; set; }
        public DbSet<T2P1> T2P1 { get; set; }

        public WipeDbContext(DbContextOptions<WipeDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<T1P1>()
                .HasOne(p => p.T1P2)
                .WithOne()
                .HasForeignKey<T1P1>(p => p.FKey)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<T1P2>()
                .HasOne(p => p.T1P3)
                .WithOne()
                .HasForeignKey<T1P2>(p => p.FKey)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<T1P3>()
                .HasOne(p => p.T1P4)
                .WithOne()
                .HasForeignKey<T1P3>(p => p.FKey)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<T2P1>()
                .HasOne(p => p.T2P2)
                .WithOne()
                .HasForeignKey<T2P2>(p => p.FKey)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<T2P2>()
                .HasOne(p => p.T2P3)
                .WithOne()
                .HasForeignKey<T2P3>(p => p.FKey)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<T2P3>()
                .HasOne(p => p.T2P4)
                .WithOne()
                .HasForeignKey<T2P4>(p => p.FKey)
                .OnDelete(DeleteBehavior.Restrict);
        }
        
    }

}
