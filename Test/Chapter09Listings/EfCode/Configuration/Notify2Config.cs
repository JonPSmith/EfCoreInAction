// Copyright (c) 2016 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Test.Chapter09Listings.EfClasses;

namespace Test.Chapter09Listings.EfCode.Configuration
{
    public static class Notify2Config
    {
        public static void Configure
        (this EntityTypeBuilder<Notify2Entity> entity)
        {
            entity
                .HasChangeTrackingStrategy(ChangeTrackingStrategy.ChangingAndChangedNotifications);
        }
    }
}
