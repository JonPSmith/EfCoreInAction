// Copyright (c) 2016 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

namespace Test.Chapter09Listings.EfClasses
{
    public class Hierarchical
    {
        public int HierarchicalId { get; set; }

        public string Name { get; set; }

        //------------------------------
        //Relationships

        public int? HierarchicalEmployeeId { get; set; }
        public Hierarchical Manager { get; set; }
    }
}