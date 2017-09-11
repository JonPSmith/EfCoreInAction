// Copyright (c) 2017 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DataLayer.EfCode;
using EfCoreInAction.Helpers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
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
        public IActionResult Books(int numBooks, [FromServices]EfCoreContext context, [FromServices]IHostingEnvironment env)
        {
            if (numBooks == 0)
                return View((object)"Error: should contain the number of books to generate.");

            Request.ThrowErrorIfNotLocal();

            context.DevelopmentWipeCreated(env.WebRootPath);
            var booksInDatabase = context.GenerateBooks(numBooks, env.WebRootPath, numWritten =>
            {
                _progress = numWritten * 100.0 / numBooks;
                return _cancel;
            });

            return View((object)$"Successfull generate. Num books in database = {booksInDatabase}.");
        }

        [HttpPost]
        public ActionResult Progress(bool cancel)
        {
            _cancel = cancel;
            return Content(_progress.ToString());
        }
    }
}