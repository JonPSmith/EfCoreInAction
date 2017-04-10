// Copyright (c) 2017 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using DataLayer.EfClasses;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataLayer.EfCode.Configurations
{
    internal static class BookConfig
    {
        public static void Configure
            (this EntityTypeBuilder<Book> entity)
        {
            entity.HasIndex(x => x.PublishedOn);//Add a non-unique index to the publish date

            entity.Property(p => p.PublishedOn) //#A
                .HasColumnType("date"); //#A

            entity.Property(p => p.Price) //#B
                .HasColumnType("decimal(9,2)"); //#B

            entity.Property(x => x.ImageUrl) //#C
                .IsUnicode(false); //#C

            //----------------------------
            //relationships

            //entity.HasOne(p => p.Promotion)       //#D
            //    .WithOne();                       //#D

            //entity.HasMany(p => p.Reviews)        //#E
            //    .WithOne()                        //#E
            //    .HasForeignKey(p => p.BookId);    //#E

            entity.HasMany(p => p.AuthorsLink)
                .WithOne(p => p.Book);
        }
    }
    /************************************************************
    #A Setting the database type to just store the date, instead of the nromal datetime2(7)
    #B Setting a smaller precision and scale or (9,2) instead of the default (18,2)
    #C The ImageUrl does not have a Unicode characters in it so we select a smaller string type, varchar
    #D This defines the One-to-ZeroOrOne relationship to the promotion that a book can optionally have
    #E This defines the One-to-Many relationship, with a book having zero to many reviews
     * ***********************************************************/
}