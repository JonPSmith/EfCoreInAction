// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using Test.Chapter10Listings.EfClasses;

namespace Test.Chapter10Listings.MappingClasses
{
    public class ListPostsDto : AutoMapper.Profile
    {
        public ListPostsDto()
        {
            CreateMap<Post, ListPostsDto>()
                .ForMember(
                    p => p.TagNames, 
                    opt => opt.MapFrom(x => x.TagLinks.Select(y => y.Tag.Name)));
        }

        public string BloggerName { get; set; }
        public string Title { get; set; }
        public DateTime LastUpdated { get; set; }
        public List<string> TagNames { get; set; }
    }
}