// Copyright (c) 2017 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using test.EfHelpers;
using Test.Chapter07Listings.EfClasses;
using Test.Chapter07Listings.EFCode;
using Xunit;
using Xunit.Extensions.AssertExtensions;

namespace test.UnitTests.DataLayer
{
    public class Ch07_AlternateKey
    {
        [Fact]
        public void TestPersonWithContactInfoOk()
        {
            //SETUP
            using (var context = new Chapter07DbContext(
                SqliteInMemory.CreateOptions<Chapter07DbContext>()))
            {
                {
                    var logs = new List<string>();
                    SqliteInMemory.SetupLogging(context, logs);
                    context.Database.EnsureCreated();

                    //ATTEMPT
                    var person = new Person
                    {
                        UserId = "me@somewhere.com",
                        ContactInfo = new ContactInfo {MobileNumber = "12345"}
                    };
                    context.Add(person);
                    context.SaveChanges();

                    //VERIFY
                    person.ContactInfo.EmailAddress.ShouldEqual("me@somewhere.com");
                }
            }
        }

        [Fact]
        public void TestPersonWithContactInfoDeleteOk()
        {
            //SETUP
            using (var context = new Chapter07DbContext(
                SqliteInMemory.CreateOptions<Chapter07DbContext>()))
            {
                {
                    context.Database.EnsureCreated();

                    var person = new Person
                    {
                        UserId = "me@somewhere.com",
                        ContactInfo = new ContactInfo { MobileNumber = "12345" }
                    };
                    context.Add(person);
                    context.SaveChanges();

                    //ATTEMPT
                    context.Remove(person);
                    context.SaveChanges();

                    //VERIFY
                    context.People.Count().ShouldEqual(0);
                    context.Set<ContactInfo>().Count().ShouldEqual(0);
                }
            }
        }
    }
}