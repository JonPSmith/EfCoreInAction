// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using Test.Chapter10Listings.EfClasses;

namespace Test.Chapter10Listings.MappingClasses
{
    public class ListBloggersDto : AutoMapper.Profile
    {
        public ListBloggersDto()
        {
            CreateMap<Blogger, ListBloggersDto>();
        }

        public string Name { get; set; }
        public string EmailAddress { get; set; }
        public int PostsCount { get; set; }
    }
}