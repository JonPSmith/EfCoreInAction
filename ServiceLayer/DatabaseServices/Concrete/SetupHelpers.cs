// Copyright (c) 2016 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using DataLayer.EfCode;
using EfCoreInAction.DatabaseHelpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;

namespace ServiceLayer.DatabaseServices.Concrete
{
    public enum DbStartupModes { UseExisting, EnsureCreated, EnsureDeletedCreated, UseMigrations}

    public static class SetupHelpers
    {
        private const string BranchNameFilename = "GitBranchName.txt";

        private const string SeedDataSearchName = "Apress books*.json";
        public const string SeedFileSubDirectory = "seedData";

        public static string GetBranchName(this string workingDirectory)
        {
            var gitBranchFilePath = Path.Combine(workingDirectory, BranchNameFilename);
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

        public static void DevelopmentEnsureCreated(this EfCoreContext db)
        {
            db.Database.EnsureCreated();

        }

        public static int SeedDatabase(this EfCoreContext context, string dataDirectory)
        {
            if (!(context.GetService<IDatabaseCreator>() as RelationalDatabaseCreator).Exists())
                throw new InvalidOperationException("The database does not exist. If you are using Migrations then run PMC command update-database to create it");

            var numBooks = context.Books.Count();
            if (numBooks == 0)
            {
                //the database is emply so we fill it from a json file
                var books = BookJsonLoader.LoadBooks(Path.Combine(dataDirectory, SeedFileSubDirectory),
                    SeedDataSearchName).ToList();
                context.Books.AddRange(books);
                context.SaveChanges();
                //We add this separately so that it has the highest Id. That will make it appear at the top of the default list
                context.Books.Add(SpecialBook.CreateSpecialBook());
                context.SaveChanges();
                numBooks = books.Count + 1;
            }

            return numBooks;
        }
    }
}