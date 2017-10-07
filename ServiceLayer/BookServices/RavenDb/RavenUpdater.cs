// Copyright (c) 2017 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.Diagnostics;
using DataLayer.EfCode;
using DataLayer.NoSql;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Logging;
using Raven.Abstractions.Data;
using Raven.Client.Document;
using ServiceLayer.Logger;

namespace ServiceLayer.BookServices.RavenDb
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

        private class LogRavenCommand : IDisposable
        {
            private readonly string _command;
            private readonly Stopwatch _stopwatch = new Stopwatch();
            private readonly ILogger _logger;

            public LogRavenCommand(string command, ILogger logger)
            {
                _command = command;
                _logger = logger;
                _stopwatch.Start();
            }

            public void Dispose()
            {
                _stopwatch.Stop();
                _logger.LogInformation(new EventId(1, RavenStore.RavenEventIdStart+ ".Write"),
                    $"Raven Write. Execute time = {_stopwatch.ElapsedMilliseconds} ms.\n" + _command);
            }
        }

        public void DeleteBook(int bookId)
        {
            using(new LogRavenCommand($"Delete: bookId {bookId}", _logger))
            using (var session = _store.OpenSession())
            {
                session.Delete(BookNoSqlDto.ConvertIdToNoSqlId(bookId));
            }
        }

        public void CreateNewBook(BookNoSqlDto book)
        {
            using (new LogRavenCommand($"Create: bookId {book.StringIdAsInt}", _logger))
            using (var bulkInsert = _store.BulkInsert())
            {
                bulkInsert.Store(book);
            }
        }

        public void UpdateBook(BookNoSqlDto book)
        {
            using (new LogRavenCommand($"Update: bookId {book.StringIdAsInt}", _logger))
            using (var bulkInsert = _store.BulkInsert(null, new BulkInsertOptions{ OverwriteExisting = true}))
            {
                bulkInsert.Store(book);
            }
        }
    }
}