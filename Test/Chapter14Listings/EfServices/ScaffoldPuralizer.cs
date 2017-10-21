// Copyright (c) 2017 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using Microsoft.EntityFrameworkCore.Design;

namespace Test.Chapter14Listings.EfServices
{
    public class ScaffoldPuralizer : IPluralizer
    {
        public string Pluralize(string name)
        {
            return Inflector.Inflector.Pluralize(name) ?? name;
        }

        public string Singularize(string name)
        {
            return Inflector.Inflector.Singularize(name) ?? name;
        }
    }
}