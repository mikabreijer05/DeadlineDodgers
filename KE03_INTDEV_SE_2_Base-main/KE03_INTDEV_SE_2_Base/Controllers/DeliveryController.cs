using KE03_INTDEV_SE_2_Base.DAL;
using Microsoft.AspNetCore.Mvc;
using KE03_INTDEV_SE_2_Base.Models;
using KE03_INTDEV_SE_2_Base.Models.ViewModels;

namespace KE03_INTDEV_SE_2_Base.Controllers;

[Route("Deliveries")]
public class DeliveryController : Controller
{
    private readonly SQLDelivery _sqlDelivery;
    private readonly SQLOrder _sqlOrder;
    public DeliveryController(SQLDelivery sqlDelivery, SQLOrder sqlOrder)
    {
        _sqlDelivery = sqlDelivery;
        _sqlOrder = sqlOrder;
    }
    // GET
    [HttpGet("")]
    public async Task<IActionResult> Index()
    {
        var deliveries = await _sqlDelivery.GetAllDeliveriesAsync();
        return View("~/Views/Deliveries/Index.cshtml", deliveries);
    }
    [HttpGet("Create")]
    public async Task<IActionResult> Create()
    {
        var orders = await _sqlOrder.GetOrdersAvailableForDeliveryAsync();
        var vehicles = await _sqlDelivery.GetAllVehiclesAsync();

        var viewModel = new CreateDeliveryViewModel
        {
            NewOrders = orders.ToList(),
            Vehicles = vehicles.ToList()
        };

        return View("~/Views/Deliveries/Create.cshtml", viewModel);
    }

    [HttpPost("Create")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateDeliveryViewModel viewModel)
    {
        var selectedProductLines = viewModel.ProductLines
            .Where(line => line.SelectedQuantity > 0)
            .Select(line => new OrderLine
            {
                OrderId = line.OrderId,
                ProductId = line.ProductId,
                Quantity = line.SelectedQuantity
            })
            .ToList();

        if (!selectedProductLines.Any())
        {
            ModelState.AddModelError("", "Selecteer minimaal één product voor de levering.");
        }

        if (viewModel.SelectedVehicleId <= 0)
        {
            ModelState.AddModelError("", "Selecteer een voertuig.");
        }

        if (!ModelState.IsValid)
        {
            var orders = await _sqlOrder.GetOrdersAvailableForDeliveryAsync();
            var vehicles = await _sqlDelivery.GetAllVehiclesAsync();

            viewModel.NewOrders = orders.ToList();
            viewModel.Vehicles = vehicles.ToList();

            return View("~/Views/Deliveries/Create.cshtml", viewModel);
        }

        var delivery = new Delivery
        {
            ToBeSentDate = viewModel.ToBeSentDate,
            Vehicle = new Vehicle
            {
                Id = viewModel.SelectedVehicleId
            },
            ProductLines = selectedProductLines
        };

        await _sqlDelivery.CreateDeliveryAsync(delivery);

        return RedirectToAction(nameof(Index));
    }
    
