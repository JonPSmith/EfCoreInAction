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

        public static string GetColumnStoreType<TEntity, TProperty>(this DbContext context, 
            TEntity source, Expression<Func<TEntity, TProperty>> model) where TEntity : class
        {
            var efType = context.Model.FindEntityType(typeof(TEntity).FullName);
            var propInfo = GetPropertyInfoFromLambda(model);
            return efType.FindProperty(propInfo.Name).Relational().ColumnType;
        }

        public static string GetColumnStoreType<TEntity>(this DbContext context, string propertyName) where TEntity : class
        {
            var efType = context.Model.FindEntityType(typeof(TEntity).FullName);
            return efType.FindProperty(propertyName).Relational().ColumnType;
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

        private static PropertyInfo GetPropertyInfoFromLambda<TEntity, TProperty>(Expression<Func<TEntity, TProperty>> model) where TEntity : class
        {
            var memberEx = (MemberExpression)model.Body;
            if (memberEx == null)
                throw new ArgumentNullException("model", "You must supply a LINQ expression that is a property.");

    }
}