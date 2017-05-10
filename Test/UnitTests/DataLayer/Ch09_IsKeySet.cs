// Copyright (c) 2016 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System.Linq;
using test.EfHelpers;
using Test.Chapter09Listings.EfClasses;
using Test.Chapter09Listings.EfCode;
using Xunit;
using Xunit.Extensions.AssertExtensions;

namespace test.UnitTests.DataLayer
{
    public class Ch09_IsKeySet
    {


        [Fact]
        public void TestIsKeySetOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<Chapter09DbContext>();

            using (var context = new Chapter09DbContext(options))
            {
                //context.Database.EnsureCreated();

                //ATTEMPT
                var storeKey = new OneEntity();
                var clientKey = new GuidKeyEntity();

                //VERIFY
                context.GetEntityIsKeySet(storeKey).ShouldBeFalse();
                context.GetEntityIsKeySet(clientKey).ShouldBeFalse();
            }
        }

        [Fact]
        public void TestIsKeySetAttachOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<Chapter09DbContext>();

            using (var context = new Chapter09DbContext(options))
            {
                //context.Database.EnsureCreated();

                //ATTEMPT
                var storeKey = new OneEntity();
                context.Attach(storeKey);
                var clientKey = new GuidKeyEntity();
                context.Attach(clientKey);

                //VERIFY
                context.GetEntityIsKeySet(storeKey).ShouldBeFalse();
                context.GetEntityIsKeySet(clientKey).ShouldBeFalse();
            }
        }

        [Fact]
        public void TestIsKeySetAfterAddOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<Chapter09DbContext>();

            using (var context = new Chapter09DbContext(options))
            {
                //context.Database.EnsureCreated();

                //ATTEMPT
                var storeKey = new OneEntity();
                context.Add(storeKey);
                var clientKey = new GuidKeyEntity();
                context.Add(clientKey);

                //VERIFY
                context.GetEntityIsKeySet(storeKey).ShouldBeTrue();
                context.GetEntityIsKeySet(clientKey).ShouldBeTrue();
            }
        }

        //[Fact]
        //public void TestGetPrimaryKeyInfoOk()
        //{
        //    //SETUP
        //    var options = SqliteInMemory.CreateOptions<Chapter09DbContext>();

        //    using (var context = new Chapter09DbContext(options))
        //    {
        //        //context.Database.EnsureCreated();

        //        //ATTEMPT
        //        var storeKey = new TrackedOne();
        //        context.Add(storeKey);
        //        var clientKey = new GuidKeyEntity();
        //        context.Add(clientKey);

        //        //VERIFY
        //        var sk = context.GetEntityPrimaryKeys(storeKey).Single();
        //        var ck = context.GetEntityPrimaryKeys(clientKey).Single();
        //    }
        //}

    }
}
