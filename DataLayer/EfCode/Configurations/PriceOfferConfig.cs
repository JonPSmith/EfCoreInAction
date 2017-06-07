// Copyright (c) 2017 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using DataLayer.EfClasses;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataLayer.EfCode.Configurations
{
    public static class PriceOfferConfig
    {
        public static void Configure
            (this EntityTypeBuilder<PriceOffer> entity)
        {
            entity.Property(p => p.NewPrice)
                .HasColumnType("decimal(9,2)");
        }
    }
}