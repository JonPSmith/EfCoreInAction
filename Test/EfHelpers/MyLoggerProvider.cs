// Copyright (c) 2016 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace test.EfHelpers
{
    public class MyLoggerProvider : ILoggerProvider //#A
    {
        private readonly List<string> _logs; //#B

        public MyLoggerProvider(List<string> logs) //#B
        {
            _logs = logs;
        }

        public ILogger CreateLogger(string categoryName) //#C
        {
            return new MyLogger(_logs);
        }

        public void Dispose()
        { }

        private class MyLogger : ILogger //#D
        {
            private readonly List<string> _logs; //#E

            public MyLogger(List<string> logs) //#E
            {
                _logs = logs;
            }

            public bool IsEnabled(LogLevel logLevel) //#F
            {
                return logLevel >= LogLevel.Information;
            }

            public void Log<TState>(LogLevel logLevel, EventId eventId, //#G
                TState state, Exception exception, 
                Func<TState, Exception, string> formatter)
            {
                //_logs.Add(formatter(state, exception)); //#H
                _logs.Add($"{logLevel}: {state.ToString()}" +        //#I
                    (exception == null                    //#I
                       ? ""                               //#I
                       : ", Exception = \n" + exception));//#I
                Console.WriteLine(formatter(state, exception));//#J
            }

            public IDisposable BeginScope<TState>(TState state) //#K
            {
                return null;
            }
        }
    }
    /******************************************************************
    #A I need to provide a ILoggerProvider, which has a method to create a logger
    #B This logger logs to a List<string> so we need to hold a reference to it
    #C This method is called whenever a new logger is created. The category name is the name of the logger, which you could use to select different loggers
    #D This in my logger, which conforms to the ILogger interface
    #E I pass in the List<string> that my logger will log to
    #F Well behaved logging callers, such as EF Core, can check this to see whether they should log or not
    #G This is the inner method that is called for all logging. Normally the developer will call something like LogInformation, which then calls this method
    #H Microsoft say the the standard way to format a log is to use the formatter, but its not very useful. see  https://github.com/aspnet/Logging/issues/442
    #I Here is my formatted version
    #J I also write to the console. When using Resharper to run the unit test in debug mode you get a window, so you can see the console output
    #K This is for scoped logging. You can find out about this at https://msdn.microsoft.com/en-us/magazine/mt694089.aspx
     * ****************************************************************/
}