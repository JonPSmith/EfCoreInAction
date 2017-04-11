// Copyright (c) 2016 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System.ComponentModel.DataAnnotations.Schema;

namespace Test.Chapter07Listings.EfClasses
{
    public class EmployeeShortFk
    {
        public int EmployeeShortFkId { get; set; }

        public string Name { get; set; }

        //------------------------------
        //Relationships

        public int? ManagerId { get; set; }
        [ForeignKey(nameof(ManagerId))]      //#A
        public EmployeeShortFk Manager { get; set; }
    }
    /************************************************
    #A This Data Annotation defines which property is the Foreign key for the 'Manager' navigational property
     * ***********************************************/
}