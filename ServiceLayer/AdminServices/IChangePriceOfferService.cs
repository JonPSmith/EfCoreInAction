using DataLayer.EfClasses;

namespace ServiceLayer.AdminServices
{
    public interface IChangePriceOfferService
    {
        Book OrgBook { get; }

        PriceOffer GetOriginal(int id)      //#A
            ;

        Book UpdateBook(PriceOffer promotion)//#D
            ;
    }
}