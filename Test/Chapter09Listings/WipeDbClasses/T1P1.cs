// Copyright (c) 2017 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using Test.Chapter09Listings.EfClasses;

namespace Test.Chapter09Listings.WipeDbClasses
{
    public class T1P1
    {
        public int Id { get; set; }

        public T1P2 T1P2 { get; set; }

        public int FKey { get; set; }
    }
}