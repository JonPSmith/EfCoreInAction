// // Copyright (c) 2017 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// // Licensed under MIT licence. See License.txt in the project root for license information.

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Test.Chapter10Listings.EfClasses;

namespace Test.Chapter10Listings.EfCode.Configuration
{
    public static class MyUniqueConfig
    {
        public static void Configure
            (this EntityTypeBuilder<MyUnique> entity)
        {
            entity.HasIndex(p => p.UniqueString)
                .IsUnique()
                .HasName("UniqueError_MyUnique_UniqueString");
        }
    }
}