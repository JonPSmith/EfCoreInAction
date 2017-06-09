// Copyright (c) 2017 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage;

namespace test.EfHelpers
{
    public static class DatabaseMetadata
    {
        public static string GetTableName<TEntity>(this DbContext context)
        {
            var efType = context.Model.FindEntityType(typeof(TEntity).FullName);
            var relational = efType.Relational();
            return relational.TableName;
        }

        //NOTE: This will not handle a circular relationship: e.g. EntityA->EntityB->EntityA
        public static  IEnumerable<string> 
            GetTableNamesInOrderForDelete //#A
                (this DbContext context)
        {
            var cannotWipe = context.Model.GetEntityTypes() //#B
                .FirstOrDefault(x => x.GetForeignKeys()     //#B
                .SingleOrDefault(y => y.PrincipalEntityType == x) //#B
                ?.DeleteBehavior == DeleteBehavior.Restrict); //#B
            if (cannotWipe != null)
                throw new InvalidOperationException(
                    $"You cannot delete all the {cannotWipe} rows in one go.");

            var principals = context.Model.GetEntityTypes()    //#C
                .SelectMany(x => x.GetForeignKeys()            //#C
                .Select(y => y.PrincipalEntityType)).Distinct()//#C
                .ToList();                                     //#C

            principals.Sort((itemA, itemB) => //#D
                {
                    var fKeysA = itemA.GetForeignKeys()
                        .ToList();
                    if (fKeysA.Any(
                        x => x.PrincipalEntityType == itemB))
                        return -1; //#E
                    var fKeysB = itemB.GetForeignKeys().ToList();
                    if (fKeysB.Any(
                        x => x.PrincipalEntityType == itemA))
                        return 1; //#F
                    return 0; //#G
                }
            );
            var allEntitiesInOrder =                  //#H
                context.Model.GetEntityTypes()        //#H
                .Where(x => !principals.Contains(x))  //#H
                .Union(principals);                   //#H
            return allEntitiesInOrder.Select(x => x.Relational().TableName);//#I
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

        public static IEnumerable<IProperty> GetProperties<TEntity>(this DbContext context)
        {
            var efType = context.Model.FindEntityType(typeof(TEntity).FullName);
            return efType.GetProperties();
        }

        public static string GetColumnName<TEntity, TProperty>(this DbContext context, TEntity source, 
            Expression<Func<TEntity, TProperty>> model) where TEntity : class
        {
            var efType = context.Model.FindEntityType(typeof(TEntity).FullName);
            var propInfo = GetPropertyInfoFromLambda(model);
            return efType.FindProperty(propInfo.Name).Relational().ColumnName;
        }

        public static string GetColumnName<TEntity>(this DbContext context, string propertyName) where TEntity : class
        {
            var efType = context.Model.FindEntityType(typeof(TEntity).FullName);
            return efType.FindProperty(propertyName).Relational().ColumnName;
        }

        public static string GetColumnNameSqlite<TEntity, TProperty>(this DbContext context, TEntity source,
            Expression<Func<TEntity, TProperty>> model) where TEntity : class
        {
            var efType = context.Model.FindEntityType(typeof(TEntity).FullName);
            var propInfo = GetPropertyInfoFromLambda(model);
            return efType.FindProperty(propInfo.Name).Sqlite().ColumnName;
        }

        public static string GetColumnStoreType<TEntity, TProperty>(this DbContext context, 
            TEntity source, Expression<Func<TEntity, TProperty>> model) where TEntity : class
        {
            var efType = context.Model.FindEntityType(typeof(TEntity).FullName);
            var propInfo = GetPropertyInfoFromLambda(model);
            var efProperty = efType.FindProperty(propInfo.Name);

            return GetStoreType(context, efProperty);
        }

        public static string GetColumnStoreType<TEntity>(this DbContext context, string propertyName) where TEntity : class
        {
            var efType = context.Model.FindEntityType(typeof(TEntity).FullName);
            var efProperty = efType.FindProperty(propertyName);

            return GetStoreType(context, efProperty);
        }

        //---------------------------------------------------
        //internal methods

        internal static PropertyInfo GetPropertyInfoFromLambda<TEntity, TProperty>(Expression<Func<TEntity, TProperty>> model) where TEntity : class
        {
            var memberEx = (MemberExpression)model.Body;
            if (memberEx == null)
                throw new ArgumentNullException("model", "You must supply a LINQ expression that is a property.");

            var propInfo = typeof(TEntity).GetProperty(memberEx.Member.Name);
            if (propInfo == null)
                throw new ArgumentNullException("model", "The member you gave is not a property.");
            return propInfo;
        }

        //-----------------------------------------------------
        //private methods

        private static string GetStoreType(DbContext context, Microsoft.EntityFrameworkCore.Metadata.IProperty efProperty)
        {
            var typeMapper = context.GetService<IRelationalTypeMapper>();
            var mappings = typeMapper.FindMapping(efProperty);

            return mappings.StoreType;
        }


    }
}