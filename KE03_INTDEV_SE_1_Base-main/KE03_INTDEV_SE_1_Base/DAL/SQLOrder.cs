using Dapper;
using KE03_INTDEV_SE_1_Base.Models;

namespace KE03_INTDEV_SE_1_Base.DAL;

public class SQLOrder : SQLDAL
{
    public IEnumerable<Order> GetOrdersByCustomerId(int customerId)
    {
        const string sql = """
                           SELECT
                               o.OrderId   AS Id,
                               o.OrderDate,
                               o.AccountId AS CustomerId,
                               op.OPId     AS Id,
                               op.OrderId,
                               op.ProductId,
                               op.Quantity,
                               p.ProdPrice AS PricePerProduct,
                               p.ProductId AS Id,
                               p.ProdName  AS Name,
                               p.ProdPrice AS Price
                           FROM dbo.[Order] o
                           JOIN dbo.[OrderProduct] op ON o.OrderId = op.OrderId
                           JOIN dbo.[Product] p       ON op.ProductId = p.ProductId
                           WHERE o.AccountId = @CustomerId
                           ORDER BY o.OrderDate DESC
                           """;

        try
        {
            connection.Open();

            var orderDictionary = new Dictionary<int, Order>();

            connection.Query<Order, OrderLine, Product, Order>(
                sql,
                (order, orderLine, product) =>
                {
                    if (!orderDictionary.TryGetValue(order.Id, out var existingOrder))
                    {
                        existingOrder = order;
                        orderDictionary.Add(order.Id, existingOrder);
                    }

                    orderLine.Order = existingOrder;
                    orderLine.Product = product;
                    existingOrder.AddOrderLine(orderLine);

                    if (existingOrder.Products.All(p => p.Id != product.Id))
                    {
                        existingOrder.AddProduct(product);
                    }

                    return existingOrder;
                },
                new { CustomerId = customerId },
                splitOn: "Id,Id");

            return orderDictionary.Values.ToList();
        }
        finally
        {
            CloseConnection();
        }
    }

    public void AddOrder(Order order)
    {
        const string insertOrderSql = """
                                      INSERT INTO dbo.[Order] (AccountId, OrderDate)
                                      VALUES (@CustomerId, @OrderDate);

                                      SELECT CAST(SCOPE_IDENTITY() AS int);
                                      """;

        const string insertOrderProductSql = """
                                             INSERT INTO dbo.[OrderProduct] (OrderId, ProductId, Quantity)
                                             VALUES (@OrderId, @ProductId, @Quantity);
                                             """;

        try
        {
            connection.Open();

            using var transaction = connection.BeginTransaction();

            order.Id = connection.QuerySingle<int>(
                insertOrderSql,
                new { order.CustomerId, order.OrderDate },
                transaction);

            foreach (var orderLine in order.OrderLines)
            {
                connection.Execute(
                    insertOrderProductSql,
                    new
                    {
                        OrderId = order.Id,
                        orderLine.ProductId,
                        orderLine.Quantity
                    },
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