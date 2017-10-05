// // Copyright (c) 2017 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// // Licensed under MIT licence. See License.txt in the project root for license information.

using Microsoft.EntityFrameworkCore;
using Test.Chapter14Listings.EfClasses;

namespace Test.Chapter14Listings.EFCode
{
    public class MappingDbContext : DbContext
    {
        public DbSet<ScalarEntity> ScalarEntities { get; set; }

        public MappingDbContext(
            DbContextOptions<MappingDbContext> options)
            : base(options)
        { }

    }
}