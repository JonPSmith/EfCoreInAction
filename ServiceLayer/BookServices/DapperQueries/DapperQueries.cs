// Copyright (c) 2016 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using Dapper;
using DataLayer.EfCode;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Logging;
using ServiceLayer.Logger;

namespace ServiceLayer.BookServices.DapperQueries
{
    public static class DapperQueries
    {
        private class LogIt : IDisposable
        {
            private readonly string _command;
            private readonly ILogger _myLogger;
            private readonly Stopwatch stopwatch = new Stopwatch();

            public LogIt(string command, EfCoreContext context)
            {
                _command = command;
                _myLogger = context.GetService<ILoggerFactory>().CreateLogger(nameof(DapperQueries));
                stopwatch.Start();
            }

            public void Dispose()
            {
                stopwatch.Stop();
                _myLogger.LogInformation(new EventId(1, LogParts.DapperEventName), 
                    $"Dapper Query. Execute time = {stopwatch.ElapsedMilliseconds} ms.\n"+ _command);
            }
        }

        public static IEnumerable<BookListDto>
            BookListQuery(this EfCoreContext context, SortFilterPageOptions options)
        {
            var command = BuildQueryString(options, false);
            using(new LogIt(command, context))
            {
                return context.Database.GetDbConnection()
                    .Query<BookListDto>(command, new
                    {
                        pageSize = options.PageSize,
                        skipRows = options.PageSize * (options.PageNum - 1),
                        filterVal = options.FilterValue
                    });
            }
        }

        public static int BookListCount(this EfCoreContext context, SortFilterPageOptions options)
        {
            var command = BuildQueryString(options, true);
            using (new LogIt(command, context))
            {
                return context.Database.GetDbConnection()
                    .ExecuteScalar<int>(command, new
                    {
                        filterVal = options.FilterValue
                    });
            }
        }

        private static string BuildQueryString
            (SortFilterPageOptions options, bool justCount)
        {
            var selectOptTop = FormSelectPart(options, justCount);
            var filter = FormFilter(options);
            var sort = justCount ? "" : FormSort(options);
            var optOffset = FormOffsetEnding(options, justCount);

            var command = selectOptTop + filter + sort + optOffset + "\n";
            return command;
        }

        private static string FormFilter(SortFilterPageOptions options)
        {
            const string start = "WHERE ([b].[SoftDeleted] = 0) ";
            switch (options.FilterBy)
            {
                case QueryObjects.BooksFilterBy.NoFilter:
                    return start;
                case QueryObjects.BooksFilterBy.ByVotes:
                    return start + @"AND ((
    SELECT AVG(CAST([y0].[NumStars] AS float))
    FROM [Review] AS [y0]
    WHERE [b].[BookId] = [y0].[BookId]
) > @filterVal)";
                case QueryObjects.BooksFilterBy.ByPublicationYear:
                    return start +
@"AND (DATEPART(year, [b].[PublishedOn]) = @filterVal) 
AND ([b].[PublishedOn] <= GETUTCDATE()) ";
            }
            throw new NotImplementedException();
        }

        private static string FormSort(SortFilterPageOptions options)
        {
            const string start = "ORDER BY ";
            switch (options.OrderByOptions)
            {
                case QueryObjects.OrderByOptions.SimpleOrder:
                    return start + "[b].[BookId] DESC ";
                case QueryObjects.OrderByOptions.ByVotes:
                    return start + "[ReviewsAverageVotes] DESC ";
                case QueryObjects.OrderByOptions.ByPublicationDate:
                    return start + "[b].[PublishedOn] DESC ";
                case QueryObjects.OrderByOptions.ByPriceLowestFirst:
                    return start + "[ActualPrice] ";
                case QueryObjects.OrderByOptions.ByPriceHigestFirst:
                    return start + "[ActualPrice] DESC ";
            }
            throw new NotImplementedException();
        }

        private static string FormOffsetEnding(SortFilterPageOptions options, bool justCount)
        {
            return options.PageNum <= 1 || justCount
                ? ""
                : " OFFSET @skipRows ROWS FETCH NEXT @pageSize ROWS ONLY";
        }

        private static string FormSelectPart(SortFilterPageOptions options, bool justCount)
        {
            if (justCount)
                return "SELECT COUNT(*) FROM [Books] AS [b] ";

            var selectOpt =  options.PageNum <= 1
                ? "SELECT TOP(@pageSize) "
                : "SELECT ";

            return selectOpt +
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
";
        }
    }
}