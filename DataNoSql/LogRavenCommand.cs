// Copyright (c) 2017 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.Diagnostics;
using Microsoft.Extensions.Logging;

namespace DataNoSql
{
    public class LogRavenCommand : IDisposable
    {
        private readonly string _command;
        private readonly Stopwatch _stopwatch = new Stopwatch();
        private readonly ILogger _logger;

        public LogRavenCommand(string command, ILogger logger)
        {
            _command = command;
            _logger = logger;
            _stopwatch.Start();
        }

        public void Dispose()
        {
            _stopwatch.Stop();
            _logger.LogInformation(new EventId(1, RavenStore.RavenEventIdStart + ".Write"),
                $"Raven Command. Execute time = {_stopwatch.ElapsedMilliseconds} ms.\n" + _command);
        }
    }
}