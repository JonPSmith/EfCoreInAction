// Copyright (c) 2017 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using test.EfHelpers;
using test.Helpers;
using Test.Chapter07Listings.EfClasses;
using Test.Chapter07Listings.EFCode;
using Xunit;
using Xunit.Extensions.AssertExtensions;

namespace test.UnitTests.DataLayer
{
    public class Ch07_ShadowProperties
    {
        [Fact]
        public void TestShadowPropertyRequiredOk()
        {
            //SETUP
            using (var context = new Chapter07DbContext(
                SqliteInMemory.CreateOptions<Chapter07DbContext>()))
            {
                var logs = new List<string>();
                SqliteInMemory.SetupLogging(context, logs);

                context.Database.EnsureCreated();

                //ATTEMPT
                var attendee = new Attendee
                {
                    Name = "Person1",
                    Ticket = new Ticket {TicketType = Ticket.TicketTypes.VIP},
                    Required = new RequiredTrack {Track = TrackNames.EfCore}
                };
                context.Add(attendee);
                context.SaveChanges();

                //VERIFY
                context.Set<RequiredTrack>().Count().ShouldEqual(1);
            }
        }

        [Fact]
        public void TestShadowPropertyRequiredNotAddedOk()
        {
            //SETUP
            using (var context = new Chapter07DbContext(
                SqliteInMemory.CreateOptions<Chapter07DbContext>()))
            {
                var logs = new List<string>();
                SqliteInMemory.SetupLogging(context, logs);
                context.Database.EnsureCreated();

                //ATTEMPT
                var attendee = new Attendee
                {
                    Name = "Person1",
                    Ticket = new Ticket {TicketType = Ticket.TicketTypes.VIP}
                };
                context.Add(attendee);
                //context.SaveChanges();
                var ex = Assert.Throws<DbUpdateException>(() => context.SaveChanges());

                //VERIFY
                ex.InnerException.Message.ShouldEqual(
                    "SQLite Error 19: 'NOT NULL constraint failed: Attendees.RequiredTrackId'.");
                //context.Set<RequiredTrack>().Count().ShouldEqual(1);
            }
        }

        [Fact]
        public void TestShadowPropertyReplaceOk()
        {
            //SETUP
            using (var context = new Chapter07DbContext(
                SqliteInMemory.CreateOptions<Chapter07DbContext>()))
            {
                context.Database.EnsureCreated();

                var attendee = new Attendee
                {
                    Name = "Person1",
                    Ticket = new Ticket {TicketType = Ticket.TicketTypes.VIP},
                    Required = new RequiredTrack {Track = TrackNames.EfCore}
                };
                context.Add(attendee);
                context.SaveChanges();

                //ATTEMPT
                attendee.Required = new RequiredTrack {Track = TrackNames.AspNetCore};
                context.SaveChanges();

                //VERIFY
                context.Set<RequiredTrack>().Count().ShouldEqual(2);
            }
        }

        [Fact]
        public void TestShadowPropertyDeleteOk()
        {
            //SETUP
            using (var context = new Chapter07DbContext(
                SqliteInMemory.CreateOptions<Chapter07DbContext>()))
            {
                context.Database.EnsureCreated();

                var attendee = new Attendee
                {
                    Name = "Person1",
                    Ticket = new Ticket {TicketType = Ticket.TicketTypes.VIP},
                    Required = new RequiredTrack {Track = TrackNames.EfCore}
                };
                context.Add(attendee);
                context.SaveChanges();

                //ATTEMPT
                context.Remove(attendee);
                context.SaveChanges();

                //VERIFY
                context.Attendees.Count().ShouldEqual(0);
                context.Set<RequiredTrack>().Count().ShouldEqual(1);
            }
        }


        [Fact]
        public void TestShadowPropertyOptionalOk()
        {
            //SETUP
            using (var context = new Chapter07DbContext(
                SqliteInMemory.CreateOptions<Chapter07DbContext>()))
            {
                context.Database.EnsureCreated();

                //ATTEMPT
                var attendee = new Attendee
                {
                    Name = "Person1",
                    Ticket = new Ticket {TicketType = Ticket.TicketTypes.VIP},
                    Required = new RequiredTrack {Track = TrackNames.EfCore},
                    Optional = new OptionalTrack {Track = TrackNames.EfCore}
                };
                context.Add(attendee);
                context.SaveChanges();

                //VERIFY
                context.Set<RequiredTrack>().Count().ShouldEqual(1);
                context.Set<OptionalTrack>().Count().ShouldEqual(1);
            }
        }

        //[Fact]
        //public void TestShadowPropertySqlOk()
        //{
        //    //SETUP
        //    var connection = this.GetUniqueDatabaseConnectionString();
        //    var optionsBuilder =
        //        new DbContextOptionsBuilder<Chapter07DbContext>();

        //    optionsBuilder.UseSqlServer(connection);
        //    using (var context = new Chapter07DbContext(optionsBuilder.Options))
        //    {
        //            context.Database.EnsureCreated();
        //            var orgCount = context.Set<RequiredTrack>().Count();
        //            var attendee = new Attendee
        //            {
        //                Name = "Person1",
        //                Ticket = new Ticket { TicketType = Ticket.TicketTypes.VIP },
        //                Required = new RequiredTrack { Track = TrackNames.EfCore }
        //            };
        //            context.Add(attendee);
        //            context.SaveChanges();

        //            //VERIFY
        //            context.Set<RequiredTrack>().Count().ShouldEqual(orgCount + 1);
        //    }
        //}
    }
}