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
            entity.HasIndex(x => x.PublishedOn);

            entity.Property(p => p.PublishedOn)
                .HasColumnType("date");        

            entity.Property(p => p.Price)      
                .HasColumnType("decimal(9,2)");

            entity.Property(x => x.ImageUrl)
                .IsUnicode(false);

            //Model-level query filter

            entity
                .HasQueryFilter(p => !p.SoftDeleted);

            //----------------------------
            //relationships

            entity.HasOne(p => p.Promotion) //#A
                .WithOne() //#A
                .HasForeignKey<PriceOffer>(p => p.BookId); //#A

            entity.HasMany(p => p.Reviews)     //#B
                .WithOne()                     //#B
                .HasForeignKey(p => p.BookId); //#B
        }
    }
    /************************************************************
    #A This defines the One-to-One relationship to the promotion that a book can optionally have. The foreign key is in the PriceOffer
    #B This defines the One-to-Many relationship, with a book having zero to many reviews
     * ***********************************************************/
}