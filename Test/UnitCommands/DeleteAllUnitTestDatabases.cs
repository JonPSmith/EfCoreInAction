// Copyright (c) 2017 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using test.Attributes;
using test.EfHelpers;
using Xunit.Abstractions;

namespace test.UnitCommands
{
    public class DeleteAllUnitTestDatabases
    {
        private readonly ITestOutputHelper _output;

        public DeleteAllUnitTestDatabases(ITestOutputHelper output)
        {
            _output = output;
        }

        //Run this method to wipe ALL the test database for the current branch
        //You need to run it in debug mode - that stops it being run when you "run all" unit tests
        [RunnableInDebugOnly]
        public void DeleteAllTestDatabasesOk()
        {
            var numDeleted = SqlDatabaseHelpers.DeleteAllUnitTestBranchDatabases();
            _output.WriteLine("This deleted {0} databases.", numDeleted);
        }
    }
}