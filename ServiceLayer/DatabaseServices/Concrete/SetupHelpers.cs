// Copyright (c) 2016 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using DataLayer.EfCode;
using EfCoreInAction.DatabaseHelpers;
using Microsoft.EntityFrameworkCore;

namespace ServiceLayer.DatabaseServices.Concrete
{
    public enum DbStartupModes { UseExisting, EnsureCreated, EnsureDeletedCreated, UseMigrations}

    public static class SetupHelpers
    {
        private const string BranchNameFilename = "GitBranchName.txt";

        private const string SeedDataSearchName = "Apress books*.json";
        public const string SeedFileSubDirectory = "seedData";

        public static string GetBranchName(this string dataDirectory)
        {
            var gitBranchFilePath = Path.Combine(dataDirectory, BranchNameFilename);
            return File.ReadAllText(gitBranchFilePath);
        }

        /// <summary>
        /// This forms the connection string with a database name that includes the git branch name
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="gitBranchName"></param>
        /// <returns>returns a connection string with the database name changed by appending that gitBranchName</returns>
        public static string FormDatabaseConnection(this string connectionString, string gitBranchName)
        {
            if (connectionString == null)
                throw new InvalidOperationException("You must set the default connection string in the appsetting file.");
            if (gitBranchName == null)
                throw new InvalidOperationException("I expected a branch name.");

            //In development mode, so we make a new database for each branch, as they could be different
            var builder = new SqlConnectionStringBuilder(connectionString);
            builder.InitialCatalog += $".{gitBranchName}";

            return builder.ToString();
        }

        /// <summary>
        /// This handles database creation and seeding based in the startupMode 
        /// </summary>
        /// <param name="db"></param>
        /// <param name="dataDirectory"></param>
        /// <param name="startupMode"></param>
        /// <returns>Number of books added, or -1 if database already existed</returns>
        public static int EnsureDatabaseCreatedAndSeeded(this EfCoreContext db, string dataDirectory, DbStartupModes startupMode)
        {
            if (startupMode == DbStartupModes.UseMigrations)
            {
                db.Database.Migrate();
            }
            else if (startupMode != DbStartupModes.UseExisting)
            {
                if (startupMode == DbStartupModes.EnsureDeletedCreated)
                    db.Database.EnsureDeleted();

                if (!db.Database.EnsureCreated()) return -1;  //Database exists - leave it as it is
            }

            var numBooks = db.Books.Count();
            if (numBooks == 0)
            {
                //the database is emply so we fill it from a json file
                var books = BookJsonLoader.LoadBooks(Path.Combine(dataDirectory, SeedFileSubDirectory),
                    SeedDataSearchName).ToList();
                db.Books.AddRange(books);
                db.SaveChanges();
                //We add this separately so that it has the highest Id. That will make it appear at the top of the default list
                db.Books.Add(SpecialBook.CreateSpecialBook());
                db.SaveChanges();
                numBooks = books.Count + 1;
            }

            return numBooks;
        }
    }
}