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
        public void ByHandBookAndPriceOffer()
        {
            //SETUP
            var inMemDb = new SqliteInMemory();

            using (var context = inMemDb.GetContextWithSetup())
            {
                context.SeedDatabaseFourBooks();  //REMOVE FROM BOOK

                //ATTEMPT
                var result = context.Books.
                    Select(p => new BookDto
                    {
                        BookId = p.BookId,
                        Title = p.Title,
                        Price = p.Price,
                        PromotionNewPrice = p.Promotion == null
                            ? (decimal?)null
                            : p.Promotion.NewPrice,
                        PromotionPromotionalText = p.Promotion == null
                            ? null
                            : p.Promotion.PromotionalText,
                        Reviews = p.Reviews
                            .Select(x => new ReviewDto
                            {
                                NumStars = x.NumStars
                            })
                            .ToList()
                    })
                    .ToList();

                //VERIFY
                result.Count.ShouldEqual(4);
                result.Where(x => x.Title != "Quantum Networking").All(x => x.PromotionNewPrice == null).ShouldBeTrue();
                result.Last().PromotionNewPrice.ShouldNotBeNull();
                result.Last().PromotionPromotionalText.ShouldNotBeNull();
                foreach (var log in inMemDb.Logs)
                {
                    _output.WriteLine(log);
                }
            }
        }

        [Fact]
        public void MapBookAndPriceOffer()
        {
            //SETUP
            var inMemDb = new SqliteInMemory();  //REMOVE FROM BOOK

            var config = new MapperConfiguration(cfg => {
                cfg.CreateMap<Book, BookDto>(); //#A
                cfg.CreateMap<Review, ReviewDto>(); //#B
            });
            using (var context = inMemDb.GetContextWithSetup())
            {
                context.SeedDatabaseFourBooks();  //REMOVE FROM BOOK

                //ATTEMPT
                var result = context.Books. //#C
                    ProjectTo<BookDto>(config) //#D
                    .ToList(); //#E
            /*************************************************************
            #A I have to tell AutoMapper to create a map between the Book entity class and the BookPriceOfferDto class. It does this by matching the names in both classes
            #B I also add a mapping from the Review entity class and the ReviewDto class. This is a nested DTO as it is used in the BookPriceOfferDto class
            #C I use the normal access to the books via the application's DbContext DbSet<T> propery, Books
            #D I use AutoMapper's ProjectTo<T> method to create the LINQ needed to map the Book, and its navigational properties to the DTO
            #E Finally I use ToList to get EF Core to build and execute the database access
             * *************************************************************/
             
                //VERIFY
                result.Count.ShouldEqual(4);
                result.Where(x => x.Title != "Quantum Networking").All(x => x.PromotionNewPrice == null).ShouldBeTrue();
                result.Last().PromotionNewPrice.ShouldNotBeNull();
                result.Last().PromotionPromotionalText.ShouldNotBeNull();
                result.Last().Reviews.Count.ShouldEqual(2);
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
                        m => m.MapFrom(s => s.Promotion == null 
                        ? s.Price : s.Promotion.NewPrice))
                    .ForMember(p => p.AuthorsOrdered, 
                        m => m.MapFrom(s => string.Join(", ",
                            s.AuthorsLink
                            .OrderBy(q => q.Order)
                            .Select(q => q.Author.Name))))
                    .ForMember(p => p.ReviewsAverageVotes,
                        m => m.MapFrom(s => s.Reviews.Count == 0
                            ? null
                            : (double?) s.Reviews
                                .Select(q => q.NumStars)
                                .Average()));
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