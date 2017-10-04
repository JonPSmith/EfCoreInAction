// Copyright (c) 2017 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using DataLayer.EfClasses;
using DataLayer.EfCode;
using Newtonsoft.Json;

namespace ServiceLayer.DatabaseServices.Concrete
{
    public class BookGenerator
    {
        private readonly bool _makeBookTitlesDistinct;
        private readonly ImmutableList<BookData> _loadedBookData;
        private Dictionary<string, Author> _authorDict = new Dictionary<string, Author>();
        private int NumBooksInSet => _loadedBookData.Count;

        public ImmutableDictionary<string, Author> AuthorDict => _authorDict.ToImmutableDictionary();

        public BookGenerator(string filePath, bool makeBookTitlesDistinct = false)
        {
            _makeBookTitlesDistinct = makeBookTitlesDistinct;
            _loadedBookData = JsonConvert.DeserializeObject<List<BookData>>(File.ReadAllText(filePath))
                .ToImmutableList();
        }

        public class BookData
        {
            public DateTime PublishDate { get; set; }
            public string Title { get; set; }
            public string Authors { get; set; }
        }

        public void WriteBooks(int numBooks, EfCoreContext context, Func<int, bool> progessCancel)
        {
            _authorDict = context.Authors.ToDictionary(k => k.Name);
            var numWritten = 0;
            var batch = new List<Book>();
            foreach (var book in GenerateBooks(numBooks))
            {
                batch.Add(book);
                if (batch.Count < NumBooksInSet) continue;

                //have a batch to write out
                if (progessCancel(numWritten))
                {
                    return;
                }
                context.AddRange(batch);
                context.SaveChanges();
                numWritten += batch.Count;
                batch.Clear();
            }

            //write any final batch out
            if (batch.Count > 0)
            {
                context.AddRange(batch);
                context.SaveChanges();
                numWritten += batch.Count;
            }
            progessCancel(numWritten);
        }

        public IEnumerable<Book> GenerateBooks(int numBooks)
        {
            for (int i = 0; i < numBooks; i++)
            {
                var reviews = new List<Review>();
                for (int j = 0; j < i % 12; j++)
                {
                    reviews.Add(new Review { VoterName = j.ToString(), NumStars = (Math.Abs(3 - j) % 4) + 2 });
                }
                var book = new Book
                {
                    Title = _loadedBookData[i % _loadedBookData.Count].Title,
                    Description = $"Book{i:D4} Description",
                    Price = (i + 1),
                    PublishedOn = _loadedBookData[i % _loadedBookData.Count].PublishDate,
                    Publisher = "Manning",
                    Reviews = reviews,
                    AuthorsLink = new List<BookAuthor>()
                };
                if (i >= NumBooksInSet && _makeBookTitlesDistinct)
                    book.Title += $" (copy {Math.Truncate((i * 1.0) / NumBooksInSet)})";
                if (i % 7 == 0)
                {
                    book.Promotion = new PriceOffer
                    {
                        NewPrice = book.Price * 0.5m,
                        PromotionalText = "today only - 50% off! "
                    };
                }

                AddAuthorsToBook(book, _loadedBookData[i % _loadedBookData.Count].Authors);
                yield return book;
            }
        }

        private void AddAuthorsToBook(Book book, string authors)
        {
            byte order = 0;
            foreach(var authorName in ExtractAuthorsFromBookData(authors))
            {
                if (!_authorDict.ContainsKey(authorName))
                {
                    _authorDict[authorName] = new Author {Name = authorName};
                }
                book.AuthorsLink.Add(new BookAuthor { Book = book, Author = _authorDict[authorName], Order = order++});
            }
        }

        private static IEnumerable<string> ExtractAuthorsFromBookData(string authors)
        {
            return authors.Replace(" and ", ",").Replace(" with ", ",")
                .Split(',').Select(x => x.Trim()).Where(x => x.Length > 1);
        }
    }
}