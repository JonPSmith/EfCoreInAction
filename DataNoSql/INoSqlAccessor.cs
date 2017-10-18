// Copyright (c) 2017 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using Microsoft.Extensions.Logging;
using Raven.Client.Linq;

namespace DataNoSql
{
    public interface INoSqlAccessor : IDisposable
    {
        string Command { get; set; }

        IRavenQueryable<BookListNoSql> BookListQuery();
    }
}