using System;
using System.Collections.Generic;
using System.IO;
using DataLayer.EfCode;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using ServiceLayer.BookServices.RavenDb;
using ServiceLayer.DatabaseServices.Concrete;

namespace EfCoreInAction
{
    public static class DatabaseStartupHelpers
    {
  private const string WwwRootDirectory = "wwwroot\\";

        public static string GetWwwRootPath()
        {
            return Path.Combine(Directory.GetCurrentDirectory(), WwwRootDirectory);
        }

        public static IServiceCollection RegisterDbContextWithRavenDb<TContext>(this IServiceCollection serviceCollection,
            Action<DbContextOptionsBuilder> optionsAction,
            string ravenDbConnectionString) where TContext : DbContext
        {
            AddCoreServices<EfCoreContext>(serviceCollection, (p, b) => optionsAction.Invoke(b), ServiceLifetime.Scoped);

            serviceCollection.AddSingleton(p => new RavenStore(ravenDbConnectionString));
            serviceCollection.AddTransient(provider =>
            {
                //resolve another classes from DI
                var options = provider.GetService<DbContextOptions<EfCoreContext>>();
                var storeSource = provider.GetService<RavenStore>();
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

        //see https://github.com/aspnet/EntityFrameworkCore/issues/9033#issuecomment-317104564
        public static IWebHost MigrateDatabase(this IWebHost webHost)
        {
            using (var scope = webHost.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                using (var context = services.GetRequiredService<EfCoreContext>())
                {
                    try
                    {
                        context.ProductionMigrateDatabase(GetWwwRootPath());
                    }
                    catch (Exception ex)
                    {
                        var logger = services.GetRequiredService<ILogger<Program>>();
                        logger.LogError(ex, "An error occurred while migrating the database.");
                    }
                    try
                    {
                        context.SeedDatabase(GetWwwRootPath());
                    }
                    catch (Exception ex)
                    {
                        var logger = services.GetRequiredService<ILogger<Program>>();
                        logger.LogError(ex, "An error occurred while seeding the database.");
                    }
                }
            }

            return webHost;
        }

        public static IWebHost SetupDevelopmentDatabase(this IWebHost webHost)
        {
            using (var scope = webHost.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                using (var context = services.GetRequiredService<EfCoreContext>())
                {
                    try
                    {
                        context.DevelopmentEnsureCreated(GetWwwRootPath());
                        context.SeedDatabase(GetWwwRootPath());
                    }
                    catch (Exception ex)
                    {
                        var logger = services.GetRequiredService<ILogger<Program>>();
                        logger.LogError(ex, "An error occurred while setting upor seeding the development database.");
                    }
                }
            }

            return webHost;
        }
    }
}