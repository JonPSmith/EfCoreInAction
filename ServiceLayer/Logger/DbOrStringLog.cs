// =====================================================
// EfCoreExample - Example code to go with book
// Filename: DbOrStringLog.cs
// Date Created: 2016/09/11
// 
// Under the MIT License (MIT)
// 
// Written by Jon P Smith : GitHub JonPSmith, www.thereformedprogrammer.net
// =====================================================

using System.Text;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace ServiceLayer.Logger
{
    public class DbOrStringLog
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public LogLevel LogLevel { get; private set; }

        public string LogHeader { get; private set; }

        public string LogMain { get; private set; }

        public bool IsDb { get; private set; }

        public DbOrStringLog(LogLevel logLevel, string eventString, object dbCommandLogData)
        {
            LogLevel = logLevel;
            //see https://github.com/aspnet/EntityFramework/issues/6934 and https://github.com/aspnet/EntityFramework/pull/6201
            var dbComm = dbCommandLogData as DbCommandLogData;
            IsDb = dbComm != null;
            LogHeader = FormLogHeader(eventString, dbComm);
            LogMain = FormLogMain(eventString, dbComm);
        }

        public override string ToString()
        {
            return $"{LogLevel}: {LogHeader}";
        }

        //---------------------------------------------------
        //private methods

        private static string FormLogMain(string eventString, DbCommandLogData dbComm)
        {
            if (dbComm == null)
                return eventString;

            var sb = new StringBuilder();
            foreach (var param in dbComm.Parameters)
            {
                sb.Append($"-- Param ({param.Direction}): {param.Name} = {param.Value}\n");
            }
            sb.Append(dbComm.CommandText);

            return sb.ToString();
        }

        private static string FormLogHeader(string eventString, DbCommandLogData dbComm)
        {
            return dbComm == null 
                ? eventString
                : $"DBCommand ({dbComm.ElapsedMilliseconds}ms): {dbComm.CommandText}";
        }

    }
}