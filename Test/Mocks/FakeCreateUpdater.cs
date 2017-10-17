// Copyright (c) 2017 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System.Collections.Generic;
using DataNoSql;
using Microsoft.Extensions.Logging;
using Raven.Client.Document;

namespace test.Mocks
{
    public class FakeCreateUpdater : IUpdateCreator
    {
        private readonly FakeNoSqlUpdater _updater = new FakeNoSqlUpdater();

        public List<string> Logs => _updater.Logs;
        public string AllLogs => string.Join(",", Logs);

        public INoSqlUpdater CreateSqlUpdater(ILogger logger)
        {
            return _updater;
        }
    }
}