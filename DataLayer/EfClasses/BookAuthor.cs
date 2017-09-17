// Copyright (c) 2016 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

namespace DataLayer.EfClasses
{
    public class BookAuthor               
    {

        public int BookId { get; private set; }
        public int AuthorId { get; private set; }
        public byte Order { get; private set; }   

        //-----------------------------
        //Relationships

        public Book Book { get; private set; }      
        public Author Author { get; private set; }

        internal BookAuthor() { }

        internal BookAuthor(Book book, Author author, byte order)
        {
            Book = book;
            Author = author;
            Order = order;
        }
    }
}