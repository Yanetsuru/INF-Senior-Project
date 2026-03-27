using INF_Senior_Project.Models;
using System.ComponentModel.DataAnnotations.Schema;
namespace INF_Senior_Project.Models;

public class OrderItem
{
    public int Id { get; set; }

    public int OrderId { get; set; }
    public int ProductId { get; set; }

    public int Quantity { get; set; }
    public decimal Price { get; set; }

    // Navigation
    [ForeignKey("OrderId")]
    public Order Order { get; set; }

    [ForeignKey("ProductId")]
    public Product Product { get; set; }
}