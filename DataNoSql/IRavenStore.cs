using Microsoft.Extensions.Logging;
using Raven.Client.Document;

namespace DataNoSql
{
    public interface IRavenStore
    {
        DocumentStore Store { get; }

        INoSqlUpdater CreateSqlUpdater(ILogger logger);

        INoSqlAccessor CreateNoSqlAccessor(ILogger logger);
    }
}