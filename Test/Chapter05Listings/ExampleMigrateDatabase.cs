using System;
using DataLayer.EfCode;
using EfCoreInAction;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ServiceLayer.DatabaseServices.Concrete;

namespace Test.Chapter05Listings
{
    public static class ExampleMigrateDatabase
    {
        //see https://github.com/aspnet/EntityFrameworkCore/issues/9033#issuecomment-317104564
        public static IWebHost MigrateDatabase
            (this IWebHost webHost) //#A
        {
            using (var scope = webHost.Services.CreateScope()) //#B
            {
                var services = scope.ServiceProvider;    //#C
                using (var context = services            //#C
                    .GetRequiredService<EfCoreContext>())//#C
                {
                    try
                    {
                        context.Database.Migrate(); //#D
                        //Possible seed database here //#E
                    }
                    catch (Exception ex) //#F
                    {
                        var logger = services
                            .GetRequiredService<ILogger<Program>>();
                        logger.LogError(ex,
                        "An error occurred while migrating the database.");

                        throw; //#G
                    }
                }
            }

            return webHost; //#H
        }
        /******************************************************
        #A I create an extension method that takes in IWebHost and returns IWebHost. That means I can chain multiple startup code, each of which can access the services set up by ASP.NET Core
        #B This creates a scoped service provider. Once the using block is left then all the services will be unavailable. This is the recommended way to obtain services outside of an HTTP request
        #C This creates an instance of the application's DbContext that only has a lifetime of the outer using statement
        #D Then I call EF Core's Migrate command to apply any outstanding migrations at startup.
        #E You cab add a method here to seed the database if that is required
        #F If there is an exception I log the information so that I can diagnose it. 
        #G I rethrow the exception becuase I don't want the application to carry on if there was a problem with migrating the database
        #D I return the IWebHost so that if there are additional code to run at startup they can be chained behind this extention 
            * ****************************************************/
    }
}