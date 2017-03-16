// Copyright (c) 2017 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;

namespace BizLogic.Orders
{
    public class PlaceOrderInDto
    {
        public bool AcceptTAndCs { get; private set; }

        public Guid UserId { get; private set; }

        public IImmutableList<OrderLineItem> LineItems { get; private set; }

        public PlaceOrderInDto(bool acceptTAndCs, Guid userId, IImmutableList<OrderLineItem> lineItems)
        {
            AcceptTAndCs = acceptTAndCs;
            UserId = userId;
            LineItems = lineItems;
        }
    }
}