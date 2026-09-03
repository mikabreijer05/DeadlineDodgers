using iteratie3matrix.Models;
using Microsoft.Data.SqlClient;

namespace iteratie3matrix.DAL;

public class OrderRepository
{
    // WHAT: Handles Order data access
    // WHY: Keeps SQL logic away from ViewModels

    private readonly SQLDAL _sqlDal;

    public OrderRepository()
    {
        _sqlDal = new SQLDAL();
    }

    // =========================
    // RETRY HELPER
    // =========================

    private async Task OpenWithRetry(SqlConnection connection, int retries = 3)
    {
        for (int attempt = 1; attempt <= retries; attempt++)
        {
            try
            {
                await connection.OpenAsync();
                return;
            }
            catch (SqlException)
            {
                if (attempt == retries)
                    throw;

                await Task.Delay(2000 * attempt); // exponential backoff
            }
        }
    }

    // =========================
    // GET ALL ORDERS
    // =========================

    public async Task<List<Order>> ListAsync()
    {
        var orders = new List<Order>();

        const string sql = """
                       SELECT OrderId,
                              OrderDate,
                              StatusId,
                              AccountId,
                              AddressId
                       FROM [Order]
                       ORDER BY OrderDate ASC;
                       """;

        await using var connection = _sqlDal.CreateConnection();

        await OpenWithRetry(connection);

        await using var command = new SqlCommand(sql, connection);
        await using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            orders.Add(new Order
            {
                OrderId = reader.GetInt32(0),
                OrderDate = reader.GetDateTime(1),
                StatusId = reader.GetInt32(2),
                AccountId = reader.GetInt32(3),
                AddressId = reader.GetInt32(4)
            });
        }

        return orders;
    }

    // =========================
    // GET SINGLE ORDER
    // =========================

    public async Task<Order?> GetAsync(int id)
    {
        const string sql = """
                           SELECT OrderId, OrderDate, StatusId, AccountId, AddressId
                           FROM [Order]
                           WHERE OrderId = @OrderId;
                           """;

        await using var connection = _sqlDal.CreateConnection();

        await OpenWithRetry(connection);

        await using var command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@OrderId", id);

        await using var reader = await command.ExecuteReaderAsync();

        if (!await reader.ReadAsync())
            return null;

        return new Order
        {
            OrderId = reader.GetInt32(0),
            OrderDate = reader.GetDateTime(1),
            StatusId = reader.GetInt32(2),
            AccountId = reader.GetInt32(3),
            AddressId = reader.GetInt32(4),
        };
    }

    // =========================
    // UPDATE STATUS
    // =========================

    public async Task UpdateStatusAsync(int orderId, int statusId)
    {
        const string sql = """
                           UPDATE [Order]
                           SET StatusId = @StatusId
                           WHERE OrderId = @OrderId;
                           """;

        await using var connection = _sqlDal.CreateConnection();

        await OpenWithRetry(connection);

        await using var command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@OrderId", orderId);
        command.Parameters.AddWithValue("@StatusId", statusId);

        await command.ExecuteNonQueryAsync();
    }


    // =========================
    // GET ORDER PRODUCTS (DETAIL PAGE)
    // =========================

    public async Task<List<OrderProductItem>> GetOrderProductsAsync(int orderId)
    {
        const string sql = """
        SELECT op.ProductId,
               p.ProdName,
               op.Quantity
        FROM OrderProduct op
        INNER JOIN Product p ON p.ProductId = op.ProductId
        WHERE op.OrderId = @OrderId;
    """;

        var items = new List<OrderProductItem>();

        await using var connection = _sqlDal.CreateConnection();
        await OpenWithRetry(connection);

        await using var command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@OrderId", orderId);

        await using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            items.Add(new OrderProductItem
            {
                ProductId = reader.GetInt32(0),
                ProductName = reader.GetString(1),
                Quantity = reader.GetInt32(2)
            });
        }

        return items;
    }

    public async Task<int> GetTotalPackageCountAsync(int orderId)
    {
        const string sql = """
        SELECT SUM(Quantity)
        FROM OrderProduct
        WHERE OrderId = @OrderId;
    """;

        await using var connection = _sqlDal.CreateConnection();
        await OpenWithRetry(connection);

        await using var command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@OrderId", orderId);

        var result = await command.ExecuteScalarAsync();

        return result == DBNull.Value || result == null
            ? 0
            : Convert.ToInt32(result);
    }
}