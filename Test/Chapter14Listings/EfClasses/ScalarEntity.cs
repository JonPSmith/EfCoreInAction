// // Copyright (c) 2017 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// // Licensed under MIT licence. See License.txt in the project root for license information.

using System.ComponentModel.DataAnnotations;

namespace Test.Chapter14Listings.EfClasses
{
    public class ScalarEntity
    {
        public int Id { get; set; }

        public string StringMax { get; set; }

        [Required]
        [MaxLength(20)]
        public string String20 { get; set; }

        public string StringAscii { get; set; }
    }
}