using System.Globalization;
using DataLayer.EfClasses;
using DataLayer.EfCode;
using EfCoreInAction.Helpers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using ServiceLayer.AdminServices;
using ServiceLayer.AdminServices.Concrete;
using ServiceLayer.DatabaseServices.Concrete;

namespace EfCoreInAction.Controllers
{
    public class AdminController : BaseTraceController
    {
        private readonly EfCoreContext _context;
        private readonly IHostingEnvironment _env;

        public AdminController(EfCoreContext context, IHostingEnvironment env)
        {
            _context = context;
            _env = env;
        }

        //-----------------------------------------------
        //Admin actions that are called on a sepcific book

        public IActionResult ChangePubDate(int id)
        {
            Request.ThrowErrorIfNotLocal();

            var service = new ChangePubDateService(_context);
            var dto = service.GetOriginal(id);
            SetupTraceInfo(); //REMOVE THIS FOR BOOK as it could be confusing
            return View(dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ChangePubDate(ChangePubDateDto dto)
        {
            Request.ThrowErrorIfNotLocal();

            var service = new ChangePubDateService(_context);
            service.UpdateBook(dto);
            SetupTraceInfo(); //REMOVE THIS FOR BOOK as it could be confusing
            return View("BookUpdated", "Successfully changed publication date");
        }

        public IActionResult ChangePromotion(int id)
        {
            Request.ThrowErrorIfNotLocal();

            var service = new ChangePriceOfferService(_context);
            var priceOffer = service.GetOriginal(id);
            ViewData["BookTitle"] = service.OrgBook.Title;
            ViewData["OrgPrice"] = service.OrgBook.Price < 0 
                ? "Not currently for sale"
                : service.OrgBook.Price.ToString("c", new CultureInfo("en-US"));
            SetupTraceInfo();
            return View(priceOffer);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ChangePromotion(PriceOffer dto)
        {
            Request.ThrowErrorIfNotLocal();

            var service = new ChangePriceOfferService(_context);
            var book = service.UpdateBook(dto);
            SetupTraceInfo();
            return View("BookUpdated", "Successfully added/changed a promotion");
        }


        public IActionResult AddBookReview(int id)
        {
            Request.ThrowErrorIfNotLocal();

            var service = new AddReviewService(_context);
            var review = service.GetBlankReview(id);
            ViewData["BookTitle"] = service.BookTitle;
            SetupTraceInfo();
            return View(review);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddBookReview(Review dto)
        {
            Request.ThrowErrorIfNotLocal();

            var service = new AddReviewService(_context);
            var book = service.AddReviewToBook(dto);
            SetupTraceInfo();
            return View("BookUpdated", "Successfully added a review");
        }

        //------------------------------------------------
        //Amdin commands that are called from the top menu

        public IActionResult ResetDatabase()
        {
            Request.ThrowErrorIfNotLocal();

            var numBooks = _context.EnsureDatabaseCreatedAndSeeded(_env.WebRootPath, DbStartupModes.EnsureDeletedCreated);
            SetupTraceInfo();
            return View("BookUpdated", $"Successfully reset the database and added {numBooks} books.");
        }

    }
}
