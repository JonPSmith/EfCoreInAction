// Copyright (c) 2017 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System.Linq;
using DataLayer.EfCode;

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

        public void AddReviewToBook(int bookId, //#A
            int numStars, string comment, string voterName)
        {
            var book = _context.Books.Find(bookId); //#B
            book.AddReview(_context,           //#C
                numStars, comment, voterName); //#C
            _context.SaveChangesWithReviewCheck(); //#D
        }
    }
    /**********************************************************
    #A This method is called by the ASP.NET Core action to add a new review to a book
    #B I find the book that the user wants to add a review to
    #C I call the AddReview method in the Book instance loaded
    #D I call a special version of the SaveChanges which checks if the AverageVotes or ReviewsCounts are different to the values it obtained when it loaded the Book entity.
     * *******************************************************/

}
