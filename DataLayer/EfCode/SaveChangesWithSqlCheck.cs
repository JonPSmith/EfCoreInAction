// // Copyright (c) 2017 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// // Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace DataLayer.EfCode
{
    /// <summary>
    /// This is the signiture of the method called on a SqlException happening in SaveChangesWithChecking (sync and async)
    /// </summary>
    /// <param name="exception">This is the Sql Exception that occured</param>
    /// <param name="entitiesThatErrored">DbEntityEntry objects that represents the entities that could not be saved to the database</param>
    /// <returns>return ValidationResult with error, or null if cannot handle this error</returns>
    public delegate ValidationResult FormatSqlException(
        System.Data.SqlClient.SqlException exception, IReadOnlyList<EntityEntry> entitiesThatErrored);

    public class SaveChangesWithSqlCheck
    {
        private readonly DbContext _context;
        private readonly Dictionary<int, FormatSqlException> _sqlMethodDict;

        public SaveChangesWithSqlCheck(DbContext context, 
            Dictionary<int, FormatSqlException> sqlMethodDict) //#A
        {
            _context = context 
                ?? throw new ArgumentNullException(nameof(context));
            _sqlMethodDict = sqlMethodDict 
                ?? throw new ArgumentNullException(nameof(sqlMethodDict));
        }

        public ValidationResult SaveChangesWithChecking() //#B
        {
            try
            {
                _context.SaveChanges(); //#C
            }
            catch (DbUpdateException e) //#D
            {
                var error = CheckHandleError(e); //#E
                if (error != null)
                {
                    return error; //#F
                }
                throw; //#G
            }
            return null; //#H
        }

        private ValidationResult CheckHandleError //#I
            (DbUpdateException e)
        {
            var sqlEx = e.InnerException as SqlException; //#J
            if (sqlEx != null
                && _sqlMethodDict
                   .ContainsKey(sqlEx.Number)) //#K
            {
                return 
                    _sqlMethodDict[sqlEx.Number] //#L
                        (sqlEx, e.Entries); //#L
            }
            return null; //#M
        }
    }
    /****************************************************************************
    #A The developer provides a dictionary of all the sql errors they want to format, plus a method to do that formatting
    #B This is the method you call to do the SaveChanges, but also capture and format the sql errors you have registered
    #C I call SaveChanges inside an try...catch block
    #D I catch the DbUpdateException to see if I have a formatter for that SQL error
    #E This method will either return a ValidationError if it could format the error, or null if it couldn't
    #F It managed to format the error, so return that
    #G It didn't manage to format the error, so we re-throe the original error
    #H If it gets to here then there were no errors, so it returns null to show that
    #I This private method handles the lookup and calling of any error formaters that have been registered
    #J I try to convert the InnerException to SqlException. It will be null if InnerException is null, or the InnerException wasn't of type SqlException
    #K This only passes if the InnerException was an SqlException, and our dictionary contains a method to format the error message
    #L I call that formatting method, which has a predefined sigiture, and I return its result.
    #M Otherwise I return null to say I couldn't format the error

     * *************************************************************************/
}