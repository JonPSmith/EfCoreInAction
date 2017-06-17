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
             int maxDepth = 10, params Type[] excludeTypes) //#B
        {
            var allEntities = context.Model
                .GetEntityTypes()
                .Where(x => !excludeTypes.Contains(x.ClrType))
                .ToList(); //#C

            ThrowExceptionOnNotWipeableEntities(allEntities); //#D

            var principalsDict = allEntities             //#E
                .SelectMany(x => x.GetForeignKeys()
                    .Select(y => y.PrincipalEntityType)).Distinct()
                .ToDictionary(k => k, v => //#F
                    v.GetForeignKeys()
                        .Where(y => y.PrincipalEntityType != v)
                        .Select(y => y.PrincipalEntityType).ToList());

            var result = allEntities //#G
                .Where(x => !principalsDict.ContainsKey(x)) //#G
                .ToList(); //#G

            var reversePrincipals = new List<IEntityType>();
            int depth = 0; //#H
            while (principalsDict.Keys.Any()) //#I
            {
                foreach (var principalNoLinks in
                    principalsDict
                        .Where(x => !x.Value.Any()).ToList())//#J
                {
                    reversePrincipals.Add(principalNoLinks.Key);//#K
                    principalsDict
                        .Remove(principalNoLinks.Key);//#L
                    foreach (var removeLink in
                        principalsDict.Where(x =>
                            x.Value.Contains(principalNoLinks.Key)))//#M
                    {
                        removeLink.Value
                            .Remove(principalNoLinks.Key);//#N
                    }
                }
                if (++depth >= maxDepth) //#O
                    ThrowExceptionMaxDepthReached(
                        principalsDict.Keys.ToList(), depth);
            }
            reversePrincipals.Reverse();//#P
            result.AddRange(reversePrincipals);//#Q
            return result.Select(FormTableNameWithSchema);//#R
        }
        /************************************************************************
        #A This method looks at the relationships and returns the tables names in the right order to wipe all their rows without incurring a foreign key delete constraint
        #B You can exclude entity classes that you need to handle yourself, for instance - any references that only contain circular references
        #C This gets the IEntityType for all the entities, other than those that were excluded. This contains the information on how each table is built, with its relationships
        #D This contains a check for the hierarchical (entity that references itself) case where an entity refers to itself - if the delete behavior of this foreign key is set to restrict then you cannot simply delete all the rows in one go
        #E I extract all the principal entities from the entities we are considering ...
        #F ... And put them in a dictionary, with the value being all the links to other principal entities. Note I remove any self reference links as these are automatically handled
        #G I start the list of entities to delete by putting all the dependant entities first, as I must delete the rows in these first, and the order doesn't matter
        #H I keep a count of the times I have been round the loop trying to resolve the the relationships
        #I While there are entities with links to other entities I need to keep going round
        #J Now loop through all the relationships that don't have a link to another principal (or that link has already been marked as wiped)
        #K I mark the entity for deletion - this list is in reverse order to what I must do
        #M I remove it from the dictionary so that it isn't looked at again
        #N ... and remove the reference to that entity from any existing dependants still in the dictionary
        #O If I have overstepped the depth limit I throw an exception, with information on what entities had still to be processed. This can happen for certain circular references.
        #P When I get to here I have the list of entities in the reverse order to how I should wipe them, so I reverse the list
        #Q I now produce combined list with the dependants at the front and the principals at the back in the right order
        #R Finally I return a collection of table names, with a optional schema, in the right order
        * ***********************************************************************/

        public static void WipeAllDataFromDatabase(this DbContext context, 
            int maxDepth = 10, params Type[] excludeTypes)
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