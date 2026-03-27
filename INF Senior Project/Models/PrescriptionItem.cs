using INF_Senior_Project.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace INF_Senior_Project.Models;
public class PrescriptionItem
{
    public int Id { get; set; }

    public int PrescriptionId { get; set; }
    public int ProductId { get; set; }

    public int Quantity { get; set; }

    // Navigation
    [ForeignKey("PrescriptionId")]
    public Prescription Prescription { get; set; }

    [ForeignKey("ProductId")]
    public Product Product { get; set; }
}