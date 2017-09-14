// Copyright (c) 2017 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.Linq;
using DataLayer.EfClasses;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace DataLayer.EfCode
{
    public static class SaveChangesBookFixer
    {
        public static int BookSaveChanges(EfCoreContext context)
        {
            try
            {
                return context.SaveChanges(); //#B
            }
            catch (DbUpdateConcurrencyException ex) //#C
            {
                var entitiesToFix = ex.Entries
                    .Where(x => x.Entity is Book).ToList();
                foreach (var entity in entitiesToFix)
                {
                    FixReviewCachedValues(context, entity);
                }
                if (entitiesToFix.Count > 0)
                    return context.SaveChanges();
                throw;      //Has errors that this cannot handle
            }
        }

        private static void FixReviewCachedValues(
            EfCoreContext context,
            EntityEntry entry)
        {
            var book = (Book)entry.Entity;

            var databaseEntity =  
                context.Books.AsNoTracking() 
                    .SingleOrDefault(p => p.BookId == book.BookId);
            if (databaseEntity == null)
                //Book has been deleted, so ignore it
                return; 

            //We need to fix the ReviewCount and the AverageReview 
            var countProp = entry.Property(nameof(Book.ReviewsCount));
            var averageProp = entry.Property(nameof(Book.ReviewsCount));
            var reviews = context.Set<Review>().Where(x => x.BookId == book.BookId).ToList();
            countProp.CurrentValue = reviews.Count;
            countProp.OriginalValue = countProp.CurrentValue;
            averageProp.CurrentValue = reviews.Any()
                ? reviews.Average(x => (double?) x.NumStars)
                : null;
            averageProp.OriginalValue = averageProp.CurrentValue;
        }
    }
}