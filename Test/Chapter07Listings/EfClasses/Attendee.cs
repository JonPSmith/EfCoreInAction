// Copyright (c) 2017 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

namespace Test.Chapter07Listings.EfClasses
{
    public class Attendee
    {
        public int AttendeeId { get; set; }
        public string Name { get; set; }

        public int TicketId { get; set; }
        public Ticket Ticket { get; set; }

    }
}