using Dapper;
using KE03_INTDEV_SE_2_Base.Models;

namespace KE03_INTDEV_SE_2_Base.DAL;

public class SQLOrder(IConfiguration configuration) : SQLDAL(configuration)
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
        try
        {
            connection.Open();

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

            var orderId = await connection.QuerySingleAsync<int>(query, parameters);
            return orderId;
        }
        finally
        {
            CloseConnection();
        }
    }

    /// <summary>
    /// Retrieves a single order by ID with order lines and customer info
    /// </summary>
    public async Task<Order?> GetOrderByIdAsync(int orderId)
{
    try
    {
        connection.Open();

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

        var order = (await connection.QueryAsync<Order, Address, Order>(
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

            var orderLines = await connection.QueryAsync<OrderLine>(orderLinesQuery, new { OrderId = orderId });

            foreach (var line in orderLines)
            {
                order.OrderLines.Add(line);
            }
        }

        return order;
    }
    finally
    {
        CloseConnection();
    }
}

    /// <summary>
    /// Retrieves all orders with customer names
    /// </summary>
    public async Task<IEnumerable<Order>> GetAllOrdersAsync()
{
    try
    {
        connection.Open();

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

        var orders = (await connection.QueryAsync<Order, Address, Order>(
            query,
            (order, address) =>
            {
                order.Address = address;
                order.AddressId = address?.AddressId ?? order.AddressId;
                return order;
            },
            splitOn: "AddressId")).ToList();

        foreach (var order in orders)
        {
            var orderLinesQuery = @"
    SELECT 
        op.OrderId,
        op.ProductId,
        op.Quantity,
        p.ProdName,
        p.ProdPrice,
        p.ProdDimensions AS PackageDimensionId,
        pd.PackageDimensions AS PackageDimension
    FROM [dbo].[OrderProduct] op
    LEFT JOIN [dbo].[Product] p 
        ON op.ProductId = p.ProductId
    LEFT JOIN [dbo].[PackageDimensions] pd 
        ON p.ProdDimensions = pd.PDId
    WHERE op.OrderId = @OrderId";

            var orderLines = await connection.QueryAsync<OrderLine>(orderLinesQuery, new { OrderId = order.Id });

            foreach (var line in orderLines)
            {
                order.OrderLines.Add(line);
            }
        }

        return orders;
    }
    finally
    {
        CloseConnection();
    }
}

    /// <summary>
    /// Retrieves all orders for a specific customer
    /// </summary>
    public async Task<IEnumerable<Order>> GetOrdersByCustomerIdAsync(int customerId)
    {
        try
        {
            connection.Open();

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

            return await connection.QueryAsync<Order, Address, Order>(
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
        finally
        {
            CloseConnection();
        }
    }

    /// <summary>
    /// Updates an existing order
    /// </summary>
    public async Task<bool> UpdateOrderAsync(Order order)
{
    try
    {
        connection.Open();

        using var transaction = connection.BeginTransaction();

        var statusExistsQuery = @"
            SELECT COUNT(1)
            FROM [dbo].[Status]
            WHERE StatusId = @StatusId";

        var statusExists = await connection.QuerySingleAsync<int>(
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

        var currentAddress = await connection.QuerySingleOrDefaultAsync<Address>(
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

            addressId = await connection.QuerySingleAsync<int>(
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

        var rowsAffected = await connection.ExecuteAsync(
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
    finally
    {
        CloseConnection();
    }
}

        /// <summary>
        /// Retrieves all available order statuses.
        /// </summary>
        public async Task<IEnumerable<(int StatusId, string Status)>> GetAllStatusesAsync()
        {
            try
            {
                connection.Open();

                var query = @"
            SELECT StatusId, Status
            FROM [dbo].[Status]
            ORDER BY StatusId";

                return await connection.QueryAsync<(int StatusId, string Status)>(query);
            }
            finally
            {
                CloseConnection();
            }
        }

    /// <summary>
    /// Deletes an order by ID
    /// </summary>
    public async Task<bool> DeleteOrderAsync(int orderId)
    {
        try
        {
            connection.Open();

            using var transaction = connection.BeginTransaction();

            var deleteOrderProductsQuery = @"
            DELETE FROM [dbo].[OrderProduct]
            WHERE OrderId = @OrderId";

            await connection.ExecuteAsync(
                deleteOrderProductsQuery,
                new { OrderId = orderId },
                transaction);

            var deleteOrderQuery = @"
            DELETE FROM [dbo].[Order]
            WHERE OrderId = @OrderId";

            var rowsAffected = await connection.ExecuteAsync(
                deleteOrderQuery,
                new { OrderId = orderId },
                transaction);

            transaction.Commit();

            return rowsAffected > 0;
        }
        finally
        {
            CloseConnection();
        }
    }

    /// <summary>
    /// Creates a new order line item in OrderProduct table
    /// </summary>
    public async Task<bool> CreateOrderLineAsync(OrderLine orderLine)
    {
        try
        {
            connection.Open();

            var query = @"
            INSERT INTO [dbo].[OrderProduct] (OrderId, ProductId, Quantity)
            VALUES (@OrderId, @ProductId, @Quantity)";

            var result = await connection.ExecuteAsync(query, new
            {
                orderLine.OrderId,
                orderLine.ProductId,
                orderLine.Quantity
            });

            return result > 0;
        }
        finally
        {
            CloseConnection();
        }
    }

    /// <summary>
    /// Retrieves all order lines for a specific order with product details
    /// </summary>
    public async Task<IEnumerable<OrderLine>> GetOrderLinesByOrderIdAsync(int orderId)
    {
        try
        {
            connection.Open();

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

            return await connection.QueryAsync<OrderLine>(query, new { OrderId = orderId });
        }
        finally
        {
            CloseConnection();
        }
    }

    /// <summary>
    /// Deletes an order line from OrderProduct table
    /// </summary>
    public async Task<bool> DeleteOrderLineAsync(int orderId, int productId)
    {
        try
        {
            connection.Open();

            var query = @"
            DELETE FROM [dbo].[OrderProduct] 
            WHERE OrderId = @OrderId AND ProductId = @ProductId";

            var rowsAffected = await connection.ExecuteAsync(query, new { OrderId = orderId, ProductId = productId });
            return rowsAffected > 0;
        }
        finally
        {
            CloseConnection();
        }
    }

    public async Task<IEnumerable<Order>> GetOrdersAvailableForDeliveryAsync()
    {
        const string ordersQuery = @"
        SELECT
            o.OrderId AS Id,
            o.OrderDate,
            o.AddressId,
            o.AccountId AS CustomerId,
            o.StatusId,
            s.Status AS OrderStatus,
            COALESCE(a.CustName, a.AccName, 'Onbekende klant') AS CustomerName
        FROM dbo.[Order] o
        LEFT JOIN dbo.Status s
            ON s.StatusId = o.StatusId
        LEFT JOIN dbo.Account a
            ON a.AccId = o.AccountId
        WHERE s.Status IN ('Nieuw', 'Gedeeltelijk in behandeling')
        ORDER BY o.OrderId;
    ";

        const string orderLinesQuery = @"
        SELECT
            op.OrderId,
            op.ProductId,
            op.Quantity,
            op.Quantity - ISNULL(delivered.DeliveredQuantity, 0) AS RemainingQuantity,
            p.ProdName,
            p.ProdPrice,
            p.ProdQuantity AS ProductQuantity,
            p.ProdDimensions AS PackageDimensionId,
            pd.PackageDimensions AS PackageDimension
        FROM dbo.OrderProduct op
        LEFT JOIN dbo.Product p
            ON p.ProductId = op.ProductId
        LEFT JOIN dbo.PackageDimensions pd
            ON pd.PDId = TRY_CONVERT(int, p.ProdDimensions)
        OUTER APPLY
        (
            SELECT SUM(dop.Quantity) AS DeliveredQuantity
            FROM dbo.DeliveryOrderProduct dop
            WHERE dop.OPId = op.OPId
        ) delivered
        WHERE op.OrderId = @OrderId
          AND op.Quantity > ISNULL(delivered.DeliveredQuantity, 0)
        ORDER BY p.ProdName;
    ";

        try
        {
            await connection.OpenAsync();

            var orders = (await connection.QueryAsync<Order>(ordersQuery)).ToList();

            foreach (var order in orders)
            {
                var orderLines = await connection.QueryAsync<dynamic>(
                    orderLinesQuery,
                    new
                    {
                        OrderId = order.Id
                    }
                );

                foreach (var line in orderLines)
                {
                    order.OrderLines.Add(new OrderLine
                    {
                        OrderId = line.OrderId,
                        ProductId = line.ProductId,
                        Quantity = line.Quantity,
                        RemainingQuantity = line.RemainingQuantity,
                        ProdName = line.ProdName,
                        ProdPrice = line.ProdPrice,
                        PackageDimensionId = line.PackageDimensionId,
                        PackageDimension = line.PackageDimension,
                        Product = new Product
                        {
                            Id = line.ProductId,
                            Name = line.ProdName,
                            Price = line.ProdPrice,
                            Quantity = line.ProductQuantity
                        }
                    });
                }
            }

            return orders
                .Where(order => order.OrderLines.Any())
                .ToList();
        }
        finally
        {
            CloseConnection();
        }
    }
    

    public async Task AddOrderAsync(Order order)
    {
        await CreateOrderAsync(order);
    }
}