// Copyright (c) 2016 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

namespace DataLayer.EfClasses
{
    public class BookAuthor               
    {
        public int BookId { get; private set; }  //#A
        public int AuthorId { get; private set; }//#A
        public byte Order { get; internal set; }   

        //-----------------------------
        //Relationships

        public Book Book { get; internal set; }      
        public Author Author { get; internal set; }  
    }
    /************************************************************
    A# The primary key is make up of the two foreign keys
     * ********************************************************/

}