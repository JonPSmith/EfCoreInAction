// Copyright (c) 2017 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using Autofac;
using Autofac.Extensions.DependencyInjection;
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
    public class ExampleStartupAutoFac
    {
        public ExampleStartupAutoFac(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; private set; }

        // This method gets called by the runtime. 
        //Use this method to add services to the container.
        public IServiceProvider ConfigureServices //#A
            (IServiceCollection services)
        {
            // Add framework services.
            services.AddMvc();
            var connection = Configuration                
                .GetConnectionString("DefaultConnection");
            services.AddDbContext<EfCoreContext>(
                options => options.UseSqlServer(connection,
                b => b.MigrationsAssembly("DataLayer")));

            // Add Autofac
            var containerBuilder = new ContainerBuilder(); //#B
            containerBuilder.RegisterModule             //#C
                <ServiceLayer.Utils.MyAutoFacModule>(); //#C
            containerBuilder.Populate(services); //#D
            var container = containerBuilder.Build(); //#E
            return new AutofacServiceProvider(container);//#F
        }
        /*******************************************************
        #A I needed to change the method's return type from void to IServiceProvider
        #B I create an AutoFac container builder, which I use to add all the services to
        #C I use my MyAutoFacModule class to register everything that I want as a service in the ServiceLayer
        #D I need this to add all the services which were added using 'normal' ASP.NET Core service registering approach, sure and AddMVC and AddDbContext
        #E This builds an AutoFac IContainer, which holds all the services to be available via DI
        #F I then use this IContainer to create an alternative DI provider via AutoFac
         * ********************************************************/

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