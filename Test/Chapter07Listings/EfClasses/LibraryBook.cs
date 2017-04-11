// Copyright (c) 2016 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.


using System.ComponentModel.DataAnnotations;

namespace Test.Chapter07Listings.EfClasses
{
    public class LibraryBook
    {
        public int LibraryBookId { get; set; }

        public string BookRef { get; set; }

        //-----------------------------------
        //Relationships
    
        [MaxLength(256)]
        [Required]
        public string LibrarianUserId { get; set; }  
        public Person Librarian { get; set; }

        public int? OnLoanToPersonId { get; set; }
        public Person OnLoanTo { get; set; }
    }
}