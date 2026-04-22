using INF_Senior_Project.Models;
using Microsoft.AspNetCore.Mvc;

namespace INF_Senior_Project.ViewModels
{
    public class InventoryDashboardViewModel
    {
        public List<Product> Products { get; set; }
        public List<Supplier> Suppliers { get; set; }
    }
}
