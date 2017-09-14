// Copyright (c) 2016 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System.Linq;
using DataLayer.EfClasses;
using DataLayer.EfCode;
using Microsoft.EntityFrameworkCore;
using test.EfHelpers;
using Xunit;
using Xunit.Abstractions;
using Xunit.Extensions.AssertExtensions;

namespace test.UnitTests.DataLayer
{
    public class Ch03_Delete
    {
        private readonly ITestOutputHelper _output;

        public Ch03_Delete(ITestOutputHelper output)
        {
            _output = output;
        }


        [Fact]
        public void TestBookWithRelatedLinks()
        {
            //SETUP
            var options =
                this.NewMethodUniqueDatabaseSeeded4Books();

            using (var context = new EfCoreContext(options))
            {
                var logIt = new LogDbContext(context);

                //ATTEMPT
                var book = context.Books          
                    .Include(p => p.Reviews)      //#A
                    .Include(p => p.AuthorsLink)  //#A
                    .Single(p => p.Title          //#B
                         == "Quantum Networking");//#B

                context.Books.Remove(book);       //#C
                context.SaveChanges();            //#D
                /**********************************************************
                #A The three Includes make sure that the three dependent relationships are loaded with the Book
                #B This finds the "Quantum Networking" book, which I know has a promotion, 2 reviews and one BookAuthor link
                #B I then delete that book
                #C The SaveChanges calls DetectChanges which finds a tracked Book entity which is marked as deleted. It then deletes its dependent relationships and then deletes the book
                * *******************************************************/

                //VERIFY
                context.Books.Count().ShouldEqual(3);
                context.Set<Review>().Count().ShouldEqual(0);
                context.Set<BookAuthor>().Count().ShouldEqual(3); //three books left, each with a one author
                foreach (var log in logIt.Logs)
                {
                    _output.WriteLine(log);
                }
            }
        }

        [Fact]
        public void TestBookWithRelatedLinksWithoutIncludes()
        {
            //SETUP
            var options =
                this.NewMethodUniqueDatabaseSeeded4Books();

            using (var context = new EfCoreContext(options))
            {
                var logIt = new LogDbContext(context);

                //ATTEMPT
                var book = context.Books
                    .Single(p => p.Title 
                         == "Quantum Networking");

                context.Books.Remove(book); 
                context.SaveChanges();                 

                //VERIFY
                context.Books.Count().ShouldEqual(3);
                context.Set<Review>().Count().ShouldEqual(0);
                context.Set<BookAuthor>().Count().ShouldEqual(3); //three books left, each with a one author
                foreach (var log in logIt.Logs)
                {
                    _output.WriteLine(log);
                }
            }
        }
    }
}