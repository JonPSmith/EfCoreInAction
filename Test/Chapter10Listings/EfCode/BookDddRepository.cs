// // Copyright (c) 2017 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// // Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.Linq;
using DataLayer.QueryObjects;
using Microsoft.EntityFrameworkCore;
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

        public IQueryable<BookDdd> GetBookList( //#E
            DddSortFilterPageOptions options)
        {
            var booksQuery = _context.Books
                .AsNoTracking()
                .OrderBooksBy(options.OrderByOptions) //#F
                .FilterBooksBy(options.FilterBy,  //#F
                    options.FilterValue);

            options.SetupRestOfDto(booksQuery); //#F

            return booksQuery.Page(options.PageNum - 1,
                options.PageSize);
        }
    }
    /*******************************************************
    #A I create the repository by passing in the applicatoins's DbContext
    #B This simply adds the book to the context
    #C This finds an existing book using its primary key
    #D This tries to delete the book with the given primary key. It returns true if it found a book to delete, ot false if it didn't find a book with that primary key
    #E This is the DDD equivalent to the ListBooksService class, but it passes back IQueryable<BookDdd> rather than the IQueryable<BoolListDto> that the original verision did
    #F These query objects are copies of the query objects in the original, non-DDD, design. They use the IQueryable<BookDdd> rather that the original IQueryable<BoolListDto>
     * ****************************************************/
}