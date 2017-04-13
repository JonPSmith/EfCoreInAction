// Copyright (c) 2017 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Test.Chapter07Listings.EfClasses;

namespace Test.Chapter07Listings.EFCode.Configurations
{
    public static class BookBaseConfig
    {
        public static void Configure
            (this EntityTypeBuilder<BookBase> entity)
        {
            entity.HasDiscriminator(b => b.HasPromotion)
                .HasValue<BookBase>(false)
                .HasValue<BookPromote>(true);

            //entity.Property(b => b.HasPromotion)
            //    .Metadata.IsReadOnlyAfterSave = false;

            entity.Property<decimal>("OrgPrice")
                .HasField("_orgPrice");
        }

    }
}