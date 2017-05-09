// Copyright (c) 2017 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Test.Chapter08Listings.EfClasses
{
    public class Employee
    {
        public int ConcurrentSalaryId { get; set; }

        public string Name { get; set; }

        [ConcurrencyCheck]
        public int Salary { get; set; } //#A

        public void UpdateSalaryDisconnected //#B
            (DbContext context, 
             int orgSalary, int newSalary)
        {
            Salary = newSalary; //#C
            context.Entry(this).Property(p => p.Salary) //#D
                .OriginalValue = orgSalary; //#D
        }
    }
    /************************************************************************
    #A The Salery property is set as a concurrency token by the attribute [ConcurrencyCheck]
    #B This method is used to update the Salary in a disconnected state
    #C I set the Salary to the new value
    #D I set the OriginalValue, which holds the data read from the database, to the original value that was shown to the user in the first part of the update
    * ********************************************************************/
}