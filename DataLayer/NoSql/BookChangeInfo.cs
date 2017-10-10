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
    internal class BookChangeInfo //#A
    {
        private readonly Book _book; //#B

        public int BookId { get; } //#C
        public EntityState State { get; } //#D

        public int FinalBookId => _book?.BookId ?? BookId; //#E

        private BookChangeInfo(int bookId, //#F
            EntityEntry entity) //#F
        {
            BookId = bookId;
            _book = entity.Entity as Book;  //#G

            if (_book != null) //#H
            {
                var softDeletedProp = entity.Property(  //#I
                    nameof(_book.SoftDeleted));         //#I

                if (softDeletedProp.IsModified) //#J
                {                               //#J
                    State = _book.SoftDeleted   //#J
                        ? EntityState.Deleted   //#J
                        : EntityState.Added;    //#J
                }
                else if (entity.State ==        //#K
                    EntityState.Deleted)        //#K
                {                               //#K
                    State = _book.SoftDeleted   //#K
                        ? EntityState.Unchanged //#K
                        : EntityState.Deleted;  //#K
                }
                else
                {
                    State = _book.SoftDeleted   //#L
                        ? EntityState.Unchanged //#L
                        : entity.State;         //#L
                }
            }
            else
            {
                State = EntityState.Modified; //#M
            }
        }
    /*******************************************************************
    #A Each class holds the correct State to give to the NoSQL updater, plus a way to get the Final BookId after the SaveChanges method is called
    #B If the instance is a Book entity then I keep a reference to it, as its BookId may change after the SaveChanges method is called
    #C This holds the BookId before the the SaveChanges method is called. It may be negative or positive, but it will link all entities that are linked to the same Book entity
    #D This holds the State that NoSQL needs to know about. It might be different from the State EF Core is using.
    #E This property can be used after he SaveChanges method call to access the correct BookId
    #F The constuctor takes in the BookId provided by the IBookId interface and the entity itself
    #G This takes a copy of the entity if it is of type Book. The Book entity always takes precidence in any update
    #H If the entity is of type Book then we need to handle the SoftDeleted state, as that affects whether we want a book list view for that book
    #I I find the SoftDeleted property, as I need to see if this property was changed
    #J If the Book entity's SoftDeleted was changed then it defines whether the book list contains this book or not
    #K If the Book is deleted, then we ensure we don't try to delete it again if its not already been excluded from the book list because of the SoftDeleted property was true
    #L Otherwise the Book's state will be used, unless the Book is already SoftDeleted
    #M If its a linked entity that has changed then this can only cause a update of the book list view
     * *******************************************************************/

        public static IImmutableList<BookChangeInfo> FindBookChanges(IEnumerable<EntityEntry> changes)
        {
            return changes
                .Select(x => new {entity = x, bookRef = x.Entity as IBookId})
                .Where(x => x.entity.State != EntityState.Unchanged && x.bookRef != null)
                .Select(x => new BookChangeInfo(x.bookRef.BookId, x.entity))
                .ToImmutableList();
        }
    }
}