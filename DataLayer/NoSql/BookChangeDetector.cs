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
    internal class BookChangeDetector
    {
        public int BookId { get; private set; }
        public int FinalBookId => Book?.BookId ?? BookId;
        public EntityEntry Entity { get; private set; }
        public EntityState State { get; private set; }

        public Book Book { get; private set; }

        public BookChangeDetector(int bookId, EntityEntry entity, Book book)
        {
            BookId = bookId;
            Entity = entity;
            Book = book;

            if (Book != null)
            {
                //Its the actual book entity, so we need to work out the state in a bit more detail
                State = (entity.State == EntityState.Deleted || Book.SoftDeleted)
                    ? EntityState.Deleted //it is removed from NoSql if deleted or soft deleted
                    : entity.State == EntityState.Modified &&
                      ((bool) Entity.Property(nameof(Book.SoftDeleted)).OriginalValue)
                        //If the book was soft deleted and is now un-soft deleted, then treat it as a new entry
                        ? EntityState.Added
                        : entity.State; //otherwise use the state it is at
            }
            else
            {
                //If its not the Book, then the state is modified
                State = EntityState.Modified;
            }
        }

        public static IImmutableList<BookChangeDetector> FindBookChanges(IEnumerable<EntityEntry> changes)
        {
            var result = new List<BookChangeDetector>();
            foreach (var entity in changes.Where(x => x.State != EntityState.Unchanged))
            {
                var bookRef = entity.Entity as IBookId;
                if (bookRef == null) continue;

                result.Add(new BookChangeDetector(bookRef.BookId, entity, entity.Entity as Book));
            }
            return result.ToImmutableList();
        }
    }
}