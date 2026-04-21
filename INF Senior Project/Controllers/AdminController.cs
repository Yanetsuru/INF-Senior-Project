using INF_Senior_Project.Data;
using INF_Senior_Project.Models;
using Microsoft.AspNetCore.Mvc;

namespace INF_Senior_Project.Controllers
{
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        private bool IsAdmin()
        {
            return HttpContext.Session.GetString("UserRole") == "Admin";
        }

        public IActionResult AuditLogs()
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");

            var logs = _context.AuditLogs
                .OrderByDescending(l => l.Timestamp)
                .ToList();

            return View(logs);
        }

        public IActionResult Users()
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");

            var users = _context.Users.ToList();
            return View(users);
        }

        public async Task<IActionResult> ToggleUser(int id)
        {
            var user = await _context.Users.FindAsync(id);

            user.IsActive = !user.IsActive;

            await _context.SaveChangesAsync();

            return RedirectToAction("Users");
        }

        public IActionResult Dashboard()
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");

            ViewBag.UserCount = _context.Users.Count();
            ViewBag.LogCount = _context.AuditLogs.Count();
            ViewBag.ActiveUsers = _context.Users.Count(u => u.IsActive);

            return View();
        }
        public IActionResult Settings()
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");

            var settings = _context.SystemSettings.FirstOrDefault();

            if (settings == null)
            {
                settings = new SystemSettings();
                _context.SystemSettings.Add(settings);
                _context.SaveChanges();
            }

            return View(settings);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Settings(SystemSettings settings)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");

            if (ModelState.IsValid)
            {
                _context.SystemSettings.Update(settings);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Dashboard");
        }
    }
}
