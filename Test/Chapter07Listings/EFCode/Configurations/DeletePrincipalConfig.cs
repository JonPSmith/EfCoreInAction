// Copyright (c) 2017 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Test.Chapter07Listings.EfClasses;

namespace Test.Chapter07Listings.EFCode.Configurations
{
    public class DeletePrincipalConfig : IEntityTypeConfiguration<DeletePrincipal>
    {
        public void Configure
            (EntityTypeBuilder<DeletePrincipal> entity)
        {
            entity.HasOne(p => p.DependentSetNull)
                .WithOne()
                .HasForeignKey<DeleteDependentSetNull>(p => p.DeletePrincipalId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(p => p.DependentRestrict)
                .WithOne()
                .HasForeignKey<DeleteDependentRestrict>(p => p.DeletePrincipalId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(p => p.DependentCascade)
                .WithOne()
                .HasForeignKey<DeleteDependentCascade>(p => p.DeletePrincipalId)
                .OnDelete(DeleteBehavior.Cascade);
        }

    }
}