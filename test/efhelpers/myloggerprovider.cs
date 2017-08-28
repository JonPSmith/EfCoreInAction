// Copyright (c) 2016 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace test.EfHelpers
{
    public class MyLoggerProvider : ILoggerProvider
    {
        private readonly List<string> _logs;
        private LogLevel _logLevel;

        public MyLoggerProvider(List<string> logs, LogLevel logLevel = LogLevel.Information)
        {
            _logs = logs;
            _logLevel = logLevel;
        }

        public ILogger CreateLogger(string categoryName)
        {
            return new MyLogger(_logs, _logLevel);
        }

        public void Dispose()
        {
        }

        private class MyLogger : ILogger
        {
            private readonly List<string> _logs;
            private readonly LogLevel _logLevel;

            public MyLogger(List<string> logs, LogLevel logLevel)
            {
                _logs = logs;
                _logLevel = logLevel;
            }

            public bool IsEnabled(LogLevel logLevel)
            {
                return logLevel >= _logLevel;
            }

            public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
            {
                _logs.Add(formatter(state, exception));
                Console.WriteLine(formatter(state, exception));
            }

            public IDisposable BeginScope<TState>(TState state)
            {
                return null;
            }
        }
    }
}