// Copyright (c) 2017 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using System.Runtime.CompilerServices;
using DataLayer.EfCode;
using Microsoft.EntityFrameworkCore;

[assembly: InternalsVisibleTo("Test")]
namespace DataLayer.NoSql
{
    internal class NoSqlUpdater
    {
        private readonly EfCoreContext _context;

        public NoSqlUpdater(EfCoreContext context)
        {
            _context = context;
        }

        public void UpdateNoSql(IImmutableList<BookChanges> booksToUpdate)
        {
            foreach (var bookToUpdate in booksToUpdate)
            {
                switch (bookToUpdate.State)
                {
                    case EntityState.Deleted:
                        //Send delete of BookId to NoSql
                        break;
                    case EntityState.Modified:
                        var modifiedBook = BookNoSqlDto.ProjectBook(_context.Books, bookToUpdate.BookId);
                        //Send updated information to NoSQL
                        break;
                    case EntityState.Added:
                        var newBook = BookNoSqlDto.ProjectBook(_context.Books, bookToUpdate.BookId);
                        //Send new book information to NoSQL
                        break;
                    case EntityState.Unchanged:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
    }
}