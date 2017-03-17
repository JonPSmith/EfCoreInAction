using DataLayer.EfClasses;

namespace ServiceLayer.AdminServices
{
    public interface IChangePubDateService
    {
        ChangePubDateDto GetOriginal(int id);
        Book UpdateBook(ChangePubDateDto dto);
    }
}