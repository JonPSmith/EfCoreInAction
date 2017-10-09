// Copyright (c) 2017 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System.Collections.Generic;

namespace DataNoSql
{
    public interface INoSqlUpdater
    {
        void DeleteBook(int bookId);
        void CreateNewBook(BookListNoSql book);
        void UpdateBook(BookListNoSql book);
        void BulkLoad(IList<BookListNoSql> books);
    }
}