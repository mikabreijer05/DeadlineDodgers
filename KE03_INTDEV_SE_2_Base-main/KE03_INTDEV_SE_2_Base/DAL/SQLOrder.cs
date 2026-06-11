using Dapper;
using KE03_INTDEV_SE_2_Base.Models;

namespace KE03_INTDEV_SE_2_Base.DAL;

public class SQLOrder : SQLDAL
{
    /// <summary>
    /// Creates a new order in the database
    /// </summary>
    public async Task<int> CreateOrderAsync(Order order)
    {
        using (var conn = CreateConnection("sa", "FJL7MzZPckC37uheZHp"))
        {
            await conn.OpenAsync();

            var query = @"
                INSERT INTO [dbo].[Order] (OrderDate, AddressId, AccountId, StatusId, CouponId, DeliveryTogether)
                OUTPUT INSERTED.OrderId
                VALUES (@OrderDate, @AddressId, @AccountId, @StatusId, @CouponId, @DeliveryTogether)";

            var parameters = new
            {
                order.OrderDate,
                AddressId = 1,
                order.CustomerId,
                order.StatusId,
                CouponId = (int?)null,
                DeliveryTogether = false
            };

            var orderId = await conn.QuerySingleAsync<int>(query, parameters);
            return orderId;
        }
    }

    /// <summary>
    /// Retrieves a single order by ID with order lines and customer info
    /// </summary>
    public async Task<Order?> GetOrderByIdAsync(int orderId)
    {
        using (var conn = CreateConnection("sa", "FJL7MzZPckC37uheZHp"))
        {
            await conn.OpenAsync();

            var query = @"
                SELECT 
                    o.OrderId AS Id, 
                    o.OrderDate, 
                    o.AccountId AS CustomerId,
                    a.CustName AS CustomerName,
                    o.StatusId,
                    s.Status AS OrderStatus
                FROM [dbo].[Order] o
                LEFT JOIN [dbo].[Account] a ON o.AccountId = a.AccId
                LEFT JOIN [dbo].[Status] s ON o.StatusId = s.StatusId
                WHERE o.OrderId = @OrderId";

            var order = await conn.QuerySingleOrDefaultAsync<Order?>(query, new { OrderId = orderId });

            if (order != null)
            {
                // Get order lines with product information
                var orderLinesQuery = @"
                    SELECT 
                        op.OrderId,
                        op.ProductId,
                        op.Quantity,
                        p.ProdName,
                        p.ProdPrice
                    FROM [dbo].[OrderProduct] op
                    LEFT JOIN [dbo].[Product] p ON op.ProductId = p.ProductId
                    WHERE op.OrderId = @OrderId";

                var orderLines = await conn.QueryAsync<OrderLine>(orderLinesQuery, new { OrderId = orderId });

                foreach (var line in orderLines)
                {
                    order.OrderLines.Add(line);
                }
            }

            return order;
        }
    }

    /// <summary>
    /// Retrieves all orders with customer names
    /// </summary>
    public async Task<IEnumerable<Order>> GetAllOrdersAsync()
    {
        using (var conn = CreateConnection("sa", "FJL7MzZPckC37uheZHp"))
        {
            await conn.OpenAsync();

            var query = @"
                SELECT 
                    o.OrderId AS Id, 
                    o.OrderDate, 
                    o.AccountId AS CustomerId,
                    a.CustName AS CustomerName,
                    o.StatusId,
                    s.Status AS OrderStatus
                FROM [dbo].[Order] o
                LEFT JOIN [dbo].[Account] a ON o.AccountId = a.AccId
                LEFT JOIN [dbo].[Status] s ON o.StatusId = s.StatusId
                ORDER BY o.OrderDate DESC";

            var orders = await conn.QueryAsync<Order>(query);

            // Fetch order lines for each order
            foreach (var order in orders)
            {
                var orderLinesQuery = @"
                    SELECT 
                        op.OrderId,
                        op.ProductId,
                        op.Quantity,
                        p.ProdName,
                        p.ProdPrice
                    FROM [dbo].[OrderProduct] op
                    LEFT JOIN [dbo].[Product] p ON op.ProductId = p.ProductId
                    WHERE op.OrderId = @OrderId";

                var orderLines = await conn.QueryAsync<OrderLine>(orderLinesQuery, new { OrderId = order.Id });

                foreach (var line in orderLines)
                {
                    order.OrderLines.Add(line);
                }
            }

            return orders;
        }
    }

