using INF_Senior_Project.Data;
using INF_Senior_Project.Models;
using INF_Senior_Project.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace INF_Senior_Project.Controllers
{
    public class PharmacistController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PharmacistController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Dashboard()
        {
            var role = HttpContext.Session.GetString("UserRole");

            if (role != "Pharmacist")
            {
                return RedirectToAction("Login", "Account");
            }

            var today = DateTime.Today;

            var ordersToday = _context.Orders
                .Where(o => o.OrderDate.Date == today)
                .ToList();

            int LowStockQuantity = _context.SystemSettings.First().LowStockThreshold;

            var vm = new PharmacistDashboardViewModel
            {
                TodayOrdersCount = ordersToday.Count,
                TodayRevenue = ordersToday.Sum(o => o.TotalAmount),

                RecentOrders = _context.Orders
                    .OrderByDescending(o => o.OrderDate)
                    .Take(5)
                    .ToList(),
                 
                LowStockProducts = _context.Products
            .Where(p => p.Quantity < LowStockQuantity)
            .ToList()
            };

            return View(vm);
        }

        
        public IActionResult CreateOrder()
        {
            var vm = new CreateOrderViewModel
            {
                Products = _context.Products.ToList()
            };

            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder(CreateOrderViewModel model)
        {
            if (model.Items == null || !model.Items.Any())
            {
                ModelState.AddModelError("", "Order must contain at least one item.");
            }

            if (!ModelState.IsValid)
            {
                model.Products = _context.Products.ToList();
                return View(model);
            }

            var order = new Order
            {
                OrderDate = DateTime.Now,
                PharmacistId = GetCurrentUserId(),
                TotalAmount = 0
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            decimal total = 0;

            foreach (var item in model.Items)
            {
                var product = await _context.Products.FindAsync(item.ProductId);

                if (product == null || product.Quantity < item.Quantity)
                {
                    ModelState.AddModelError("", $"Not enough stock for {product?.Name}");
                    model.Products = _context.Products.ToList();
                    return View(model);
                }

                product.Quantity -= item.Quantity;

                var orderItem = new OrderItem
                {
                    OrderId = order.Id,
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    Price = product.Price
                };

                total += product.Price * item.Quantity;

                _context.OrderItems.Add(orderItem);
            }

            order.TotalAmount = total;

            await _context.SaveChangesAsync();
            Log("Create", "Order", order.Id);
            return RedirectToAction("Dashboard");
        }

        private int GetCurrentUserId()
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                throw new Exception("User is not logged in.");
            }

            return userId.Value;
        }

        public IActionResult MySales()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var role = HttpContext.Session.GetString("UserRole");

            if (userId == null || role != "Pharmacist")
                return RedirectToAction("Login", "Account");

            // Get all orders created by this pharmacist
            var orders = _context.Orders
                .Where(o => o.PharmacistId == userId) 
                .OrderByDescending(o => o.OrderDate)
                .ToList();

            var vm = new MySalesViewModel
            {
                Orders = orders,
                TotalRevenue = orders.Sum(o => o.TotalAmount)
            };

            return View(vm);
        }

        public IActionResult OrderDetails(int id)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var role = HttpContext.Session.GetString("UserRole");

            if (userId == null || role != "Pharmacist")
                return RedirectToAction("Login", "Account");

            var order = _context.Orders
                .Include(o => o.Items)
                .ThenInclude(i => i.Product)
                .FirstOrDefault(o => o.Id == id && o.PharmacistId == userId);

            if (order == null)
                return NotFound();

            return View(order);
        }

        public IActionResult CreatePrescription(Prescription model)
        {
            var vm = new CreatePrescriptionViewModel
            {
                Products = _context.Products.ToList()
            };

            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> CreatePrescription(CreatePrescriptionViewModel model)
        {
            if (model.Items == null || !model.Items.Any())
            {
                ModelState.AddModelError("", "Prescription must contain at least one item.");
            }

            if (!ModelState.IsValid)
            {
                model.Products = _context.Products.ToList();
                return View(model);
            }

            var prescription = new Prescription
            {
                PatientName = model.PatientName,
                DoctorName = model.DoctorName,
                DateIssued = model.DateIssued,
                Notes = model.Notes
            };

            _context.Prescriptions.Add(prescription);
            await _context.SaveChangesAsync();

            foreach (var item in model.Items)
            {
                var product = await _context.Products.FindAsync(item.ProductId);

                var prescItem = new PrescriptionItem
                {
                    PrescriptionId = prescription.Id,
                    ProductId = item.ProductId,
                    Quantity = item.Quantity
                };

                _context.PrescriptionItems.Add(prescItem);
            }

            await _context.SaveChangesAsync();

            Log("Create", "Prescription", prescription.Id);

            return RedirectToAction("Prescriptions");
        }

        public IActionResult Prescriptions()
        {
            var list = _context.Prescriptions
                .OrderByDescending(p => p.DateIssued)
                .ToList();

            return View(list);
        }

        public IActionResult PrescriptionDetails(int id)
        {
            var prescription = _context.Prescriptions
                .Include(p => p.Items)
                .ThenInclude(i => i.Product)
                .FirstOrDefault(p => p.Id == id);

            if (prescription == null)
                return NotFound();

            return View(prescription);
        }

        public async Task<IActionResult> FulfillPrescription(int id)
        {
            var prescription = _context.Prescriptions
                .Include(p => p.Items)
                .ThenInclude(i => i.Product)
                .FirstOrDefault(p => p.Id == id);

            if (prescription == null)
                return NotFound();

            if (prescription.IsFulfilled)
            {
                TempData["Error"] = "Prescription already fulfilled.";
                return RedirectToAction("PrescriptionDetails", new { id });
            }

            // 🚫 STOCK CHECK
            foreach (var item in prescription.Items)
            {
                if (item.Product.Quantity < item.Quantity)
                {
                    TempData["Error"] = $"Not enough stock for {item.Product.Name}";
                    return RedirectToAction("PrescriptionDetails", new { id });
                }
            }

            // 🧾 CREATE ORDER
            var order = new Order
            {
                OrderDate = DateTime.Now,
                PharmacistId = GetCurrentUserId(),
                TotalAmount = 0
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            decimal total = 0;

            // 🔄 CREATE ORDER ITEMS + REDUCE STOCK
            foreach (var item in prescription.Items)
            {
                var product = item.Product;

                product.Quantity -= item.Quantity;

                var orderItem = new OrderItem
                {
                    OrderId = order.Id,
                    ProductId = product.Id,
                    Quantity = item.Quantity,
                    Price = product.Price
                };

                total += product.Price * item.Quantity;

                _context.OrderItems.Add(orderItem);
            }

            order.TotalAmount = total;

            // ✅ MARK AS FULFILLED
            prescription.IsFulfilled = true;

            await _context.SaveChangesAsync();

            Log("Fulfill", "Prescription", prescription.Id);

            TempData["Success"] = "Prescription fulfilled successfully!";

            return RedirectToAction("PrescriptionDetails", new { id });
        }

        private void Log(string action, string entity, int entityId)
        {
            var log = new AuditLog
            {
                Action = action,
                Entity = entity,
                EntityId = entityId,
                UserName = "Yane"
            };

            _context.AuditLogs.Add(log);
            _context.SaveChanges();
        }
    }
}
