// Copyright (c) 2016 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.Globalization;
using System.Linq;
using DataLayer.EfCode;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Logging;
using test.EfHelpers;
using test.Helpers;
using Xunit;
using Xunit.Abstractions;
using Xunit.Extensions.AssertExtensions;

namespace test.UnitTests.DataLayer
{
    public class Ch02_ClientServerEvaluation
    {
        private readonly ITestOutputHelper _output;

        public Ch02_ClientServerEvaluation(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void TestClientServerComplexBookOk()
        {
            //SETUP
            var options =
                this.ClassUniqueDatabaseSeeded4Books();

            using (var context = new EfCoreContext(options))
            {
                var logIt = new LogDbContext(context, LogLevel.Debug);

                //ATTEMPT
                var book = context.Books
                    .Select(p => new
                    {
                        p.BookId,                                //#A
                        p.Title,                                 //#A
                        //… other properties left out 
                        AuthorsString = string.Join(", ",        //#B
                            p.AuthorsLink                        //#A
                            .OrderBy(q => q.Order)               //#A
                            .Select(q => q.Author.Name)),        //#A
                    }
                    ).First();
                /*********************************************************
                #A These parts of the select can be converted to SQL and run on the server
                #B The String.Join is executed on the client in software
                * *******************************************************/

                //VERIFY
                book.AuthorsString.Length.ShouldBeInRange(1, 100);
                foreach (var log in logIt.Logs)
                {
                    _output.WriteLine(log);
                }
            }
        }

        //[Fact]
        //public void TestClientServerSortOnEvaluatedPortion()
        //{
        //    //SETUP
        //    var options =
        //        this.ClassUniqueDatabaseSeeded4Books();

        //    using (var context = new EfCoreContext(options))
        //    {
        //        var logIt = new LogDbContext(context);

        //        //ATTEMPT
        //        var books = context.Books
        //            .Select(p => new
        //            {
        //                p.BookId,                        
        //                p.Title,                         
        //                //… other properties left out 
        //                AuthorsString = string.Join(", ",
        //                    p.AuthorsLink                
        //                    .OrderBy(q => q.Order)       
        //                    .Select(q => q.Author.Name)),
        //            }
        //            ).OrderBy(p => p.AuthorsString).ToList();


        //        //VERIFY
        //        books.Select(x => x.BookId).ShouldEqual(new []{3,4,1,2});
        //        foreach (var log in logIt.Logs)
        //        {
        //            _output.WriteLine(log);
        //        }
        //    }
        //}

        [Fact]
        public void TestClientServerSortOnEvaluatedPortionThorwError()
        {
            //SETUP
            var options =
                    this.ClassUniqueDatabaseSeeded4Books();

            using (var context = new EfCoreContext(options))
            {
                //ATTEMPT
                var ex = Assert.Throws<InvalidOperationException>(() => context.Books
                    .Select(p => new
                        {
                            p.BookId,
                            p.Title,
                            //… other properties left out 
                            AuthorsString = string.Join(", ",
                                p.AuthorsLink
                                    .OrderBy(q => q.Order)
                                    .Select(q => q.Author.Name)),
                        }
                    ).OrderBy(p => p.AuthorsString).ToList());


                //VERIFY
                Assert.Contains("could not be translated.", ex.Message);
            }
        }
    }
}