using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iteratie3matrix.Models;

namespace iteratie3matrix.Data;

public class OrderRepository
{
    public Task<List<Order>> ListAsync()
    {
        return Task.FromResult(new List<Order>
    {
        new Order { OrderId = 999, OrderDate = DateTime.Now }
    });
    }
}
