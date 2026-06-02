using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KE03_INTDEV_SE_2_Base_main.Models;
using KE03_INTDEV_SE_2_Base.Models;

namespace KE03_INTDEV_SE_2_Base.Models
{
    public class Customer : Account
    {
        public ICollection<Order> Orders { get; } = new List<Order>();
    }
}