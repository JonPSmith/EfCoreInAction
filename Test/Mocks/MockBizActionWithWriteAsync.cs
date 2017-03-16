// Copyright (c) 2017 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BizLogic.GenericInterfaces;
using DataLayer.EfClasses;
using DataLayer.EfCode;

namespace test.Mocks
{
    public interface IMockBizActionWithWriteAsync : IBizActionAsync<MockBizActionWithWriteModes, string> { }


    public class MockBizActionWithWriteAsync : BizActionErrors, IMockBizActionWithWriteAsync
    {

        private readonly EfCoreContext _context;

        public MockBizActionWithWriteAsync(EfCoreContext context)
        {
            _context = context;
        }

        public async Task<string> ActionAsync(MockBizActionWithWriteModes mode)
        {
            if (mode == MockBizActionWithWriteModes.BizError)
                AddError("There is a biz error.");

            var numBooks = (short) (mode == MockBizActionWithWriteModes.SaveChangesError ? 200 : 1);

            var order = new Order
            {
                CustomerName = Guid.NewGuid(),
                LineItems = new List<LineItem>
                    {
                        new LineItem
                        {
                            BookId = _context.Books.First().BookId,
                            LineNum = 1,
                            BookPrice = 123,
                            NumBooks = numBooks
                        }
                    }
            };

            _context.Orders.Add(order);

            return mode.ToString();
        }

    }
}