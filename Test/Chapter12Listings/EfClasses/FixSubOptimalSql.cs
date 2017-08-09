// Copyright (c) 2017 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System.Collections;
using System.Collections.Generic;

namespace Test.Chapter12Listings.EfClasses
{
    public class FixSubOptimalSql
    {
        public int FixSubOptimalSqlId { get; set; }

        public string Name { get; set; }

        public decimal? AverageVotes { get; set; }

        public ICollection<Ch12Review> Reviews { get; set; }
    }
}