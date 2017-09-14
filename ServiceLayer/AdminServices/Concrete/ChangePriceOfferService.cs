// Copyright (c) 2017 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System.Linq;
using DataLayer.EfClasses;
using DataLayer.EfCode;

namespace ServiceLayer.AdminServices.Concrete
{
    public class ChangePriceOfferService : IChangePriceOfferService
    {
        private readonly EfCoreContext _context;

        public ChangePriceOfferService(EfCoreContext context)
        {
            _context = context;
        }

        public ChangePriceOfferDto GetOfferData(int id) 
        {
            var book = _context.Find<Book>(id);
            return new ChangePriceOfferDto
            {
                BookId = book.BookId,
                Title = book.Title,
                NewPrice = book.ActualPrice,
                OrgPrice = book.OrgPrice,
                PromotionalText = book.PromotionalText
            };
        }

        public string AddPromotion(ChangePriceOfferDto dto)
        {
            var book = _context.Find<Book>(dto.BookId);
            var error = book.AddPromotion(dto.NewPrice, dto.PromotionalText);
            if (error != null)
                return error;
            _context.SaveChanges(); 
            return null;     
        }

        public void RemovePromotion(int bookId)
        {
            var book = _context.Find<Book>(bookId);
            book.RemovePromotion();
            _context.SaveChanges();
        }
    }

}
