// Copyright (c) 2016 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DataLayer.EfClasses
{
    public class Author
    {
        public const int NameLength = 100;

        public int AuthorId { get; private set; }
        [Required]
        [MaxLength(NameLength)]
        public string Name { get; private set; }

        //------------------------------
        //Relationships

        public ICollection<BookAuthor> 
            BooksLink { get; set; }

        //Needed by EF Core
        internal Author () { }

        public Author(string name)
        {
            Name = name;
        }
    }

}