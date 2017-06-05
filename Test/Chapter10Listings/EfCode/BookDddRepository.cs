// // Copyright (c) 2017 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// // Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.Linq;
using DataLayer.QueryObjects;
using Microsoft.EntityFrameworkCore;
using ServiceLayer.BookServices;
using ServiceLayer.BookServices.QueryObjects;
using Test.Chapter10Listings.EfClasses;
using Test.Chapter10Listings.QueryObjects;

namespace Test.Chapter10Listings.EfCode
{
    public class BookDddRepository
    {
        private readonly Chapter10DbContext _context;

        public BookDddRepository(Chapter10DbContext context) //#A
        {
            _context = context;
        }

        public void AddBook(BookDdd book) //#B
        {
            _context.Add(book);
        }

        public BookDdd FindBook(int bookId) //#C
        {
            return _context.Find<BookDdd>(bookId);
        }

        public bool DeleteBook(int bookId) //#D
        {
            var book = FindBook(bookId);
            if (book == null)
                return false;
            _context.Remove(book);
            return true;
        }

        public IQueryable<BookListDto> GetBookList( //#E
            SortFilterPageOptions options)
        {
            var booksQuery = _context.Books
                .AsNoTracking()
                .MapBookDddToDto() //#F
                .OrderBooksBy(options.OrderByOptions)
                .FilterBooksBy(options.FilterBy,
                    options.FilterValue);

            options.SetupRestOfDto(booksQuery);

            return booksQuery.Page(options.PageNum - 1,
                options.PageSize);
        }
    }
    /*******************************************************
    #A I create the repository by passing in the applicatoins's DbContext
    #B This simply adds the book to the context
    #C This finds an existing book using its primary key
    #D This tries to delete the book with the given primary key. It returns true if it found a book to delete, ot false if it didn't find a book with that primary key
    #E This is the DDD equivalent to the ListBooksService class found in the ServiceLayer. It applies a select query, sorts and filters based on the content of the options passed in
    #F This is the only change from how the ListBooksService class works, simply because I used a different entity name
     * ****************************************************/
}