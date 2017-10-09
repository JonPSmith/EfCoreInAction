// Copyright (c) 2017 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using System.Linq;
using System.Runtime.CompilerServices;
using DataLayer.EfCode;
using DataNoSql;
using Microsoft.EntityFrameworkCore;

[assembly: InternalsVisibleTo("Test")]
namespace DataLayer.NoSql
{
    internal class ApplyChangeToNoSql
    {
        private readonly EfCoreContext _context;
        private readonly INoSqlUpdater _updater;

        public ApplyChangeToNoSql(EfCoreContext context, INoSqlUpdater updater)
        {
            _context = context;
            _updater = updater;
        }

        public void UpdateNoSql(IImmutableList<BookChanges> booksToUpdate)
        {
            if (_updater == null || !booksToUpdate.Any()) return;

            foreach (var bookToUpdate in booksToUpdate)
            {
                switch (bookToUpdate.State)
                {
                    case EntityState.Deleted:
                        _updater.DeleteBook(bookToUpdate.BookId);
                        break;
                    case EntityState.Modified:
                        var modifiedBook = _context.Books.ProjectBook( bookToUpdate.BookId);
                        _updater.UpdateBook(modifiedBook);
                        break;
                    case EntityState.Added:
                        var newBook = _context.Books.ProjectBook(bookToUpdate.BookId);
                        _updater.CreateNewBook(newBook);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
    }
}