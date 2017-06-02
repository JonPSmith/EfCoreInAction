// // Copyright (c) 2017 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// // Licensed under MIT licence. See License.txt in the project root for license information.

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Test.Chapter10Listings.EfClasses;

namespace Test.Chapter10Listings.EfCode.Configuration
{
    public static class BookDddConfig
    {
        public static void Configure
            (this EntityTypeBuilder<BookDdd> entity)
        {
            entity.HasKey(k => k.BookId);

            //see https://github.com/aspnet/EntityFramework/issues/6674
            entity.Metadata
                .FindNavigation(nameof(BookDdd.Reviews))
                .SetPropertyAccessMode
                (PropertyAccessMode.Field);

            entity.Metadata
                .FindNavigation(nameof(BookDdd.AuthorsLink))
                .SetPropertyAccessMode
                (PropertyAccessMode.Field);
        }
    }
}