// Copyright (c) 2017 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System.Linq;
using AutoMapper;
using AutoMapper.QueryableExtensions;
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

        [Fact]
        public void BadProjection()
        {
            //SETUP
            var config = new MapperConfiguration(
                cfg => {
                    cfg.CreateMap<MyClass, BadDto>();
                });

            //ATTEMPT
            var queryable = new[]
            {
                new MyClass {MyString = "1", SubClass = new MySubClass {MyString = "Inner1"}},
                new MyClass {MyString = "2", SubClass = new MySubClass {MyString = "Inner2"}},
            }.AsQueryable();
            var ex = Assert.Throws<AutoMapperMappingException>( () => queryable.ProjectTo<BadDto>(config).ToList());

            //VERIFY
            ex.Message.StartsWith("Unable to create a map expression from MySubClass.MyString (System.String) to BadDto.SubClassMyString (System.Int32)\r\n\r\nMapping types:\r\nMyClass -> BadDto")
                .ShouldBeTrue();
        }


        [Fact]
        public void ValidateProjection()
        {
            //SETUP
            var config = new MapperConfiguration(
                cfg => {
                    cfg.CreateMap<MyClass, BadDto>();
                });

            //ATTEMPT
            var ex = Assert.Throws< AutoMapperConfigurationException> (() => config.AssertConfigurationIsValid());

            //VERIFY
            ex.Message.StartsWith("\nUnmapped members were found. Review the types and members below.")
                .ShouldBeTrue();
        }

    }
}