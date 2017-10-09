// Copyright (c) 2017 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using Microsoft.Extensions.Logging;
using Raven.Client.Document;

namespace DataNoSql
{
    public class NullRavenStore : IRavenStore
    {
        public DocumentStore Store { get; } = null;

        public NullRavenStore(string connectionString)
        {    
        }

        public INoSqlUpdater CreateSqlUpdater(ILogger logger)
        {
            return null;
        }
    }
}