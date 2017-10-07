// Copyright (c) 2017 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using DataLayer.NoSql;
using Microsoft.Extensions.Configuration;
using Raven.Client;
using Raven.Client.Document;
using test.EfHelpers;

namespace test.Helpers
{
    internal static class RavenDbHelpers
    {
        public static DocumentStore FormDocumentStore()
        {
            var ravenDbTestConnection = AppSettings.GetConfiguration().GetConnectionString("RavenDb-Test");
            var infoDict = (from part in ravenDbTestConnection.Split(';')
                    let namedPart = part.Substring(0, part.IndexOf('='))
                    select new {key = namedPart, value = part.Substring(namedPart.Length + 1)})
                .ToDictionary(x => x.key, y => y.value);
            return new DocumentStore
            {
                Url = infoDict["Url"],
                DefaultDatabase = infoDict["Database"],
                ApiKey = infoDict["ApiKey"]
            };
        }

        public static int NumEntriesInDb(this IDocumentStore store)
        {
            try
            {
                using (IDocumentSession session = store.OpenSession())
                {
                    return session.Query<BookNoSqlDto>().Count();
                }
            }
            catch (InvalidOperationException e)
            {
                return -1;
            }
        }


        public static IEnumerable<BookNoSqlDto> CreateDummyBooks(int numBooks = 10, bool stepByYears = false)
        {
            var booksQueryable = EfTestData.CreateDummyBooks(numBooks, stepByYears).AsQueryable();
            foreach (var book in booksQueryable)
            {
                yield return BookNoSqlDto.ProjectBook(booksQueryable, book.BookId);
            }

        }

        public static void SeedDummyBooks(this IDocumentStore store, int numBooks = 10, bool stepByYears = false)
        {
            using (var session = store.OpenSession())
            {
                foreach (var dto in CreateDummyBooks(numBooks, stepByYears))
                {
                    session.Store(dto);
                    session.SaveChanges();
                }
            }
        }

    }
}