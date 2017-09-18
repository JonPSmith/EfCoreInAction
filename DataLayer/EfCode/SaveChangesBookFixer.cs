// Copyright (c) 2017 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System.Linq;
using DataLayer.EfClasses;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace DataLayer.EfCode
{
    public static class SaveChangesBookFixer
    {
        public static int SaveChangesWithReviewCheck //#A
            (this EfCoreContext context)
        {
            try
            {
                return context.SaveChanges(); //#B
            }
            catch (DbUpdateConcurrencyException ex) //#C
            {
                var entityToFix = ex.Entries //#D
                    .SingleOrDefault(x => x.Entity is Book); //#D
                if (entityToFix == null) //#E
                    throw; //#E

                if ( FixReviewCachedValues(context, entityToFix)) //#F
                    return context.SaveChanges();                 //#F
            }
            return 0; //#G
        }

        private static bool FixReviewCachedValues(
            EfCoreContext context,
            EntityEntry entry)
        {
            var book = (Book) entry.Entity; //#H

            var actualReviews = book.Reviews                     //#I
                .Where(x =>                                      //#I
                    context.Entry(x).State == EntityState.Added) //#I
                .Union(context.Set<Review>().AsNoTracking()      //#I
                    .Where(x => x.BookId == book.BookId))        //#I
                .ToList();                                       //#I

            var databaseEntity =                                //#J
                context.Books.AsNoTracking()                    //#J
                .SingleOrDefault(p => p.BookId == book.BookId); //#J
            if (databaseEntity == null) //#K
                return false;           //#K

            var databaseEntry = context.Entry(databaseEntity); //#L

            //We need to fix the ReviewCount and the AverageReview 
            var countProp = entry.Property(nameof(Book.ReviewsCount));  //#M
            var averageProp = entry.Property(nameof(Book.AverageVotes)); //#M
            //I take the ones in the database and the ones waiting to be written out
            var reviewCount = actualReviews.Count; //#N
            countProp.CurrentValue = reviewCount; //#O
            countProp.OriginalValue =                             //#P
                databaseEntry.Property(nameof(Book.ReviewsCount)) //#P
                .CurrentValue;                                    //#P
            averageProp.CurrentValue = reviewCount > 0            //#Q
                ? actualReviews.Average(x => (double?) x.NumStars)//#Q
                : null;                                           //#Q
            averageProp.OriginalValue =                           //#R
                databaseEntry.Property(nameof(Book.AverageVotes)) //#R
                .CurrentValue;                                    //#R

            return true; //#S
        }
    }
    /*********************************************************************
    #A This is the method you call instead of SaveChanges. It will automatially handle any concurrentcy issues.
    #B I call the normal SaveChanges method within a try/catch block. If it works then it just returns. If there is a DbUpdateConcurrencyException it will enter the catch part and execute code to fix the problem
    #C This method can only handle Book entities, so I filter those out
    #D I only expect one Book concurrency issue, so I check that is the case
    #E If its not a book I rethrow the exception as I can't handle it
    #F I now call my private method to handle this book concurrecy issue. If it returns true then it has updated the book entity, so SaveChanges needs to be called again to save the changes
    #G If someone deleted the book you were updating then we leave that as it was and return 0 to say nothing was updated 
    #H I cast the entity to a book so that I can access the properties I know
    #I This complex statement gets the combination of the reviews in the database and any new reviews that are being added. That is what the cached values must match
    #J I also need to load what the current values for the book entity in the database. I need that later to stop EF Core seeing a concurrency error again
    #K If there is no book in the database then its deleted. I this case I leave the book deleted and I don't save the updated book
    #L I get the EntityEntry class of the databaseEntity, as I need to access its currentValues
    #M I get references to the PropertyEntry for the ReviewsCount and AverageVotes in the Book entity as I cannot set these values via the setter, as it private
    #N I recalculate the reviews count using the actual number of reviews, both new ones and the ones in the database
    #O I update the ReviewsCount property to this recalculated value
    #P I now set the OriginalValue of the ReviewsCount property to the last read value. This stops EF Core decalring a DbUpdateConcurrencyException
    #Q I set the AverageVotes value to the recalulated the average votes value the actual number of reviews, both new ones and the ones in the database
    #R I now set the OriginalValue of the AverageVotes property to the last read value. This stops EF Core decalring a DbUpdateConcurrencyException
    #S I return true to say that SaveChanges needs to be called again to update the Book entity with the corrected data
     * *******************************************************************/
}