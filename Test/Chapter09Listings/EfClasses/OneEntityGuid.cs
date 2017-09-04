// Copyright (c) 2016 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;

namespace Test.Chapter09Listings.EfClasses
{
    public class OneEntityGuid
    {
        public Guid Id { get; set; }

        public int MyInt { get; set; }

        public Guid? MyEntityGuidId { get; set; }
    }
}