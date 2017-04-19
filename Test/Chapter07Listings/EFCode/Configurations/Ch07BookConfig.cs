// Copyright (c) 2017 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using DataLayer.EfClasses;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Test.Chapter07Listings.EfClasses;

namespace Test.Chapter07Listings.EFCode.Configurations
{
    public static class Ch07BookConfig
    {
        public static void Configure
            (this EntityTypeBuilder<Ch07Book> entity)
        {
            entity.HasKey(p => p.BookId);

            //entity.Property(p => p.CachedPrice)
            //    .HasField("_cachedPrice");

            entity.Property<decimal>("NormalPrice")
                .HasField("_normalPrice");

            entity.HasOne(p => p.Promotion)
                .WithOne()
                .HasForeignKey<PriceOffer>(p => p.BookId);

            entity.Metadata
                .FindNavigation(nameof(Ch07Book.Promotion))
                .SetPropertyAccessMode(PropertyAccessMode.Field);
        }

    }
}