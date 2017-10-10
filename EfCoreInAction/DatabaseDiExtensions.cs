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
        public static IServiceCollection //#A
            RegisterDbContextWithRavenDb //#A
            (this IServiceCollection serviceCollection,
                Action<DbContextOptionsBuilder> optionsAction,//#B
                string ravenDbConnectionString) //#C
        {
            AddCoreServices<EfCoreContext>(serviceCollection,//#D
                (p, b) => optionsAction.Invoke(b),           //#D
                ServiceLifetime.Scoped);                     //#D

            serviceCollection.AddSingleton<IRavenStore>(      //#E
                p => new RavenStore(ravenDbConnectionString));//#E
            serviceCollection.AddTransient(provider => //#F
            {
                var options = provider.                           //#G
                    GetService<DbContextOptions<EfCoreContext>>();//#G
                var storeSource = provider.                       //#H
                    GetService<IRavenStore>();                    //#H
                var logger = provider.                            //#H
                    GetService<ILogger<RavenUpdater>>();          //#H

                return new EfCoreContext(options,          //#I
                    storeSource.CreateSqlUpdater(logger)); //#I
            });

            return serviceCollection; //#J
        }
        /***************************************************************
        #A This is my static extention method. Its signiture is the same as EF Core's AddDbContext method, but with an extra parameter
        #B This is the normal options action that you provide to EF Core's AddDbContext method
        #C This is my extra parameter, which is the connection string to my RavenDb NoSQL database
        #D This method, and its other associated method DbContextOptionsFactory set up the other services EF Core needs. I copied the code from EF Core's GitHub. You will find them at https://github.com/aspnet/EntityFrameworkCore/blob/dev/src/EFCore/EntityFrameworkServiceCollectionExtensions.cs
        #E I register an interface, IRavenStore, as a singleton. RavenDb requires a singleton to hold the data to acces the Raven store. Its a bit like your application's DbContext, but for RavenDb
        #F I now want to register my application's DbContext as a transient, that is, once instance for each HTTP request. To register my application's DbContext I need some services that have already been registered, so I use a factory pattern
        #G To create my application's DbContext I need the options, which were registered by the AddCoreServices called earlier
        #H I want to create the Raven NoSqlUpdater, but to do that I need the RavenStore and a logger
        #I Now I can create my application's DbContext, using my IRavenStore to create the correct INoSqlUpdater for Raven
        #J The standard signature for a service registering extension method is to return the serviceCollection to allow for fluent chaining
         * ****************************************************************/

        private static void AddCoreServices<TContext>(        //#J
            IServiceCollection serviceCollection,             //#J
            Action<IServiceProvider, DbContextOptionsBuilder> //#J
                optionsAction,                                //#J
            ServiceLifetime optionsLifetime)                  //#J
            where TContext : DbContext                        //#J
        {
            serviceCollection
                .AddMemoryCache()
                .AddLogging();

            serviceCollection.TryAdd(
                new ServiceDescriptor(
                    typeof(DbContextOptions<TContext>),
                    p => DbContextOptionsFactory<TContext>
                        (p, optionsAction),
                    optionsLifetime));

            serviceCollection.Add(
                new ServiceDescriptor(
                    typeof(DbContextOptions),
                    p => p.GetRequiredService
                        <DbContextOptions<TContext>>(),
                    optionsLifetime));
        }

        private static DbContextOptions<TContext>            //#J
            DbContextOptionsFactory<TContext>(               //#J
            IServiceProvider applicationServiceProvider,     //#J
            Action<IServiceProvider, DbContextOptionsBuilder>//#J 
                optionsAction)                               //#J
            where TContext : DbContext                       //#J
        {
            var builder = new DbContextOptionsBuilder<TContext>(
                new DbContextOptions<TContext>(new 
                Dictionary<Type, IDbContextOptionsExtension>()));

            builder.UseApplicationServiceProvider(
                applicationServiceProvider);

            optionsAction?.Invoke(
                applicationServiceProvider, builder);

            return builder.Options;
        }

    }
}