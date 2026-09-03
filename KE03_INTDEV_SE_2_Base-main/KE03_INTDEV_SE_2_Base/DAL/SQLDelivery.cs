using Dapper;
using KE03_INTDEV_SE_2_Base.Models;

namespace KE03_INTDEV_SE_2_Base.DAL;

public class SQLDelivery(IConfiguration configuration) : SQLDAL(configuration)
{
    public async Task<int> CreateDeliveryAsync(Delivery delivery)
    {
        const string insertDeliverySql = @"
        INSERT INTO dbo.Delivery
        (
            DeliveryId,
            ToBeSentDate,
            VehicleId,
            Signed,
            Printed
        )
        VALUES
        (
            @DeliveryId,
            @ToBeSentDate,
            @VehicleId,
            @Signed,
            @Printed
        );
        ";

        const string getNextDeliveryIdSql = @"
        SELECT ISNULL(MAX(DeliveryId), 0) + 1
        FROM dbo.Delivery;
        ";

        const string insertDeliveryOrderProductSql = @"
        INSERT INTO dbo.DeliveryOrderProduct
        (
            DOPId,
            DeliveryId,
            OPId,
            Quantity
        )
        VALUES
        (
            @DOPId,
            @DeliveryId,
            @OPId,
            @Quantity
        );
        ";

        const string getNextDopIdSql = @"
        SELECT ISNULL(MAX(DOPId), 0) + 1
        FROM dbo.DeliveryOrderProduct;
        ";

        try
        {
            await connection.OpenAsync();

            int deliveryId = await connection.QuerySingleAsync<int>(getNextDeliveryIdSql);

            await connection.ExecuteAsync(
                insertDeliverySql,
                new
                {
                    DeliveryId = deliveryId,
                    ToBeSentDate = delivery.ToBeSentDate,
                    VehicleId = delivery.Vehicle?.Id,
                    Signed = false,
                    Printed = false
                }
            );

            var affectedOrderIds = new HashSet<int>();

            if (delivery.ProductLines != null)
            {
                foreach (OrderLine productLine in delivery.ProductLines)
                {
                    int dopId = await connection.QuerySingleAsync<int>(getNextDopIdSql);

                    int? opId = await GetOrderProductIdAsync(
                        productLine.OrderId,
                        productLine.ProductId
                    );

                    if (opId == null)
                    {
                        continue;
                    }

                    await connection.ExecuteAsync(
                        insertDeliveryOrderProductSql,
                        new
                        {
                            DOPId = dopId,
                            DeliveryId = deliveryId,
                            OPId = opId.Value,
                            Quantity = productLine.Quantity
                        }
                    );

                    affectedOrderIds.Add(productLine.OrderId);
                }
            }

            foreach (int orderId in affectedOrderIds)
            {
                await UpdateOrderDeliveryStatusAsync(orderId);
            }

            return deliveryId;
        }
        finally
        {
            CloseConnection();
        }
    }

