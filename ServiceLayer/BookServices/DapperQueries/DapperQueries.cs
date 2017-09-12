// Copyright (c) 2016 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System.Collections.Generic;
using Dapper;
using DataLayer.EfCode;
using Microsoft.EntityFrameworkCore;

namespace ServiceLayer.BookServices.DapperQueries
{
    public static class DapperQueries
    {
        public static IEnumerable<BookListDto>
            BookListQuery(this EfCoreContext context, SortFilterPageOptions options)
        {
             return context.Database.GetDbConnection()
                .Query<BookListDto>(BuildQueryString(options), new
                    {
                        pageSize = options.PageSize,
                        skipRows = options.PageSize * (options.PageNum - 1)
                    });
        }

        private static string BuildQueryString
            (SortFilterPageOptions options)
        {
            var selectOptTop = options.PageNum <= 1
                ? "SELECT TOP(@pageSize) "
                : "SELECT ";
            var optOffset = options.PageNum <= 1
                ? ""
                : " OFFSET @skipRows ROWS FETCH NEXT @pageSize ROWS ONLY";

            var command = selectOptTop +
                          @"[b].[BookId], [b].[Title], [b].[Price], [b].[PublishedOn], CASE
    WHEN [p.Promotion].[PriceOfferId] IS NULL
    THEN [b].[Price] ELSE [p.Promotion].[NewPrice]
END AS [ActualPrice], 
[p.Promotion].[PromotionalText] AS [PromotionPromotionalText], 
[dbo].AuthorsStringUdf([b].[BookId]) AS [AuthorsOrdered], 
(
    SELECT COUNT(*)
    FROM [Review] AS [r]
    WHERE [b].[BookId] = [r].[BookId]
) AS [ReviewsCount], (
    SELECT AVG(CAST([y].[NumStars] AS float))
    FROM [Review] AS [y]
    WHERE [b].[BookId] = [y].[BookId]
) AS [ReviewsAverageVotes]
FROM [Books] AS [b]
LEFT JOIN [PriceOffers] AS [p.Promotion] ON [b].[BookId] = [p.Promotion].[BookId]
WHERE [b].[SoftDeleted] = 0
ORDER BY [b].[BookId] DESC" + optOffset;

            return command;
        }
    }
}