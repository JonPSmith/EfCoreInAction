// Copyright (c) 2016 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using Microsoft.EntityFrameworkCore;

namespace test.Chapter03Listings
{
    public class SimpleDbContext : DbContext
    {
        public DbSet<ExampleEntity> SingleEntities { get; set; }

        public SimpleDbContext(
            DbContextOptions<SimpleDbContext> options)
            : base(options)
        {}
    }
}