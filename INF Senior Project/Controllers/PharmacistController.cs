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

            var vm = new PharmacistDashboardViewModel
            {
                TodayOrdersCount = ordersToday.Count,
                TodayRevenue = ordersToday.Sum(o => o.TotalAmount),

                RecentOrders = _context.Orders
                    .OrderByDescending(o => o.OrderDate)
                    .Take(5)
                    .ToList(),

                // Low stock products = all products with quantity < 20
                LowStockProducts = _context.Products
            .Where(p => p.Quantity < 20)
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
    }
}
