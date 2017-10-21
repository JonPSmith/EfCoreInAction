// Copyright (c) 2017 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Design.Internal;
using Microsoft.EntityFrameworkCore.Scaffolding.Internal;
using Microsoft.Extensions.DependencyInjection;

namespace Test.Chapter14Listings.EfServices
{
    public enum DatabaseProviders { SqlServer, MySql } //#A

    //Thanks to Erik Ejlskov Jensen for help on how to do this - GitHub https://github.com/ErikEJ
    public class DesignTimeBuilder 
    {
        public const string SqlServerProviderName =    //leave out of book 
            "Microsoft.EntityFrameworkCore.SqlServer"; //leave out of book 
        public const string MySqlProviderName =        //leave out of book 
            "Pomelo.EntityFrameworkCore.MySql";        //leave out of book 

        private readonly List<string> _errors   //#B
            = new List<string>();               //#B
        private readonly List<string> _warnings //#B
            = new List<string>();               //#B

        public ImmutableList<string> Errors =>   //#C
            _errors.ToImmutableList();           //#C
        public ImmutableList<string> Warnings => //#C
            _warnings.ToImmutableList();         //#C

        public ServiceProvider GetScaffolderService(DbContext context, bool addPrualizer = true) //leave out of book
        {
            var providerName = context.Database.ProviderName;

            switch (providerName)
            {
                case SqlServerProviderName:
                    return GetScaffolderService(DatabaseProviders.SqlServer, addPrualizer);
                case MySqlProviderName:
                    return GetScaffolderService(DatabaseProviders.MySql, addPrualizer);
            }

            throw new InvalidOperationException
                ($"We currently do not support {providerName}");
        }

        public ServiceProvider GetScaffolderService //#D
            (DatabaseProviders databaseProvider,     //#D
             bool addPrualizer = true)               //#D
        {
            var reporter = new OperationReporter(      //#E
                new OperationReportHandler(            //#E
                    m => _errors.Add(m),               //#E
                    m => _warnings.Add(m)));           //#E
                                                       //#E
            // Add base services for scaffolding       //#E
            var serviceCollection =                    //#E
                new ServiceCollection()                //#E
                .AddScaffolding(reporter)              //#E
                .AddSingleton<IOperationReporter,      //#E
                    OperationReporter>()               //#E
                .AddSingleton<IOperationReportHandler, //#E
                    OperationReportHandler>();         //#E

            if (addPrualizer)                          //#F
                serviceCollection.AddSingleton         //#F
                    <IPluralizer, ScaffoldPuralizer>();//#F

            switch (databaseProvider) //#G
            {
                case DatabaseProviders.SqlServer:
                {
                    var designProvider =                  //#H
                        new SqlServerDesignTimeServices();//#H
                    designProvider.                  //#I
                        ConfigureDesignTimeServices( //#I
                            serviceCollection);      //#I
                    return serviceCollection         //#I
                            .BuildServiceProvider(); //#I
                }
                case DatabaseProviders.MySql:
                {
                    var designProvider =               //#J
                        new MySqlDesignTimeServices(); //#J
                    designProvider.                  //#I
                        ConfigureDesignTimeServices( //#I
                            serviceCollection);      //#I
                    return serviceCollection         //#I
                        .BuildServiceProvider();     //#I
                    }
                default:
                    throw new ArgumentOutOfRangeException(
                        nameof(databaseProvider), 
                        databaseProvider, null);
            }
        }
    }
    /**************************************************************
    #A I use a enum to select which database provider's design services I want to use. I also have a method (not shown) that will select the correct enum based on the current DbContext
    #B Just like the command line versions the design time commands can return errors or warnings. They are placed in this lists
    #C I provide the Errors and Warnings as immutable lists
    #D This method will return the design services for the chosen type of database provider. The addPrualizer parameter adds/leave out a pluralizer used to make classes singluar and tables plural
    #E All this code is required to create the scaffolder design time service
    #F I optionally add a pluralizer - see https://docs.microsoft.com/en-us/ef/core/what-is-new/#database-management
    #G In this case I only support two types of database providers - SQL Server and MySQL
    #H This creates the SQL Server design time service for the loaded SQL Server database provider Nuget package
    #I I add the services the scaffolder needs and and return the built service
    #J This creates the MySql design time service for the loaded MySql database provider Nuget package
     * ************************************************************/
}