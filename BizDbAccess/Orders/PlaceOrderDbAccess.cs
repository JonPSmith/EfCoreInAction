// Copyright (c) 2017 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataLayer.EfClasses;
using DataLayer.EfCode;
using Microsoft.EntityFrameworkCore;

namespace BizDbAccess.Orders
{
    public interface IPlaceOrderDbAccess
    {
        /// <summary>
        /// This finds any books that fits the BookIds given to it
        /// </summary>
        /// <param name="bookIds"></param>
        /// <returns>A dictionary with the BookId as the key, and the Book as the value</returns>
        IDictionary<int, Book> FindBooksByIdsWithPriceOffers(IEnumerable<int> bookIds);

        void Add(Order newOrder);
    }

    public class PlaceOrderDbAccess : IPlaceOrderDbAccess
    {
        private readonly EfCoreContext _context;

        public PlaceOrderDbAccess(EfCoreContext context)//#A
        {
            _context = context;
        }

        /// <summary>
        /// This finds any books that fits the BookIds given to it, with any optional promotion
        /// </summary>
        /// <param name="bookIds"></param>
        /// <returns>A dictionary with the BookId as the key, and the Book as the value</returns>
        public IDictionary<int, Book> 
            FindBooksByIdsWithPriceOffers               
               (IEnumerable<int> bookIds)               
        {
            return _context.Books                       
                .Where(x => bookIds.Contains(x.BookId)) 
                .ToDictionary(key => key.BookId);       
        }

        public void Add(Order newOrder)                 
        {                                               
            _context.Orders.Add(newOrder);              
        }                                               
    }

}