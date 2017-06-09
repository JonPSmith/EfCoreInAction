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
            var items = context.Model.GetEntityTypes()
                .ToList();

            items.Sort((itemA, itemB) =>
                {
                    var fKeysA = itemA.GetForeignKeys()
                        .ToList(); //#B
                    if (fKeysA.SingleOrDefault(x =>
                                x.PrincipalEntityType == itemA)
                            ?.DeleteBehavior ==
                        DeleteBehavior.Restrict) //#C
                        throw new InvalidOperationException(
                            $"You cannot delete all the {itemA} rows in one go.");

                    if (fKeysA.Any(
                        x => x.PrincipalEntityType == itemB))
                        return -1; //#D
                    var fKeysB = itemB.GetForeignKeys().ToList();
                    if (fKeysB.Any(
                        x => x.PrincipalEntityType == itemA))
                        return 1; //#E
                    return 0;
                }
            );
            return items.Select(x => x.Relational().TableName);
        }
        /************************************************************************
        #A This method looks at the relationships and returns the tables names so that the dependent entities come before the principal entities
        #B I use the IEntityType's GetForeignKeys method to find all the foreign keys in this entity
        #C I catch the Hierarchical case where an entity refers to itself - if the delete behavior of this foreign key is set to restrict then you cannot simply delete all the rows in one go
        #D If itemA's foreign keys point to itemB, then I must delete itemA first to make sure I don't have a cascade delete problems
        #E If itemB's foreign keys point to itemA, then I must delete itemB first to make sure I don't have a cascade delete problems
        #F It itemA has no foreign keys then it is likely to be a principal, so we put it later in the list
        #F It itemB has no foreign keys then it is likely to be a principal, so we put it later in the list
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