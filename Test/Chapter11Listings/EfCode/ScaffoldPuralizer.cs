// Copyright (c) 2017 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.DependencyInjection;

namespace Test.Chapter11Listings.EfCode
{
    public class MyDesignTimeServices //#A
        : IDesignTimeServices         //#A
    {
        public void ConfigureDesignTimeServices//#B
            (IServiceCollection services)      //#B
        {
            services.AddSingleton                  //#C
                <IPluralizer, ScaffoldPuralizer>();//#C
        }
    }

    public class ScaffoldPuralizer : IPluralizer //#D
    {
        public string Pluralize(string name)//#E
        {                                   //#E
            return Inflector.Inflector      //#E
                .Pluralize(name) ?? name;   //#E
        }                                   //#E

        public string Singularize(string name)//#F
        {                                     //#F
            return Inflector.Inflector        //#F
                .Singularize(name) ?? name;   //#F
        }                                     //#F
    }
    /*************************************************************************
    #A The design time methods look for a class which implements the IDesignTimeServices interface. If one exists it will call its method ConfigureDesignTimeServices
    #B This method is here to allow other services to be added to the design time services
    #C I add my puralizer, which will replace the default, noop, puralizer
    #D This is my implementation of the IPluralizer interface. In this case I'm only really interested in the Singularize method, but I implement both
    #E This takes a name and puralize it, for instance, dog would become dogs. I use a small .NET 4.5 NuGet library called Inflector
    #E This takes a name and singulaizes it, that is, cats would become cat.
     * ***********************************************************************/
}