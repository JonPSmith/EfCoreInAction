// Copyright (c) 2016 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Test.Chapter09Listings.EfCode;

namespace Test.Chapter09Listings.EfClasses
{
    public class Notify2Entity : Notification2Entity
    {
        private int _id;             //#A
        private string _myString;    //#A

        public int Id
        {
            get => _id;
            set => SetWithNotify(value, ref _id); //#B
        }

        public string MyString
        {
            get => _myString;
            set => SetWithNotify(value, ref _myString); //#B
        }
    }
}