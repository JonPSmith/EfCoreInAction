// Copyright (c) 2017 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

namespace Test.Chapter10Listings.MappingClasses
{
    public class BadDto
    {
        public string MissingName { get; set; }

        public int SubClassMyString { get; set; }
    }
}