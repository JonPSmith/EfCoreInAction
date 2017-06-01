using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace DataLayer.EfCode
{
    public class SaveChangesSqlCheck
    {
        private readonly DbContext _context;
        private readonly Dictionary<int, FormatSqlException> _sqlMethodDict;

        public SaveChangesSqlCheck(DbContext context, 
            Dictionary<int, FormatSqlException> sqlMethodDict) //#A
        {
            _context = context 
                       ?? throw new ArgumentNullException(nameof(context));
            _sqlMethodDict = sqlMethodDict 
                             ?? throw new ArgumentNullException(nameof(sqlMethodDict));
        }

        public ValidationResult SaveChangesWithSqlChecks() //#B
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
}