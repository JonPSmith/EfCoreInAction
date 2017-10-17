// Copyright (c) 2017 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using Microsoft.Extensions.Logging;

namespace DataNoSql
{
    public interface IQueryCreator
    {
        INoSqlAccessor CreateNoSqlAccessor(ILogger logger);
    }
}