using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iteratie3matrix.Models;

public class OrderProductItem
{
    // WHAT: Product inside an order
    // WHY: Used for order detail + scanning validation

    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;

    public int Quantity { get; set; }
}