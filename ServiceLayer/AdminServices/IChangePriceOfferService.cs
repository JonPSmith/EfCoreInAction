namespace ServiceLayer.AdminServices
{
    public interface IChangePriceOfferService
    {
        ChangePriceOfferDto GetOfferData(int id);
        string AddPromotion(ChangePriceOfferDto dto);
        void RemovePromotion(int bookId);
    }
}