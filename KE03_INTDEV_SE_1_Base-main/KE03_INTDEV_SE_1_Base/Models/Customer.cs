

namespace KE03_INTDEV_SE_1_Base.Models;

    public class Customer : Account
    {
        public ICollection<Order> Orders { get; } = new List<Order>();
    }
