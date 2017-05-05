// Copyright (c) 2017 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using DataLayer.EfClasses;
using Microsoft.EntityFrameworkCore;

namespace Test.Chapter07Listings.EfClasses
{
    public class Ch07Book
    {
        private readonly List<Review> _reviews = 
            new List<Review>(); //#A

        public int BookId { get; set; }
        public string Title { get; set; }

        public double? CachedVotes { get; private set; } //#B

        public IEnumerable<Review> Reviews => _reviews.ToList(); //#C

        public void AddReview(Review review) //#D
        {
            _reviews.Add(review); //#E
            CachedVotes = 
                _reviews.Average(x => x.NumStars); //#F
        }

        public void RemoveReview(Review review) //#G
        {
            _reviews.Remove(review); //#H
            CachedVotes = _reviews.Any()
                ? _reviews.Average(x => x.NumStars) //#I
                : (double?)null; //#J
        }
    }
    /*********************************************************
    #A I add a backing field, which is a list. I then tell EF Core to use this for all reads and writes
    #B This holds a pre-calculated average of the reviews. Note that it is read-only so that it cannot be changed outside this class
    #C This returns a copy of the reviews that were loaded. By taking a copy it means no one can alter the list by casting the IEnumerable<T> to List<T>
    #D I add a method to allow a new Review to be added to the _reviews collection
    #E I add the new review to the backing field _reviews. This will update the database on the call to SaveChanges
    #F I then recalculate the average votes for the book
    #G I add a method to remove a review from the _reviews collection
    #H I remove the review from the list. This will update the database on the call to SaveChanges
    #I If ther are any reviews I recalculate the average votes for the book
    #J If there are no reviews I set the value to null
    * *********************************************************/
}