// Copyright (c) 2017 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using Microsoft.EntityFrameworkCore;

namespace DataLayer.SqlCode
{
    public static class UdfDefinitions
    {
        public const string SqlScriptName = "AddUserDefinedFunctions.sql";

        public static void RegisterUdfDefintions(this ModelBuilder modelBuilder)
        {
            modelBuilder.HasDbFunction(
                    () => AuthorsStringUdf(default(int)))
                .HasSchema("dbo");
            modelBuilder.HasDbFunction(
                    () => ActualPriceUdf(default(int)))
                .HasSchema("dbo");
        }

        public static double? AverageVotesUdf(int bookId)
        {
            throw new Exception();
        }

        public static string AuthorsStringUdf(int bookId)
        {
            throw new Exception();
        }

        public static decimal ActualPriceUdf(int bookId)
        {
            throw new Exception();
        }
    }
}