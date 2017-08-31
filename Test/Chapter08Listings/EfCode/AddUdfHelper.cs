// Copyright (c) 2017 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using Microsoft.EntityFrameworkCore;

namespace Test.Chapter08Listings.EfCode
{
    public static class AddUdfHelper
    {
        public const string UdfAverageVotes = nameof(Chapter08EfCoreContext.AverageVotesUdf);

        public static void AddUdfToDatabase(this DbContext context)
        {
            using (var transaction = context.Database.BeginTransaction())
            {
                try
                {
                    context.Database.ExecuteSqlCommand(
                        $"IF OBJECT_ID('dbo.{UdfAverageVotes}', N'FN') IS NOT NULL " +
                        $"DROP FUNCTION dbo.{UdfAverageVotes}");

                    context.Database.ExecuteSqlCommand(
                        $"CREATE FUNCTION {UdfAverageVotes} (@bookId int)" +
                        @"  RETURNS float
  AS
  BEGIN
  DECLARE @result AS float
  SELECT @result = AVG(CAST([NumStars] AS float)) FROM dbo.Review AS r
       WHERE @bookId = r.BookId
  RETURN @result
  END");
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    //I left this in because you noramlly would catch the expection and return an error.
                    throw;
                }
            }
        }
    }
}
