// Copyright (c) 2017 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Design.Internal;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Scaffolding.Internal;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;

namespace Test.Chapter14Listings.EfServices
{
    public enum DatabaseProviders { SqlServer, OthersNotAdded }

    //Thanks to Erik Ejlskov Jensen for help on how to do this - GitHub https://github.com/ErikEJ
    public class DesignTimeProvider
    {
        private const string SqlServerProviderName = "Microsoft.EntityFrameworkCore.SqlServer";

        private readonly List<string> _errors = new List<string>();
        private readonly List<string> _warnings = new List<string>();

        public ImmutableList<string> Errors => _errors.ToImmutableList();
        public ImmutableList<string> Warnings => _warnings.ToImmutableList();

        public ServiceProvider GetDesignTimeProvider(DbContext context)
        {
            return GetDesignTimeProvider(DecodeDatabaseProvider(context));
        }

        public ServiceProvider GetDesignTimeProvider(DatabaseProviders databaseProvider)
        {
            var reporter = new OperationReporter(
                new OperationReportHandler(
                    m => _errors.Add(m),
                    m => _warnings.Add(m)));

            // Add base services for scaffolding
            var serviceCollection = new ServiceCollection()
                .AddScaffolding(reporter)
                .AddSingleton<IOperationReporter, OperationReporter>()
                .AddSingleton<IOperationReportHandler, OperationReportHandler>();

            switch (databaseProvider)
            {
                case DatabaseProviders.SqlServer:
                {
                    var designProvider = new SqlServerDesignTimeServices();
                    designProvider.ConfigureDesignTimeServices(serviceCollection);
                    return serviceCollection.BuildServiceProvider();
                }
                default:
                    throw new ArgumentOutOfRangeException(nameof(databaseProvider), databaseProvider, null);
            }
        }

        //----------------------------------------------
        //private methods

        private static DatabaseProviders DecodeDatabaseProvider(DbContext context)
        {
            var dbProvider = context.GetService<IDatabaseProvider>();
            if (dbProvider == null)
                throw new InvalidOperationException("Cound not find a database provider service.");

            var providerName = dbProvider.Name;

            if (providerName == SqlServerProviderName)
                return DatabaseProviders.SqlServer;

            throw new InvalidOperationException("This is not a database provider that we currently support.");
        }
    }
}