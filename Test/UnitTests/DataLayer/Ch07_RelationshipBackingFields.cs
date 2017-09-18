// Copyright (c) 2017 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using DataLayer.EfClasses;
using Microsoft.EntityFrameworkCore;
using test.EfHelpers;
using Test.Chapter07Listings.EfClasses;
using Test.Chapter07Listings.EFCode;
using Xunit;
using Xunit.Extensions.AssertExtensions;

namespace test.UnitTests.DataLayer
{
    public class Ch07_RelationshipBackingFields
    {
        [Fact]
        public void TestCreateBookOk()
        {
            //SETUP
            using (var context = new Chapter07DbContext(SqliteInMemory.CreateOptions<Chapter07DbContext>()))
            {
                context.Database.EnsureCreated();

                //ATTEMPT
                var entity = new Ch07Book
                {
                    Title = "Quantem Networking"
                };

                //VERIFY
                entity.Reviews.ShouldNotBeNull();
                entity.Reviews.Any().ShouldBeFalse();
                entity.CachedVotes.ShouldBeNull();
            }
        }

        [Fact]
        public void TestCreateBookOneReviewOk()
        {
            //SETUP
            using (var context = new Chapter07DbContext(SqliteInMemory.CreateOptions<Chapter07DbContext>()))
            {
                context.Database.EnsureCreated();

                //ATTEMPT
                var entity = new Ch07Book
                {
                    Title = "Quantem Networking"
                };
                entity.AddReview(new Review{NumStars = 5, VoterName = "Unit Test"});

                //VERIFY
                entity.Reviews.ShouldNotBeNull();
                entity.Reviews.Count().ShouldEqual(1);
                entity.CachedVotes.ShouldEqual(5);
            }
        }

        [Fact]
        public void TestCreateBookAddRemoveOneReviewOk()
        {
            //SETUP
            using (var context = new Chapter07DbContext(SqliteInMemory.CreateOptions<Chapter07DbContext>()))
            {
                context.Database.EnsureCreated();
                var entity = new Ch07Book
                {
                    Title = "Quantem Networking"
                };

                //ATTEMPT
                var review = new Review {NumStars = 5, VoterName = "Unit Test"};
                entity.AddReview(review);
                entity.RemoveReview(review);

                //VERIFY
                entity.Reviews.Count().ShouldEqual(0);
                entity.CachedVotes.ShouldEqual(null);
            }
        }

        [Fact]
        public void TestCreateBookTwoReviewOk()
        {
            //SETUP
            using (var context = new Chapter07DbContext(SqliteInMemory.CreateOptions<Chapter07DbContext>()))
            {
                context.Database.EnsureCreated();

                //ATTEMPT
                var entity = new Ch07Book
                {
                    Title = "Quantem Networking"
                };
                entity.AddReview(new Review { NumStars = 4, VoterName = "Unit Test" });
                entity.AddReview(new Review { NumStars = 2, VoterName = "Unit Test" });

                //VERIFY
                entity.Reviews.ShouldNotBeNull();
                entity.Reviews.Count().ShouldEqual(2);
                entity.CachedVotes.ShouldEqual(3);
            }
        }

        [Fact]
        public void TestCreateBookOneAddedOneRemovedReviewOk()
        {
            //SETUP
            using (var context = new Chapter07DbContext(SqliteInMemory.CreateOptions<Chapter07DbContext>()))
            {
                context.Database.EnsureCreated();

                //ATTEMPT
                var entity = new Ch07Book
                {
                    Title = "Quantem Networking"
                };
                var review = new Review {NumStars = 5, VoterName = "Unit Test"};
                entity.AddReview(review);
                entity.RemoveReview(review);

                //VERIFY
                entity.Reviews.ShouldNotBeNull();
                entity.Reviews.Count().ShouldEqual(0);
                entity.CachedVotes.ShouldBeNull();
            }
        }

        [Fact]
        public void TestSaveBookOneReviewAndReadOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<Chapter07DbContext>();
            using (var context = new Chapter07DbContext(options))
            {
                context.Database.EnsureCreated();

                //ATTEMPT
                var entity = new Ch07Book
                {
                    Title = "Quantem Networking"
                };
                entity.AddReview(new Review {NumStars = 5, VoterName = "Unit Test"});
                context.Add(entity);
                context.SaveChanges();
            }
            using (var context = new Chapter07DbContext(options))
            {
                //VERIFY
                var entity = context.Books.Include(x => x.Reviews).Single();
                entity.Reviews.ShouldNotBeNull();
                entity.Reviews.Count().ShouldEqual(1);
                entity.CachedVotes.ShouldEqual(5);
            }
        }
    }
}