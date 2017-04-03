using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace DataLayer.EfCode
{
    public static class HandleMigration
    {
        public static IEnumerable<string> MigrateDatabase(this DbContextOptions<EfCoreContext> options, Action<EfCoreContext> seedDatabase = null)
        {
            using (var context = new EfCoreContext(options, true))
            {
                var migrationLogs = context.Database.GetPendingMigrations().Select(m => $"Applied migration {m}.").ToList();

                if (!migrationLogs.Any())
                    return new[] { "No migrations needed to be applied - Seeding of database not done." };

                context.Database.Migrate();
                if (seedDatabase != null)
                {
                    seedDatabase.Invoke(context);
                    migrationLogs.Add("Finally ran database seed method.");
                }

                return migrationLogs;
            }
        }
    }
}