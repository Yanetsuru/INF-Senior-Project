using Azure.Identity;
using INF_Senior_Project.Data;
using INF_Senior_Project.Models;
using INF_Senior_Project.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Build.Tasks.Deployment.Bootstrapper;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

public class AccountController : Controller
{
    private readonly ApplicationDbContext _context;

    public AccountController(ApplicationDbContext context)
    {
        _context = context;
    }

    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        bool emailExists = await _context.Users.AnyAsync(u => u.Email == model.Email);
        if (emailExists)
        {
            ModelState.AddModelError("Email", "This email is already registered.");
            return View(model);
        }

        var user = new User
        {
            Username = model.Username,
            Email = model.Email,
            PasswordHash = HashPassword(model.Password),
            Role = model.Role
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        Log("Register", "User", user.Id);
        return RedirectToAction("Login", "Account");
    }

    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var user = _context.Users.FirstOrDefault(u => u.Email == model.Email);

        if (user != null && user.PasswordHash == HashPassword(model.Password))
        {
            if (!user.IsActive)
            {
                ViewBag.Error = "Your account has been deactivated. Contact admin.";
                return View(model);
            }
            HttpContext.Session.SetString("UserRole", user.Role);
            HttpContext.Session.SetString("UserName", user.Username);
            HttpContext.Session.SetInt32("UserId", user.Id);
            Log("Login", "User", user.Id);
            if (user.Role == "Admin")
            {
                return RedirectToAction("Dashboard", "Admin");
            }

            if (user.Role == "Pharmacist") 
            {
                return RedirectToAction("Dashboard", "Pharmacist");
            }

            if (user.Role == "InventoryManager")
            {
                return RedirectToAction("Dashboard", "Inventory");
            }
        }

            ViewBag.Error = "Invalid login credentials";
        return View();
    }

    private string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(bytes);
    }

    public IActionResult Logout()
    {
        HttpContext.Session.Remove("UserRole");
        HttpContext.Session.Remove("UserName");
        HttpContext.Session.Remove("UserId");
        return RedirectToAction("Index", "Home");
    }
    private void Log(string action, string entity, int entityId)
    {
        string username;
        if (HttpContext.Session.GetString("UserName") == null)
        {
             username = "Guest";
        }
        else
        {
            username = HttpContext.Session.GetString("UserName");
        }
        var log = new AuditLog
        {
            Action = action,
            Entity = entity,
            EntityId = entityId,
            UserName = username
        };

        _context.AuditLogs.Add(log);
        _context.SaveChanges();
    }
}