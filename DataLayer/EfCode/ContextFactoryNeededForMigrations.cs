// Copyright (c) 2017 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
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
    public class ContextFactoryNeededForMigrations : IDesignTimeDbContextFactory<EfCoreContext>
    {
        private const string ConnectionString =
            "Server=localhost;Database=EfCoreInActionDevelopment;Uid=mysqladmin;Pwd=mysqladmin;";

        public EfCoreContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<EfCoreContext>();
            optionsBuilder.UseMySql(ConnectionString,
                b => b.MigrationsAssembly("DataLayer")
                    .MaxBatchSize(1) //Needed to overcome https://github.com/PomeloFoundation/Pomelo.EntityFrameworkCore.MySql/issues/397
            );

            return new EfCoreContext(optionsBuilder.Options);
        }
    }
}