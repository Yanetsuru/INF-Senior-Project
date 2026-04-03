using INF_Senior_Project.Models;
using Microsoft.AspNetCore.Mvc;

namespace INF_Senior_Project.ViewModels
{
    public class CreatePrescriptionViewModel
    {
        public string PatientName { get; set; }
        public string DoctorName { get; set; }
        public DateTime DateIssued { get; set; } = DateTime.Now;
        public string Notes { get; set; }

        public List<PrescriptionItemInput> Items { get; set; } = new();

        public List<Product> Products { get; set; } = new();
    }

    public class PrescriptionItemInput
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
