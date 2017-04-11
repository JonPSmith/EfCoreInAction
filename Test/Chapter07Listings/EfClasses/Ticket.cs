// Copyright (c) 2017 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;

namespace Test.Chapter07Listings.EfClasses
{

    public class Ticket
    {
        public enum TicketTypes : byte { Guest = 0, VIP = 1, Staff = 3}

        public int TicketId { get; set; }
        public TicketTypes TicketType { get; set; }

        public Attendee Attendee { get; set; }
    }
}