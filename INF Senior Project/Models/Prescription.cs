using System.ComponentModel.DataAnnotations;
namespace INF_Senior_Project.Models;

public class Prescription
{
    public int Id { get; set; }

    [Required]
    public string PatientName { get; set; }

    public string DoctorName { get; set; }

    public DateTime DateIssued { get; set; }

    public string Notes { get; set; }

    public bool IsFulfilled { get; set; } = false;

    // Navigation
    public ICollection<PrescriptionItem> Items { get; set; }
}