// Copyright (c) 2017 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.
using System.ComponentModel.DataAnnotations;

namespace Test.Chapter07Listings.SplitOwnClasses
{
    public class BookSummary
    {
        public int BookId { get; set; }

        [Required] //#A
        [MaxLength(256)] //#B
        public string Title { get; set; }

        public string AuthorsString { get; set; }

        public BookDetail Details { get; set; }
    }
}