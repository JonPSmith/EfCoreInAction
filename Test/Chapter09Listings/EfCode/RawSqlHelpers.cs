// Copyright (c) 2017 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using Microsoft.EntityFrameworkCore;
using test.EfHelpers;

namespace Test.Chapter09Listings.EfCode
{
    public static class RawSqlHelpers
    {
        public const string FilterOnReviewRank = "FilterOnReviewRank";

        public static void AddUpdateSqlProcs(this DbContext context)
        {

            using (var transaction = context.Database.BeginTransaction())
            {
                try
                {
                    context.Database.ExecuteSqlCommand(
                        $"IF OBJECT_ID('dbo.{FilterOnReviewRank}') IS NOT NULL " +
                        $"DROP PROC dbo.{FilterOnReviewRank}");

                    context.Database.ExecuteSqlCommand(
                        $"CREATE PROC dbo.{FilterOnReviewRank}" +
                        @"(  @RankFilter int )
AS

SELECT * FROM dbo.Books b 
WHERE (SELECT AVG(CAST([NumStars] AS float)) FROM dbo.Review AS r WHERE b.BookId = r.BookId) >= @RankFilter
");



                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    //I leaft this in because you noramlly would catch the expection and return an error.
                    throw;

                }
            }
        }

        public static bool EnsureSqlProcsSet(this DbContext context)
        {
            var connection = context.Database.GetDbConnection().ConnectionString;
            return connection.ExecuteRowCount("sysobjects", $"WHERE type='P' AND name='{FilterOnReviewRank}'") == 1;
        }
    }
}