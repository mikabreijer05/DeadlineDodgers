using Dapper;
using KE03_INTDEV_SE_1_Base.DAL;
using KE03_INTDEV_SE_1_Base.Models;

namespace KE03_INTDEV_SE_1_Base.DAL;

public class SQLOrder : SQLDAL
{
    public IEnumerable<Order> GetOrdersByCustomerId(int customerId)
    {
        const string sql = @"
            SELECT
                o.OrderId    AS Id,
                o.OrderDate  AS OrderDate,
                o.CustomerId AS CustomerId,

                ol.OrderLineId      AS Id,
                ol.OrderId          AS OrderId,
                ol.ProductId        AS ProductId,
                ol.Quantity         AS Quantity,
                ol.PricePerProduct  AS PricePerProduct,

                p.ProductId AS Id,
                p.ProdName  AS Name,
                p.ProdPrice AS Price
            FROM dbo.[Order] o
            LEFT JOIN dbo.OrderLine ol ON ol.OrderId = o.OrderId
            LEFT JOIN dbo.Product p    ON p.ProductId = ol.ProductId
            WHERE o.CustomerId = @CustomerId
            ORDER BY o.OrderDate DESC;";

        var orderMap = new Dictionary<int, Order>();

        try
        {
            connection.Open();

            connection.Query<Order, OrderLine, Product, Order>(
                sql,
                (order, orderLine, product) =>
                {
                    if (!orderMap.TryGetValue(order.Id, out var existingOrder))
                    {
                        existingOrder = order;
                        orderMap.Add(existingOrder.Id, existingOrder);
                    }

                    if (orderLine is { Id: > 0 })
                    {
                        orderLine.Product = product;
                        existingOrder.OrderLines.Add(orderLine);

                        if (product is { Id: > 0 })
                        {
                            existingOrder.Products.Add(product);
                        }
                    }

                    return existingOrder;
                },
                new { CustomerId = customerId },
                splitOn: "Id,Id");

            return orderMap.Values.ToList();
        }
        finally
        {
            CloseConnection();
        }
    }

    public void AddOrder(Order order)
    {
        const string insertOrderSql = @"
            INSERT INTO dbo.[Order] (CustomerId, OrderDate)
            VALUES (@CustomerId, @OrderDate);
            SELECT CAST(SCOPE_IDENTITY() AS int);";

        const string insertOrderLineSql = @"
            INSERT INTO dbo.OrderLine (OrderId, ProductId, Quantity, PricePerProduct)
            VALUES (@OrderId, @ProductId, @Quantity, @PricePerProduct);";

        try
        {
            connection.Open();
            using var transaction = connection.BeginTransaction();

            order.Id = connection.QuerySingle<int>(
                insertOrderSql,
                new { order.CustomerId, order.OrderDate },
                transaction);

            foreach (var line in order.OrderLines)
            {
                connection.Execute(
                    insertOrderLineSql,
                    new { OrderId = order.Id, line.ProductId, line.Quantity, line.PricePerProduct },
                    transaction);
            }

            transaction.Commit();
        }
        finally
        {
            CloseConnection();
        }
    }
}