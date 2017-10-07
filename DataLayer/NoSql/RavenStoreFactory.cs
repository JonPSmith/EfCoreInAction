// Copyright (c) 2017 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using Raven.Client;
using Raven.Client.Document;

namespace DataLayer.NoSql
{
    public class RavenStoreSingleton
    {
        public IDocumentStore Store { get; private set; }

        public RavenStoreSingleton(string connectionString)
        {
            var store = new DocumentStore();
            store.ParseConnectionString(connectionString);
            store.Initialize();

            //Add indexes if not already present
            new BookById().Execute(store);
            new BookByActualPrice().Execute(store);
            new BookByVotes().Execute(store);

            Store = store;
        }
    }
}