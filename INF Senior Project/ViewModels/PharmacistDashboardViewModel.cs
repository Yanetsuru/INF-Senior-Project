using INF_Senior_Project.Models;
using Microsoft.AspNetCore.Mvc;

namespace INF_Senior_Project.ViewModels
{
    public class PharmacistDashboardViewModel
    {
        public decimal TodayRevenue { get; set; }
        public int TodayOrdersCount { get; set; }

        public List<Order> RecentOrders { get; set; }

        public List<Product> LowStockProducts { get; set; }
    }
}
