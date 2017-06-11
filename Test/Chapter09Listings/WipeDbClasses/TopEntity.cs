// Copyright (c) 2017 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using Test.Chapter09Listings.WipeDbClasses;

namespace Test.Chapter09Listings.EfClasses
{
    public class TopEntity
    {
        public int Id { get; set; }

        public T1P1 T1P1 { get; set; }

        public T2P1 T2P1 { get; set; }

        public int FKey { get; set; }
    }
}