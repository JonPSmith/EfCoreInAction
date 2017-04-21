// Copyright (c) 2017 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System.ComponentModel.DataAnnotations;

namespace Test.Chapter07Listings.EfClasses
{
    public class ContactInfo
    {
        public int ContactInfoId { get; set; }

        public string MobileNumber { get; set; }
        public string LandlineNumber { get; set; }

        [MaxLength(256)]
        [Required]
        public string EmailAddress { get; set; } //#A
    }
    /***********************************************************
    #A The Email address is used a a foreign key for the Person entity to link to this contact info
     * **********************************************************/
}