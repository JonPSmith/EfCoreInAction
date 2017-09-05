// Copyright (c) 2017 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Test.Chapter07Listings.SplitOwnClasses;

namespace Test.Chapter07Listings.EFCode.Configurations
{
    public class OrderInfoConfig : IEntityTypeConfiguration<OrderInfo>
    {
        public void Configure
            (EntityTypeBuilder<OrderInfo> entity)
        {
            entity
                .OwnsOne(p => p.BillingAddress);
            entity
                .OwnsOne(p => p.DeliveryAddress);
        }
        /*******************************************************************
         * ********************************************************************/
    }
}