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

                var book = new Book(
                    templateBooks[i % templateBooks.Count].Title,
                    $"Book{i:D4} Description",
                    templateBooks[i % templateBooks.Count].PublishDate,
                    "Manning",
                    (i + 1),
                    null,
                    GetAuthors(templateBooks[i % templateBooks.Count].Authors).ToArray(),
                    templateBooks[i % templateBooks.Count].Authors
                );
                for (int j = 0; j < i % 12; j++)
                {
                    book.AddReview(new Review { VoterName = j.ToString(), NumStars = (Math.Abs(3 - j) % 4) + 2 });
                }          
                if (i % 7 == 0)
                {
                    book.AddPromotion(book.ActualPrice * 0.5m, "today only - 50% off! ");
                }
                yield return book;
            }
        }

        private IEnumerable<Author> GetAuthors(string authors)
        {
            foreach(var authorName in authors.Replace(" and ", ",").Replace(" with ", ",")
                .Split(',').Select(x => x.Trim()).Where(x => x.Length > 1))
            {
                if (!_authorDict.ContainsKey(authorName))
                {
                    _authorDict.Add(authorName, new Author {Name = authorName});
                }
                yield return _authorDict[authorName];
            }
        }
    }
}