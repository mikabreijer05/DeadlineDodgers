using iteratie3matrix.Models;

namespace iteratie3matrix.Data;

public class OrderRepository
{
    public Task<List<Order>> ListAsync()
    {
        return Task.FromResult(new List<Order>
        {
            new Order
            {
                OrderId = 1001,
                OrderDate = DateTime.Today,
                StatusId = 1,
                AccountId = 1,
                AddressId = 1,
                DeliveryTogether = false
            },
            new Order
            {
                OrderId = 1002,
                OrderDate = DateTime.Today.AddDays(-1),
                StatusId = 2,
                AccountId = 2,
                AddressId = 1,
                DeliveryTogether = true
            },
            new Order
            {
                OrderId = 1003,
                OrderDate = DateTime.Today.AddDays(-2),
                StatusId = 3,
                AccountId = 3,
                AddressId = 2,
                DeliveryTogether = false
            }
        });
    }

    public Task<Order?> GetAsync(int id)
    {
        return Task.FromResult(ListAsync().Result.FirstOrDefault(x => x.OrderId == id));
    }
}