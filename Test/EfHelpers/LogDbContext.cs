// Copyright (c) 2016 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Logging;

namespace test.EfHelpers
{
    public class LogDbContext
    {
        private readonly List<string> _logs = new List<string>();

        public ImmutableList<string> Logs => _logs.ToImmutableList();

        public void ClearLogs()
        {
            _logs.Clear();
        }

        public LogDbContext(DbContext context, LogLevel logLevel = LogLevel.Information)
        {
            var loggerFactory = context.GetService<ILoggerFactory>();
            loggerFactory.AddProvider(new MyLoggerProvider(_logs, logLevel));
        }

        private class MyLoggerProvider : ILoggerProvider
        {
            private readonly List<string> _logs;
            private LogLevel _logLevel;

            public MyLoggerProvider(List<string> logs, LogLevel logLevel)
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
                private LogLevel _logLevel;

                public MyLogger(List<string> logs, LogLevel logLevel)
                {
                    _logs = logs;
                    _logLevel = logLevel;
                }

                public bool IsEnabled(LogLevel logLevel)
                {
                    return logLevel >= _logLevel;
                }

                public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception,
                    Func<TState, Exception, string> formatter)
                {
                    _logs.Add(formatter(state, exception));
                }

                public IDisposable BeginScope<TState>(TState state)
                {
                    return null;
                }
            }
        }
    }
}