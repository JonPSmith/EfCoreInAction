// Copyright (c) 2017 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using DataLayer.NoSql;
using Microsoft.Extensions.Logging;
using Raven.Client;
using Raven.Client.Document;

namespace ServiceLayer.BookServices.RavenDb
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

        public void CreateNewBook(BookNoSqlDto book)
        {
            _logger.LogInformation(new EventId(1234567, RavenStore.RavenEventIdStart + ".Create"), $"Create: BookId = {book.StringIdAsInt()}");
        }

        public void UpdateBook(BookNoSqlDto book)
        {
            _logger.LogInformation(new EventId(1234567, RavenStore.RavenEventIdStart + ".Update"), $"Update: BookId = {book.StringIdAsInt()}");
        }
    }
}