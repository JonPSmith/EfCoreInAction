using DataLayer.EfClasses;

namespace ServiceLayer.AdminServices
{
    public interface IAddReviewService
    {
        string GetTitleOfBook(int id);

        void AddReviewToBook(int bookId, int numStars, string comment, string voterName);
    }
}