    public async Task<IEnumerable<Delivery>> GetAllDeliveriesAsync()
    {
        const string sql = @"
        SELECT
            d.DeliveryId,
            d.ToBeSentDate,
            v.VehicleId,
            vt.VehicleType,
            v.LicensePlate,
            v.TotalKM,
            l.LocationCode AS ParkingLocation,
            op.OrderId,
            s.Status AS OrderStatus,
            op.ProductId,
            dop.Quantity,
            p.ProdName,
            p.ProdPrice,
            p.ProdQuantity AS ProductQuantity
        FROM dbo.Delivery d
        LEFT JOIN dbo.Vehicle v
            ON v.VehicleId = d.VehicleId
        LEFT JOIN dbo.VehicleType vt
            ON vt.VehicleTypeId = v.VehicleTypeId
        LEFT JOIN dbo.Location l
            ON l.LocationId = v.ParkingLocationId
        LEFT JOIN dbo.DeliveryOrderProduct dop
            ON dop.DeliveryId = d.DeliveryId
        LEFT JOIN dbo.OrderProduct op
            ON op.OPId = dop.OPId
        LEFT JOIN dbo.[Order] o
            ON o.OrderId = op.OrderId
        LEFT JOIN dbo.Status s
            ON s.StatusId = o.StatusId
        LEFT JOIN dbo.Product p
            ON p.ProductId = op.ProductId
        ORDER BY d.DeliveryId;
        ";

        try
        {
            await connection.OpenAsync();

            IEnumerable<dynamic> rows = await connection.QueryAsync(sql);

            Dictionary<int, Delivery> deliveries = new();

            foreach (dynamic row in rows)
            {
                int deliveryId = row.DeliveryId;

                if (!deliveries.TryGetValue(deliveryId, out Delivery? delivery))
                {
                    delivery = new Delivery
                    {
                        Id = deliveryId,
                        ToBeSentDate = row.ToBeSentDate,
                        ProductLines = new List<OrderLine>(),
                        Vehicle = row.VehicleId == null
                            ? null!
                            : new Vehicle
                            {
                                Id = row.VehicleId,
                                VehicleType = row.VehicleType,
                                LicensePlate = row.LicensePlate,
                                TotalKM = row.TotalKM,
                                ParkingLocation = row.ParkingLocation,
                                ProductDimensions = new List<string>()
                            }
                    };

                    deliveries.Add(deliveryId, delivery);
                }

                if (row.OrderId != null && row.ProductId != null)
                {
                    delivery.ProductLines.Add(
                        new OrderLine
                        {
                            OrderId = row.OrderId,
                            OrderStatus = row.OrderStatus,
                            ProductId = row.ProductId,
                            Quantity = row.Quantity,
                            ProdName = row.ProdName,
                            ProdPrice = row.ProdPrice,
                            Product = new Product
                            {
                                Id = row.ProductId,
                                Name = row.ProdName,
                                Price = row.ProdPrice,
                                Quantity = row.ProductQuantity
                            }
                        }
                    );
                }
            }

            return deliveries.Values.ToList();
        }
        finally
        {
            CloseConnection();
        }
    }
    
    public async Task<Delivery?> GetDeliveryByIdAsync(int id)
    {
        const string sql = @"
        SELECT
            d.DeliveryId,
            d.ToBeSentDate,
            v.VehicleId,
            vt.VehicleType,
            v.LicensePlate,
            v.TotalKM,
            l.LocationCode AS ParkingLocation,
            op.OrderId,
            op.ProductId,
            dop.Quantity,
            p.ProdName,
            p.ProdPrice,
            p.ProdQuantity AS ProductQuantity,
            p.ProdDimensions AS PackageDimensionId,
            pd.PackageDimensions AS PackageDimension
        FROM dbo.Delivery d
        LEFT JOIN dbo.Vehicle v
            ON v.VehicleId = d.VehicleId
        LEFT JOIN dbo.VehicleType vt
            ON vt.VehicleTypeId = v.VehicleTypeId
        LEFT JOIN dbo.Location l
            ON l.LocationId = v.ParkingLocationId
        LEFT JOIN dbo.DeliveryOrderProduct dop
            ON dop.DeliveryId = d.DeliveryId
        LEFT JOIN dbo.OrderProduct op
            ON op.OPId = dop.OPId
        LEFT JOIN dbo.Product p
            ON p.ProductId = op.ProductId
        LEFT JOIN dbo.PackageDimensions pd
            ON pd.PDId = TRY_CONVERT(int, p.ProdDimensions)
        WHERE d.DeliveryId = @DeliveryId
        ORDER BY d.DeliveryId;
        ";

        try
        {
            await connection.OpenAsync();

            IEnumerable<dynamic> rows = await connection.QueryAsync(
                sql,
                new
                {
                    DeliveryId = id
                }
            );

            Delivery? delivery = null;

            foreach (dynamic row in rows)
            {
                if (delivery == null)
                {
                    delivery = new Delivery
                    {
                        Id = row.DeliveryId,
                        ToBeSentDate = row.ToBeSentDate,
                        ProductLines = new List<OrderLine>(),
                        Vehicle = row.VehicleId == null
                            ? null!
                            : new Vehicle
                            {
                                Id = row.VehicleId,
                                VehicleType = row.VehicleType,
                                LicensePlate = row.LicensePlate,
                                TotalKM = row.TotalKM,
                                ParkingLocation = row.ParkingLocation,
                                ProductDimensions = new List<string>()
                            }
                    };
                }

                if (row.OrderId != null && row.ProductId != null)
                {
                    delivery.ProductLines.Add(
                        new OrderLine
                        {
                            OrderId = row.OrderId,
                            ProductId = row.ProductId,
                            Quantity = row.Quantity,
                            ProdName = row.ProdName,
                            ProdPrice = row.ProdPrice,
                            PackageDimensionId = row.PackageDimensionId,
                            PackageDimension = row.PackageDimension,
                            Product = new Product
                            {
                                Id = row.ProductId,
                                Name = row.ProdName,
                                Price = row.ProdPrice,
                                Quantity = row.ProductQuantity
                            }
                        }
                    );
                }
            }

            return delivery;
        }
        finally
        {
            CloseConnection();
        }
    }

