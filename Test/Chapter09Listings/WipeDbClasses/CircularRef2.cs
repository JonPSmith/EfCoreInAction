// Copyright (c) 2017 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.
namespace Test.Chapter09Listings.WipeDbClasses
{
    public class CircularRef2
    {
        public int Id { get; set; }

        public CircularRef1 Ref { get; set; }

        public int FKey { get; set; }
    }
}