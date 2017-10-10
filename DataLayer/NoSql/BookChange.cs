// Copyright (c) 2017 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore;

[assembly: InternalsVisibleTo("Test")]
namespace DataLayer.NoSql
{
    internal class BookChange //#A 
    {
        public int BookId { get; }        //#B
        public EntityState State { get; } //#C

        private BookChange(int bookId, //#D
            EntityState state)          //#D
        {                               //#D
            BookId = bookId;            //#D
            State = state;              //#D
        }                               //#D

        public static IImmutableList<BookChange>           //#E
            FindChangedBooks(IImmutableList<BookChangeInfo> //#E
                changes)                                    //#E
        {
            var booksDict = new                      //#F
                Dictionary<int, BookChangeInfo>();   //#F
            foreach (var bookChange in                      //#G
                changes.Where(                              //#G
                    x => x.State != EntityState.Unchanged)) //#G
            {
                if (booksDict.ContainsKey(bookChange.BookId) //#H
                    && booksDict[bookChange.BookId].State    //#H
                        != EntityState.Modified)             //#H
                    continue;                                //#H

                booksDict[bookChange.BookId] = bookChange;   //#I
            }

            return booksDict.Select(x => new                     //#J
                BookChange(x.Value.FinalBookId, x.Value.State)) //#J
                .ToImmutableList();                              //#J
        }
    }
    /***************************************************************************
    #A This class will provide the information on what book to change, and what state to chaneg it to
    #B This will hold the final BookId as found in the SQL database
    #C This holds three possible states: Added, Deleted, or Modified
    #D Only my static method is allowed to create an instance of this class
    #E This static method will process the BookChangeInfo classes generated before the SaveChanges method was called
    #F There might be multiple entities that suggest an update to the same book. I use a Dictionary to combine them all into one change
    #G I only look at BookChangeInfo that isn't unchanged
    #H The Book entity can set the State to Added or Deleted - these always take presidence over a Modified State when other related entities might provide
    #I Otherwise I set the dictionary entry for this bookId to the value I have
    #J At the end I return the update instructions for the NoSQL database, using the FinalBookId, which may be different to the original BookId when its adding a new Book entity
     * ***************************************************************************/
}