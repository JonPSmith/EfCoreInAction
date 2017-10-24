// Copyright (c) 2017 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Raven.Abstractions.Data;
using Raven.Abstractions.Extensions;
using Raven.Client.Document;

namespace DataNoSql
{
    public class RavenUpdater : INoSqlUpdater //#A
    {
        private readonly DocumentStore _store; //#B
        private readonly ILogger _logger;      //#C

        public RavenUpdater(DocumentStore store, //#D
            ILogger logger)                      //#D
        {                                        //#D
            _store = store;                      //#D
            _logger = logger;                    //#D
        }                                        //#D

        public void DeleteBook(int bookId)      //#E
        {
            using(new LogRavenCommand                  //#F
                ($"Delete: bookId {bookId}", _logger)) //#F
            using (var session = _store.OpenSession()) //#G
            {      
                session.Delete(                        //#H
                     BookListNoSql                     //#I
                         .ConvertIdToNoSqlId(bookId)); //#I
            }                                          
        }

        public void CreateNewBook(BookListNoSql book) //#J
        {
            using (new LogRavenCommand                            //#F
                ($"Create: bookId {book.GetIdAsInt()}", _logger)) //#F
            using (var bulkInsert = _store.BulkInsert())//#K
            {                                           //#K
                bulkInsert.Store(book);                 //#K
            }                                           //#K
        }
    /***************************************************************
    #A The RavenUpdater must implement the INoSqlUpdater interface so that the Data Layer can access it
    #B This is the RavenDB's link to the database, which is needed for every command
    #C I use this to log all the RavenDB accesses so that I get a similar level of logging as EF Core provides
    #D The RavenUpdater is created by the RavenStore's CreateSqlUpdater method. I do this so that it can access the private IDocumentStore
    #E This is the method to delete a book list view from the RavenDB database
    #F I have a class called LogRavenCommand that will time how long the command takes (via the dispose of this method) and then log the message and time)
    #G This is a RavenDB command that provides a session. Sessions allow normal Create, Read, Write and Update commands
    #H I delete the book from the RavenDB database
    #I The format of the RavenDB key is a string, which has to have a specific format to allow sorting on the key, so I have a specific method to convert the int that format
    #J This creates a new book list entry in the RavenDB database
    #K For this command Oren Eini suggested using the BulkInsert as it slightly quicker.
        * ************************************************************/
        public void UpdateBook(BookListNoSql book)
        {
            using (new LogRavenCommand($"Update: bookId {book.GetIdAsInt()}", _logger))
            using (var bulkInsert = _store.BulkInsert(null, new BulkInsertOptions{ OverwriteExisting = true}))
            {
                bulkInsert.Store(book);
            }
        }

        public void BulkLoad(IList<BookListNoSql> books)
        {
            using (new LogRavenCommand($"Bulk load: num books = {books.Count}", _logger))
            using (var bulkInsert = _store.BulkInsert(null, new BulkInsertOptions { OverwriteExisting = true }))
            {
                books.ForEach(x => bulkInsert.Store(x));
            }
        }
    }
}