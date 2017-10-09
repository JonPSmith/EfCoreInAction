// Copyright (c) 2017 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace DataNoSql
{
    public class FakeRavenUpdater : INoSqlUpdater
    {
        private readonly ILogger _logger;

        public FakeRavenUpdater(ILogger logger)
        {
            _logger = logger;
        }

        public void DeleteBook(int bookId)
        {
            _logger.LogInformation(new EventId(1234567, RavenStore.RavenEventIdStart+".Delete"), $"Delete: BookId = {bookId}" );
        }

        public void CreateNewBook(BookListNoSql book)
        {
            _logger.LogInformation(new EventId(1234567, RavenStore.RavenEventIdStart + ".Create"), $"Create: BookId = {book.GetIdAsInt()}");
        }

        public void UpdateBook(BookListNoSql book)
        {
            _logger.LogInformation(new EventId(1234567, RavenStore.RavenEventIdStart + ".Update"), $"Update: BookId = {book.GetIdAsInt()}");
        }
        public void BulkLoad(IList<BookListNoSql> books)
        {
            _logger.LogInformation(new EventId(1234567, RavenStore.RavenEventIdStart + ".BulkLoad"), $"Bulk load: num books = {books.Count}");
        }
    }
}