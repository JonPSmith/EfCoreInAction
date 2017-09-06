// Copyright (c) 2017 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DataLayer.EfClasses;
using Newtonsoft.Json;

namespace ServiceLayer.DatabaseServices.Concrete
{
    public class BookGenerator
    {

        private Dictionary<string, Author> _authorDict;
        public class BookData
        {
            public DateTime PublishDate { get; set; }
            public string Title { get; set; }
            public string Authors { get; set; }
        }

        public List<Book> GenerateBooks(string filePath, int numBooks = 100)
        {
            var templateBooks = JsonConvert.DeserializeObject<List<BookData>>(File.ReadAllText(filePath));

            _authorDict = new Dictionary<string, Author>();
            var result = new List<Book>();
            for (int i = 0; i < numBooks; i++)
            {
                var reviews = new List<Review>();
                for (int j = 0; j < i % 7; j++)
                {
                    reviews.Add(new Review { VoterName = j.ToString(), NumStars = (j % 5) + 1 });
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

                AddAuthorsToBook(book, templateBooks[i % templateBooks.Count].Authors);
                result.Add(book);
            }
            return result;
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