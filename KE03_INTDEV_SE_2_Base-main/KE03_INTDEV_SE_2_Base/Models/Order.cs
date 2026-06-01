using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KE03_INTDEV_SE_2_Base_main.Models;
{
    public class Order
    {
        public int Id { get; set; }

        public DateTime OrderDate { get; set; }

        public int CustomerId { get; set; }
        
        public Customer Customer { get; set; } = null!;

        public ICollection<Product> Products { get; } = new List<Product>();
        public ICollection<OrderLine> OrderLines { get; } = new List<OrderLine>();
    }
}
