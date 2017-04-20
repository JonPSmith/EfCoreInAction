// Copyright (c) 2017 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using DataLayer.EfClasses;
using Microsoft.EntityFrameworkCore;

namespace Test.Chapter07Listings.EfClasses
{
    public class Ch07Book
    {
        public int BookId { get; set; }
        public string Title { get; set; }

        public decimal Price { get; private set; } //#A
        public decimal CachedPrice { get; private set; } //#B
        public PriceOffer Promotion { get; private set; } //#C

    /*********************************************************
    #A This holds the standard price of the 
     * *********************************************************/

        public void SetPrice(DbContext context, decimal normalPrice)
        {
            Price = normalPrice;
            MakeSureAnyPromotionIsLoaded(context);
            CachedPrice = Promotion?.NewPrice ?? Price;
        }

        public void AddUpdatePromotion(DbContext context, PriceOffer promotion)
        {
            MakeSureAnyPromotionIsLoaded(context);
            if (Promotion == null)
            {
                Promotion = promotion;
                CachedPrice = promotion.NewPrice;
            }
            else
            {
                Promotion.NewPrice = promotion.NewPrice;
                Promotion.PromotionalText = promotion.PromotionalText;
            }
        }

        public void RemovePromotion(DbContext context)
        {
            MakeSureAnyPromotionIsLoaded(context);
            if (Promotion == null)
                throw new InvalidOperationException("There was no promotion on this book to remove.");

            context.Remove(Promotion);
            CachedPrice = Price;
        }

        //-----------------------------------------
        //private methods

        private void MakeSureAnyPromotionIsLoaded(DbContext context)
        {
            if (Promotion == null)
            {
                //We need to make sure we have any existing promotion
                context.Entry(this)
                    .Reference(r => r.Promotion).Load();
            }
        }
    }
}