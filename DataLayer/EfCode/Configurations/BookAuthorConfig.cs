// Copyright (c) 2017 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using DataLayer.EfClasses;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataLayer.EfCode.Configurations
{
    internal static class BookAuthorConfig
    {
        public static void Configure
            (this EntityTypeBuilder<BookAuthor> entity)
        {
            entity.HasKey(p => new { p.BookId, p.AuthorId }); //#A

            //-----------------------------
            //Relationships

            //entity.HasOne(pt => pt.Book)
            //    .WithMany(p => p.AuthorsLink)
            //    .HasForeignKey(pt => pt.BookId);

            //entity.HasOne(pt => pt.Author)
            //    .WithMany(t => t.BooksLink)
            //    .HasForeignKey(pt => pt.AuthorId);
        }
        /*******************************************************
        #A This uses the names of the Book and Author primary keys to form its own, composite key
         * ****************************************************/
    }
}