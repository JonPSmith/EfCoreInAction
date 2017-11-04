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
     * 
     * ***********************************************************************/
}