// Copyright (c) 2017 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System.ComponentModel.DataAnnotations;

namespace Test.Chapter07Listings.EfClasses
{
    public class BookBase
    {
        protected decimal _orgPrice;

        public int BookBaseId { get; set; }

        public bool HasPromotion { get; set; }
        [Required]
        public string Title { get; set; }

        public void SetOrgPrice(decimal normalPrice)
        {
            _orgPrice = normalPrice;
        }

        public virtual decimal GetPrice()
        {
            return _orgPrice;
        }

        //ctors
        public BookBase() { }
        protected BookBase(BookBase orgBook)
        {
            _orgPrice = orgBook._orgPrice;
            BookBaseId = orgBook.BookBaseId;
            Title = orgBook.Title;
        }
    }
}