    [HttpGet("Edit/{id:int}")]
    public async Task<IActionResult> Edit(int id)
    {
        var delivery = await _sqlDelivery.GetDeliveryByIdAsync(id);

        if (delivery == null)
        {
            return NotFound();
        }

        var orders = (await _sqlOrder.GetOrdersAvailableForDeliveryAsync()).ToList();
        var vehicles = (await _sqlDelivery.GetAllVehiclesAsync()).ToList();

        foreach (var productLine in delivery.ProductLines)
        {
            var existingOrder = orders.FirstOrDefault(order => order.Id == productLine.OrderId);

            if (existingOrder == null)
            {
                existingOrder = new Order
                {
                    Id = productLine.OrderId,
                    CustomerName = "Bestaande levering"
                };

                orders.Add(existingOrder);
            }

            var existingLine = existingOrder.OrderLines.FirstOrDefault(line => line.ProductId == productLine.ProductId);

            if (existingLine == null)
            {
                existingOrder.OrderLines.Add(new OrderLine
                {
                    OrderId = productLine.OrderId,
                    ProductId = productLine.ProductId,
                    Quantity = productLine.Quantity,
                    RemainingQuantity = productLine.Quantity,
                    ProdName = productLine.ProdName,
                    ProdPrice = productLine.ProdPrice,
                    PackageDimensionId = productLine.PackageDimensionId,
                    PackageDimension = productLine.PackageDimension,
                    Product = productLine.Product
                });
            }
            else
            {
                existingLine.RemainingQuantity += productLine.Quantity;
            }
        }

        var viewModel = new CreateDeliveryViewModel
        {
            DeliveryId = delivery.Id,
            IsEditMode = true,
            ToBeSentDate = delivery.ToBeSentDate,
            SelectedVehicleId = delivery.Vehicle?.Id ?? 0,
            SelectedOrderIds = delivery.ProductLines
                .Select(line => line.OrderId)
                .Distinct()
                .ToList(),
            NewOrders = orders,
            Vehicles = vehicles,
            ProductLines = delivery.ProductLines
                .Select(line => new CreateDeliveryProductLineViewModel
                {
                    OrderId = line.OrderId,
                    ProductId = line.ProductId,
                    ProductName = line.ProdName ?? $"Product {line.ProductId}",
                    PackageDimension = line.PackageDimension ?? "Onbekend",
                    AvailableQuantity = line.Quantity,
                    SelectedQuantity = line.Quantity
                })
                .ToList()
        };

        return View("~/Views/Deliveries/Edit.cshtml", viewModel);
    }

    [HttpPost("Edit/{id:int}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, CreateDeliveryViewModel viewModel)
    {
        var selectedProductLines = viewModel.ProductLines
            .Where(line => line.SelectedQuantity > 0)
            .Select(line => new OrderLine
            {
                OrderId = line.OrderId,
                ProductId = line.ProductId,
                Quantity = line.SelectedQuantity
            })
            .ToList();

        if (!selectedProductLines.Any())
        {
            ModelState.AddModelError("", "Selecteer minimaal één product voor de levering.");
        }

        if (viewModel.SelectedVehicleId <= 0)
        {
            ModelState.AddModelError("", "Selecteer een voertuig.");
        }

        if (!ModelState.IsValid)
        {
            var orders = await _sqlOrder.GetOrdersAvailableForDeliveryAsync();
            var vehicles = await _sqlDelivery.GetAllVehiclesAsync();

            viewModel.DeliveryId = id;
            viewModel.IsEditMode = true;
            viewModel.NewOrders = orders.ToList();
            viewModel.Vehicles = vehicles.ToList();

            return View("~/Views/Deliveries/Edit.cshtml", viewModel);
        }

        var delivery = new Delivery
        {
            Id = id,
            ToBeSentDate = viewModel.ToBeSentDate,
            Vehicle = new Vehicle
            {
                Id = viewModel.SelectedVehicleId
            },
            ProductLines = selectedProductLines
        };

        await _sqlDelivery.UpdateDeliveryAsync(delivery);

        return RedirectToAction(nameof(Index));
    }

    [HttpPost("Print/{id:int}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Print(int id)
    {
        var result = await _sqlDelivery.PrintDeliveryAsync(id);

        if (!result.DeliveryFound)
        {
            return NotFound(new
            {
                success = false,
                statusChanged = false,
                updatedOrderCount = 0,
                message = "Levering kon niet gevonden worden."
            });
        }

        if (result.StatusChanged)
        {
            return Ok(new
            {
                success = true,
                statusChanged = true,
                updatedOrderCount = result.UpdatedOrderCount,
                message = result.UpdatedOrderCount == 1
                    ? "The delivery details are being printed. 1 order status has been changed to 'Klaar om te verzenden'."
                    : $"The delivery details are being printed. {result.UpdatedOrderCount} order statuses have been changed to 'Klaar om te verzenden'."
            });
        }

        return Ok(new
        {
            success = true,
            statusChanged = false,
            updatedOrderCount = 0,
            message =
                "The delivery details are being printed. No order status was changed because the order is not fully printed yet, or it was already 'Klaar om te verzenden'."
        });
    }

    [HttpPost("Delete/{id:int}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        await _sqlDelivery.DeleteDeliveryAsync(id);

        return RedirectToAction(nameof(Index));
    }
}