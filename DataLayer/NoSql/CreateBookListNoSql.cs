// Copyright (c) 2017 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System.Linq;
using DataLayer.EfClasses;
using DataNoSql;

namespace DataLayer.NoSql
{
    public static class CreateBookListNoSql
    {
        public static BookListNoSql ProjectBook(this IQueryable<Book> books, int bookId)
        {
            var result = books.Select(p => new
            {
                p.BookId,
                authors = p.AuthorsLink
                    .OrderBy(q => q.Order)
                    .Select(q => q.Author.Name).ToList(),
                bookList = new BookListNoSql
                {
                    Id = BookListNoSql.ConvertIdToNoSqlId(bookId),
                    Title = p.Title,
                    Price = p.Price,
                    PublishedOn = p.PublishedOn,
                    ActualPrice = p.Promotion == null
                        ? p.Price
                        : p.Promotion.NewPrice,
                    PromotionPromotionalText =
                        p.Promotion == null
                            ? null
                            : p.Promotion.PromotionalText,
                    ReviewsCount = p.Reviews.Count,
                    ReviewsAverageVotes = p.Reviews.Select(y => (double?)y.NumStars).Average()
                }
            }).Single(x => x.BookId == bookId);

            result.bookList.AuthorsOrdered = string.Join(", ", result.authors);

            return result.bookList;
        }
    }
}