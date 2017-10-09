// Copyright (c) 2017 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System.Linq;
using DataLayer.EfCode;
using DataNoSql;
using EfCoreInAction.Helpers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ServiceLayer.DatabaseServices.Concrete;

namespace EfCoreInAction.Controllers
{
    public class GenerateController : Controller
    {

        //This is a hack. Shouldn't use static variables like this! Not multi-user safe!!
        private static double _progress;
        private static bool _cancel;

        // GET
        public IActionResult Index([FromServices]EfCoreContext context)
        {
            Request.ThrowErrorIfNotLocal();

            _progress = 0;
            _cancel = false;
            return View(context.Books.Count());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Books(int numBooks, bool wipeDatabase, 
            [FromServices]EfCoreContext context,
            [FromServices]DbContextOptions<EfCoreContext> options,
            [FromServices]RavenStore storeProvider,
            [FromServices]ILogger<RavenStore> ravenLogger,
            [FromServices]IHostingEnvironment env)
        {
            if (numBooks == 0)
                return View((object) "Error: should contain the number of books to generate.");

            Request.ThrowErrorIfNotLocal();

            if (wipeDatabase)
                context.DevelopmentWipeCreated(env.WebRootPath);
            options.GenerateBooks(storeProvider, ravenLogger,
                numBooks, env.WebRootPath, numWritten =>
            {
                _progress = numWritten * 100.0 / numBooks;
                return _cancel;
            });
            return
                View((object) ((_cancel ? "Cancelled" : "Successful") +
                     $" generate. Num books in database = {context.Books.Count()}."));
        }

        [HttpPost]
        public ActionResult Progress(bool cancel)
        {
            _cancel = cancel;
            return Content(_progress.ToString());
        }
    }
}