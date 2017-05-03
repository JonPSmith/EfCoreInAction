// Copyright (c) 2017 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Test.Chapter07Listings.EfClasses;

namespace Test.Chapter07Listings.EFCode.Configurations
{
    public static class Ch07BookConfig
    {
        public static void Configure
            (this EntityTypeBuilder<Ch07Book> entity)
        {
            entity.HasKey(p => p.BookId);

            //see https://github.com/aspnet/EntityFramework/issues/6674
            entity.Metadata //#A
                .FindNavigation(nameof(Ch07Book.Reviews)) //#B
                .SetPropertyAccessMode
                    (PropertyAccessMode.Field); //#C
        }
        /******************************************************
        #A Using the MetaData for this entity class I can access some of the deeper features of the entity class
        #B This finds the navigation property using the name of the property
        #C This sets the access mode so that EF Core will ONLY read/write to the backing field
         * ****************************************************/
    }
}