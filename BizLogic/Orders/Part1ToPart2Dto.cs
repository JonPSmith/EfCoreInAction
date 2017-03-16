// Copyright (c) 2017 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System.Collections.Immutable;
using DataLayer.EfClasses;

namespace BizLogic.Orders
{
    public class Part1ToPart2Dto
    {
        public IImmutableList<OrderLineItem> LineItems { get; private set; }

        public Order Order { get; private set; }

        public Part1ToPart2Dto(IImmutableList<OrderLineItem> lineItems, Order order)
        {
            LineItems = lineItems;
            Order = order;
        }
    }
}