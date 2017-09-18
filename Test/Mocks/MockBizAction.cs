// Copyright (c) 2017 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using BizLogic.GenericInterfaces;
using DataLayer.EfClasses;
using DataLayer.EfCode;

namespace test.Mocks
{
    public interface IMockBizAction : IBizAction<int, string> { }

    public class MockBizAction : BizActionErrors, IMockBizAction
    {

        private readonly EfCoreContext _context;

        public MockBizAction(EfCoreContext context)
        {
            _context = context;
        }

        public string Action(int intIn)
        {
            if (intIn < 0)
                AddError("The intInt is less than zero");

            _context.Authors.Add(new Author("MockBizAction"));

            return intIn.ToString();
        }
    }
}