// // Copyright (c) 2017 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// // Licensed under MIT licence. See License.txt in the project root for license information.
// 
namespace Test.Chapter09Listings.WipeDbClasses
{
    public class Many
    {
        public int Id { get; set; }

        public int SelfRefId { get; set; }

        public SelfRef SelfRef { get; set; }
    }
}