// Copyright (c) 2017 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using Microsoft.Extensions.Logging;
using Raven.Client.Document;

namespace DataNoSql
{
    public class RavenStore : IRavenStore, IUpdateCreator
    {
        public const string RavenEventIdStart = "EfCoreInAction.NoSql.RavenDb";

        public DocumentStore Store { get; }

        public RavenStore(string connectionString)
        {
            if (string.IsNullOrEmpty(connectionString))
                return;

            var store = new DocumentStore();
            store.ParseConnectionString(connectionString);
            store.Initialize();

            //Add indexes if not already present
            new BookById().Execute(store);
            new BookByActualPrice().Execute(store);
            new BookByVotes().Execute(store);

            Store = store;
        }

        public INoSqlUpdater CreateSqlUpdater(ILogger logger)
        {
            return Store == null ? null : new RavenUpdater(Store, logger);
        }

        public INoSqlAccessor CreateNoSqlAccessor(ILogger logger)
        {
            return Store == null ? null : new RavenBookAccesser(Store, logger);
        }
    }
}