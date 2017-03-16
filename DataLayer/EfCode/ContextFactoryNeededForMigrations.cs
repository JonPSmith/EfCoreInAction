// Copyright (c) 2017 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace DataLayer.EfCode
{

    /// <summary>
    /// This class is needed to allow Add-Migrations command to be run. 
    /// It is not a good implmentation as it has to have a constant connection sting in it
    /// but it is Ok on a local machine, which is where you want to run the command
    /// see https://docs.microsoft.com/en-us/ef/core/miscellaneous/configuring-dbcontext#using-idbcontextfactorytcontext
    /// </summary>
    public class ContextFactoryNeededForMigrations : IDbContextFactory<EfCoreContext>
    {
        private const string ConnectionString =
            "Server=(localdb)\\mssqllocaldb;Database=EfCoreInActionDb;Trusted_Connection=True;MultipleActiveResultSets=true";

        public EfCoreContext Create(DbContextFactoryOptions options)
        {
            var optionsBuilder = new DbContextOptionsBuilder<EfCoreContext>();
            optionsBuilder.UseSqlServer(ConnectionString,
                b => b.MigrationsAssembly("DataLayer"));

            return new EfCoreContext(optionsBuilder.Options);
        }
    }
}