    public async Task UpdateDeliveryAsync(Delivery delivery)
    {
        const string updateDeliverySql = @"
        UPDATE dbo.Delivery
        SET
            ToBeSentDate = @ToBeSentDate,
            VehicleId = @VehicleId
        WHERE DeliveryId = @DeliveryId;
        ";

        const string deleteDeliveryOrderProductsSql = @"
        DELETE FROM dbo.DeliveryOrderProduct
        WHERE DeliveryId = @DeliveryId;
        ";

        const string insertDeliveryOrderProductSql = @"
        INSERT INTO dbo.DeliveryOrderProduct
        (
            DOPId,
            DeliveryId,
            OPId,
            Quantity
        )
        VALUES
        (
            @DOPId,
            @DeliveryId,
            @OPId,
            @Quantity
        );
        ";

        const string getNextDopIdSql = @"
        SELECT ISNULL(MAX(DOPId), 0) + 1
        FROM dbo.DeliveryOrderProduct;
        ";

        try
        {
            await connection.OpenAsync();
            
            await connection.ExecuteAsync(
                updateDeliverySql,
                new
                {
                    DeliveryId = delivery.Id,
                    ToBeSentDate = delivery.ToBeSentDate,
                    VehicleId = delivery.Vehicle?.Id
                }
            );

            await connection.ExecuteAsync(
                deleteDeliveryOrderProductsSql,
                new
                {
                    DeliveryId = delivery.Id
                }
            );

            if (delivery.ProductLines != null)
            {
                foreach (OrderLine productLine in delivery.ProductLines)
                {
                    int dopId = await connection.QuerySingleAsync<int>(getNextDopIdSql);

                    int? opId = await GetOrderProductIdAsync(
                        productLine.OrderId,
                        productLine.ProductId
                    );

                    if (opId == null)
                    {
                        continue;
                    }

                    await connection.ExecuteAsync(
                        insertDeliveryOrderProductSql,
                        new
                        {
                            DOPId = dopId,
                            DeliveryId = delivery.Id,
                            OPId = opId.Value,
                            Quantity = productLine.Quantity
                        }
                    );
                }
            }
        }
        finally
        {
            CloseConnection();
        }
    }

    public async Task DeleteDeliveryAsync(int id)
    {
        const string deleteDeliveryOrderProductsSql = @"
        DELETE FROM dbo.DeliveryOrderProduct
        WHERE DeliveryId = @DeliveryId;
        ";

        const string deleteDeliverySql = @"
        DELETE FROM dbo.Delivery
        WHERE DeliveryId = @DeliveryId;
        ";

        try
        {
            await connection.OpenAsync();

            await connection.ExecuteAsync(
                deleteDeliveryOrderProductsSql,
                new
                {
                    DeliveryId = id
                }
            );

            await connection.ExecuteAsync(
                deleteDeliverySql,
                new
                {
                    DeliveryId = id
                }
            );
        }
        finally
        {
            CloseConnection();
        }
    }

