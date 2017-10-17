// Copyright (c) 2017 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using Microsoft.Extensions.Logging;

namespace DataNoSql
{
    public class RavenStoreSettings
    {
        public string ConnnectionString { get; set; }

        public ILogger Logger { get; set; }
    }
}