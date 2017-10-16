// Copyright (c) 2016 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("ServiceLayer")] //bit of cop out, I made the book generator easier!
namespace DataLayer.EfClasses
{
    public class Author
    {
        public const int NameLength = 100;

        public int AuthorId { get; internal set; }
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