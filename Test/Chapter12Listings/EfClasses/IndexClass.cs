// Copyright (c) 2017 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Test.Chapter12Listings.EfClasses
{
    public class IndexClass
    {
        public int Id { get; set; }

        [MaxLength(100)]
        public string NoIndex { get; set; }

        [MaxLength(100)]
        public string WithIndex { get; set; }
    }
}