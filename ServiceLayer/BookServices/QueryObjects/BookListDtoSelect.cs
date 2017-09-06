// Copyright (c) 2016 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System.Linq;
using DataLayer.EfClasses;
using DataLayer.SqlCode;

namespace ServiceLayer.BookServices.QueryObjects
{
    public static class BookListDtoSelect
    {
        public static IQueryable<BookListDto>             //#A
            MapBookToDto(this IQueryable<Book> books)     //#A
        {
            return books.Select(p => new BookListDto
            {
                BookId = p.BookId,                        //#B
                Title = p.Title,                          //#B
                Price = p.Price,                          //#B
                PublishedOn = p.PublishedOn,              //#B
                ActualPrice = p.Promotion == null         //#C
                        ? p.Price                             //#C
                        : p.Promotion.NewPrice,               //#C
                PromotionPromotionalText =                //#D
                        p.Promotion == null                   //#D
                          ? null                              //#D
                          : p.Promotion.PromotionalText,      //#D
                AuthorsOrdered = UdfDefinitions.AuthorsStringUdf(p.BookId),
                ReviewsCount = p.Reviews.Count,           //#F
                ReviewsAverageVotes = UdfDefinitions.AverageVotesUdf(p.BookId)
            });
        }
        /*********************************************************
        #A This method takes in IQueryable<Book> and returns IQueryable<BookListDto>
        #B These are simple copies of existing columns in the Books table
        #C This calculates the selling price, which is the normal price, or the promotion price if that relationship exists 
        #D The PromotionalText depends on whether a PriceOffer exists for this book
        #E This obtains an array of Authors' names, in the right order. We are using a Client vs. Server evaluation as we want the author's names combined into one string
        #F We need to calculate how many reviews there are
        #G We cannot calculate the average of zero reviews, so we need to check the count first. EF Core turns the LINQ average into the SQL AVG command that runs on the database
        * *******************************************************/

    }
}