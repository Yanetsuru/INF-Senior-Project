using Microsoft.AspNetCore.Mvc;

namespace INF_Senior_Project.Models
{
    public class SystemSettings
    {
        public int Id { get; set; }

        public int LowStockThreshold { get; set; } = 20;

        public decimal TaxPercentage { get; set; } = 0;

        public string PharmacyName { get; set; } = "Yane Pharmacy";

        public string Currency { get; set; } = "USD";
    }
}
