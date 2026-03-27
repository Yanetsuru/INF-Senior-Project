using INF_Senior_Project.Models;
using System.Collections.Generic;

namespace INF_Senior_Project.ViewModels
{
    public class MySalesViewModel
    {
        public List<Order> Orders { get; set; }
        public decimal TotalRevenue { get; set; }
    }
}