// Copyright (c) 2017 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

namespace Test.Chapter10Listings.EfClasses
{
    public class BookAuthorDdd               
    {
        public int BookId { get; set; }  
        public int AuthorId { get; set; }
        public byte Order { get; set; }   

        //-----------------------------
        //Relationships

        public BookDdd Book { get; set; }      
        public AuthorDdd Author { get; set; }

        internal BookAuthorDdd()
        {
        }
    }
}