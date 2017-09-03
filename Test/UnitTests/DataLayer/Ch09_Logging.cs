// Copyright (c) 2016 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using EfCoreInAction;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Logging;
using test.EfHelpers;
using Test.Chapter09Listings.EfClasses;
using Test.Chapter09Listings.EfCode;
using Xunit;
using Xunit.Abstractions;
using Xunit.Extensions.AssertExtensions;

namespace test.UnitTests.DataLayer
{
    public class Ch09_Logging
    {
        private readonly ITestOutputHelper _output;

        public Ch09_Logging(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void TestMyLoggerProviderInformationOk()
        {
            //SETUP
            var logs = new List<string>(); 
            ILoggerFactory loggerFactory = new LoggerFactory();

            //ATTEMPT
            loggerFactory.AddProvider(new MyLoggerProvider(logs));

            ILogger logger = loggerFactory.CreateLogger<Program>();
            logger.LogInformation("Unit Test");

            //VERIFY
            logs.Count.ShouldEqual(1);
            logs.First().ShouldEqual("Information: Unit Test");
        }

        [Fact]
        public void TestMyLoggerProviderDebugOk()
        {
            //SETUP
            var logs = new List<string>();
            ILoggerFactory loggerFactory = new LoggerFactory();

            //ATTEMPT
            loggerFactory.AddProvider(new MyLoggerProvider(logs));

            ILogger logger = loggerFactory.CreateLogger<Program>();
            logger.LogDebug("Unit Test");

            //VERIFY
            logs.Count.ShouldEqual(0);
        }

        [Fact]
        public void TestMyLoggerProviderExceptionOk()
        {
            //SETUP
            var logs = new List<string>();
            ILoggerFactory loggerFactory = new LoggerFactory();

            //ATTEMPT
            loggerFactory.AddProvider(new MyLoggerProvider(logs));

            ILogger logger = loggerFactory.CreateLogger<Program>();
            try
            {
                throw new Exception("An exception");
            }
            catch (Exception e)
            {
                logger.LogCritical(new EventId(1), e, "Unit Test");
            }

            //VERIFY
            logs.Count.ShouldEqual(1);
            logs.First().StartsWith("Critical: Unit Test, Exception = \nSystem.Exception: An exception").ShouldBeTrue();
        }

        [Theory]
        [InlineData(LogLevel.Debug, false)]
        [InlineData(LogLevel.Information, true)]
        public void TestMyLoggerIsEnabledOk(LogLevel logLevel, bool isEnabled)
        {
            //SETUP
            var logs = new List<string>();
            ILoggerFactory loggerFactory = new LoggerFactory();
            loggerFactory.AddProvider(new MyLoggerProvider(logs));

            //ATTEMPT
            ILogger logger = loggerFactory.CreateLogger<Program>();


            //VERIFY
            logger.IsEnabled(logLevel).ShouldEqual(isEnabled);
        }

        [Fact]
        public void TestMyLoggerProviderWithFilterOk()
        {
            //SETUP
            var logs = new List<string>();
            ILoggerFactory loggerFactory = new LoggerFactory();

            //ATTEMPT
            loggerFactory
                .WithFilter(new FilterLoggerSettings
                {
                    { "UnitTest1", LogLevel.Warning },
                    { "UnitTest2", LogLevel.Error }
                })
                .AddProvider(new MyLoggerProvider(logs));

            ILogger logger1 = loggerFactory.CreateLogger("UnitTest1");
            logger1.LogWarning("Unit Test1");
            ILogger logger2 = loggerFactory.CreateLogger("UnitTest2");
            logger2.LogWarning("Unit Test2");

            //VERIFY
            logs.Count.ShouldEqual(1);
            logs.First().ShouldEqual("Warning: Unit Test1");
        }


        [Fact]
        public void TestLogEFCoreOk()
        {
            //SETUP
            var logs = new List<string>();
            var options = SqliteInMemory.CreateOptions<Chapter09DbContext>();

            using (var context = new Chapter09DbContext(options))
            {
                context.Database.EnsureCreated();

                //ATTEMPT
                var loggerFactory = context.GetService<ILoggerFactory>();
                loggerFactory.AddProvider(new MyLoggerProvider(logs));

                context.Add(new MyEntity());
                context.SaveChanges();

                //VERIFY
                logs.First().ShouldEqual("Information: Executed DbCommand (0ms) [Parameters=[], CommandType='Text', CommandTimeout='30']\r\nPRAGMA foreign_keys=ON;");
            }
        }

        [Fact]
        public void TestLogEFCoreWithFilterOk()
        {
            //SETUP
            var logs = new List<string>();
            var options = SqliteInMemory.CreateOptions<Chapter09DbContext>();

            using (var context = new Chapter09DbContext(options))
            {
                context.Database.EnsureCreated();

                //ATTEMPT
                var loggerFactory = context.GetService<ILoggerFactory>();
                loggerFactory
                    .WithFilter(new FilterLoggerSettings
                    {
                        { "Microsoft", LogLevel.Error }
                    })
                    .AddProvider(new MyLoggerProvider(logs));

                context.Add(new MyEntity());
                context.SaveChanges();

                //VERIFY
                foreach (var log in logs)
                {
                    _output.WriteLine(log);
                }
                logs.Count.ShouldEqual(0);
            }
        }
    }
}
