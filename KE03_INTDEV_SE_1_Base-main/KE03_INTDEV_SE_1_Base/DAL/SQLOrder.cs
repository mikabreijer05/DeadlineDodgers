using Dapper;
using KE03_INTDEV_SE_1_Base.Models;

namespace KE03_INTDEV_SE_1_Base.DAL;

public class SQLOrder(IConfiguration configuration) : SQLDAL(configuration)
{
    public IEnumerable<Order> GetOrdersByCustomerId(int customerId)
    {
        const string sql = """
                           SELECT
                               o.OrderId      AS Id,
                               o.OrderDate,
                               o.AddressId,
                               o.AccountId    AS CustomerId,
                               s.StatusId     AS StatusId,
                               s.Status       AS StatusName,
                               a.AddressId    AS AddressId,
                               a.Street,
                               a.HouseNumber,
                               a.PostalCode,
                               a.City,
                               a.Country,
                               op.OPId        AS Id,
                               op.OrderId,
                               op.ProductId,
                               op.Quantity,
                               p.ProdPrice    AS PricePerProduct,
                               p.ProductId    AS Id,
                               p.ProdName     AS Name,
                               p.ProdPrice    AS Price
                           FROM dbo.[Order] o
                           LEFT JOIN dbo.[Address] a ON o.AddressId = a.AddressId
                           JOIN dbo.[OrderProduct] op ON o.OrderId = op.OrderId
                           JOIN dbo.[Product] p       ON op.ProductId = p.ProductId
                           JOIN dbo.[Status] s        ON o.StatusId = s.StatusId
                           WHERE o.AccountId = @CustomerId
                           ORDER BY o.OrderDate DESC
                           """;

        try
        {
            connection.Open();

            var orderDictionary = new Dictionary<int, Order>();

            connection.Query<Order, Status, Address, OrderLine, Product, Order>(
                sql,
                (order, status, address, orderLine, product) =>
                {
                    if (!orderDictionary.TryGetValue(order.Id, out var existingOrder))
                    {
                        existingOrder = order;
                        existingOrder.Address = address;
                        existingOrder.Status = status;
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
                splitOn: "StatusId,AddressId,Id,Id");

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
                                          INSERT INTO dbo.[Order] (AccountId, OrderDate, AddressId, StatusId)
                                          VALUES (
                                              @CustomerId,
                                              @OrderDate,
                                              COALESCE(
                                                  @AddressId,
                                                  (
                                                      SELECT TOP 1 aa.Address
                                                      FROM dbo.[AccountAddress] aa
                                                      WHERE aa.Account = @CustomerId
                                                      ORDER BY aa.AAId
                                                  )
                                              ),
                                              1
                                          );

                                          SELECT CAST(SCOPE_IDENTITY() AS int);
                                          """;

            const string insertOrderProductSql = """
                                                 INSERT INTO dbo.[OrderProduct] (OPId, OrderId, ProductId, Quantity)
                                                 VALUES (
                                                     (
                                                         SELECT ISNULL(MAX(op.OPId), 0) + 1
                                                         FROM dbo.[OrderProduct] op WITH (UPDLOCK, HOLDLOCK)
                                                     ),
                                                     @OrderId,
                                                     @ProductId,
                                                     @Quantity
                                                 );
                                                 """;

            try
            {
                connection.Open();

                using var transaction = connection.BeginTransaction();

                order.Id = connection.QuerySingle<int>(
                    insertOrderSql,
                    new { order.CustomerId, order.OrderDate, order.AddressId },
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