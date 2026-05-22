using DataAccessLayer.Interfaces;
using DataAccessLayer.Models;

namespace DataAccessLayer.Repositories;
public class ReviewRepository : IReviewRepository
    {
        private readonly MatrixIncDbContext _context;

        public ReviewRepository(MatrixIncDbContext context)
        {
            _context = context;
        }

        public IEnumerable<Review> GetReviewsByProductId(int productId)
        {
            return _context.Reviews
                .Where(r => r.ProductId == productId)
                .OrderByDescending(r => r.CreatedDate)
                .ToList();
        }

        public Review GetReviewById(int id)
        {
            return _context.Reviews.FirstOrDefault(r => r.Id == id);
        }

        public void AddReview(Review review)
        {
            _context.Reviews.Add(review);
            _context.SaveChanges();
        }

        public void UpdateReview(Review review)
        {
            _context.Reviews.Update(review);
            _context.SaveChanges();
        }

        public void DeleteReview(int id)
        {
            var review = GetReviewById(id);
            if (review != null)
            {
                _context.Reviews.Remove(review);
                _context.SaveChanges();
            }
        }

        public double GetAverageRating(int productId)
        {
            var reviews = GetReviewsByProductId(productId).ToList();
            if (reviews.Count == 0)
                return 0;

            return Math.Round(reviews.Average(r => r.Rating), 1);
        }

        public int GetReviewCount(int productId)
        {
            return _context.Reviews.Count(r => r.ProductId == productId);
        }
    }
