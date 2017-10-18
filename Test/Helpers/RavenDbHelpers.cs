// Copyright (c) 2017 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using DataLayer.NoSql;
using DataNoSql;
using Raven.Client;
using test.EfHelpers;

namespace test.Helpers
{
    internal static class RavenDbHelpers
    {
        public static int NumEntriesInDb(this RavenStore creator)
        {
            try
            {
                using (var context = creator.CreateNoSqlAccessor())
                {
                    return context.BookListQuery().Count();
                }
            }
            catch (InvalidOperationException e)
            {
                return -1;
            }
        }


        public static IEnumerable<BookListNoSql> CreateDummyBooks(int numBooks = 10, bool stepByYears = false)
        {
            var booksQueryable = EfTestData.CreateDummyBooks(numBooks, stepByYears).AsQueryable();
            foreach (var book in booksQueryable)
            {
                yield return booksQueryable.ProjectBook(book.BookId);
            }

        }

        public static void SeedDummyBooks(this RavenStore creator, int numBooks = 10, bool stepByYears = false)
        {
            var updater = creator.CreateSqlUpdater();
            {
                updater.BulkLoad(CreateDummyBooks(numBooks, stepByYears).ToList());
            }
        }

    }
}