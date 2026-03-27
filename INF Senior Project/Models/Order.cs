using INF_Senior_Project.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace INF_Senior_Project.Models;

public class Order
{
    public int Id { get; set; }

    public DateTime OrderDate { get; set; } = DateTime.Now;

    public decimal TotalAmount { get; set; }

    // FK → User (Pharmacist)
    public int PharmacistId { get; set; }

    [ForeignKey("PharmacistId")]
    public User Pharmacist { get; set; }

    // Navigation
    public ICollection<OrderItem> Items { get; set; }
}