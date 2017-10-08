// Copyright (c) 2016 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System.Data.SqlClient;
using System.IO;
using EfCoreInAction;
using Microsoft.Extensions.Configuration;

namespace test.Helpers
{
    public static class AppSettings
    {
        public const string ConnectionStringName = "DefaultConnection";


        public static IConfigurationRoot GetConfiguration()
        {
            var testDir = Path.Combine(TestFileHelpers.GetSolutionDirectory(), "test");
            var builder = new ConfigurationBuilder()
                .SetBasePath(testDir)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
                .AddEnvironmentVariables()
                .AddUserSecrets<Startup>();
            return builder.Build();
        }

        /// <summary>
        /// This creates a unique database name based on the branch name and the test class name
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static string GetUniqueDatabaseConnectionString<T>(this T testClass, string optionalMethodName = null)
        {
            var config = GetConfiguration();
            var orgConnect = config.GetConnectionString(ConnectionStringName);
            var builder = new SqlConnectionStringBuilder(orgConnect);
            string branchName = GetGitBranchName();

            var extraDatabaseName = $".{branchName}.{typeof(T).Name}";
            if (optionalMethodName != null) extraDatabaseName += $".{optionalMethodName}";

            builder.InitialCatalog += extraDatabaseName;

            return builder.ToString();
        }

        public static string GetGitBranchName()
        {
            var gitBranchFilePath = Path.Combine(TestFileHelpers.GetSolutionDirectory(), @"EfCoreInAction\wwwroot", "GitBranchName.txt");
            var branchName = File.ReadAllText(gitBranchFilePath);
            return branchName;
        }
    }
}