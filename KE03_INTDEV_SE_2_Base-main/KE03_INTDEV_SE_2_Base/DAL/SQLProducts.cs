using Dapper;
using KE03_INTDEV_SE_2_Base.Models;

namespace KE03_INTDEV_SE_2_Base.DAL;

public class SQLProducts : SQLDAL
{
    public IEnumerable<Product> GetAllProducts()
    {
        const string sql = @"
    SELECT
        p.ProductId          AS Id,
        p.ProdName           AS Name,
        p.ProdPrice          AS Price,
        p.ProdDescription    AS Description,
        p.ProdImage          AS ImageUrl,
        p.ProdQuantity       AS Quantity,
        p.DDCId              AS DDCId,
        p.ProdDeliveryTime   AS DeliveryTime,
        p.KortingId          AS DiscountId,
        p.ProdCost           AS Cost,
        p.ProdDimensions     AS Dimensions,
        pc.CategoryId        AS CategoryId
    FROM dbo.Product p
    LEFT JOIN dbo.ProductCategory pc
        ON pc.ProductId = p.ProductId;
";  

        try
        {
            connection.Open();
            return connection.Query<Product>(sql).ToList();
        }
        finally
        {
            CloseConnection();
        }
    }

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

            var product = connection.QueryFirstOrDefault<Product>(sql, new { Id = id });

            if (product != null)
            {
                product.CategoryId = connection.QueryFirstOrDefault<int?>(
                    "SELECT CategoryId FROM dbo.ProductCategory WHERE ProductId = @Id",
                    new { Id = id }
                ) ?? 0;
            }

            return product;
        }
        finally
        {
            CloseConnection();
        }
    }

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

    public void UpdateProduct(Product product)
    {
        try
        {
            connection.Open();

            const string updateSql = @"
                UPDATE dbo.Product
                SET
                    ProdName = @Name,
                    ProdPrice = @Price,
                    ProdImage = @ImageUrl,
                    ProdDescription = @Description
                WHERE ProductId = @Id;
            ";

            connection.Execute(updateSql, product);

            connection.Execute(
                "DELETE FROM dbo.ProductCategory WHERE ProductId = @Id",
                new { product.Id }
            );

            if (product.CategoryId > 0)
            {
                connection.Execute(
                    "INSERT INTO dbo.ProductCategory (ProductId, CategoryId) VALUES (@ProductId, @CategoryId)",
                    new { ProductId = product.Id, CategoryId = product.CategoryId }
                );
            }
        }
        finally
        {
            CloseConnection();
        }
    }

    public void DeleteProduct(int id)
    {
        try
        {
            connection.Open();

            connection.Execute(
                "DELETE FROM dbo.ProductCategory WHERE ProductId = @Id",
                new { Id = id }
            );

            connection.Execute(
                "DELETE FROM dbo.Product WHERE ProductId = @Id",
                new { Id = id }
            );
        }
        finally
        {
            CloseConnection();
        }
    }
}