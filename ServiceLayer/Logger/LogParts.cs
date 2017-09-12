// =====================================================
// EfCoreExample - Example code to go with book
// Filename: LogParts.cs
// Date Created: 2016/09/11
// 
// Under the MIT License (MIT)
// 
// Written by Jon P Smith : GitHub JonPSmith, www.thereformedprogrammer.net
// =====================================================

using System.Dynamic;
using System.Text;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace ServiceLayer.Logger
{
    public class LogParts
    {
        private const string EfCoreEventIdStartWith = "Microsoft.EntityFrameworkCore";
        public const string DapperEventName = "EfCoreInAction.Dapper";

        [JsonConverter(typeof(StringEnumConverter))]
        public LogLevel LogLevel { get; private set; }

        public EventId EventId { get; private set; }

        public string EventString { get; private set; }

        public bool IsDb {
            get
            {
                var name = EventId.Name;
                return name != null && (name.StartsWith(EfCoreEventIdStartWith)
                                        || name.StartsWith(DapperEventName)) ;
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