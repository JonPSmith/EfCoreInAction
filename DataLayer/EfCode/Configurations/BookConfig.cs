// Copyright (c) 2017 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using DataLayer.EfClasses;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataLayer.EfCode.Configurations
{
    public class BookConfig : IEntityTypeConfiguration<Book>
    {
        public void Configure
            (EntityTypeBuilder<Book> entity)
        {
            entity.Property(p => p.PublishedOn)
                .HasColumnType("date");        

            entity.Property(p => p.ActualPrice)
                .HasColumnType("decimal(9,2)");

            entity.Property(x => x.ImageUrl)
                .IsUnicode(false);

            entity.HasIndex(x => x.PublishedOn);
            entity.HasIndex(x => x.AverageVotes);
            entity.HasIndex(x => x.ActualPrice);

            //Model-level query filter
            entity
                .HasQueryFilter(p => !p.SoftDeleted);

            //----------------------------
            //relationships

            entity.HasMany(p => p.Reviews)  
                .WithOne()                     
                .HasForeignKey(p => p.BookId);

            //see https://github.com/aspnet/EntityFramework/issues/6674
            entity.Metadata //#A
                .FindNavigation(nameof(Book.Reviews))
                .SetPropertyAccessMode
                (PropertyAccessMode.Field); 

            //see https://github.com/aspnet/EntityFramework/issues/6674
            var authorsLinkNav = entity.Metadata.FindNavigation(nameof(Book.AuthorsLink));
            authorsLinkNav.SetField("_bookAuthors");
            authorsLinkNav.SetPropertyAccessMode
                (PropertyAccessMode.Field); 
        }
    }
}