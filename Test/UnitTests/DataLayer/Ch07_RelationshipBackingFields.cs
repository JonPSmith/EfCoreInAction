// Copyright (c) 2017 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using DataLayer.EfClasses;
using Microsoft.EntityFrameworkCore;
using test.EfHelpers;
using Test.Chapter07Listings.EfClasses;
using Test.Chapter07Listings.EFCode;
using Xunit;
using Xunit.Extensions.AssertExtensions;

namespace test.UnitTests.DataLayer
{
    public class Ch07_RelationshipBackingFields
    {
        [Fact]
        public void TestCreateBookPriceOk()
        {
            //SETUP
            using (var context = new Chapter07DbContext(SqliteInMemory.CreateOptions<Chapter07DbContext>()))
            {
                //var logs = new List<string>();
                //SqliteInMemory.SetupLogging(context, logs);

                context.Database.EnsureCreated();

                //ATTEMPT
                var entity = new Ch07Book
                {
                    Title = "Quantem Networking"
                };
                entity.SetNormalPrice(context, 210);

                //VERIFY
                entity.CachedPrice.ShouldEqual(210);
            }
        }

        [Fact]
        public void TestCreateBookWithPromotionOk()
        {
            //SETUP
            using (var context = new Chapter07DbContext(SqliteInMemory.CreateOptions<Chapter07DbContext>()))
            {
                context.Database.EnsureCreated();

                //ATTEMPT
                var entity = new Ch07Book
                {
                    Title = "Quantem Networking"
                };
                entity.SetNormalPrice(context, 210);
                entity.AddUpdatePromotion(context, new PriceOffer {NewPrice = 111, PromotionalText = "Test"});

                //VERIFY
                entity.CachedPrice.ShouldEqual(111);
            }
        }

        [Fact]
        public void TestWriteBookOk()
        {
            //SETUP
            using (var context = new Chapter07DbContext(SqliteInMemory.CreateOptions<Chapter07DbContext>()))
            {
                //var logs = new List<string>();
                //SqliteInMemory.SetupLogging(context, logs);

                context.Database.EnsureCreated();

                var entity = new Ch07Book
                {
                    Title = "Quantem Networking"
                };
                entity.SetNormalPrice(context, 210);
                //ATTEMPT
                context.Add(entity);
                context.SaveChanges();

                //VERIFY
                context.Books.Count().ShouldEqual(1);
            }
        }

        [Fact]
        public void TestReadBookOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<Chapter07DbContext>();
            using (var context = new Chapter07DbContext(options))
            {
                context.Database.EnsureCreated();

                var entity = new Ch07Book
                {
                    Title = "Quantem Networking"
                };
                entity.SetNormalPrice(context, 210);
                var logs = new List<string>();
                SqliteInMemory.SetupLogging(context, logs);
                context.Add(entity);
                context.SaveChanges();
            }
            using (var context = new Chapter07DbContext(options))
            {
                //ATTEMPT
                var book = context.Books.Single();

                //VERIFY
                book.CachedPrice.ShouldEqual(210);
            }
        }

        [Fact]
        public void TestReadBookWithPromotionOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<Chapter07DbContext>();
            using (var context = new Chapter07DbContext(options))
            {
                context.Database.EnsureCreated();

                var entity = new Ch07Book
                {
                    Title = "Quantem Networking"
                };
                entity.SetNormalPrice(context, 210);
                entity.AddUpdatePromotion(context, new PriceOffer { NewPrice = 111, PromotionalText = "Test" });

                context.Add(entity);
                context.SaveChanges();
            }
            using (var context = new Chapter07DbContext(options))
            {
                //ATTEMPT
                var book = context.Books.Include(p => p.Promotion).Single();

                //VERIFY
                book.Promotion.ShouldNotBeNull();
                book.CachedPrice.ShouldEqual(111);
            }
        }

        [Fact]
        public void TestBookWithPromotionRemovedOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<Chapter07DbContext>();
            using (var context = new Chapter07DbContext(options))
            {
                context.Database.EnsureCreated();

                var entity = new Ch07Book
                {
                    Title = "Quantem Networking"
                };
                entity.SetNormalPrice(context, 210);
                entity.AddUpdatePromotion(context, new PriceOffer { NewPrice = 111, PromotionalText = "Test" });

                context.Add(entity);
                context.SaveChanges();

                entity.RemovePromotion(context);
                context.SaveChanges();
            }
            using (var context = new Chapter07DbContext(options))
            {
                //ATTEMPT
                var book = context.Books.Include(p => p.Promotion).Single();

                //VERIFY
                book.Promotion.ShouldBeNull();
                book.CachedPrice.ShouldEqual(210);
            }
        }

    }
}