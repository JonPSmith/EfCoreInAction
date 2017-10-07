// Copyright (c) 2017 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using Raven.Client;
using Raven.Client.Document;

namespace ServiceLayer.BookServices.RavenDb
{
    public class RavenStoreFactory
    {
        private readonly DocumentStore _store;

        public RavenStoreFactory(string connectionString)
        {
            _store = new DocumentStore();
            _store.ParseConnectionString(connectionString);
        }

        public IDocumentStore Build()
        {
            _store.Initialize();

            //Add indexes if not already present
            new BookById().Execute(_store);
            new BookByActualPrice().Execute(_store);
            new BookByVotes().Execute(_store);

            return _store;
        }
    }
}