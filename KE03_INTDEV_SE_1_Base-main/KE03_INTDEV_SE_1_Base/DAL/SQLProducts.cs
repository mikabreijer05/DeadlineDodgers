using Dapper;
using KE03_INTDEV_SE_1_Base.Models;

namespace KE03_INTDEV_SE_1_Base.DAL;

public class SQLProducts : SQLDAL
{
    public IEnumerable<Product> GetAllProducts()
    {
        const string sql = @"
            SELECT
                p.ProductId       AS Id,
                p.ProdName        AS Name,
                c.CatName         AS Category,
                p.ProdPrice       AS Price,
                p.ProdImage       AS ImageUrl,
                p.ProdDescription AS Description,

                r.RevId               AS Id,
                    r.ProdId              AS ProductId,
                    r.ReviewRating        AS Rating,
                    r.ReviewTitle         AS Title,
                    r.reviewDescription   AS Comment,
                    r.reviewDate          AS CreatedDate,
                    a.AccName             AS Username,
                    CASE WHEN r.ReviewStatus = 'Verified' THEN 1 ELSE 0 END AS IsVerified
                FROM dbo.Product p
                LEFT JOIN dbo.ProductCategory pc ON pc.ProductId = p.ProductId
                LEFT JOIN dbo.Category c          ON c.CatId = pc.CategoryId
                LEFT JOIN dbo.Review r            ON r.ProdId = p.ProductId
                LEFT JOIN dbo.Account a           ON a.AccId = r.reviewAccount;";

        var productMap = new Dictionary<int, Product>();

        try
        {
            connection.Open();

            connection.Query<Product, Review, Product>(
                sql,
                (product, review) =>
                {
                    if (!productMap.TryGetValue(product.Id, out var existingProduct))
                    {
                        existingProduct = product;
                        productMap.Add(existingProduct.Id, existingProduct);
                    }

                    if (review is { Id: > 0 })
                    {
                        existingProduct.Reviews.Add(review);
                    }

                    return existingProduct;
                },
                splitOn: "Id");

            return productMap.Values.ToList();
        }
        
        finally
        {
            CloseConnection();
        }
    }
    public Product? GetProductById(int id)
    {
        const string sql = @"
            SELECT
                p.ProductId       AS Id,
                p.ProdName        AS Name,
                c.CatName         AS Category,
                p.ProdPrice       AS Price,
                p.ProdImage       AS ImageUrl,
                p.ProdDescription AS Description
            FROM dbo.Product p
            LEFT JOIN dbo.ProductCategory pc ON pc.ProductId = p.ProductId
            LEFT JOIN dbo.Category c          ON c.CatId = pc.CategoryId
            WHERE p.ProductId = @Id;";

        try
        {
            connection.Open();
            return connection.QuerySingleOrDefault<Product>(sql, new { Id = id });
        }
        finally
        {
            CloseConnection();
        }
    }
}
