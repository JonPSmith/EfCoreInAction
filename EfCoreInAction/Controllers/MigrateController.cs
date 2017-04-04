using System.Collections.Immutable;
using Autofac.Core;
using DataLayer.EfCode;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ServiceLayer.DatabaseServices.Concrete;

namespace EfCoreInAction.Controllers
{
    public class MigrateController : Controller
    {
        private IHostingEnvironment _env;

        public MigrateController(IHostingEnvironment env)
        {
            _env = env;
        }

        public IActionResult Index([FromServices]DbContextOptions<EfCoreContext> options, [FromServices]IHostingEnvironment env)
        {
            var logs = options.MigrateDatabase(c => c.SeedDatabase(_env.WebRootPath)).ToImmutableList();
            return View(logs);
        }

        /// <summary>
        /// This tests the Exception filter will pick up the exception that ASP.NET Core DI will throw in migrations are outstanding
        /// </summary>
        /// <returns></returns>
        public IActionResult MigrateAspNetDi()
        {
            throw new OutstandingMigrationException();
        }

        /// <summary>
        /// This tests the Exception filter will pick up the exception that AutoFac DI will throw in migrations are outstanding
        /// </summary>
        /// <returns></returns>
        public IActionResult MigrateAutoFac()
        {
            var exception = new DependencyResolutionException("Test",
                new DependencyResolutionException("Test",
                    new OutstandingMigrationException()));
            throw exception;
        }
    }
}