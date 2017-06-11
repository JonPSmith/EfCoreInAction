// Copyright (c) 2016 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

namespace Test.Chapter09Listings.WipeDbClasses
{
    public class SelfRef
    {
        public int SelfRefId { get; set; }

        public string Name { get; set; }

        //------------------------------
        //Relationships

        public int? SelfRefEmployeeId { get; set; }
        public SelfRef Manager { get; set; }
    }
}