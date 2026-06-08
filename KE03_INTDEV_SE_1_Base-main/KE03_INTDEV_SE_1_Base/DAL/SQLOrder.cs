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
                o.OrderId       AS OrderId,
                o.OrderDate     AS OrderDate,

                op.ProductId    AS ProductId,
                op.Quantity      AS Quantity,
                p.ProdPrice     AS PricePerProduct,

                p.ProductId     AS productId, -- Renamed to avoid conflict with existing columns
                p.ProdName      AS Name,
                p.ProdPrice     AS ProdPrice
            FROM dbo.[Order] o
            LEFT JOIN dbo.OrderProduct op ON op.OrderId = o.OrderId
            LEFT JOIN dbo.Product p    ON p.ProductId = op.ProductId
            WHERE o.AccountId = @CustomerId
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
                splitOn: "OrderId,ProductId,productId"); 

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

        const string insertOrderProductSql = @"
            INSERT INTO dbo.OrderProduct (OrderId, ProductId, Quantity)
            VALUES (@OrderId, @ProductId, @Quantity);";

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
                    insertOrderProductSql,
                    new { OrderId = order.Id, line.ProductId, line.Quantity },
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