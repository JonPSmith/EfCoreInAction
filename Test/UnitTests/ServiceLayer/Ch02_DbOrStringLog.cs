// Copyright (c) 2016 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Data;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using ServiceLayer.Logger;
using Xunit;
using Xunit.Extensions.AssertExtensions;

namespace test.UnitTests.ServiceLayer
{
    public class Ch02_DbOrStringLog
    {
        [Fact]
        public void NonDbLog()
        {
            //SETUP

            //ATTEMPT
            var log = new DbOrStringLog(LogLevel.Error, "Hello world", null);

            //VERIFY
            log.LogLevel.ShouldEqual(LogLevel.Error);
            log.IsDb.ShouldEqual(false);
            log.LogHeader.ShouldEqual("Hello world");
            log.LogMain.ShouldEqual("Hello world");
        }

        [Fact]
        public void DbLogNoParams()
        {
            //SETUP
            var dbComm = new DbCommandLogData("Command Text", CommandType.Text, 999, new List<DbParameterLogData>(), 123);

            //ATTEMPT
            var log = new DbOrStringLog(LogLevel.Information, "Hello world", dbComm);

            //VERIFY
            log.LogLevel.ShouldEqual(LogLevel.Information);
            log.IsDb.ShouldEqual(true);
            log.LogHeader.ShouldEqual("DBCommand (123ms): Command Text");
            log.LogMain.ShouldEqual("Command Text");
        }

        [Fact]
        public void DbLogWithParams()
        {
            //SETUP
            var dbParams = new List<DbParameterLogData>()
            {
                new DbParameterLogData("Param1", 1, true, ParameterDirection.Input, DbType.Byte, false, 1, 1, 1),
                new DbParameterLogData("Param2", 20, true, ParameterDirection.Output, DbType.Byte, false, 1, 1, 1)
            };
            var dbComm = new DbCommandLogData("Command Text\nwith linefeed", CommandType.Text, 999, dbParams, 123);

            //ATTEMPT
            var log = new DbOrStringLog(LogLevel.Information, "Hello world", dbComm);

            //VERIFY
            log.LogLevel.ShouldEqual(LogLevel.Information);
            log.IsDb.ShouldEqual(true);
            log.LogHeader.ShouldEqual("DBCommand (123ms): Command Text\nwith linefeed");
            log.LogMain.ShouldEqual("-- Param (Input): Param1 = 1\n"+
                "-- Param (Output): Param2 = 20\n"+
                "Command Text\nwith linefeed");
        }
    }
}