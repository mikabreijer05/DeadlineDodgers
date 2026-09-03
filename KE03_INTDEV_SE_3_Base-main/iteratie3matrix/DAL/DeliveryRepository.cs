using Microsoft.Data.SqlClient;
using iteratie3matrix.Models;

namespace iteratie3matrix.DAL;

public class DeliveryRepository
{
    private readonly SQLDAL _sql;

    public DeliveryRepository()
    {
        _sql = new SQLDAL();
    }

    // =====================
    // GET CART FOR VEHICLE
    // =====================
    public async Task<CartLoad?> GetCartForVehicleAsync(int vehicleId)
    {
        const string sql = """
            SELECT TOP 1 DeliveryId
            FROM [Delivery]
            WHERE VehicleId = @VehicleId
            ORDER BY ToBeSentDate ASC;
        """;

        await using var conn = _sql.CreateConnection();
        await conn.OpenAsync();

        await using var cmd = new SqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@VehicleId", vehicleId);

        var result = await cmd.ExecuteScalarAsync();
        if (result == null)
            return null;

        int deliveryId = (int)result;

        return await BuildCartAsync(deliveryId);
    }

    // =====================
    // BUILD FULL CART
    // =====================
    public async Task<CartLoad> BuildCartAsync(int deliveryId)
    {
        var cart = new CartLoad
        {
            CartNumber = $"DEL-{deliveryId}"
        };

        const string sql = """
                SELECT ProductId, Quantity
                FROM OrderProduct
                WHERE OPId IN (
                    SELECT OPId
                    FROM DeliveryOrderProduct
                    WHERE DeliveryId = @DeliveryId
                );
            """;

        await using var conn = _sql.CreateConnection();
        await conn.OpenAsync();

        await using var cmd = new SqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@DeliveryId", deliveryId);

        await using var reader = await cmd.ExecuteReaderAsync();

        var items = new List<CartItem>();

        while (await reader.ReadAsync())
        {
            items.Add(new CartItem
            {
                ProductId = reader.GetInt32(0),
                RequiredQuantity = reader.GetInt32(1),
                Remaining = reader.GetInt32(1)
            });
        }

        cart.Items = items;
        cart.ExpectedPackages = items.Sum(x => x.RequiredQuantity);
        cart.ScannedPackages = 0;

        return cart;
    }
}