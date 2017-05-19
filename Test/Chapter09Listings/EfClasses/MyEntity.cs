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

        //Note: I don't normally use the form of collection, but I used it so that this entity is set up the same as the NotifyOne entity
        public ICollection<ManyEntity> Collection { get; } = new HashSet<ManyEntity>();
    }
}