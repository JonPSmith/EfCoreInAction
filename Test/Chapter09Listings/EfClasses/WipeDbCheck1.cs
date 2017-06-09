// // Copyright (c) 2017 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// // Licensed under MIT licence. See License.txt in the project root for license information.

namespace Test.Chapter09Listings.EfClasses
{
    public class WipeDbCheck1
    {
        public int Id { get; set; }

        public WipeDbCheck2 Dependant { get; set; }

        public int WipeDbCheck2Id { get; set; }
    }
}