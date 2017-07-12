using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Test.Chapter11Listings.EfCode;

namespace Test.Chapter11Listings.Migrations
{
    [DbContext(typeof(Chapter11MigrateDb))]
    [Migration("20170712144551_SplitTable")]
    partial class SplitTable
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.1.1")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Test.Chapter11Listings.EfClasses.Addresses", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Address");

                    b.Property<int>("CustFK");

                    b.HasKey("Id");

                    b.HasIndex("CustFK")
                        .IsUnique();

                    b.ToTable("Addresses");
                });

            modelBuilder.Entity("Test.Chapter11Listings.EfClasses.Customer", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name");

                    b.HasKey("Id");

                    b.ToTable("Customers");
                });

            modelBuilder.Entity("Test.Chapter11Listings.EfClasses.Addresses", b =>
                {
                    b.HasOne("Test.Chapter11Listings.EfClasses.Customer")
                        .WithOne("AddressData")
                        .HasForeignKey("Test.Chapter11Listings.EfClasses.Addresses", "CustFK")
                        .OnDelete(DeleteBehavior.Cascade);
                });
        }
    }
}
