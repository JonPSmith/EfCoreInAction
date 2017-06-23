// Copyright (c) 2017 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using DataLayer.EfCode;
using Microsoft.EntityFrameworkCore;

namespace Test.Chapter11Listings
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var db = new EfCoreContext(null)) //#A
            {
                db.Database.Migrate(); //#B
            }
            //... then start the rest of your code
        }
    }
    /************************************************************
    #A in a console application the setting up of the appliction's DbContext is done in its OnConfiguring method
    #B Calling the Migrate method will apply any outstanding migrations to the database it is attached to. If there are no outstanding migrations then it does nothing
     * **********************************************************/
}