using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataLayer.EfCode;
using EfCoreInAction.Logger;
using EfCoreInAction.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ServiceLayer.DatabaseServices.Concrete;

namespace EfCoreInAction
{
    public class Startup
    {
        private IHostingEnvironment _env;

        public Startup(IHostingEnvironment env)
        {
            _env = env;
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var gitBranchName = _env.WebRootPath.GetBranchName();

            // Add framework services.
            services.AddMvc();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton(_env);
            //This makes the Git branch name available via injection
            services.AddSingleton(new AppInformation(gitBranchName));

            var connection = Configuration.GetConnectionString("DefaultConnection");
            if (_env.IsDevelopment())
            {
                //if running in development mode then we alter the connection to have the branch name in it
                connection = connection.FormDatabaseConnection(gitBranchName);
            }
            services.AddDbContext<EfCoreContext>(options => options.UseSqlServer(connection,
                b => b.MigrationsAssembly("DataLayer")));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, 
            ILoggerFactory loggerFactory, IHttpContextAccessor httpContextAccessor)
        {
            //Remove the standard loggers because they slow the applictaion down
            //loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            //loggerFactory.AddDebug();
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
            //see https://blogs.msdn.microsoft.com/dotnet/2016/09/29/implementing-seeding-custom-conventions-and-interceptors-in-ef-core-1-0/
            using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                //if in development mode we use 
                serviceScope.ServiceProvider.GetService<EfCoreContext>().EnsureDatabaseCreatedAndSeeded(_env.WebRootPath,
                    _env.IsDevelopment() ? DbStartupModes.EnsureCreated : DbStartupModes.UseMigrations);
            }
        }
    }
}
