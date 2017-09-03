// Copyright (c) 2017 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Test.Chapter07Listings.EfClasses;

namespace Test.Chapter07Listings.EFCode.Configurations
{
    public class EmployeeShortFkConfig : IEntityTypeConfiguration<EmployeeShortFk>
    {
        public void Configure
            (EntityTypeBuilder<EmployeeShortFk> entity)
        {
            entity.HasOne(p => p.Manager)
                .WithOne()
                .HasForeignKey<EmployeeShortFk>(p => p.ManagerId);
        }
    }
}