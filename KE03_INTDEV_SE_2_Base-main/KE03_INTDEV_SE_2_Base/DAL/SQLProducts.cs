using Dapper;
using KE03_INTDEV_SE_2_Base_main.Models;
using KE03_INTDEV_SE_2_Base.Models;

namespace KE03_INTDEV_SE_2_Base.DAL;

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

                r.RevId             AS Id,
                r.ProdId            AS ProductId,
                r.ReviewRating      AS Rating,
                r.ReviewTitle       AS Title,
                r.reviewDescription AS Comment,
                r.reviewDate        AS CreatedDate,
                a.AccName           AS Username,

                CASE WHEN r.ReviewStatus = 'Verified' THEN 1 ELSE 0 END AS IsVerified

            FROM dbo.Product p
            LEFT JOIN dbo.ProductCategory pc ON pc.ProductId = p.ProductId
            LEFT JOIN dbo.Category c          ON c.CatId = pc.CategoryId
            LEFT JOIN dbo.Review r            ON r.ProdId = p.ProductId
            LEFT JOIN dbo.Account a           ON a.AccId = r.reviewAccount;
        ";

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

                    if (review != null && review.Id > 0)
                    {
                        existingProduct.Reviews.Add(review);
                    }

                    return existingProduct;
                },
                splitOn: "Id"
            );

            return productMap.Values.ToList();
        }
        finally
        {
            CloseConnection();
        }
        throw new Exception("DAL is being called");
    }

    // -----------------------------
    // GET SINGLE PRODUCT
    // -----------------------------
    public Product GetProductById(int id)
    {
        const string sql = @"
            SELECT
                ProductId AS Id,
                ProdName AS Name,
                ProdPrice AS Price,
                ProdImage AS ImageUrl,
                ProdDescription AS Description
            FROM dbo.Product
            WHERE ProductId = @Id;
        ";

        try
        {
            connection.Open();
            return connection.QueryFirstOrDefault<Product>(sql, new { Id = id });
        }
        finally
        {
            CloseConnection();
        }
    }

    // -----------------------------
    // ADD PRODUCT
    // -----------------------------
    public void AddProduct(Product product)
    {
        const string sql = @"
            INSERT INTO dbo.Product (ProdName, ProdPrice, ProdImage, ProdDescription)
            VALUES (@Name, @Price, @ImageUrl, @Description);
        ";

        try
        {
            connection.Open();
            connection.Execute(sql, product);
        }
        finally
        {
            CloseConnection();
        }
    }

    // -----------------------------
    // UPDATE PRODUCT
    // -----------------------------
    public void UpdateProduct(Product product)
    {
        const string sql = @"
            UPDATE dbo.Product
            SET
                ProdName = @Name,
                ProdPrice = @Price,
                ProdImage = @ImageUrl,
                ProdDescription = @Description
            WHERE ProductId = @Id;
        ";

        try
        {
            connection.Open();
            connection.Execute(sql, product);
        }
        finally
        {
            CloseConnection();
        }
    }

    // -----------------------------
    // DELETE PRODUCT
    // -----------------------------
    public void DeleteProduct(int id)
    {
        const string sql = @"
            DELETE FROM dbo.Product
            WHERE ProductId = @Id;
        ";

        try
        {
            connection.Open();
            connection.Execute(sql, new { Id = id });
        }
        finally
        {
            CloseConnection();
        }
    }
}