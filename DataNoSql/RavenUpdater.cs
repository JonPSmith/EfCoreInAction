// Copyright (c) 2017 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Extensions.Logging;
using Raven.Abstractions.Data;
using Raven.Abstractions.Extensions;
using Raven.Client.Document;

namespace DataNoSql
{
    public class RavenUpdater : INoSqlUpdater
    {
        private readonly DocumentStore _store;
        private readonly ILogger _logger;

        public RavenUpdater(DocumentStore store, ILogger logger)
        {
            _store = store;
            _logger = logger;
        }

        public void DeleteBook(int bookId)
        {
            using(new LogRavenCommand($"Delete: bookId {bookId}", _logger))
            using (var session = _store.OpenSession())
            {
                session.Delete(BookListNoSql.ConvertIdToNoSqlId(bookId));
            }
        }

        public void CreateNewBook(BookListNoSql book)
        {
            using (new LogRavenCommand($"Create: bookId {book.GetIdAsInt()}", _logger))
            using (var bulkInsert = _store.BulkInsert())
            {
                bulkInsert.Store(book);
            }
        }

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