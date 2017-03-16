// Copyright (c) 2017 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System.Linq;
using DataLayer.EfCode;
using ServiceLayer.BizRunners;
using test.EfHelpers;
using test.Mocks;
using Xunit;
using Xunit.Extensions.AssertExtensions;

namespace test.UnitTests.ServiceLayer
{
    public class Ch04_RunnerWriteDb
    {
        [Theory]
        [InlineData(1, false)]
        [InlineData(-1, true)]
        public void RunAction(int input, bool hasErrors)
        {
            //SETUP
            var options = EfInMemory.CreateNewContextOptions();
            using (var context = new EfCoreContext(options))
            {
                var action = new MockBizAction(context);
                var runner = new RunnerWriteDb<int,string>(action, context);

                //ATTEMPT
                var output = runner.RunAction(input);

                //VERIFY
                output.ShouldEqual(input.ToString());
                runner.HasErrors.ShouldEqual(hasErrors);
                context.Authors.Count().ShouldEqual(hasErrors ? 0 : 1);
            }
        }
    }
}