// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using Test.Chapter10Listings.EfClasses;

namespace Test.Chapter10Listings.MappingClasses
{
    public class ListPostsWithSubDto : AutoMapper.Profile
    {
        public ListPostsWithSubDto()
        {
            CreateMap<Post, ListPostsWithSubDto>();
            CreateMap<PostTag, PostTagDto>();
        }

        public string BloggerName { get; set; }
        public string Title { get; set; }
        public DateTime LastUpdated { get; set; }
        public List<PostTagDto> TagLinks { get; set; }
    }

    public class PostTagDto
    {
        public string TagName { get; set; }
    }
}