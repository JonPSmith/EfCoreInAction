// Copyright (c) 2017 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System.Linq;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using DataLayer.EfClasses;
using test.EfHelpers;
using Test.Chapter10Listings.MappingClasses;
using Xunit;
using Xunit.Extensions.AssertExtensions;

namespace Test.UnitTests.ServiceLayer
{
    public class Ch10_AutoMapperSelectQuery
    {


        [Fact]
        public void MapBookAndPriceOffer()
        {
            //SETUP
            var config = new MapperConfiguration(cfg => {
                cfg.CreateMap<Book, BookPriceOfferDto>();
            });
            var inMemDb = new SqliteInMemory();

            using (var context = inMemDb.GetContextWithSetup())
            {
                context.SeedDatabaseFourBooks();

                //ATTEMPT
                var dtos = context.Books.ProjectTo<BookPriceOfferDto>(config).ToList();

                //VERIFY
                dtos.Count.ShouldEqual(4);
                dtos.Last().PromotionNewPrice.ShouldNotBeNull();
            }
        }



    }
}