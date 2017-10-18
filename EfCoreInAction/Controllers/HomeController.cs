using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataLayer.EfCode;
using DataNoSql;
using EfCoreInAction.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ServiceLayer.BookServices;
using ServiceLayer.BookServices.Concrete;
using ServiceLayer.BookServices.RavenDb;
using ServiceLayer.Logger;

namespace EfCoreInAction.Controllers
{
    public class HomeController : BaseTraceController
    {
        private readonly EfCoreContext _context;

        public HomeController(EfCoreContext context)
        {                                           
            _context = context;                     
        }

        public IActionResult Index
            (SortFilterPageOptions options)
        {
            var listService =
                new ListBooksService(_context);

            var bookList = listService     
                .SortFilterPage(options)
                .ToList();               

            SetupTraceInfo();           //REMOVE THIS FOR BOOK as it could be confusing

            return View(new BookListCombinedDto
                (options, bookList));
        }

        /// <summary>
        /// This provides the filter search dropdown content
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        [HttpGet]
        public JsonResult GetFilterSearchContent    //#A
            (SortFilterPageOptions options)         //#B
        {
            var service = new                       //#C
                BookFilterDropdownService(_context);//#C

            var traceIdent = HttpContext.TraceIdentifier; //REMOVE THIS FOR BOOK as it could be confusing

            return Json(                            //#D
                new TraceIndentGeneric<IEnumerable<DropdownTuple>>(
                traceIdent,
                service.GetFilterDropDownValues(    //#E
                    options.FilterBy)));            //#E
        }
        /****************************************************
        #A This method is called by the URL Home/GetFilterSearchContent
        #B It also gets the sort, filter, page options from the URL
        #C We create the BookFilterDropdownService using the applications's DbContext provided by ASP.NET Core
        #D This converts normal .NET objects into JSON format to send back to the AJAX Get call
        #E The GetFilterDropDownValues method calculates the right data needed for the chosen filter type 
         * **************************************************/


        public IActionResult About()
        {
            var isLocal = Request.IsLocal();
            return View(isLocal);
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
