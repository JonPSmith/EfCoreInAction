// Copyright (c) 2017 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DataLayer.EfClasses;
using DataLayer.EfCode;
using Newtonsoft.Json;

namespace ServiceLayer.DatabaseServices.Concrete
{
    public class BookGenerator
    {
        public int WriteBatchSize { get; set; } = 500;

        private Dictionary<string, Author> _authorDict;
        public class BookData
        {
            public DateTime PublishDate { get; set; }
            public string Title { get; set; }
            public string Authors { get; set; }
        }

        public void WriteBooks(string filePath, int numBooks, EfCoreContext context, Func<int, bool> progessCancel)
        {
            var numWritten = 0;
            var batch = new List<Book>();
            foreach (var book in GenerateBooks(filePath, numBooks))
            {
                batch.Add(book);
                if (batch.Count < WriteBatchSize) continue;

                //have a btach to write out
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

        public IEnumerable<Book> GenerateBooks(string filePath, int numBooks)
        {
            var templateBooks = JsonConvert.DeserializeObject<List<BookData>>(File.ReadAllText(filePath));

            _authorDict = new Dictionary<string, Author>();
            for (int i = 0; i < numBooks; i++)
            {
                var reviews = new List<Review>();
                for (int j = 0; j < i % 12; j++)
                {
                    reviews.Add(new Review { VoterName = j.ToString(), NumStars = (Math.Abs(3 - j) % 4) + 2 });
                }
                var book = new Book
                {
                    Title = templateBooks[i % templateBooks.Count].Title,
                    Description = $"Book{i:D4} Description",
                    Price = (i + 1),
                    PublishedOn = templateBooks[i % templateBooks.Count].PublishDate,
                    Publisher = "Manning",
                    Reviews = reviews,
                    AuthorsLink = new List<BookAuthor>()
                };
                if (i % 7 == 0)
                {
                    book.Promotion = new PriceOffer
                    {
                        NewPrice = book.Price * 0.5m,
                        PromotionalText = "today only - 50% off! "
                    };
                }

                AddAuthorsToBook(book, templateBooks[i % templateBooks.Count].Authors);
                yield return book;
            }
        }

        private void AddAuthorsToBook(Book book, string authors)
        {
            byte order = 0;
            foreach(var authorName in authors.Replace(" and ", ",").Replace(" with ", ",")
                .Split(',').Select(x => x.Trim()).Where(x => x.Length > 1))
            {
                if (!_authorDict.ContainsKey(authorName))
                {
                    _authorDict.Add(authorName, new Author {Name = authorName});
                }
                book.AuthorsLink.Add(new BookAuthor { Book = book, Author = _authorDict[authorName], Order = order++});
            }
        }
    }
}