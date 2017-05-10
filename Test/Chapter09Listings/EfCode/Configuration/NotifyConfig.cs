// Copyright (c) 2016 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Test.Chapter09Listings.EfClasses;

namespace Test.Chapter09Listings.EfCode.Configuration
{
    public static class NotifyConfig
    {
        public static void Configure
        (this EntityTypeBuilder<NotifyEntity> entity)
        {
            entity
                .HasChangeTrackingStrategy(ChangeTrackingStrategy.ChangedNotifications);
        }
    }
}
