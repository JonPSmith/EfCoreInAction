// Copyright (c) 2017 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace DataLayer.EfCode
{
    public static class DbContextValidationHelper
    {
        //see https://blogs.msdn.microsoft.com/dotnet/2016/09/29/implementing-seeding-custom-conventions-and-interceptors-in-ef-core-1-0/
        //for why I call DetectChanges before ChangeTracker, and why I then turn ChangeTracker.AutoDetectChangesEnabled off/on around SaveChanges

        public static async Task<ImmutableList<ValidationResult>> SaveChangesWithCheckingAsync(this DbContext context)
        {
            var result = context.ExecuteValidation();
            if (result.Any()) return result;

            context.ChangeTracker.AutoDetectChangesEnabled = false;
            await context.SaveChangesAsync().ConfigureAwait(false);
            context.ChangeTracker.AutoDetectChangesEnabled = true;
            return result;
        }

        //see https://blogs.msdn.microsoft.com/dotnet/2016/09/29/implementing-seeding-custom-conventions-and-interceptors-in-ef-core-1-0/
        //for why I call DetectChanges before ChangeTracker, and why I then turn ChangeTracker.AutoDetectChangesEnabled off/on around SaveChanges

        public static ImmutableList<ValidationResult> //#A
            SaveChangesWithChecking(this DbContext context)//#B
        {
            var result = context.ExecuteValidation(); //#C
            if (result.Any()) return result;   //#D

            context.ChangeTracker
                .AutoDetectChangesEnabled = false;//#E
            context.SaveChanges(); //#F
            context.ChangeTracker
                .AutoDetectChangesEnabled = true; //#G
            return result; //#H
        }
        /********************************************************************
         #A The SaveChangesWithChecking returns a list of ValidationResults. If it is an empty collection then the data was saved. If it has errors then the data wasn't saved
         #B SaveChangesWithChecking is an extension method, which means I can call it in the same way I call SaveChanges
         #C I create a private method to do the validation as I need to apply this in SaveChangesWithChecking and SaveChangesWithCheckingAsync
         #D If there are errors then I return them immediately and don't call SaveChanges
         #E There aren't any errors so I am going to call SaveChanges. I turn off AutoDetectChangesEnabled because ExecuteValidation has already called DetectChanges
         #F Now I call SaveChanges
         #G I turn AutoDetectChangesEnabled back on
         #H I return the empty set of errors, which tells the caller that everything is OK
         * *****************************************************************/

        private static ImmutableList<ValidationResult>
            ExecuteValidation(this DbContext context)
        {
            var result = new List<ValidationResult>();
            context.ChangeTracker.DetectChanges(); //#A
            foreach (var entry in 
                context.ChangeTracker.Entries() //#B
                    .Where(e =>
                       (e.State == EntityState.Added) ||   //#C
                       (e.State == EntityState.Modified))) //#C
            {
                var entity = entry.Entity;
                var valProvider = new 
                    ValidationDbContextServiceProvider(context);//#D
                var valContext = new 
                    ValidationContext(entity, valProvider, null);
                var entityErrors = new List<ValidationResult>();
                if (!Validator.TryValidateObject(           //#E
                    entity, valContext, entityErrors, true))//#E
                {
                    result.AddRange(entityErrors); //#F
                }
            }
            return result.ToImmutableList(); //#G
        }
        /*************************************************************
        #A I call ChangeTracker.DetectChanges to make sure I have found all the possible changes
        #B I then use EF Core's ChangeTracker to get access to all the entity classes it is tracking
        #C I filter out only those that need to be added to, or update the database
        #D I have created a simple class that implements the IServiceProvider interface, which makes the current DbContext available in the IValidatableObject.Validate method 
        #E The Validator.TryValidateObject is the method which handles all validation checking for me
        #F If there are errors I add them to the list
        #F Finally I return the list of all the errors found
         * *********************************************************/
    }
}