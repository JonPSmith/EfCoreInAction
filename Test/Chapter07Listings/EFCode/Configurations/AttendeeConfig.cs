// Copyright (c) 2017 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Test.Chapter07Listings.EfClasses;

namespace Test.Chapter07Listings.EFCode.Configurations
{
    public static class AttendeeConfig
    {
        public static void Configure
            (this EntityTypeBuilder<Attendee> entity)
        {
            entity.HasOne(p => p.Ticket)
                .WithOne(p => p.Attendee)
                .HasForeignKey<Attendee>(p => p.TicketId);
        }
    }
}