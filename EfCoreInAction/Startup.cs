using System;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using DataLayer.EfCode;
using EfCoreInAction.Filters;
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
        private readonly IHostingEnvironment _env;

        public Startup(IHostingEnvironment env)
        {
            _env = env;
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables()
                .AddInMemoryCollection();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            var gitBranchName = _env.WebRootPath.GetBranchName();

            // Add framework services.
            //This adds an exception filter to catch the OutstandingMigrationException
            services.AddMvc(options => options.Filters.Add(typeof(MigrateExceptionFilter)));

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

            var containerBuilder = new ContainerBuilder();
            containerBuilder.RegisterModule<ServiceLayer.Utils.MyAutoFacModule>();
            containerBuilder.Populate(services);
            var container = containerBuilder.Build();
            return new AutofacServiceProvider(container);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app,
            ILoggerFactory loggerFactory, IHttpContextAccessor httpContextAccessor)
        {
            //loggerFactory.AddConsole(Configuration.GetSection("Logging"));   //removed because it slows things down! 
            //loggerFactory.AddDebug();  //removed because it slows things down! 
            loggerFactory.AddProvider(new RequestTransientLogger(() => httpContextAccessor));

            if (_env.IsDevelopment())
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
