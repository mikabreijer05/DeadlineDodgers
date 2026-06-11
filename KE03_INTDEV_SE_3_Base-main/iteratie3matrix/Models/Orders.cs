using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iteratie3matrix.Models;

public class Order
{
    public int OrderId { get; set; }

    public DateTime OrderDate { get; set; }

    public int StatusId { get; set; }

    public int AccountId { get; set; }

    public int AddressId { get; set; }

    public bool DeliveryTogether { get; set; }
}
