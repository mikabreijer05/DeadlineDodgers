using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KE03_INTDEV_SE_2_Base_main.Models;

namespace KE03_INTDEV_SE_2_Base.Models
{
    public class Order
    {
        public int Id { get; set; }

        public DateTime OrderDate { get; set; }
        public string? Address { get; set; }

        public int CustomerId { get; set; }

        public string? CustomerName { get; set; }

        public Customer Customer { get; set; } = null!;

        public int? StatusId { get; set; }

        public string? OrderStatus { get; set; }

        public ICollection<Product> Products { get; } = new List<Product>();

        public ICollection<OrderLine> OrderLines { get; } = new List<OrderLine>();
    }
}