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
    public class Ch07_OneToOneRelationships
    {
        [Fact]
        public void TestOption1OneToOneOk()
        {
            //SETUP
            using (var context = new Chapter07DbContext(
                SqliteInMemory.CreateOptions<Chapter07DbContext>()))
            {
                {
                    context.Database.EnsureCreated();

                    //ATTEMPT
                    var attendees = new List<Attendee>
                    {
                        new Attendee
                        {
                            Name = "Person1", Ticket = new Ticket{TicketType = Ticket.TicketTypes.Guest},
                            //Required = new RequiredTrack()
                        },
                        new Attendee
                        {
                            Name = "Person2", Ticket = new Ticket {TicketType = Ticket.TicketTypes.VIP },
                            //Required = new RequiredTrack()
                        },
                        new Attendee
                        {
                            Name = "Person3", Ticket = new Ticket{TicketType = Ticket.TicketTypes.Guest},
                            //Required = new RequiredTrack()
                        },
                    };
                    context.AddRange(attendees);
                    context.SaveChanges();

                    //VERIFY
                    context.Tickets.Count().ShouldEqual(3);
                }
            }
        }

        [Fact]
        public void TestOption1OneToOneNoTicketBad()
        {
            //SETUP
            using (var context = new Chapter07DbContext(
                SqliteInMemory.CreateOptions<Chapter07DbContext>()))
            {
                {
                    context.Database.EnsureCreated();

                    //ATTEMPT
                    context.Add(new Attendee {Name = "Person1",});//Required = new RequiredTrack() });
                    var ex = Assert.Throws<DbUpdateException>(() => context.SaveChanges());

                    //VERIFY
                    ex.InnerException.Message.ShouldEqual("SQLite Error 19: 'FOREIGN KEY constraint failed'.");
                }
            }
        }

        [Fact]
        public void TestOption1OneToOneDuplicateTicketBad()
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
                    var dupTicket = new Ticket {TicketType = Ticket.TicketTypes.Guest};
                    var attendees = new List<Attendee>
                    {
                        new Attendee {Name = "Person1", Ticket = dupTicket, },//Required = new RequiredTrack()},
                        new Attendee {Name = "Person2", Ticket = dupTicket, },//Required = new RequiredTrack()},
                    };
                    context.AddRange(attendees);
                    context.SaveChanges();
                    //var ex = Assert.Throws<DbUpdateException>(() => context.SaveChanges());

                    //VERIFY
                    //ex.InnerException.Message.ShouldEqual("SQLite Error 19: 'UNIQUE constraint failed: Attendees.TicketId'.");
                    context.Tickets.Count().ShouldEqual(1);
                }
            }
        }

        //[Fact]
        //public void TestOption1OneToOneSqlOk()
        //{
        //    //SETUP
        //    var connection = this.GetUniqueDatabaseConnectionString();
        //    var optionsBuilder =
        //        new DbContextOptionsBuilder<Chapter07DbContext>();

        //    optionsBuilder.UseSqlServer(connection);
        //    using (var context = new Chapter07DbContext(optionsBuilder.Options))
        //    {
        //        {
        //            context.Database.EnsureCreated();
        //            var orgTicketesCount = context.Tickets.Count();

        //            //ATTEMPT
        //            var attendees = new List<Attendee>
        //            {
        //                new Attendee
        //                {
        //                    Name = "Person1", Ticket = new Ticket{TicketType = Ticket.TicketTypes.Guest},
        //                    //Required = new RequiredTrack()
        //                },
        //                new Attendee
        //                {
        //                    Name = "Person2", Ticket = new Ticket {TicketType = Ticket.TicketTypes.VIP },
        //                    //Required = new RequiredTrack()
        //                },
        //                new Attendee
        //                {
        //                    Name = "Person3", Ticket = new Ticket{TicketType = Ticket.TicketTypes.Guest},
        //                    //Required = new RequiredTrack()
        //                },
        //            };
        //            context.AddRange(attendees);
        //            context.SaveChanges();

        //            //VERIFY
        //            context.Tickets.Count().ShouldEqual(orgTicketesCount + 3);
        //        }
        //    }
        //}


    }
}