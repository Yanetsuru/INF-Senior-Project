using System.ComponentModel.DataAnnotations;

namespace INF_Senior_Project.Models
{
    public class Product
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string Category { get; set; }

        [Range(0, double.MaxValue)]
        public decimal Price { get; set; }

        public int Quantity { get; set; }

        public DateTime ExpirationDate { get; set; }

        [Required(ErrorMessage = "Please select a supplier")]
        public int? SupplierId { get; set; }
        public Supplier? Supplier { get; set; }

        // Navigation
        public ICollection<OrderItem> OrderItems { get; set; }
        public ICollection<PrescriptionItem> PrescriptionItems { get; set; }
    }
}