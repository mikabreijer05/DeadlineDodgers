using Dapper;
using KE03_INTDEV_SE_2_Base.Models;

namespace KE03_INTDEV_SE_2_Base.DAL;

public class SQLOrder : SQLDAL
{
    private static string FormatAddress(Address? address)
    {
        if (address == null)
        {
            return string.Empty;
        }

        return string.Join(", ", new[]
        {
            string.Join(" ", new[] { address.Street, address.HouseNumber }
                .Where(value => !string.IsNullOrWhiteSpace(value))),
            string.Join(" ", new[] { address.PostalCode, address.City }
                .Where(value => !string.IsNullOrWhiteSpace(value))),
            address.Country
        }.Where(value => !string.IsNullOrWhiteSpace(value)));
    }

    private static bool AddressChanged(Address? currentAddress, Address? newAddress)
    {
        if (currentAddress == null && newAddress == null)
        {
            return false;
        }

        if (currentAddress == null || newAddress == null)
        {
            return true;
        }

        return
            !string.Equals(currentAddress.Street?.Trim(), newAddress.Street?.Trim(), StringComparison.OrdinalIgnoreCase) ||
            !string.Equals(currentAddress.HouseNumber?.Trim(), newAddress.HouseNumber?.Trim(), StringComparison.OrdinalIgnoreCase) ||
            !string.Equals(currentAddress.PostalCode?.Trim(), newAddress.PostalCode?.Trim(), StringComparison.OrdinalIgnoreCase) ||
            !string.Equals(currentAddress.City?.Trim(), newAddress.City?.Trim(), StringComparison.OrdinalIgnoreCase) ||
            !string.Equals(currentAddress.Country?.Trim(), newAddress.Country?.Trim(), StringComparison.OrdinalIgnoreCase);
    }
    private static bool HasAnyAddressValue(Address? address)
    {
        return address != null &&
               (!string.IsNullOrWhiteSpace(address.Street) ||
                !string.IsNullOrWhiteSpace(address.HouseNumber) ||
                !string.IsNullOrWhiteSpace(address.PostalCode) ||
                !string.IsNullOrWhiteSpace(address.City) ||
                !string.IsNullOrWhiteSpace(address.Country));
    }
    
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
                order.AddressId,
                AccountId = order.CustomerId,
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
                        o.AddressId,
                        o.AccountId AS CustomerId,
                        a.CustName AS CustomerName,
                        o.StatusId,
                        s.Status AS OrderStatus,
                        addr.AddressId,
                        addr.Street,
                        addr.HouseNumber,
                        addr.PostalCode,
                        addr.City,
                        addr.Country
                    FROM [dbo].[Order] o
                    LEFT JOIN [dbo].[Account] a ON o.AccountId = a.AccId
                    LEFT JOIN [dbo].[Status] s ON o.StatusId = s.StatusId
                    LEFT JOIN [dbo].[Address] addr ON o.AddressId = addr.AddressId
                    WHERE o.OrderId = @OrderId";

            var order = (await conn.QueryAsync<Order, Address, Order>(
                query,
                (order, address) =>
                {
                    order.Address = address;
                    order.AddressId = address?.AddressId ?? order.AddressId;
                    return order;
                },
                new { OrderId = orderId },
                splitOn: "AddressId")).SingleOrDefault();

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
                        o.AddressId,
                        o.AccountId AS CustomerId,
                        a.CustName AS CustomerName,
                        o.StatusId,
                        s.Status AS OrderStatus,
                        addr.AddressId,
                        addr.Street,
                        addr.HouseNumber,
                        addr.PostalCode,
                        addr.City,
                        addr.Country
                    FROM [dbo].[Order] o
                    LEFT JOIN [dbo].[Account] a ON o.AccountId = a.AccId
                    LEFT JOIN [dbo].[Status] s ON o.StatusId = s.StatusId
                    LEFT JOIN [dbo].[Address] addr ON o.AddressId = addr.AddressId
                    ORDER BY o.OrderDate DESC";

