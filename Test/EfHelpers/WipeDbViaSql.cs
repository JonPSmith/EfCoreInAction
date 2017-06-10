// // Copyright (c) 2017 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// // Licensed under MIT licence. See License.txt in the project root for license information.
// 

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace test.EfHelpers
{
    public static class WipeDbViaSql
    {

        //NOTE: This will not handle a circular relationship: e.g. EntityA->EntityB->EntityA
        public static IEnumerable<string>
            GetTableNamesInOrderForWipe //#A
            (this DbContext context)
        {
            var allEntities = context.Model.GetEntityTypes().ToList();

            ThrowExceptionOnNotWipeableEntities(allEntities);

            var principalsDict = allEntities //#C
                .SelectMany(x => x.GetForeignKeys() //#C
                    .Where(y => y.PrincipalEntityType != x) //#C
                    .Select(y => y.PrincipalEntityType)).Distinct() //#C
                .ToDictionary(k => k,
                    v => v.GetForeignKeys().Select(y => y.PrincipalEntityType).ToList());

            var result = allEntities
                .Where(x => !principalsDict.ContainsKey(x))
                .ToList();

            var reverseResult = new List<IEntityType>();

            while (principalsDict.Keys.Any())
            {
                foreach (var principalNoLinks in principalsDict.Where(x => !x.Value.Any()).ToList())
                {
                    reverseResult.Add(principalNoLinks.Key);
                    foreach (var removeLink in principalsDict.Where(x => x.Value.Contains(principalNoLinks.Key)))
                    {
                        removeLink.Value.Remove(principalNoLinks.Key);
                    }
                    principalsDict.Remove(principalNoLinks.Key);
                }
            }
            reverseResult.Reverse();

            result.AddRange(reverseResult);

            return result.Select(x => x.Relational().TableName);
        }
        /************************************************************************
        #A This method looks at the relationships and returns the tables names so that the dependent entities come before the principal entities
        #B I catch the hierarchical case where an entity refers to itself - if the delete behavior of this foreign key is set to restrict then you cannot simply delete all the rows in one go
        #C I extract all the principal entities to sort. The dependent entities don't take part in the sort, as we know they must come at the beginning
        #D Now I sort the principal entities in case I have the case of EntityA -> EntityB -> EntityC. In that case EntityB must be deleted before EntityA
        #E If itemA's foreign keys point to itemB, then I must delete itemA first to make sure I don't have a cascade delete problems
        #F If itemB's foreign keys point to itemA, then I must delete itemB first to make sure I don't have a cascade delete problems
        #G Otherwise the order doesn't matter
        #H I now produce combined list with the dependants at the front and the principals at the back
        #I Finally I extract the table names from the ordered list
        * ***********************************************************************/

        public static void WipeAllDataFromDatabase(this DbContext context)
        {
            foreach (var tableName in
                context.GetTableNamesInOrderForWipe())
            {
                context.Database
                    .ExecuteSqlCommand(
                        $"DELETE FROM {tableName}");
            }
        }

        //------------------------------------------------
        //private methods

        private static void ThrowExceptionOnNotWipeableEntities(List<IEntityType> allEntities)
        {
            var cannotWipes = allEntities //#B
                .SelectMany(x => x.GetForeignKeys()         //#B
                .Where(y => y.PrincipalEntityType == x      //#B
                   && y.DeleteBehavior == DeleteBehavior.Restrict))
                   .ToList(); //#B
            if (cannotWipes.Any())
                throw new InvalidOperationException(
                    "You cannot delete all the rows in one go in entity(s): " +
                    string.Join(", ", cannotWipes.Select(x => x.DeclaringEntityType.Name)));
        }


    }
}