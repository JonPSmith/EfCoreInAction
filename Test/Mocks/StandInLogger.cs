// Copyright (c) 2017 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace test.Mocks
{
    public class StandInLogger : ILogger
    {

        private readonly List<string> _logs;
        private readonly LogLevel _logLevel;

        public StandInLogger(List<string> logs, LogLevel logLevel = LogLevel.Information)
        {
            _logs = logs;
            _logLevel = logLevel;
        }

        public bool IsEnabled(LogLevel logLevel) //#F
        {
            return logLevel >= _logLevel;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, //#G
            TState state, Exception exception,
            Func<TState, Exception, string> formatter)
        {
            if (!IsEnabled(logLevel)) //#H
                return;

            //_logs.Add(formatter(state, exception)); //#I
            _logs.Add($"{logLevel}: " + //#J
                        formatter(state, exception) + //#J
                        (exception == null            //#J
                            ? ""                       //#J
                            : ", Exception = \n" + exception));//#J
            Console.WriteLine(formatter(state, exception));//#K
        }

        public IDisposable BeginScope<TState>(TState state) //#L
        {
            return null;
        }
        
    }
}