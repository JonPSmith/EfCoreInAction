// Copyright (c) 2016 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using test.Helpers;
using Test.Helpers;
using Xunit;
using Xunit.Extensions.AssertExtensions;

namespace test.UnitTests.Tests
{
    public class Ch02_TestFileHelpers
    {
        [Fact]
        public void TestGetCallingAssemblyTopLevelDirOk()
        {
            //SETUP

            //ATTEMPT
            var testDir = TestData.GetCallingAssemblyTopLevelDir();

            //VERIFY
            testDir.EndsWith(@"EfCoreInAction\Test").ShouldBeTrue(testDir);
        }

        [Fact]
        public void TestGetTestProjectDirectoryOk()
        {
            //SETUP

            //ATTEMPT
            var testDir = TestData.GetTestDataDir();

            //VERIFY
            testDir.EndsWith(@"EfCoreInAction\Test\TestData").ShouldBeTrue(testDir);
        }

    }
}