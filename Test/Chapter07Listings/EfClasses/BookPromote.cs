// Copyright (c) 2017 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

namespace Test.Chapter07Listings.EfClasses
{
    public class BookPromote : BookBase
    {
        public decimal DiscountPrice { get; set; }

        public string PromotionMessage { get; set; }

        public override decimal GetPrice()
        {
            return DiscountPrice;
        }

        public decimal GetOrgPrice()
        {
            return _orgPrice;
        }

        public BookPromote() { }

        public BookPromote(BookBase orgBook) : base(orgBook)
        {
            HasPromotion = true;    //I do this because I am using update
        }
    }
}