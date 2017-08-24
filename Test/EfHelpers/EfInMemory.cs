// Copyright (c) 2016 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using DataLayer.EfCode;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace test.EfHelpers
{
    public class EfInMemory
    {
        public static DbContextOptions<EfCoreContext> CreateNewContextOptions()
        {
            // Create a fresh service provider, and therefore a fresh 
            // InMemory database instance.
            var serviceProvider = new ServiceCollection()
                .AddEntityFrameworkInMemoryDatabase()
                .BuildServiceProvider();

            // Create a new options instance telling the context to use an in-memory database
            var builder = new DbContextOptionsBuilder<EfCoreContext>();
            builder.UseInMemoryDatabase(Guid.NewGuid().ToString()) //the database name is set to a unique Guid
                   .UseInternalServiceProvider(serviceProvider);

            return builder.Options;
        }
    }
}