    public async Task<IEnumerable<Vehicle>> GetAllVehiclesAsync()
    {
        const string sql = @"
        SELECT
            v.VehicleId AS Id,
            vt.VehicleType,
            v.LicensePlate,
            v.TotalKM,
            l.LocationCode AS ParkingLocation,
            ISNULL(MAX(CASE WHEN UPPER(LTRIM(RTRIM(pd.PackageDimensions))) = 'XS' THEN vpd.MaxAmount END), 0) AS MaxXsPackages,
            ISNULL(MAX(CASE WHEN UPPER(LTRIM(RTRIM(pd.PackageDimensions))) = 'S' THEN vpd.MaxAmount END), 0) AS MaxSPackages,
            ISNULL(MAX(CASE WHEN UPPER(LTRIM(RTRIM(pd.PackageDimensions))) = 'M' THEN vpd.MaxAmount END), 0) AS MaxMPackages,
            ISNULL(MAX(CASE WHEN UPPER(LTRIM(RTRIM(pd.PackageDimensions))) = 'L' THEN vpd.MaxAmount END), 0) AS MaxLPackages,
            ISNULL(MAX(CASE WHEN UPPER(LTRIM(RTRIM(pd.PackageDimensions))) = 'XL' THEN vpd.MaxAmount END), 0) AS MaxXlPackages
        FROM dbo.Vehicle v
        LEFT JOIN dbo.VehicleType vt
            ON vt.VehicleTypeId = v.VehicleTypeId
        LEFT JOIN dbo.Location l
            ON l.LocationId = v.ParkingLocationId
        LEFT JOIN dbo.VehiclePackageDimensions vpd
            ON vpd.VehicleTypeId = v.VehicleTypeId
        LEFT JOIN dbo.PackageDimensions pd
            ON pd.PDId = vpd.PDId
        GROUP BY
            v.VehicleId,
            vt.VehicleType,
            v.LicensePlate,
            v.TotalKM,
            l.LocationCode
        ORDER BY v.VehicleId ASC;
        ";

        try
        {
            await connection.OpenAsync();

            return await connection.QueryAsync<Vehicle>(sql);
        }
        finally
        {
            CloseConnection();
        }
    }

    private async Task<int?> GetOrderProductIdAsync(int orderId, int productId)
    {
        const string sql = @"
        SELECT TOP 1 OPId
        FROM dbo.OrderProduct
        WHERE OrderId = @OrderId
          AND ProductId = @ProductId;
        ";

        return await connection.QueryFirstOrDefaultAsync<int?>(
            sql,
            new
            {
                OrderId = orderId,
                ProductId = productId
            }
        );
    }
    private async Task UpdateOrderDeliveryStatusAsync(int orderId)
    {
        const int partiallyInProgressStatusId = 8;

        const string getInProgressStatusIdSql = @"
        SELECT TOP 1 StatusId
        FROM dbo.Status
        WHERE Status = 'In behandeling';
        ";

        const string getQuantitiesSql = @"
        SELECT
            ISNULL(SUM(op.Quantity), 0) AS OrderedQuantity,
            ISNULL(SUM(delivered.DeliveredQuantity), 0) AS DeliveredQuantity
        FROM dbo.OrderProduct op
        OUTER APPLY
        (
            SELECT SUM(dop.Quantity) AS DeliveredQuantity
            FROM dbo.DeliveryOrderProduct dop
            WHERE dop.OPId = op.OPId
        ) delivered
        WHERE op.OrderId = @OrderId;
        ";

        const string updateOrderStatusSql = @"
        UPDATE dbo.[Order]
        SET StatusId = @StatusId
        WHERE OrderId = @OrderId;
        ";

        int? inProgressStatusId = await connection.QueryFirstOrDefaultAsync<int?>(
            getInProgressStatusIdSql
        );

        if (inProgressStatusId == null)
        {
            throw new InvalidOperationException("Status 'In behandeling' kon niet gevonden worden in dbo.Status.");
        }

        dynamic quantities = await connection.QuerySingleAsync(
            getQuantitiesSql,
            new
            {
                OrderId = orderId
            }
        );

        int orderedQuantity = quantities.OrderedQuantity;
        int deliveredQuantity = quantities.DeliveredQuantity;

        int newStatusId = deliveredQuantity >= orderedQuantity
            ? inProgressStatusId.Value
            : partiallyInProgressStatusId;

        await connection.ExecuteAsync(
            updateOrderStatusSql,
            new
            {
                OrderId = orderId,
                StatusId = newStatusId
            }
        );
    }
    
