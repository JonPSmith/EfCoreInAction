// Copyright (c) 2017 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using DataLayer.EfClasses;
using DataNoSql;

namespace DataLayer.NoSql
{
    public static class CreateBookListNoSql
    {
        private class BookWithParts
        {
            public int BookId { get; set; }
            public List<string> Authors { get; set; }
            public BookListNoSql Book { get; set; }

            public void UpdateBooksAuthorsOrdered()
            {
                Book.AuthorsOrdered = string.Join(", ", Authors);
            }
        }

        public static BookListNoSql ProjectBook(this IQueryable<Book> books, int bookId)
        {
            var results = books.ProjectBooksNoAuthors();
            var result = results.Single(x => x.BookId == bookId);
            result.UpdateBooksAuthorsOrdered();
            return result.Book;
        }

        public static IList<BookListNoSql> ProjectBooks(this IQueryable<Book> books)
        {
            var results = books.ProjectBooksNoAuthors().ToList();
            results.ForEach(x => x.UpdateBooksAuthorsOrdered());
            return results.Select(x => x.Book).ToList();
        }

        private static IQueryable<BookWithParts> ProjectBooksNoAuthors(this IQueryable<Book> books)
        {
            return books.Select(p => new BookWithParts
            {
                BookId = p.BookId,
                Authors = p.AuthorsLink
                    .OrderBy(q => q.Order)
                    .Select(q => q.Author.Name).ToList(),
                Book = new BookListNoSql
                {
                    Id = BookListNoSql.ConvertIdToNoSqlId(p.BookId),
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
            });
        }

    }
}