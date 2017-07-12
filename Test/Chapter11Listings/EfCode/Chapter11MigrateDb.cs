// Copyright (c) 2017 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using Microsoft.EntityFrameworkCore;
using Test.Chapter11Listings.EfClasses;

namespace Test.Chapter11Listings.EfCode
{
    public class Chapter11MigrateDb : DbContext
    {
        private const string ConnectionString = 
            @"Server=(localdb)\mssqllocaldb;Database=Chapter11MigrateDb;Trusted_Connection=True";

        public DbSet<CustomerAndAddresses> CustomerAndAddresses { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(ConnectionString);
        }
    }

    /***************************************************************************
    * How to create a migration in Visual Studio PMC
    * 
    * a) Use the PMC command
    *    Add-Migration Initial -Project Test -StartupProject Test -Context Chapter11MigrateDb -OutputDir Chapter11Listings\Migrations
    * b) Use PMC command
    *    Update-database -Project Test -StartupProject Test -Context Chapter11MigrateDb
    ***************************************************************************/
}