            var orders = (await conn.QueryAsync<Order, Address, Order>(
                query,
                (order, address) =>
                {
                    order.Address = address;
                    order.AddressId = address?.AddressId ?? order.AddressId;
                    return order;
                },
                splitOn: "AddressId")).ToList();

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
                        o.AddressId,
                        o.AccountId AS CustomerId,
                        a.CustName AS CustomerName,
                        o.StatusId,
                        s.Status AS OrderStatus,
                        addr.AddressId,
                        addr.Street,
                        addr.HouseNumber,
                        addr.PostalCode,
                        addr.City,
                        addr.Country
                    FROM [dbo].[Order] o
                    LEFT JOIN [dbo].[Account] a ON o.AccountId = a.AccId
                    LEFT JOIN [dbo].[Status] s ON o.StatusId = s.StatusId
                    LEFT JOIN [dbo].[Address] addr ON o.AddressId = addr.AddressId
                    WHERE o.AccountId = @CustomerId
                    ORDER BY o.OrderDate DESC";

            return await conn.QueryAsync<Order, Address, Order>(
                query,
                (order, address) =>
                {
                    order.Address = address;
                    order.AddressId = address?.AddressId ?? order.AddressId;
                    return order;
                },
                new { CustomerId = customerId },
                splitOn: "AddressId");
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

                using var transaction = conn.BeginTransaction();

                var statusExistsQuery = @"
                    SELECT COUNT(1)
                    FROM [dbo].[Status]
                    WHERE StatusId = @StatusId";

                var statusExists = await conn.QuerySingleAsync<int>(
                    statusExistsQuery,
                    new { order.StatusId },
                    transaction);

                if (statusExists == 0)
                {
                    transaction.Rollback();
                    return false;
                }

                var currentAddressQuery = @"
                    SELECT
                        addr.AddressId,
                        addr.Street,
                        addr.HouseNumber,
                        addr.PostalCode,
                        addr.City,
                        addr.Country
                    FROM [dbo].[Order] o
                    LEFT JOIN [dbo].[Address] addr ON o.AddressId = addr.AddressId
                    WHERE o.OrderId = @OrderId";

                var currentAddress = await conn.QuerySingleOrDefaultAsync<Address>(
                    currentAddressQuery,
                    new { OrderId = order.Id },
                    transaction);

                var addressId = order.AddressId;

                if (HasAnyAddressValue(order.Address) && AddressChanged(currentAddress, order.Address))
                {
                    var insertAddressQuery = @"
                        INSERT INTO [dbo].[Address] (
                            Street,
                            HouseNumber,
                            PostalCode,
                            City,
                            Country
                        )
                        OUTPUT INSERTED.AddressId
                        VALUES (
                            @Street,
                            @HouseNumber,
                            @PostalCode,
                            @City,
                            @Country
                        )";

                    addressId = await conn.QuerySingleAsync<int>(
                        insertAddressQuery,
                        new
                        {
                            order.Address.Street,
                            order.Address.HouseNumber,
                            order.Address.PostalCode,
                            order.Address.City,
                            order.Address.Country
                        },
                        transaction);
                }

                var updateOrderQuery = @"
                    UPDATE [dbo].[Order]
                    SET OrderDate = @OrderDate,
                        AccountId = @CustomerId,
                        StatusId = @StatusId,
                        AddressId = @AddressId
                    WHERE OrderId = @OrderId";

                var rowsAffected = await conn.ExecuteAsync(
                    updateOrderQuery,
                    new
                    {
                        order.OrderDate,
                        order.CustomerId,
                        order.StatusId,
                        AddressId = addressId,
                        OrderId = order.Id
                    },
                    transaction);

                transaction.Commit();

                return rowsAffected > 0;
            }
        }

        /// <summary>
        /// Retrieves all available order statuses.
        /// </summary>
        public async Task<IEnumerable<(int StatusId, string Status)>> GetAllStatusesAsync()
        {
            using (var conn = CreateConnection("sa", "FJL7MzZPckC37uheZHp"))
            {
                await conn.OpenAsync();

                var query = @"
                    SELECT StatusId, Status
                    FROM [dbo].[Status]
                    ORDER BY StatusId";

                return await conn.QueryAsync<(int StatusId, string Status)>(query);
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

            using var transaction = conn.BeginTransaction();

            var deleteOrderProductsQuery = @"
                DELETE FROM [dbo].[OrderProduct]
                WHERE OrderId = @OrderId";

            await conn.ExecuteAsync(
                deleteOrderProductsQuery,
                new { OrderId = orderId },
                transaction);

            var deleteOrderQuery = @"
                DELETE FROM [dbo].[Order]
                WHERE OrderId = @OrderId";

            var rowsAffected = await conn.ExecuteAsync(
                deleteOrderQuery,
                new { OrderId = orderId },
                transaction);

            transaction.Commit();

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