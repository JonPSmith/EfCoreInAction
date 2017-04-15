// Copyright (c) 2017 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using Microsoft.EntityFrameworkCore;
using Test.Chapter07Listings.EfClasses;
using Test.Chapter07Listings.EFCode.Configurations;

namespace Test.Chapter07Listings.EFCode
{
    public class Chapter07DbContext : DbContext
    {
        public DbSet<Employee> Employees { get; set; }
        public DbSet<EmployeeShortFk> EmployeeShortFks { get; set; }
        public DbSet<Person> People { get; set; }
        public DbSet<LibraryBook> LibraryBooks { get; set; }

        //One-to-One versions
        public DbSet<Attendee> Attendees { get; set; }
        public DbSet<Ticket> Tickets { get; set; }

        //Table-per-hierarchy
        public DbSet<Payment> Payments { get; set; } //#A

        public DbSet<SoldIt> SoldThings { get; set; } //#B

        public Chapter07DbContext(
            DbContextOptions<Chapter07DbContext> options)
            : base(options)
        { }

        protected override void OnModelCreating
            (ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Attendee>().Configure();
            modelBuilder.Entity<Person>().Configure();
            modelBuilder.Entity<Payment>().Configure(); //#C
        }
    }
    /**TPH**************************************************
    #A This defines the property through which I can access all the payments, both PaymentCash and PaymentCard
    #B This is the list of sold items, with a required link to a Payment
    #C I call the configureration code for the payment TPH via its extension method, Configure
     * ******************************************************/
}