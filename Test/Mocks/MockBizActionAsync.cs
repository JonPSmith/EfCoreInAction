// Copyright (c) 2017 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System.Threading.Tasks;
using BizLogic.GenericInterfaces;
using DataLayer.EfClasses;
using DataLayer.EfCode;

namespace test.Mocks
{
    public interface IMockBizActionAsync : IBizActionAsync<int, string> { }

    public class MockBizActionAsync : BizActionErrors, IMockBizActionAsync
    {

        private readonly EfCoreContext _context;

        public MockBizActionAsync(EfCoreContext context)
        {
            _context = context;
        }

        public Task<string> ActionAsync(int intIn)
        {
            if (intIn < 0)
                AddError("The intInt is less than zero");

            _context.Authors.Add(new Author("MockBizAction"));

            return Task.FromResult(intIn.ToString());
        }
    }
}