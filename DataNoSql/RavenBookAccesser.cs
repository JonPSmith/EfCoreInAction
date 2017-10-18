// Copyright (c) 2017 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System.Diagnostics;
using Microsoft.Extensions.Logging;
using Raven.Client;
using Raven.Client.Linq;

namespace DataNoSql
{
    public class RavenBookAccesser : INoSqlAccessor
    {
        private readonly IDocumentStore _store;
        private readonly Stopwatch _stopwatch = new Stopwatch();
        private readonly ILogger _logger;

        private IDocumentSession _session;

        public string Command { get; set; }

        public RavenBookAccesser(IDocumentStore store, ILogger logger)
        {
            _logger = logger;
            _store = store;
            _stopwatch.Start();
        }
        public IRavenQueryable<BookListNoSql> BookListQuery()
        {
            _session = _store.OpenSession();
            _stopwatch.Start();
            return _session.Query<BookListNoSql>();
        }

        public void Dispose()
        {
            _session.Dispose();
            _stopwatch.Stop();
            _logger.LogInformation(new EventId(1, RavenStore.RavenEventIdStart + ".Write"),
                $"Raven Command. Execute time = {_stopwatch.ElapsedMilliseconds} ms.\n" + Command);
        }

    }
}