using DataAccessLayer.Models;

namespace DataAccessLayer.Interfaces;

public interface IReviewRepository
{
    IEnumerable<Review> GetReviewsByProductId(int productId);
    Review GetReviewById(int id);
    void AddReview(Review review);
    void UpdateReview(Review review);
    void DeleteReview(int id);
    double GetAverageRating(int productId);
    int GetReviewCount(int productId);
}