// Copyright (c) 2016 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

namespace Test.Chapter07Listings.EfClasses
{
    public class Employee
    {
        public int EmployeeId { get; set; }

        public string Name { get; set; }

        //------------------------------
        //Relationships

        public int? ManagerEmployeeId { get; set; } //#A
        public Employee Manager { get; set; }
    }
    /************************************************
    #A This Foreign Key uses the <navigationalPropertyName><PrimaryKeyName> pattern
     * ***********************************************/
}