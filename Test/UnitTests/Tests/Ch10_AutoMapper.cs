// Copyright (c) 2017 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System.Linq;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using DataLayer.EfClasses;
using test.EfHelpers;
using Test.Chapter10Listings.MappingClasses;
using Xunit;
using Xunit.Extensions.AssertExtensions;

namespace test.UnitTests.Tests
{
    public class Ch10_AutoMapper
    {


        [Fact]
        public void StandardProjection()
        {
            //SETUP
            var config = new MapperConfiguration( //#A
                cfg => {
                    cfg.CreateMap<MyClass, MyDto>(); //#B
            });

            //ATTEMPT
            var queryable = new[]
            {
                new MyClass {MyString = "1", SubClass = new MySubClass {MyString = "Inner1"}},
                new MyClass {MyString = "2", SubClass = new MySubClass {MyString = "Inner2"}},
            }.AsQueryable();
            var d = queryable.ProjectTo<MyDto>(config).ToList();

            //VERIFY
            d.Count.ShouldEqual(2);
            d[0].MyString.ShouldEqual("1");
            d[0].SubClassMyString.ShouldEqual("Inner1");
            d[1].MyString.ShouldEqual("2");
            d[1].SubClassMyString.ShouldEqual("Inner2");
        }



    }
}