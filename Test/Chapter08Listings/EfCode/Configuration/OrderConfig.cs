// Copyright (c) 2017 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Test.Chapter08Listings.EfClasses;

namespace Test.Chapter08Listings.EfCode.Configuration
{
    public static class OrderConfig
    {
        public static void ConfigureOrder
            (this ModelBuilder modelBuilder)
        {
            modelBuilder.HasSequence<int>( //#A
                    "OrderNumbers", "shared") //#A
                .StartsAt(1000)   //#B
                .IncrementsBy(5); //#B

            modelBuilder.Entity<Order>()
                .Property(o => o.OrderNo)
                .HasDefaultValueSql(
                    "NEXT VALUE FOR shared.OrderNumbers"); //#C
        }
        /**************************************************************
        #A This creates a sequence ‘OrderNumber’ in the schema ‘shared’. If no schema is provided it will use the default schema.
        #B These are optional, and allow you to control the squence start and increment. The default is start at 1 and increments by 1
        #C A column can access the sequence number via a default constraint. Each time the ‘NEXT VALUE…’ command is called the sequence is incremented.
         * ***********************************************************/
    }
}
