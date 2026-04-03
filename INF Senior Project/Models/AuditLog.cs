using Microsoft.AspNetCore.Mvc;

namespace INF_Senior_Project.Models
{
    public class AuditLog
    {
        public int Id { get; set; }

        public string Action { get; set; } // "Create", "Edit", "Delete"

        public string Entity { get; set; } // "Product", "Order", etc.

        public int EntityId { get; set; }

        public string UserName { get; set; }

        public DateTime Timestamp { get; set; } = DateTime.Now;
    }
}
