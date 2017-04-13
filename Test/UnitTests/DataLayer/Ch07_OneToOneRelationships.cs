// Copyright (c) 2017 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.EntityFrameworkCore;
using test.EfHelpers;
using test.Helpers;
using Test.Chapter07Listings.EfClasses;
using Test.Chapter07Listings.EFCode;
using Xunit;
using Xunit.Abstractions;
using Xunit.Extensions.AssertExtensions;

namespace test.UnitTests.DataLayer
{
    public class Ch07_OneToOneRelationships
    {
        private readonly ITestOutputHelper _output;

        public Ch07_OneToOneRelationships(ITestOutputHelper output)
        {
            _output = output;
        }

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
                            Required = new RequiredTrack()
                        },
                        new Attendee
                        {
                            Name = "Person2", Ticket = new Ticket {TicketType = Ticket.TicketTypes.VIP },
                            Required = new RequiredTrack()
                        },
                        new Attendee
                        {
                            Name = "Person3", Ticket = new Ticket{TicketType = Ticket.TicketTypes.Guest},
                            Required = new RequiredTrack()
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
        public void TestOption1OneToOneDeleteOk()
        {
            //SETUP
            using (var context = new Chapter07DbContext(
                SqliteInMemory.CreateOptions<Chapter07DbContext>()))
            {
                {
                    context.Database.EnsureCreated();
                    var attendee = new Attendee
                    {
                        Name = "Person1",
                        Ticket = new Ticket(),
                        Required = new RequiredTrack()
                    };
                    context.Add(attendee);
                    context.SaveChanges();

                    //ATTEMPT
                    context.Remove(attendee);
                    context.SaveChanges();

                    //VERIFY
                    context.Attendees.Count().ShouldEqual(0);
                    context.Tickets.Count().ShouldEqual(1);
                    context.Set<RequiredTrack>().Count().ShouldEqual(1);
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
                    context.Add(new Attendee {Name = "Person1", Required = new RequiredTrack() });
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
                        new Attendee {Name = "Person1", Ticket = dupTicket, Required = new RequiredTrack()},
                        new Attendee {Name = "Person2", Ticket = dupTicket, Required = new RequiredTrack()},
                        new Attendee {Name = "Person2", Ticket = dupTicket, Required = new RequiredTrack()},
                        new Attendee {Name = "Person3", Ticket = new Ticket(), Required = new RequiredTrack()}
                    };
                    context.AddRange(attendees);
                    ListStates(context, "After AddRange", attendees);
                    context.SaveChanges();
                    ListStates(context, "After SaveChanges", attendees);
                    //var ex = Assert.Throws<DbUpdateException>(() => context.SaveChanges());

                    //VERIFY
                    //ex.InnerException.Message.ShouldEqual("SQLite Error 19: 'UNIQUE constraint failed: Attendees.TicketId'.");
                    context.Tickets.Count().ShouldEqual(1);
                    context.Attendees.Count().ShouldEqual(2);
                }
            }
        }

        private void ListStates(DbContext context, string message,  List<Attendee> entities)
        {
            var line = message + ": " + string.Join(", ", entities.Select(x => context.Entry(x).State));
            _output.WriteLine(line);
        }

        [Fact]
        public void TestOption1OneToOneSqlOk()
        {
            //SETUP
            var connection = this.GetUniqueDatabaseConnectionString();
            var optionsBuilder =
                new DbContextOptionsBuilder<Chapter07DbContext>();

            optionsBuilder.UseSqlServer(connection);
            using (var context = new Chapter07DbContext(optionsBuilder.Options))
            {
                {
                    var logger = new LogDbContext(context);

                    context.Database.EnsureCreated();
                    var orgTicketesCount = context.Tickets.Count();
                    var orgAttendeesCount = context.Attendees.Count();

                    var dupTicket = new Ticket { TicketType = Ticket.TicketTypes.Guest };
                    var attendees = new List<Attendee>
                    {
                        new Attendee {Name = "Person1", Ticket = dupTicket, Required = new RequiredTrack()},
                        new Attendee {Name = "Person2", Ticket = dupTicket, Required = new RequiredTrack()}
                    };
                    context.AddRange(attendees);
                    context.SaveChanges();

                    //VERIFY
                    context.Tickets.Count().ShouldEqual(orgTicketesCount + 1);
                    context.Attendees.Count().ShouldEqual(orgAttendeesCount + 2);
                }
            }
        }


    }
}