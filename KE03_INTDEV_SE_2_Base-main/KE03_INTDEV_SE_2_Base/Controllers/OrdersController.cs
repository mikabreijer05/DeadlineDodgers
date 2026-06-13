using KE03_INTDEV_SE_2_Base.DAL;
using Microsoft.AspNetCore.Mvc;
using KE03_INTDEV_SE_2_Base.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace KE03_INTDEV_SE_2_Base.Controllers;

public class OrdersController : Controller
{
    private readonly SQLOrder _orderDAL;

    public OrdersController(SQLOrder orderDAL)
    {
        _orderDAL = orderDAL;
    }

    // GET: Order
    public async Task<IActionResult> Index()
    {
        var orders = await _orderDAL.GetAllOrdersAsync();
        return View(orders);
    }

    // GET: Order/Details/5
    public async Task<IActionResult> Details(int id)
    {
        if (id == null || id <= 0)
        {
            return NotFound();
        }

        var order = await _orderDAL.GetOrderByIdAsync(id);
        if (order == null)
        {
            return NotFound();
        }

        return View(order);
    }

    // GET: Order/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: Order/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Order order)
    {
        if (ModelState.IsValid)
        {
            await _orderDAL.AddOrderAsync(order);
            return RedirectToAction(nameof(Index));
        }
        return View(order);
    }

    // GET: Order/Edit/5
    public async Task<IActionResult> Edit(int id)
    {
        if (id == null || id <= 0)
        {
            return NotFound();
        }

        var order = await _orderDAL.GetOrderByIdAsync(id);
        if (order == null)
        {
            return NotFound();
        }

        order.Address ??= new Address();

        await PopulateStatusOptionsAsync(order.StatusId);

        return View(order);
    }

    // POST: Order/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Order order)
    {
        if (id != order.Id)
        {
            return NotFound();
        }

        order.Address ??= new Address();

        ModelState.Remove(nameof(order.Customer));
        ModelState.Remove(nameof(order.CustomerName));
        ModelState.Remove(nameof(order.OrderStatus));
        ModelState.Remove(nameof(order.Products));
        ModelState.Remove(nameof(order.OrderLines));

        if (!IsValidOrderStatus(order.StatusId))
        {
            ModelState.AddModelError(nameof(order.StatusId), "Please select a valid order status.");
        }

        if (ModelState.IsValid)
        {
            var updated = await _orderDAL.UpdateOrderAsync(order);

            if (!updated)
            {
                ModelState.AddModelError(string.Empty, "Order could not be updated. Please check if the order and status still exist.");
                await PopulateStatusOptionsAsync(order.StatusId);
                return View(order);
            }

            TempData["OrderUpdateSuccess"] = "Order was updated successfully.";
            return RedirectToAction(nameof(Index));
        }

        await PopulateStatusOptionsAsync(order.StatusId);

        return View(order);
    }

    private static bool IsValidOrderStatus(int? statusId)
    {
        return statusId is >= 1 and <= 7;
    }

    private Task PopulateStatusOptionsAsync(int? selectedStatusId = null)
    {
        ViewBag.StatusOptions = new List<SelectListItem>
        {
            new SelectListItem { Value = "1", Text = "Nieuw", Selected = selectedStatusId == 1 },
            new SelectListItem { Value = "2", Text = "In behandeling", Selected = selectedStatusId == 2 },
            new SelectListItem { Value = "3", Text = "Klaar om te verzenden", Selected = selectedStatusId == 3 },
            new SelectListItem { Value = "4", Text = "Verzonden", Selected = selectedStatusId == 4 },
            new SelectListItem { Value = "5", Text = "Afgeleverd", Selected = selectedStatusId == 5 },
            new SelectListItem { Value = "6", Text = "Geannuleerd", Selected = selectedStatusId == 6 },
            new SelectListItem { Value = "7", Text = "Retour", Selected = selectedStatusId == 7 }
        };

        return Task.CompletedTask;
    }

    // GET: Order/Delete/5
    public async Task<IActionResult> Delete(int id)
    {
        if (id == null || id <= 0)
        {
            return NotFound();
        }

        var order = await _orderDAL.GetOrderByIdAsync(id);
        if (order == null)
        {
            return NotFound();
        }
        return View(order);
    }

    // POST: Order/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        await _orderDAL.DeleteOrderAsync(id);
        TempData["OrderDeleteSuccess"] = "Order was deleted successfully.";
        return RedirectToAction(nameof(Index));
    }
}