// Copyright (c) 2017 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using DataLayer.EfClasses;
using Microsoft.EntityFrameworkCore;

namespace Test.Chapter07Listings.EfClasses
{
    public class Ch07Book
    {
        private decimal _normalPrice;

        public int BookId { get; set; }

        public string Title { get; set; }

        public decimal CachedPrice { get; private set; }

        public PriceOffer Promotion { get; private set; }

        public void SetNormalPrice(DbContext context, decimal normalPrice)
        {
            _normalPrice = normalPrice;
            MakeSureAnyPromotionIsLoaded(context);
            CachedPrice = Promotion?.NewPrice ?? _normalPrice;
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
            CachedPrice = _normalPrice;
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