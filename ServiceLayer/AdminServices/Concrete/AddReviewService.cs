// Copyright (c) 2017 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System.Linq;
using DataLayer.EfClasses;
using DataLayer.EfCode;
using Microsoft.EntityFrameworkCore;

namespace ServiceLayer.AdminServices.Concrete
{
    public class AddReviewService : IAddReviewService
    {
        private readonly EfCoreContext _context;

        public AddReviewService(EfCoreContext context)
        {
            _context = context;
        }

        public string GetTitleOfBook(int id) //#A
        {
            return _context.Books    
                .Where(p => p.BookId == id)
                .Select(p => p.Title)      
                .Single();                 
        }

        public void AddReviewToBook(int bookId, int numStars, string comment, string voterName)
        {
            var book = _context.Books   
                .Single(k => k.BookId == bookId);
            book.AddReview(_context, numStars, comment, voterName);
            _context.BookSaveChanges();
        }
    }

}
