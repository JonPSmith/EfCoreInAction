// Copyright (c) 2016 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Test.Chapter09Listings.EfCode;

namespace Test.Chapter09Listings.EfClasses
{
    public class NotifyEntity : NotificationEntity
    {
        private int _id;
        private string _myString;

        public int Id
        {
            get => _id;
            set => SetWithNotify(value, ref _id);
        }

        public string MyString
        {
            get => _myString;
            set => SetWithNotify(value, ref _myString);
        }

        public ICollection<SubNotifyEntity> Collection { get; } = new ObservableHashSet<SubNotifyEntity>();

    }
}