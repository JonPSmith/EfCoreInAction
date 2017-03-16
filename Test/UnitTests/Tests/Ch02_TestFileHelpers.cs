// Copyright (c) 2016 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using test.Helpers;
using Xunit;
using Xunit.Extensions.AssertExtensions;

namespace test.UnitTests.Tests
{
    public class Ch02_TestFileHelpers
    {
        [Fact]
        public void TestGetSolutionDirectoryOk()
        {
            //SETUP

            //ATTEMPT
            var testDir = TestFileHelpers.GetSolutionDirectory();

            //VERIFY
            testDir.EndsWith(@"EfCoreInAction").ShouldBeTrue(testDir);
        }

        [Fact]
        public void TestGetTestProjectDirectoryOk()
        {
            //SETUP

            //ATTEMPT
            var testDir = TestFileHelpers.GetTestDataFileDirectory();

            //VERIFY
            testDir.EndsWith(@"EfCoreInAction\test\TestData").ShouldBeTrue(testDir);
        }

    }
}