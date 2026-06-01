using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Models
{
    public class Customer : Account
    {
        public ICollection<Order> Orders { get; } = new List<Order>();
    }
}