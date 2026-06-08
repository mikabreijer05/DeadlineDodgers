using System;
using System.Collections.Generic;
using System.Linq;
using Dapper;
using KE03_INTDEV_SE_2_Base.Models;

namespace KE03_INTDEV_SE_2_Base.DAL
{
    public class SQLReview : SQLDAL
    {
        public IEnumerable<Review> GetPendingReviews()
        {
            const string sql = @"
            SELECT
                r.RevId             AS ReviewId,
                r.ProdId            AS ProductId,
                p.ProdName          AS ProductName,
                a.AccName           AS Username,
                r.ReviewRating      AS Rating,
                r.ReviewTitle       AS Title,
                r.reviewDescription AS Comment,
                r.reviewDate        AS CreatedDate,
                CASE WHEN r.ReviewStatus = 'Approved' THEN 1 ELSE 0 END AS IsVerified,
                r.ReviewStatus      AS ReviewStatus
            FROM dbo.Review r
            LEFT JOIN dbo.Account a ON a.AccId = r.reviewAccount
            LEFT JOIN dbo.Product p ON p.ProductId = r.ProdId
            WHERE r.ReviewStatus = 'Pending'
            ORDER BY r.reviewDate DESC;
            ";

            try
            {
                connection.Open();
                return connection.Query<Review>(sql).ToList();
            }
            finally
            {
                CloseConnection();
            }
        }

        public IEnumerable<Review> GetVerifiedReviews()
        {
            const string sql = @"
            SELECT
                r.RevId             AS ReviewId,
                r.ProdId            AS ProductId,
                p.ProdName          AS ProductName,
                a.AccName           AS Username,
                r.ReviewRating      AS Rating,
                r.ReviewTitle       AS Title,
                r.reviewDescription AS Comment,
                r.reviewDate        AS CreatedDate,
                CASE WHEN r.ReviewStatus = 'Approved' THEN 1 ELSE 0 END AS IsVerified,
                r.ReviewStatus      AS ReviewStatus
            FROM dbo.Review r
            LEFT JOIN dbo.Account a ON a.AccId = r.reviewAccount
            LEFT JOIN dbo.Product p ON p.ProductId = r.ProdId
            WHERE r.ReviewStatus = 'Approved'
            ORDER BY r.reviewdate DESC;
            ";

            try
            {
                connection.Open();
                return connection.Query<Review>(sql).ToList();
            }
            finally
            {
                CloseConnection();
            }
        }

        public IEnumerable<Review> GetAllReviews()
        {
            const string sql = @"
            SELECT
                r.RevId             AS ReviewId,
                r.ProdId            AS ProductId,
                p.ProdName          AS ProductName,
                a.AccName           AS Username,
                r.ReviewRating      AS Rating,
                r.ReviewTitle       AS Title,
                r.reviewDescription AS Comment,
                r.reviewDate        AS CreatedDate,
                CASE WHEN r.ReviewStatus = 'Approved' THEN 1 ELSE 0 END AS IsVerified,
                r.ReviewStatus      AS ReviewStatus
            FROM dbo.Review r
            LEFT JOIN dbo.Account a ON a.AccId = r.reviewAccount
            LEFT JOIN dbo.Product p ON p.ProductId = r.ProdId
            ORDER BY r.reviewdate DESC;
            ";

            try
            {
                connection.Open();
                return connection.Query<Review>(sql).ToList();
            }
            finally
            {
                CloseConnection();
            }
        }

        public Review GetReviewById(int reviewId)
        {
            const string sql = @"
            SELECT
                r.RevId             AS ReviewId,
                r.ProdId            AS ProductId,
                p.ProdName          AS ProductName,
                a.AccName           AS Username,
                r.ReviewRating      AS Rating,
                r.ReviewTitle       AS Title,
                r.reviewDescription AS Comment,
                r.reviewDate        AS CreatedDate,
                CASE WHEN r.ReviewStatus = 'Approved' THEN 1 ELSE 0 END AS IsVerified,
                r.ReviewStatus      AS ReviewStatus
            FROM dbo.Review r
            LEFT JOIN dbo.Account a ON a.AccId = r.reviewAccount
            LEFT JOIN dbo.Product p ON p.ProductId = r.ProdId
            WHERE r.RevId = @ReviewId;
            ";

            try
            {
                connection.Open();
                return connection.QueryFirstOrDefault<Review>(sql, new { ReviewId = reviewId });
            }
            finally
            {
                CloseConnection();
            }
        }

        public void ApproveReview(int reviewId)
        {
            const string sql = "UPDATE dbo.Review SET ReviewStatus = 'Approved' WHERE RevId = @ReviewId;";

            try
            {
                connection.Open();
                connection.Execute(sql, new { ReviewId = reviewId });
            }
            finally
            {
                CloseConnection();
            }
        }

        public void RejectReview(int reviewId)
        {
            const string sql = "UPDATE dbo.Review SET ReviewStatus = 'Rejected' WHERE RevId = @ReviewId;";

            try
            {
                connection.Open();
                connection.Execute(sql, new { ReviewId = reviewId });
            }
            finally
            {
                CloseConnection();
            }
        }

        public void DeleteReview(int reviewId)
        {
            const string sql = "DELETE FROM dbo.Review WHERE RevId = @ReviewId;";

            try
            {
                connection.Open();
                connection.Execute(sql, new { ReviewId = reviewId });
            }
            finally
            {
                CloseConnection();
            }
        }

        public int GetPendingReviewsCount()
        {
            const string sql = "SELECT COUNT(*) FROM dbo.Review WHERE ReviewStatus = 'Pending';";

            try
            {
                connection.Open();
                return connection.QueryFirstOrDefault<int>(sql);
            }
            finally
            {
                CloseConnection();
            }
        }
    }
}
