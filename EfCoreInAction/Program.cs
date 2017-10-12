using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

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
                    if (env.IsDevelopment())
                    {
                        config.AddUserSecrets<Startup>();
                    }
                })
                //Normally you would configure logging here, but I needed access to IHttpContextAccessor, so I had to do it in the Configure method
                //.ConfigureLogging((hostingContext, logging) =>
                //{
                //    logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
                //    logging.AddConsole();
                //    logging.AddDebug();
                //})
                .UseStartup<Startup>()
                .Build()
                .MigrateDatabase();

            webHost.Run();
        }


    }
}
