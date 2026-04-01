using INF_Senior_Project.Models;
using Microsoft.AspNetCore.Mvc;

namespace INF_Senior_Project.ViewModels
{
    public class CreateOrderViewModel
    {
        public List<Product> Products { get; set; } = new List<Product>();

        public List<OrderItemInputModel> Items { get; set; } = new();

        public decimal TotalAmount { get; set; }
    }
    public class OrderItemInputModel
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
