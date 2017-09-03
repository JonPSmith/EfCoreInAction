// Copyright (c) 2017 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Test.Chapter08Listings.EfClasses;

namespace Test.Chapter08Listings.EfCode.Configuration
{
    public class InMemoryConfig : IEntityTypeConfiguration<InMemoryTest>
    {
        public void Configure
            (EntityTypeBuilder<InMemoryTest> entity)
        {
            entity
                .ForSqlServerIsMemoryOptimized();
        }
        /**************************************************************
         * 
         * ***********************************************************/
    }
}
