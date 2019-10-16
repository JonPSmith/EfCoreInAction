using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using DataLayer.EfCode;
using EfCoreInAction.Logger;
using EfCoreInAction.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NetCore.AutoRegisterDi;
using ServiceLayer.BookServices;
using ServiceLayer.DatabaseServices.Concrete;

namespace EfCoreInAction
{
    public class Startup
    {

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var gitBranchName = DatabaseStartupHelpers.GetWwwRootPath().GetBranchName();

            // Add framework services.
            services.AddMvc();

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            //This makes the Git branch name available via injection
            services.AddSingleton(new AppInformation(gitBranchName));

            var connection = Configuration.GetConnectionString("DefaultConnection");
            if (Configuration["ENVIRONMENT"] == "Development")
            {
                //if running in development mode then we alter the connection to have the branch name in it
                connection = connection.FormDatabaseConnection(gitBranchName);
            }
            services.AddDbContext<EfCoreContext>(options => options.UseSqlServer(connection,
                b => b.MigrationsAssembly("DataLayer")));
            services.AddControllersWithViews();
            services.AddRazorPages();

            //replace AutoFac not so easy to use in ASP.NET Core 3 - uses one of my libraries
            services.RegisterAssemblyPublicNonGenericClasses(Assembly.GetAssembly(typeof(BookListDto)))
                .Where(c => c.Name.EndsWith("Service"))
                .AsPublicImplementedInterfaces();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, 
            ILoggerFactory loggerFactory, IHttpContextAccessor httpContextAccessor)
        {
            loggerFactory.AddProvider(new RequestTransientLogger(() => httpContextAccessor));
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });
        }
    }
}
