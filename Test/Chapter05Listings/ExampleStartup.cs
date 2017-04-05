// Copyright (c) 2017 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using DataLayer.EfCode;
using EfCoreInAction.Logger;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ServiceLayer.AdminServices;
using ServiceLayer.AdminServices.Concrete;

namespace test.Chapter05Listings
{
    public class ExampleStartup
    {
        public ExampleStartup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. 
        //Use this method to add services to the container.
        public void ConfigureServices
            (IServiceCollection services)//#A
        {
            // Add framework services.
            services.AddMvc();  //#B
            var connection = Configuration                //#C
                .GetConnectionString("DefaultConnection");//#C
            services.AddDbContext<EfCoreContext>(          //#D
                options => options.UseSqlServer(connection,//#D
                b => b.MigrationsAssembly("DataLayer")));//#E

            services.AddTransient
                <IChangePubDateService, ChangePubDateService>(); //#A

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        }
        /*******************************************************
        #A This is the method in ASP.NET to set up services
        #B This sets up a series of services to use Controllers etc.
        #C I get the connection string from the appsettings.json file, which can be changed when I deploy
        #D Now I configure the application's DbContext to use SQL Server and provide the connection
        #E Because I am using EF Core's Add-Migrations command I need to tell it which project my application's DbContext is in 
         * ********************************************************/
        /******************************************************
        #A This registers the ChangePubDateService class as a service, with the IChangePubDateService interface as the way to access it
         * ****************************************************/

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, EfCoreContext db,
            ILoggerFactory loggerFactory, IHttpContextAccessor httpContextAccessor)
        {
            //loggerFactory.AddConsole(Configuration.GetSection("Logging"));   removed because it slows things down!   
            loggerFactory.AddProvider(new RequestTransientLogger(() => httpContextAccessor));

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}