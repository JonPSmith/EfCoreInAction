// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

namespace Test.Chapter10Listings.EfClasses
{
    public class PostTag
    {
        public int PostId { get; set; }
        public int TagId { get; set; }

        //-------------------------------------------
        //relationships

        public Post Post { get; set; }
        public Tag Tag { get; set; }
    }
}