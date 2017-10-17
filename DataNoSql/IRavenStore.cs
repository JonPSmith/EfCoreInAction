using Microsoft.Extensions.Logging;
using Raven.Client.Document;

namespace DataNoSql
{
    public interface IRavenStore
    {
        DocumentStore Store { get; }
    }
}