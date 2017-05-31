// Copyright (c) 2017 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System.Linq;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using DataLayer.EfClasses;
using ServiceLayer.BookServices;
using test.EfHelpers;
using Test.Chapter10Listings.MappingClasses;
using Xunit;
using Xunit.Abstractions;
using Xunit.Extensions.AssertExtensions;

namespace test.UnitTests.ServiceLayer
{
    public class Ch10_AutoMapperSelectQuery
    {
        private readonly ITestOutputHelper _output;

        public Ch10_AutoMapperSelectQuery(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void MapBookAndPriceOffer()
        {
            //SETUP
            var inMemDb = new SqliteInMemory();  //REMOVE FROM BOOK

            var config = new MapperConfiguration(cfg => { //#A
                cfg.CreateMap<Book, BookPriceOfferDto>(); //#A
            });
            using (var context = inMemDb.GetContextWithSetup())
            {
                context.SeedDatabaseFourBooks();  //REMOVE FROM BOOK

                //ATTEMPT
                var result = context.Books. //#B
                    ProjectTo<BookPriceOfferDto>(config) //#C
                    .ToList(); //#D
            /*************************************************************
            #A I have to tell AutoMapper to create a map between the Book entity class and the BookPriceOfferDto class. It does this by matching the names in both classes
            #B I use the normal access to the books via the application's DbContext DbSet<T> propery, Books
            #C I use AutoMapper's ProjectTo<T> method to create the LINQ needed to map the Book, and its navigational properties to the DTO
            #D Finally I use ToList to get EF Core to build and execute the database access
             * *************************************************************/
             
                //VERIFY
                result.Count.ShouldEqual(4);
                result.Where(x => x.Title != "Quantum Networking").All(x => x.PromotionNewPrice == null).ShouldBeTrue();
                result.Last().PromotionNewPrice.ShouldNotBeNull();
                foreach (var log in inMemDb.Logs)
                {
                    _output.WriteLine(log);
                }
            }
        }


        [Fact]
        public void MapBookListDto()
        {
            //SETUP
            var inMemDb = new SqliteInMemory();

            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Book, BookListDto>()
                    .ForMember(p => p.ActualPrice,
                        m => m.MapFrom(s => s.Promotion == null ? s.Price : s.Promotion.NewPrice))
                    .ForMember(p => p.AuthorsOrdered, m => m.MapFrom(s => string.Join(", ",
                        s.AuthorsLink
                            .OrderBy(q => q.Order)
                            .Select(q => q.Author.Name))))
                    .ForMember(p => p.ReviewsAverageVotes,
                        m => m.MapFrom(s => s.Reviews.Count == 0
                            ? null
                            : (decimal?) s.Reviews
                                .Select(q => q.NumStars).Average()));
            });
            using (var context = inMemDb.GetContextWithSetup())
            {
                context.SeedDatabaseFourBooks();  //REMOVE FROM BOOK
                inMemDb.ClearLogs();

                //ATTEMPT
                var result = context.Books.
                    ProjectTo<BookListDto>(config)
                    .ToList();

                //VERIFY
                result.Count.ShouldEqual(4);
                var qNetBook = result.Last();
                qNetBook.Title.ShouldEqual("Quantum Networking");
                qNetBook.ActualPrice.ShouldEqual(219);
                qNetBook.PromotionPromotionalText.ShouldEqual("Save $1 if you order 40 years ahead!");
                qNetBook.ReviewsCount.ShouldEqual(2);
                qNetBook.ReviewsAverageVotes.ShouldEqual(5);
                foreach (var log in inMemDb.Logs)
                {
                    _output.WriteLine(log);
                }
            }
        }
    }
}