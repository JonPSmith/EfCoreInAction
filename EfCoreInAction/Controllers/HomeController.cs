using System.Collections.Generic;
using System.Linq;
using DataLayer.EfCode;
using Microsoft.AspNetCore.Mvc;
using ServiceLayer.BookServices;
using ServiceLayer.BookServices.Concrete;
using ServiceLayer.Logger;

namespace EfCoreInAction.Controllers
{
    public class HomeController : Controller
    {
        private readonly EfCoreContext _context;

        public HomeController(EfCoreContext context)   //#A
        {                                              //#A
            _context = context;                        //#A
        }                                              //#A

        public IActionResult Index                     //#B
            (SortFilterPageOptions options)            //#C
        {
            var listService =                          //#D
                new ListBooksService(_context);        //#D

            var bookList = listService                 //#E
                .SortFilterPage(options)               //#E
                .ToList();                             //#F

            var traceIdent = HttpContext.TraceIdentifier; //REMOVE THIS FOR BOOK as it could be confusing

            return View(new BookListCombinedDto         //#G
                (traceIdent, options, bookList));       //#G
        }
        /***************************************************
        #A The applications's DbContext is provided by ASP.NET Core
        #B This is an ASP.NET action. It is called when the Home page is called up by the user
        #C The options parameter is filled with various sort, filter, page options via the URL
        #D The ListBooksService is created using the applications's DbContext in the field _context
        #E The SortFilterPage method is called with the sort, filter, page options provided
        #F The .ToList() is what executes the LINQ commands, which causes EF Core to translate the LINQ into the appropriate SQL to access the database
        #G It then sends the options (to fill in the various controls at the top of the page) and the actual list of BookListDTo's to display as a HTML table
          * *************************************************/

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
            ViewData["Message"] = "Your application description page.";

            return View();
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
