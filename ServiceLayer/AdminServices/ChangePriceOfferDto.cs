// Copyright (c) 2017 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System.ComponentModel.DataAnnotations;
using DataLayer.EfClasses;

namespace ServiceLayer.AdminServices
{
    public class ChangePriceOfferDto
    {
        public int BookId { get; set; }
        public string Title { get; set; }
        public decimal NewPrice { get; set; }
        public decimal OrgPrice { get; set; }
        [MaxLength(Book.PromotionalTextLength)]
        public string PromotionalText { get; set; }
    }
}