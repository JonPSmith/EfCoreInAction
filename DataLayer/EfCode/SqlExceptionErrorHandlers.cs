// // Copyright (c) 2017 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// // Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace DataLayer.EfCode
{
    public static class SqlExceptionErrorHandlers
    {
        private static readonly Regex UniqueConstraintRegex = new Regex("'UniqueError_([a-zA-Z0-9]*)_([a-zA-Z0-9]*)'",
            RegexOptions.Compiled);

        /// <summary>
        /// Generalised Unique Key handler. if it finds a string in the form 'UniqueError_EntityName_PropertyName'
        /// Then makes a friendly message, otherwise returns null
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="entitiesNotSaved"></param>
        /// <returns></returns>
        public static ValidationResult UniqueConstraintExceptionHandler(SqlException ex, IEnumerable<EntityEntry> entitiesNotSaved)
        {
            var message = ex.Errors[0].Message;
            var matches = UniqueConstraintRegex.Matches(ex.Errors[0].Message);

            if (matches.Count == 0)
                //fails to match our Unique Key format
                return null;

            var returnError = $"Cannot have a duplicate {matches[0].Groups[2].Value} in {matches[0].Groups[1].Value}.";

            var openingBadValue = message.IndexOf("(", StringComparison.OrdinalIgnoreCase);
            if (openingBadValue > 0)
                returnError += $" Duplicate value was '{message.Substring(openingBadValue + 1, message.Length - openingBadValue - 3)}'.";

            return new ValidationResult(returnError, new[] { matches[0].Groups[2].Value });
        }
    }
}