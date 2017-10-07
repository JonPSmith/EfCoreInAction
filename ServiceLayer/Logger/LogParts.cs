// =====================================================
// EfCoreExample - Example code to go with book
// Filename: LogParts.cs
// Date Created: 2016/09/11
// 
// Under the MIT License (MIT)
// 
// Written by Jon P Smith : GitHub JonPSmith, www.thereformedprogrammer.net
// =====================================================

using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using ServiceLayer.BookServices.RavenDb;

namespace ServiceLayer.Logger
{
    public class LogParts
    {
        private const string EfCoreEventIdStartWith = "Microsoft.EntityFrameworkCore";

        [JsonConverter(typeof(StringEnumConverter))]
        public LogLevel LogLevel { get; private set; }

        public EventId EventId { get; private set; }

        public string EventString { get; private set; }

        public bool IsDb
        {
            get
            {
                var name = EventId.Name;
                return name != null && (name.StartsWith(EfCoreEventIdStartWith)
                                        || name.StartsWith(RavenStore.RavenEventIdStart));
            }
        }

        public LogParts(LogLevel logLevel, EventId eventId, string eventString)
        {
            LogLevel = logLevel;
            EventId = eventId;
            EventString = eventString;
        }

        public override string ToString()
        {
            return $"{LogLevel}: {EventString}";
        }

    }
}