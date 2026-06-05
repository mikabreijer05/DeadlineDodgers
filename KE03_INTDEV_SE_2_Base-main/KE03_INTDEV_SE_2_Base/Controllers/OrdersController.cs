using KE03_INTDEV_SE_2_Base.DAL;
using Microsoft.AspNetCore.Mvc;
using KE03_INTDEV_SE_2_Base.Models;

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

        if (ModelState.IsValid)
        {
            await _orderDAL.UpdateOrderAsync(order);
            return RedirectToAction(nameof(Index));
        }
        return View(order);
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
        return RedirectToAction(nameof(Index));
    }
}