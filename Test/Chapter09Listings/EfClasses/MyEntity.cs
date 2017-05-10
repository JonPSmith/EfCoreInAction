// Copyright (c) 2016 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Test.Chapter09Listings.EfClasses
{
    public class MyEntity
    {
        public int Id { get; set; }

        public string MyString { get; set; }

        public OneEntity OneToOne { get; set; }

        public ICollection<ManyEntity> Collection { get; } = new Collection<ManyEntity>();
    }
}