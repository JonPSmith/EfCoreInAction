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

        public IActionResult ForceMigrateError()
        {
            var exception = new DependencyResolutionException("Test",
                new DependencyResolutionException("Test",
                    new OutstandingMigrationException()));
            throw exception;

        }
    }
}