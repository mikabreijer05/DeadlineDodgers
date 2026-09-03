using Dapper;
using KE03_INTDEV_SE_2_Base.Models;

namespace KE03_INTDEV_SE_2_Base.DAL;

/// <summary>
/// Handles all database operations for Products using Dapper (SQL access layer).
/// This class is responsible for reading/writing products from/to the database.
/// </summary>
public class SQLProducts(IConfiguration configuration) : SQLDAL(configuration)
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
            ProdDeliveryTime,
            KortingId,
            ProdCost,
            ProdCatId,
            ProdDimensions
        )
        VALUES
        (
            @Name,
            @Price,
            @ImageUrl,
            @Description,
            @Quantity,
            @DeliveryTime,
            @DiscountId,
            @Cost,
            @CategoryId,
            @DimensionId
        );

        SELECT CAST(SCOPE_IDENTITY() AS INT);
        ";

        try
        {
            connection.Open();

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
    
    public IEnumerable<Category> GetAllCategories()
    {
        const string sql = @"
        SELECT
            CatId AS Id,
            CatName AS Name
        FROM dbo.Category
        ORDER BY CatName;
        ";

        try
        {
            connection.Open();
            return connection.Query<Category>(sql).ToList();
        }
        finally
        {
            CloseConnection();
        }
    }
    public Category AddCategory(string categoryName)
    {
        const string getNextCategoryIdSql = @"
        SELECT ISNULL(MAX(CatId), 0) + 1
        FROM dbo.Category;
        ";

        const string insertCategorySql = @"
        INSERT INTO dbo.Category
        (
            CatId,
            CatName
        )
        VALUES
        (
            @Id,
            @Name
        );
        ";

        try
        {
            connection.Open();

            int newCategoryId = connection.QuerySingle<int>(getNextCategoryIdSql);

            var category = new Category
            {
                Id = newCategoryId,
                Name = categoryName.Trim()
            };

            connection.Execute(insertCategorySql, category);

            return category;
        }
        finally
        {
            CloseConnection();
        }
    }
    
    public IEnumerable<dynamic> GetAllDiscounts()
    {
        const string sql = @"
        SELECT
            DiscountId AS Id,
            CONCAT(DiscountType, ' - ', DiscountValue) AS Name
        FROM dbo.ProductDiscount
        ORDER BY DiscountType, DiscountValue;
        ";

        try
        {
            connection.Open();
            return connection.Query(sql).ToList();
        }
        finally
        {
            CloseConnection();
        }
    }

    public IEnumerable<dynamic> GetAllPackageDimensions()
    {
        const string sql = @"
        SELECT
            PDId AS Id,
            PackageDimensions AS Name
        FROM dbo.PackageDimensions
        ORDER BY PDId;
        ";

        try
        {
            connection.Open();
            return connection.Query(sql).ToList();
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
        const string updateSql = @"
        UPDATE dbo.Product
        SET
            ProdName = @Name,
            ProdPrice = @Price,
            ProdImage = @ImageUrl,
            ProdDescription = @Description,
            ProdQuantity = @Quantity,
            ProdDeliveryTime = @DeliveryTime,
            KortingId = @DiscountId,
            ProdCost = @Cost,
            ProdCatId = @CategoryId,
            ProdDimensions = @DimensionId
        WHERE ProductId = @Id;
        ";

        try
        {
            connection.Open();

            connection.Execute(updateSql, product);

            connection.Execute(
                "DELETE FROM dbo.ProductCategory WHERE ProductId = @Id",
                new { product.Id }
            );

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