using Dapper;

using KE03_INTDEV_SE_1_Base.Models;

namespace KE03_INTDEV_SE_1_Base.DAL;

public class SQLReview : SQLDAL
{
    public IEnumerable<Review> GetReviewsByProductId(int productId)
    {
        const string sql = @"
            SELECT
                r.RevId             AS Id,
                r.ProdId            AS ProductId,
                a.AccName           AS Username,
                r.ReviewRating      AS Rating,
                r.ReviewTitle       AS Title,
                r.reviewDescription AS Comment,
                r.reviewDate        AS CreatedDate,
                CASE WHEN r.ReviewStatus = 'Verified' THEN 1 ELSE 0 END AS IsVerified
            FROM dbo.Review r
            LEFT JOIN dbo.Account a ON a.AccId = r.reviewAccount
            WHERE r.ProdId = @ProductId
            ORDER BY r.reviewDate DESC;";

        try
        {
            connection.Open();
            return connection.Query<Review>(sql, new { ProductId = productId }).ToList();
        }
        finally
        {
            CloseConnection();
        }
    }

    public double GetAverageRating(int productId)
    {
        const string sql = @"
            SELECT ISNULL(AVG(CAST(ReviewRating AS float)), 0)
            FROM dbo.Review
            WHERE ProdId = @ProductId;";

        try
        {
            connection.Open();
            return connection.ExecuteScalar<double>(sql, new { ProductId = productId });
        }
        finally
        {
            CloseConnection();
        }
    }

    public int GetReviewCount(int productId)
    {
        const string sql = @"
            SELECT COUNT(*)
            FROM dbo.Review
            WHERE ProdId = @ProductId;";

        try
        {
            connection.Open();
            return connection.ExecuteScalar<int>(sql, new { ProductId = productId });
        }
        finally
        {
            CloseConnection();
        }
    }

    public void AddReview(Review review)
    {
        // NOTE: maps Username -> Account row. Adjust if your schema differs.
        const string sql = @"
            INSERT INTO dbo.Review
                (ProdId, reviewAccount, ReviewRating, ReviewTitle, reviewDescription, reviewDate, ReviewStatus)
            VALUES
                (@ProductId,
                 (SELECT TOP 1 AccId FROM dbo.Account WHERE AccName = @Username),
                 @Rating, @Title, @Comment, @CreatedDate,
                 CASE WHEN @IsVerified = 1 THEN 'Verified' ELSE 'Unverified' END);";

        try
        {
            connection.Open();
            connection.Execute(sql, new
            {
                review.ProductId,
                review.Username,
                review.Rating,
                review.Title,
                review.Comment,
                review.CreatedDate,
                review.IsVerified
            });
        }
        finally
        {
            CloseConnection();
        }
    }
}