    /// <summary>
    /// Retrieves all orders for a specific customer
    /// </summary>
    public async Task<IEnumerable<Order>> GetOrdersByCustomerIdAsync(int customerId)
    {
        using (var conn = CreateConnection("sa", "FJL7MzZPckC37uheZHp"))
        {
            await conn.OpenAsync();

            var query = @"
                SELECT 
                    o.OrderId AS Id, 
                    o.OrderDate, 
                    o.AccountId AS CustomerId,
                    a.CustName AS CustomerName,
                    o.StatusId,
                    s.Status AS OrderStatus
                FROM [dbo].[Order] o
                LEFT JOIN [dbo].[Account] a ON o.AccountId = a.AccId
                LEFT JOIN [dbo].[Status] s ON o.StatusId = s.StatusId
                WHERE o.AccountId = @CustomerId
                ORDER BY o.OrderDate DESC";

            return await conn.QueryAsync<Order>(query, new { CustomerId = customerId });
        }
    }

    /// <summary>
    /// Updates an existing order
    /// </summary>
    public async Task<bool> UpdateOrderAsync(Order order)
    {
        using (var conn = CreateConnection("sa", "FJL7MzZPckC37uheZHp"))
        {
            await conn.OpenAsync();

            var query = @"
                UPDATE [dbo].[Order]
                SET OrderDate = @OrderDate, AccountId = @CustomerId, StatusId = @StatusId
                WHERE OrderId = @OrderId";

            var rowsAffected = await conn.ExecuteAsync(query, new { order.OrderDate, order.Id, order.CustomerId, order.StatusId });
            return rowsAffected > 0;
        }
    }

    /// <summary>
    /// Deletes an order by ID
    /// </summary>
    public async Task<bool> DeleteOrderAsync(int orderId)
    {
        using (var conn = CreateConnection("sa", "FJL7MzZPckC37uheZHp"))
        {
            await conn.OpenAsync();

            var query = "DELETE FROM [dbo].[Order] WHERE OrderId = @OrderId";
            var rowsAffected = await conn.ExecuteAsync(query, new { OrderId = orderId });
            return rowsAffected > 0;
        }
    }

    /// <summary>
    /// Creates a new order line item in OrderProduct table
    /// </summary>
    public async Task<bool> CreateOrderLineAsync(OrderLine orderLine)
    {
        using (var conn = CreateConnection("sa", "FJL7MzZPckC37uheZHp"))
        {
            await conn.OpenAsync();

            var query = @"
                INSERT INTO [dbo].[OrderProduct] (OrderId, ProductId, Quantity)
                VALUES (@OrderId, @ProductId, @Quantity)";

            var result = await conn.ExecuteAsync(query, new { orderLine.OrderId, orderLine.ProductId, orderLine.Quantity });
            return result > 0;
        }
    }

    /// <summary>
    /// Retrieves all order lines for a specific order with product details
    /// </summary>
    public async Task<IEnumerable<OrderLine>> GetOrderLinesByOrderIdAsync(int orderId)
    {
        using (var conn = CreateConnection("sa", "FJL7MzZPckC37uheZHp"))
        {
            await conn.OpenAsync();

            var query = @"
                SELECT 
                    op.OrderId,
                    op.ProductId,
                    op.Quantity,
                    p.ProdName,
                    p.ProdPrice
                FROM [dbo].[OrderProduct] op
                LEFT JOIN [dbo].[Product] p ON op.ProductId = p.ProductId
                WHERE op.OrderId = @OrderId";

            return await conn.QueryAsync<OrderLine>(query, new { OrderId = orderId });
        }
    }

    /// <summary>
    /// Deletes an order line from OrderProduct table
    /// </summary>
    public async Task<bool> DeleteOrderLineAsync(int orderId, int productId)
    {
        using (var conn = CreateConnection("DD-admin", "FJL7MzZPckC37uheZHp"))
        {
            await conn.OpenAsync();

            var query = @"
                DELETE FROM [dbo].[OrderProduct] 
                WHERE OrderId = @OrderId AND ProductId = @ProductId";

            var rowsAffected = await conn.ExecuteAsync(query, new { OrderId = orderId, ProductId = productId });
            return rowsAffected > 0;
        }
    }

    public async Task AddOrderAsync(Order order)
    {
        await CreateOrderAsync(order);
    }
}