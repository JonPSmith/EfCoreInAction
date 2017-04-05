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

                try
                {
                    context.Database.Migrate();
                }
                catch (Exception e)
                {
                    //Add logging here!
                    migrationLogs.Add($"{e.GetType().Name} during Migration. See logs for details.");
                    return migrationLogs;
                }
                migrationLogs.Add("Migration was successful.");
                if (seedDatabase != null)
                {
                    try
                    {
                        seedDatabase.Invoke(context);
                    }
                    catch (Exception e)
                    {
                        //Add logging here!
                        migrationLogs.Add($"{e.GetType().Name} during seeding the database. See logs for details.");
                        return migrationLogs;
                    }
                    migrationLogs.Add("Successfully ran database seed method.");
                }

                return migrationLogs;
            }
        }
    }
}