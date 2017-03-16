// Copyright (c) 2017 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.
namespace test.Mocks
{
    public enum MockBizActionTransact2Modes { Ok, BizErrorPart1, BizErrorPart2, ThrowExceptionPart2}

    public class TransactBizActionDto
    {
        public TransactBizActionDto(MockBizActionTransact2Modes mode)
        {
            Mode = mode;
        }

        public MockBizActionTransact2Modes Mode { get; private set; }
    }
}