// // Copyright (c) 2017 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// // Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.Data.SqlClient;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace DataLayer.EfCode
{
    public static class SqlErrorFormatters
    {
        private static readonly Regex UniqueConstraintRegex = 
            new Regex("'UniqueError_([a-zA-Z0-9]*)_([a-zA-Z0-9]*)'",
                RegexOptions.Compiled);

        /// <summary>
        /// Generalised Unique Key handler. if it finds a string in the form 'UniqueError_EntityName_PropertyName'
        /// Then makes a friendly message, otherwise returns null
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="entitiesNotSaved"></param>
        /// <returns></returns>
        public static ValidationResult UniqueErrorFormatter //#A
            (SqlException ex, //#B
             IReadOnlyList<EntityEntry> entitiesNotSaved) //#C
        {
            var message = ex.Errors[0].Message;
            var matches = UniqueConstraintRegex //#D
                .Matches(message); //#D

            if (matches.Count == 0) //#E
                return null; //#E

            var returnError = "Cannot have a duplicate "+ //#F
                matches[0].Groups[2].Value + " in " +     //#F
                matches[0].Groups[1].Value + ".";         //#F

            var openingBadValue = message.IndexOf("(");   //#G
            if (openingBadValue > 0)
            {
                var dupPart = message.Substring(openingBadValue + 1, //#H
                    message.Length - openingBadValue - 3);           //#H
                returnError += $" Duplicate value was '{dupPart}'."; //#H
            }

            return new ValidationResult(returnError, //#I
                new[] { matches[0].Groups[2].Value }); //#J
        }
        /*****************************************************************
        #A I have created a method to handle the Unique SQL error 
        #B The SqlException is passed in, as this holds the information we need to decode the error
        #C The called provides the entities not saved by default. I don't use this in this case
        #D I use Regex to both check the constraint name matches what I expected, and to extract the entity class name and the property name from the constraint
        #E If there is no match then this isn't an exception that the method is designed to handle. I return null to report that I couldn't handle the exception
        #F I form the first part of the user-friendly message
        #G I know the format of the SQL Violation in unique index error, so I try to extract the duplicate value
        #H I add the information about the duplicate value 
        #I I return the user-friendly error message in a ValidationResult
        #J I also send back the property that the error related to in case this can be used to highlight the offending property on the input form
         * ***************************************************************/
    }
}