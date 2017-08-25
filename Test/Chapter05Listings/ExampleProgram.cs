using System.IO;
using EfCoreInAction;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace Test.Chapter05Listings
{
    public class ExampleProgram
    {

        public static void ExampleMain(string[] args)
        {
            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .Build()
                .MigrateDatabase(); //#A
    }
    /**********************************************************
    #A The recommended way to run any startup code is to add it to the end of the BuildWebHost in the ASP.NET Core Program file
    * ********************************************************/
}
