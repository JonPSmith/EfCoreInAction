// Copyright (c) 2017 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using DataLayer.EfCode;
using DataNoSql;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;

namespace EfCoreInAction
{
    public static class DatabaseDiExtensions
    {
        public static IServiceCollection RegisterDbContextWithRavenDb<TContext>(this IServiceCollection serviceCollection,
            Action<DbContextOptionsBuilder> optionsAction,
            string ravenDbConnectionString) where TContext : DbContext
        {
            AddCoreServices<EfCoreContext>(serviceCollection, (p, b) => optionsAction.Invoke(b), ServiceLifetime.Scoped);

            serviceCollection.AddSingleton<IRavenStore>(p => new RavenStore(ravenDbConnectionString));
            serviceCollection.AddTransient(provider =>
            {
                //resolve another classes from DI
                var options = provider.GetService<DbContextOptions<EfCoreContext>>();
                var storeSource = provider.GetService<IRavenStore>();
                var logger = provider.GetService<ILogger<RavenUpdater>>();

                //pass any parameters
                return new EfCoreContext(options, storeSource.CreateSqlUpdater(logger));
            });

            return serviceCollection;
        }

        private static void AddCoreServices<TContext>(
            IServiceCollection serviceCollection,
            Action<IServiceProvider, DbContextOptionsBuilder> optionsAction,
            ServiceLifetime optionsLifetime)
            where TContext : DbContext
        {
            serviceCollection
                .AddMemoryCache()
                .AddLogging();

            serviceCollection.TryAdd(
                new ServiceDescriptor(
                    typeof(DbContextOptions<TContext>),
                    p => DbContextOptionsFactory<TContext>(p, optionsAction),
                    optionsLifetime));

            serviceCollection.Add(
                new ServiceDescriptor(
                    typeof(DbContextOptions),
                    p => p.GetRequiredService<DbContextOptions<TContext>>(),
                    optionsLifetime));
        }

        private static DbContextOptions<TContext> DbContextOptionsFactory<TContext>(
            IServiceProvider applicationServiceProvider,
            Action<IServiceProvider, DbContextOptionsBuilder> optionsAction)
            where TContext : DbContext
        {
            var builder = new DbContextOptionsBuilder<TContext>(
                new DbContextOptions<TContext>(new Dictionary<Type, IDbContextOptionsExtension>()));

            builder.UseApplicationServiceProvider(applicationServiceProvider);

            optionsAction?.Invoke(applicationServiceProvider, builder);

            return builder.Options;
        }
    }
}