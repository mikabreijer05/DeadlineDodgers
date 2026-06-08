using System;
using System.Collections.Generic;
using System.Linq;
using Dapper;
using KE03_INTDEV_SE_2_Base.Models;

namespace KE03_INTDEV_SE_2_Base.DAL
{
    // Klasse voor alle databasebewerkingen rondom reviews
    public class SQLReview : SQLDAL
    {
        // Haalt alle reviews op die nog wachten op goedkeuring
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
                // Open databaseverbinding
                connection.Open();

                // Voer query uit en zet resultaten om naar Review-objecten
                return connection.Query<Review>(sql).ToList();
            }
            finally
            {
                // Sluit databaseverbinding
                CloseConnection();
            }
        }

        // Haalt alle goedgekeurde reviews op
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

        // Haalt alle reviews op, ongeacht hun status
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

        // Haalt één specifieke review op aan de hand van het ID
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

                // Geeft de review terug of null als deze niet bestaat
                return connection.QueryFirstOrDefault<Review>(
                    sql,
                    new { ReviewId = reviewId }
                );
            }
            finally
            {
                CloseConnection();
            }
        }

        // Keurt een review goed
        public void ApproveReview(int reviewId)
        {
            const string sql =
                "UPDATE dbo.Review SET ReviewStatus = 'Approved' WHERE RevId = @ReviewId;";

            try
            {
                connection.Open();

                // Update de status naar Approved
                connection.Execute(sql, new { ReviewId = reviewId });
            }
            finally
            {
                CloseConnection();
            }
        }

        // Wijst een review af
        public void RejectReview(int reviewId)
        {
            const string sql =
                "UPDATE dbo.Review SET ReviewStatus = 'Rejected' WHERE RevId = @ReviewId;";

            try
            {
                connection.Open();

                // Update de status naar Rejected
                connection.Execute(sql, new { ReviewId = reviewId });
            }
            finally
            {
                CloseConnection();
            }
        }

        // Verwijdert een review definitief uit de database
        public void DeleteReview(int reviewId)
        {
            const string sql =
                "DELETE FROM dbo.Review WHERE RevId = @ReviewId;";

            try
            {
                connection.Open();

                // Verwijder review met opgegeven ID
                connection.Execute(sql, new { ReviewId = reviewId });
            }
            finally
            {
                CloseConnection();
            }
        }

        // Telt hoeveel reviews nog wachten op goedkeuring
        public int GetPendingReviewsCount()
        {
            const string sql =
                "SELECT COUNT(*) FROM dbo.Review WHERE ReviewStatus = 'Pending';";

            try
            {
                connection.Open();

                // Geef het aantal pending reviews terug
                return connection.QueryFirstOrDefault<int>(sql);
            }
            finally
            {
                CloseConnection();
            }
        }
    }
}