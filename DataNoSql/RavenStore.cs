// Copyright (c) 2017 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using Microsoft.Extensions.Logging;
using Raven.Client.Document;

namespace DataNoSql
{
    public class RavenStore : IUpdateCreator, IQueryCreator
    {
        public const string RavenEventIdStart = "EfCoreInAction.NoSql.RavenDb";
        private readonly DocumentStore _store;
        private readonly ILogger _logger;

        public RavenStore(string connectionString, ILogger logger)
        {
            if (string.IsNullOrEmpty(connectionString))
                return;
            _logger = logger;

            var store = new DocumentStore();
            store.ParseConnectionString(connectionString);
            store.Initialize();

            //Add indexes if not already present
            new BookById().Execute(store);
            new BookByActualPrice().Execute(store);
            new BookByVotes().Execute(store);

            _store = store;
        }

        public INoSqlUpdater CreateSqlUpdater()
        {
            return new RavenUpdater(_store, _logger);
        }

        public INoSqlAccessor CreateNoSqlAccessor()
        {
            return new RavenBookAccesser(_store, _logger);
        }
    }
}