// Copyright (c) 2017 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System.Linq;
using DataLayer.EfClasses;
using DataLayer.EfCode;
using Microsoft.EntityFrameworkCore;

namespace ServiceLayer.AdminServices.Concrete
{
    public class ChangePriceOfferService
    {
        private readonly EfCoreContext _context;

        public Book OrgBook { get; private set; } 

        public ChangePriceOfferService(EfCoreContext context)
        {
            _context = context;
        }

        public PriceOffer GetOriginal(int id)      //#A
        {
            OrgBook = _context.Books               //#B
                .Include(r => r.Promotion)         //#B
                .Single(k => k.BookId == id);      //#B

            return OrgBook?.Promotion              //#C
                ?? new PriceOffer                  //#C
                   {                               //#C
                       BookId = id,                //#C
                       NewPrice = OrgBook.Price    //#C
                   };                              //#C
        }

        public Book UpdateBook(PriceOffer promotion)//#D
        {
            var book = _context.Books               //#E
                .Include(r => r.Promotion)          //#E
                .Single(k => k.BookId               //#E
                      == promotion.BookId);         //#E
            book.Promotion = promotion;             //#F
            _context.SaveChanges();                 //#G
            return book;                            //#H
        }
    }
    /*********************************************************
    #A This method gets a PriceOffer class to send to the user to update
    #B This loads the book with any existing Promotion
    #C I return either the existing Promotion for editing, or create a new one. The important point is to set the BookId, as we need to pass that through to the second stage
    #D This method handles the second part of the update, i.e. performing a selective update of the chosen book
    #E This loads the book, with any existing promotion, which is important as otherwise our new PriceOffer will clash, and throw an error
    #F I now set the book's Promotion property to the new PriceOffer class returned
    #G The SaveChanges uses its DetectChanges method, which sees that the Book Promotion property has changed. It then sorts out the changes in the relationship.
    #H The method returns the updated book
     * ******************************************************/
}