    public async Task<PrintDeliveryResult> PrintDeliveryAsync(int deliveryId)
    {
        const string deliveryExistsSql = @"
        SELECT COUNT(1)
        FROM dbo.Delivery
        WHERE DeliveryId = @DeliveryId;
        ";

        const string markDeliveryPrintedSql = @"
        UPDATE dbo.Delivery
        SET Printed = 1
        WHERE DeliveryId = @DeliveryId
          AND Printed = 0;
        ";

        const string getReadyToSendStatusIdSql = @"
        SELECT TOP 1 StatusId
        FROM dbo.Status
        WHERE Status = 'Klaar om te verzenden';
        ";

        const string updateFullyPrintedOrdersSql = @"
        UPDATE o
        SET o.StatusId = @ReadyToSendStatusId
        FROM dbo.[Order] o
        INNER JOIN
        (
            SELECT
                op.OrderId
            FROM dbo.OrderProduct op
            WHERE op.OrderId IN
            (
                SELECT DISTINCT op2.OrderId
                FROM dbo.DeliveryOrderProduct dop2
                INNER JOIN dbo.OrderProduct op2
                    ON op2.OPId = dop2.OPId
                WHERE dop2.DeliveryId = @DeliveryId
            )
            GROUP BY op.OrderId
            HAVING
                SUM(op.Quantity) <=
                (
                    SELECT ISNULL(SUM(printedDop.Quantity), 0)
                    FROM dbo.OrderProduct printedOp
                    INNER JOIN dbo.DeliveryOrderProduct printedDop
                        ON printedDop.OPId = printedOp.OPId
                    INNER JOIN dbo.Delivery printedDelivery
                        ON printedDelivery.DeliveryId = printedDop.DeliveryId
                    WHERE printedOp.OrderId = op.OrderId
                      AND printedDelivery.Printed = 1
                )
        ) fullyPrintedOrders
            ON fullyPrintedOrders.OrderId = o.OrderId
        WHERE ISNULL(o.StatusId, 0) <> @ReadyToSendStatusId;
        ";

        try
        {
            await connection.OpenAsync();

            using var transaction = connection.BeginTransaction();

            int deliveryExists = await connection.QuerySingleAsync<int>(
                deliveryExistsSql,
                new
                {
                    DeliveryId = deliveryId
                },
                transaction
            );

            if (deliveryExists == 0)
            {
                transaction.Rollback();

                return new PrintDeliveryResult
                {
                    DeliveryFound = false,
                    StatusChanged = false,
                    UpdatedOrderCount = 0
                };
            }

            await connection.ExecuteAsync(
                markDeliveryPrintedSql,
                new
                {
                    DeliveryId = deliveryId
                },
                transaction
            );

            int? readyToSendStatusId = await connection.QueryFirstOrDefaultAsync<int?>(
                getReadyToSendStatusIdSql,
                transaction: transaction
            );

            if (readyToSendStatusId == null)
            {
                transaction.Rollback();
                throw new InvalidOperationException("Status 'Klaar om te verzenden' kon niet gevonden worden in dbo.Status.");
            }

            int updatedOrderCount = await connection.ExecuteAsync(
                updateFullyPrintedOrdersSql,
                new
                {
                    DeliveryId = deliveryId,
                    ReadyToSendStatusId = readyToSendStatusId.Value
                },
                transaction
            );

            transaction.Commit();

            return new PrintDeliveryResult
            {
                DeliveryFound = true,
                StatusChanged = updatedOrderCount > 0,
                UpdatedOrderCount = updatedOrderCount
            };
        }
        finally
        {
            CloseConnection();
        }
    }
}