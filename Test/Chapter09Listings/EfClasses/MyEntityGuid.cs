// Copyright (c) 2017 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;

namespace Test.Chapter09Listings.EfClasses
{
    public class MyEntityGuid
    {
        public Guid Id { get; set; }

        public string MyString { get; set; }

        public OneEntityGuid OneToOne { get; set; }
    }
}