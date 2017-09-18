// Copyright (c) 2017 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using BizLogic.GenericInterfaces;
using DataLayer.EfClasses;
using DataLayer.EfCode;

namespace test.Mocks
{
    public interface IMockBizActionPart2 : IBizAction<TransactBizActionDto, TransactBizActionDto> { }

    public class MockBizActionPart2 : BizActionErrors, IMockBizActionPart2
    {

        private readonly EfCoreContext _context;

        public MockBizActionPart2(EfCoreContext context)
        {
            _context = context;
        }

        public TransactBizActionDto Action(TransactBizActionDto dto)
        {
            if (dto.Mode == MockBizActionTransact2Modes.BizErrorPart2)
                AddError("Failed in Part2");
            if (dto.Mode == MockBizActionTransact2Modes.ThrowExceptionPart2)
                throw new InvalidOperationException("I have thrown an exception.");

            _context.Authors.Add(new Author("Part2"));

            return dto;
        }
    }
}