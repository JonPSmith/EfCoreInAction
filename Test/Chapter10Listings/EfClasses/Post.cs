// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.Collections;
using System.Collections.Generic;

namespace Test.Chapter10Listings.EfClasses
{
    public class Post
    {
        public int PostId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime LastUpdated { get; set; }

        //-------------------------------------------
        //relationships

        public int BlogId { get; set; }
        public Blogger Blogger { get; set; }
        public ICollection<PostTag> TagLinks { get; set; }
    }
}