// Copyright (c) 2017 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.
namespace Test.Chapter07Listings.EfClasses
{
    public class RequiredTrack
    {
        public int RequiredTrackId { get; set; }

        public TrackNames Track { get; set; }

        public Attendee Attend { get; set; }
    }
}