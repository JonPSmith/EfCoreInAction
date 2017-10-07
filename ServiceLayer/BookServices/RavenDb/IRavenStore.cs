using DataLayer.NoSql;
using Microsoft.Extensions.Logging;
using Raven.Client.Document;

namespace ServiceLayer.BookServices.RavenDb
{
    public interface IRavenStore
    {
        DocumentStore Store { get; }
        INoSqlUpdater CreateSqlUpdater(ILogger logger);
    }
}