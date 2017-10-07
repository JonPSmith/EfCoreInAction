// Copyright (c) 2017 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Runtime.CompilerServices;
using DataLayer.EfClasses;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

[assembly: InternalsVisibleTo("Test")]
namespace DataLayer.NoSql
{
    internal class BookChanges
    {
        public BookChanges(int bookId, EntityState state)
        {
            BookId = bookId;
            State = state;
        }

        public int BookId { get; private set; }
        public EntityState State { get; private set; }

        /// <summary>
        /// This works out what Books have been updated
        /// NOTE: It doesn't currently catch changes to the Author's Name, which would need all the Books with that Author updated
        /// </summary>
        /// <param name="changes">The tracked changes to look at</param>
        /// <returns>A list of BookChanges that </returns>
        public static IImmutableList<BookChanges> FindChangedBooks(IImmutableList<BookChangeDetector> changes)
        {
            var booksDict = new Dictionary<int, BookChangeDetector>();
            foreach (var taggedBook in changes)
            {
                if (booksDict.ContainsKey(taggedBook.BookId) && 
                    (booksDict[taggedBook.BookId].State == EntityState.Added || booksDict[taggedBook.BookId].State == EntityState.Deleted))
                    //The book is already set as added or deleted, so don't let anything change that
                    continue;

                booksDict[taggedBook.BookId] = taggedBook;
            }

            return booksDict.Select(x => new BookChanges(x.Value.FinalBookId, x.Value.State)).ToImmutableList();
        }
    }
}