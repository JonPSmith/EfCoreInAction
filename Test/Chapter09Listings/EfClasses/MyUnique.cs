// // Copyright (c) 2017 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// // Licensed under MIT licence. See License.txt in the project root for license information.

using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Test.Chapter09Listings.EfClasses
{
    public class MyUnique
    {
        public int MyUniqueId { get; set; }

        public string UniqueString { get; set; }
    }
}