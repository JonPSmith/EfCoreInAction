// Copyright (c) 2017 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using DataLayer.EfClasses;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataLayer.EfCode.Configurations
{
    public static class LineItemConfig
    {
        public static void Configure
            (this EntityTypeBuilder<LineItem> entity)
        {
            entity.HasOne(p => p.ChosenBook)
                .WithMany()
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}