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
        public static IImmutableList<BookChanges> FindChangedBooks(IEnumerable<EntityEntry> changes)
        {
            var booksDict = new Dictionary<int, EntityState>();
            foreach (var entity in changes)
            {
                var bookRef = entity.Entity as IBookId;
                if (bookRef == null) continue;

                if (booksDict.ContainsKey(bookRef.BookId) && 
                    (booksDict[bookRef.BookId] == EntityState.Added || booksDict[bookRef.BookId] == EntityState.Deleted))
                    //The book is already set as added or deleted, so don't let anything change that
                    continue;

                if (entity.Entity is Book book)
                {
                    //The book entity state has presidence
                    booksDict[bookRef.BookId] =
                        (entity.State == EntityState.Deleted || book.SoftDeleted)
                            ? EntityState.Deleted //it is removed from NoSql if deleted or soft deleted
                            : entity.State == EntityState.Modified &&
                               ((bool) entity.Property(nameof(Book.SoftDeleted)).OriginalValue)
                               //If the book was soft deleted and is now un-soft deleted, then treat it as a new entry
                                ? EntityState.Added
                                : entity.State; //otherwise use the state it is at
                }
                else
                {
                    //Any change to a related entity sets the book as modified
                    booksDict[bookRef.BookId] = EntityState.Modified;
                }
            }

            return booksDict.Select(x => new BookChanges(x.Key, x.Value)).ToImmutableList();
        }
    }
}