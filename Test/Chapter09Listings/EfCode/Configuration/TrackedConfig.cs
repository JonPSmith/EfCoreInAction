// Copyright (c) 2016 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Test.Chapter09Listings.EfClasses;

namespace Test.Chapter09Listings.EfCode.Configuration
{
    public static class TrackedConfig
    {
        public static void Configure
        (this EntityTypeBuilder<TrackedEntity> entity)
        {
            //entity.HasOne(p => p.OneToOne)
            //    .WithOne()
            //    .HasForeignKey<TrackedOne>(x => x.TrackedEntityId);

            //entity.HasMany(p => p.Collection)
            //    .WithOne()
            //    .HasForeignKey(x => x.TrackedEntityId);
        }
    }
}
