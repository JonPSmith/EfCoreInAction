using DataLayer.EfClasses;

namespace ServiceLayer.AdminServices
{
    public interface IAddReviewService
    {
        string BookTitle { get; }

        Review GetBlankReview(int id) //#A
            ;

        Book AddReviewToBook(Review review)//#D
            ;
    }
}