// Copyright (c) 2017 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using DataLayer.EfClasses;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataLayer.EfCode.Configurations
{
    public static class BookAuthorConfig
    {
        public static void Configure
            (this EntityTypeBuilder<BookAuthor> entity)
        {
            entity.HasKey(p => new { p.BookId, p.AuthorId }); //#A

            //-----------------------------
            //Relationships

            entity.HasOne(pt => pt.Book)        //#C
                .WithMany(p => p.AuthorsLink)   //#C
                .HasForeignKey(pt => pt.BookId);//#C

            entity.HasOne(pt => pt.Author)        //#C
                .WithMany(t => t.BooksLink)       //#C
                .HasForeignKey(pt => pt.AuthorId);//#C
        }
        /*******************************************************
        #A This uses the names of the Book and Author primary keys to form its own, composite key
        #B I configure the one-to-many relationship from the Book to BookAuthor entity class
        #C ... and I then configure the other one-to-many relationship from the Author to the BookAuthor entity class
         * ****************************************************/
    }
}