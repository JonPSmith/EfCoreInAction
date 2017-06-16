// Copyright (c) 2017 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

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
            (this DbContext context, 
             int maxDepth = 10, params Type[] excludeTypes)
        {
            var allEntities = context.Model
                .GetEntityTypes()
                .Where(x => !excludeTypes.Contains(x.ClrType))
                .ToList(); //#B

            ThrowExceptionOnNotWipeableEntities(allEntities); //#C

            var principalsDict = allEntities             //#D
                .SelectMany(x => x.GetForeignKeys()
                    .Select(y => y.PrincipalEntityType)).Distinct()
                .ToDictionary(k => k, v => //#E
                    v.GetForeignKeys()
                        .Where(y => y.PrincipalEntityType != v)
                        .Select(y => y.PrincipalEntityType).ToList());

            var result = allEntities //#F
                .Where(x => !principalsDict.ContainsKey(x)) //#F
                .ToList(); //#F

            var reversePrincipals = new List<IEntityType>();
            int depth = 0;
            while (principalsDict.Keys.Any()) //#G
            {
                foreach (var principalNoLinks in
                    principalsDict
                        .Where(x => !x.Value.Any()).ToList())//#H
                {
                    reversePrincipals.Add(principalNoLinks.Key);//#I
                    principalsDict
                        .Remove(principalNoLinks.Key);//#J
                    foreach (var removeLink in
                        principalsDict.Where(x =>
                            x.Value.Contains(principalNoLinks.Key)))//#K
                    {
                        removeLink.Value
                            .Remove(principalNoLinks.Key);//#K
                    }
                }
                if (depth++ > maxDepth)
                    ThrowExceptionMaxDepthReached(principalsDict.Keys.ToList(), depth);
            }
            reversePrincipals.Reverse();//#M
            result.AddRange(reversePrincipals);//#N
            return result.Select(FormTableNameWithSchema);//#O
        }
        /************************************************************************
        #A This method looks at the relationships and returns the tables names in the right order to wipe all their rows without incurring a foreign key delete constraint
        #B This gets me the IEntityType, which contains all the information on how the database is built, for all the entities in the DbContext
        #C This contains a check for the hierarchical (entity that references itself) case where an entity refers to itself - if the delete behavior of this foreign key is set to restrict then you cannot simply delete all the rows in one go
        #D I extract all the principal entities...
        #E ... And put them in a dictionary, with the value being all the links to other principal entities. Note I remove any self reference links as these are automatically handled
        #F I start the list of entities to delete by putting all the dependant entities first, as I must delete the rows in these first, and the order doesn't matter
        #G While there are entities with links to other entities I need to keep going round
        #H Now loop through all the relationships that don't have a link to another principal (or that link has already been marked as wiped)
        #I I mark the entity for deletion - this list is in reverse order to what I must do
        #J I remove it from the dictionary so that it isn't looked at again
        #K ... and remove the reference to that entity from any existing dependants still in the dictionary
        #M When I get to here I have the list of entities in the reverse order to how I should wipe them, so I reverse the list
        #N I now produce combined list with the dependants at the front and the principals at the back in the right order
        #O Finally I return a collection of table names, with a optional schema, in the right order
        * ***********************************************************************/

        public static void WipeAllDataFromDatabase(this DbContext context, int maxDepth = 10, params Type[] excludeTypes)
        {
            foreach (var tableName in
                context.GetTableNamesInOrderForWipe(maxDepth, excludeTypes))
            {
                context.Database
                    .ExecuteSqlCommand(
                        $"DELETE FROM {tableName}");
            }
        }

        //------------------------------------------------
        //private methods

        private static string FormTableNameWithSchema(IEntityType entityType)
        {
            var relational = entityType.Relational();
            return "[" + (relational.Schema == null
                       ? ""
                       : relational.Schema + "].[")
                   + relational.TableName + "]";
        }

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

        private static void ThrowExceptionMaxDepthReached(List<IEntityType> principalsDictKeys, int maxDepth)
        {
            throw new InvalidOperationException(
                $"It looked to a depth of {maxDepth} and didn't finish. Possible circular reference?\nentity(s) left: " +
                string.Join(", ", principalsDictKeys.Select(x => x.ClrType.Name)));
        }


    }
}