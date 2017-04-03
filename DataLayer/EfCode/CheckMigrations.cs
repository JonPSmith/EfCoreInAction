using System;
using System.Linq;
using DataLayer.EfCode;
using Microsoft.EntityFrameworkCore;

namespace TryAspNetCoreMirgate.EfCore
{
    public static class CheckMigrations
    {
        private static bool _cachedOutstandingMigrations = true;

        public static void ThrowExceptionIfPendingMigrations(this DbContext context, bool ignoreMigations = false)
        {
            if (!ignoreMigations && HasOutstandingMigrations(context))
                throw new OutstandingMigrationException();
        }

        /// <summary>
        /// This is a safe method that Unit Test can call to clear the cache state
        /// </summary>
        internal static void UnitTestClearCache()
        {
            _cachedOutstandingMigrations = true;
        }

        private static bool HasOutstandingMigrations(DbContext context)
        {
            if (_cachedOutstandingMigrations)
            {
                //We either don't know, or there are migrations, so we have to check every time
                _cachedOutstandingMigrations = context.Database.GetPendingMigrations().Any();
            }
            return _cachedOutstandingMigrations;
        }
    }
}