// Copyright (c) 2017 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System.Collections.Generic;
using DataLayer.NoSql;

namespace test.Mocks
{
    public class FakeNoSqlUpdater :INoSqlUpdater
    {
        public List<string> Logs { get; } = new List<string>();

        public string AllLogs => string.Join(",", Logs);

        public void DeleteBook(int bookId)
        {
            Logs.Add($"Delete: BookId = {bookId}");
        }

        public void CreateNewBook(BookNoSqlDto book)
        {
            Logs.Add($"Create: BookId = {book.StringIdAsInt}");
        }

        public void UpdateBook(BookNoSqlDto book)
        {
            Logs.Add($"Update: BookId = {book.StringIdAsInt}");
        }
    }
}