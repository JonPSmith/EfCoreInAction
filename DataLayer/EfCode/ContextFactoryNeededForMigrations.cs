// Copyright (c) 2017 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace DataLayer.EfCode
{

    /// <summary>
    /// This class is needed to allow Add-Migrations command to be run. 
    /// It is not a good implmentation as it has to have a constant connection sting in it
    /// but it is Ok on a local machine, which is where you want to run the command
    /// see https://docs.microsoft.com/en-us/ef/core/miscellaneous/configuring-dbcontext#using-idesigntimedbcontextfactorytcontext
    /// </summary>
    public class ContextFactoryNeededForMigrations   //#A
        : IDesignTimeDbContextFactory<EfCoreContext> //#A
    {
        private const string ConnectionString = //#B
            "Server=(localdb)\\mssqllocaldb;Database=EfCoreInActionDb;Trusted_Connection=True;MultipleActiveResultSets=true";

        public EfCoreContext CreateDbContext(string[] args) //#C
        {
            var optionsBuilder = new                      //#D
                DbContextOptionsBuilder<EfCoreContext>(); //#D
            optionsBuilder.UseSqlServer(ConnectionString, //#E
                b => b.MigrationsAssembly("DataLayer"));  //#E

            return new EfCoreContext(optionsBuilder.Options); //#F
        }
    }
    /*****************************************************************
    #A This is my class that implements the IDesignTimeDbContextFactory<T> interface, where the <T> is the application's DbContext. The EF Core migration tools need this class to obtain a fully configured instance of the application's DbContext
    #B I provide a connection string to a database. Some of the migration commands, such as Update-Database, will access this database
    #C This is the method that I must implement. The database migrations tools call this to get an instance of the applications's DbContext
    #D I create the DbContextOptionsBuilder<T> builder that I need for configuring the database options
    #E Here I select the SQL Server database provider with the connection string to the database. I also add any options I need, in this case I tell EF Core where the database migrations are
    #F Finally I use these options to create an instance on the application's DbContext that the migration tools can use
     * ***************************************************************/
}