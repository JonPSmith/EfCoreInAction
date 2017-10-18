using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace EfCoreInAction
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //see https://docs.microsoft.com/en-us/aspnet/core/fundamentals/logging?tabs=aspnetcore2x#how-to-add-providers
            var webHost = new WebHostBuilder()
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    var env = hostingContext.HostingEnvironment;
                    config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                        .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true);
                    config.AddEnvironmentVariables();
                })
                .ConfigureLogging((hostingContext, logging) =>
                {
                    logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
                    //logging.AddConsole();
                    //logging.AddDebug();
                })
                .UseStartup<Startup>()
                .Build()
                .SetupDevelopmentDatabase();

            webHost.Run();
        }


    }
}
