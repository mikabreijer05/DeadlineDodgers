using Dapper;
using KE03_INTDEV_SE_2_Base.Models;

namespace KE03_INTDEV_SE_2_Base.DAL;

/// <summary>
/// Handles all database operations for Products using Dapper (SQL access layer).
/// This class is responsible for reading/writing products from/to the database.
/// </summary>
public class SQLProducts : SQLDAL
{
    /// <summary>
    /// Gets all products from the database.
    /// Includes category relation so products can be grouped in the UI.
    /// </summary>
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

            // Executes SQL and maps result rows into Product objects
            return connection.Query<Product>(sql).ToList();
        }
        finally
        {
            // Ensures DB connection is always closed even if an error happens
            CloseConnection();
        }
    }

    /// <summary>
    /// Gets a single product by its ID.
    /// Used for Edit page.
    /// </summary>
    public Product GetProductById(int id)
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
            p.ProdDimensions     AS Dimensions
        FROM dbo.Product p
        WHERE p.ProductId = @Id;
    ";

        try
        {
            connection.Open();

            // Fetch single product or null if not found
            var product = connection.QueryFirstOrDefault<Product>(sql, new { Id = id });

            // Category is stored in a separate table, so we fetch it separately
            if (product != null)
            {
                product.CategoryId = connection.QueryFirstOrDefault<int?>(
                    "SELECT CategoryId FROM dbo.ProductCategory WHERE ProductId = @Id",
                    new { Id = id }
                ) ?? 0; // fallback to 0 if no category exists
            }

            return product;
        }
        finally
        {
            CloseConnection();
        }
    }

    /// <summary>
    /// Inserts a new product into the database.
    /// Also inserts category relation if one is selected.
    /// </summary>
    public void AddProduct(Product product)
    {
        const string sql = @"
        INSERT INTO dbo.Product
        (
            ProdName,
            ProdPrice,
            ProdImage,
            ProdDescription,
            ProdQuantity,
            DDCId,
            ProdDeliveryTime,
            KortingId,
            ProdCost,
            ProdDimensions
        )
        VALUES
        (
            @Name,
            @Price,
            @ImageUrl,
            @Description,
            @Quantity,
            @DDCId,
            @DeliveryTime,
            @DiscountId,
            @Cost,
            @Dimensions
        );

        -- Returns the ID of the newly created product
        SELECT CAST(SCOPE_IDENTITY() AS INT);
    ";

        try
        {
            connection.Open();

            // Insert product and get generated ID back from SQL Server
            int newProductId = connection.QuerySingle<int>(sql, product);

            // If a category is selected, store relationship in linking table
            if (product.CategoryId > 0)
            {
                connection.Execute(
                    "INSERT INTO dbo.ProductCategory (ProductId, CategoryId) VALUES (@ProductId, @CategoryId)",
                    new
                    {
                        ProductId = newProductId,
                        CategoryId = product.CategoryId
                    }
                );
            }
        }
        finally
        {
            CloseConnection();
        }
    }

    /// <summary>
    /// Updates an existing product.
    /// Also resets and reassigns category relation.
    /// </summary>
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
                ProdDescription = @Description,
                ProdQuantity = @Quantity,
                DDCId = @DDCId,
                ProdDeliveryTime = @DeliveryTime,
                KortingId = @DiscountId,
                ProdCost = @Cost,
                ProdDimensions = @Dimensions
            WHERE ProductId = @Id;
        ";

            // Update main product fields
            connection.Execute(updateSql, product);

            // Remove old category link (prevents duplicates or outdated links)
            connection.Execute(
                "DELETE FROM dbo.ProductCategory WHERE ProductId = @Id",
                new { product.Id }
            );

            // Insert new category link if valid
            if (product.CategoryId > 0)
            {
                connection.Execute(
                    "INSERT INTO dbo.ProductCategory (ProductId, CategoryId) VALUES (@ProductId, @CategoryId)",
                    new
                    {
                        ProductId = product.Id,
                        CategoryId = product.CategoryId
                    }
                );
            }
        }
        finally
        {
            CloseConnection();
        }
    }

    /// <summary>
    /// Deletes a product completely.
    /// Also removes category relations first to avoid foreign key issues.
    /// </summary>
    public void DeleteProduct(int id)
    {
        try
        {
            connection.Open();

            // Remove category link first (prevents FK constraint errors)
            connection.Execute(
                "DELETE FROM dbo.ProductCategory WHERE ProductId = @Id",
                new { Id = id }
            );

            // Then remove the product itself
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