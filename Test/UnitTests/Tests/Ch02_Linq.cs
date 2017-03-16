// Copyright (c) 2016 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using Xunit.Extensions.AssertExtensions;

namespace test.UnitTests.Tests
{
    public class Ch02_Linq
    {

        [Fact]
        public void SimpleLinqExampleChapter01()
        {
            //SETUP
            var numsQ = new[] { 1, 5, 4, 2, 3 }
                 .AsQueryable();            //#A

            var result = numsQ
                .OrderBy(x => x)            //#B
                .Where(x => x > 3)          //#C
                .ToArray();                 //#D
            /**************************************************
            A# This turns an array of integers into a queryable object
            B# First we order the numbers
            c# Then we filter out all the numbers 3 and below
            D# This turns the query back into an array. The result is an array of ints { 4, 5 }
             * *************************************************/
            //VERIFY
            result.ShouldEqual(new[] { 4, 5 });
        }




        [Fact]
        public void TestMySpecialMethod()
        {
            MySpecialMethod().ShouldEqual(new[] { 4, 5 });
        }

        public IQueryable<int>
            FilterAndOrder(IQueryable<int> original)
        {
            return original
                .Where(n => n > 2)
                .OrderBy(n => n);
        }

        public int[] MySpecialMethod()
        {
            var numsQ = new[] {1, 5, 4, 2, 3}
                .AsQueryable();                 //#A

            //ATTEMPT
            var part1 = FilterAndOrder(numsQ);  //#B
            var part2 = part1.Skip(1);          //#C

            var result = part2.ToArray();       //#D

            return result;
        }
        /*****************************************************************
         #A The array is turned into IQueryable<int>
         #B LINQ commands to filter and sort are added, and returned as IQueryable<int>
         #C This adds a command to skip the first item after the previous commands
         #D The ToArray causes the LINQ commands to be executed, and the returns an int array
         ****************************************************************/

        [Fact]
        public void TestSortStringsAndLen()
        {
            SortStringsAndLen().ShouldEqual(new List<Tuple<int, string>>
            {
                new Tuple<int, string>(5, "short"),
                new Tuple<int, string>(6, "larger"),
                new Tuple<int, string>(9, "very long"),
            });
        }

        public List<Tuple<int, string>> SortStringsAndLen()
        {
            var strQ = new[] 
                { "short", "very long", "larger" }
                .AsQueryable();

            var query = from str in strQ             //#A
                        let len = str.Length         //#B
                        orderby len                  //#C
                        select new 
                        Tuple<int, string>(len, str);//#D
            return query.ToList();                   //#E
        }
        /****************************************************
        #A This is a Standard Query’ style LINQ command
        #B The 'let' keyword allows you to hold a emp variable
        #C We order the results by the length of the string
        #D We also put the lenfth of the string in the result
        #E As with the lambda style we need a command to execute the query
         * *************************************************/

    }
}