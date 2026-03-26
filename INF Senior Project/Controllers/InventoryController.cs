using INF_Senior_Project.Data;
using INF_Senior_Project.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Build.Tasks.Deployment.Bootstrapper;
using Microsoft.EntityFrameworkCore;

namespace INF_Senior_Project.Controllers
{
    public class InventoryController : Controller
    {
        private readonly ApplicationDbContext _context;

        public InventoryController(ApplicationDbContext context)
        {
            _context = context;
        }

        // LIST + SEARCH + SORT
        public async Task<IActionResult> InventoryDashboard(string search, string sortOrder)
        {
            var products = from p in _context.Products.Include(p => p.Supplier)
                           select p;

            // 🔍 SEARCH
            if (!string.IsNullOrEmpty(search))
            {
                products = products.Where(p =>
                    p.Name.Contains(search) ||
                    p.Category.Contains(search) ||
                    p.Supplier.Name.Contains(search));
            }

            // 🔃 SORTING
            products = sortOrder switch
            {
                "name_desc" => products.OrderByDescending(p => p.Name),
                "price" => products.OrderBy(p => p.Price),
                "price_desc" => products.OrderByDescending(p => p.Price),
                "quantity" => products.OrderBy(p => p.Quantity),
                "quantity_desc" => products.OrderByDescending(p => p.Quantity),
                "exp" => products.OrderBy(p => p.ExpirationDate),
                "exp_desc" => products.OrderByDescending(p => p.ExpirationDate),
                _ => products.OrderBy(p => p.Name),
            };

            return View(await products.ToListAsync());
        }

        // CREATE - Show the form for adding a new product
        public IActionResult Create()
        {
            ViewData["Suppliers"] = new SelectList(_context.Suppliers, "Id", "Name");
            return View();
        }

        // CREATE - Handle form submission for adding a new product
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Models.Product product)
        {
            if (ModelState.IsValid)
            {
                _context.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(InventoryDashboard));
            }
            ViewData["Suppliers"] = new SelectList(_context.Suppliers, "Id", "Name", product.SupplierId);
            return View(product);
        }
        // Edit - Show the form to edit an existing product
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            ViewData["Suppliers"] = new SelectList(_context.Suppliers, "Id", "Name", product.SupplierId);
            return View(product);
        }

        // Edit - Handle form submission for updating an existing product
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Models.Product product)
        {
            if (id != product.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(product);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(InventoryDashboard));
            }
            ViewData["Suppliers"] = new SelectList(_context.Suppliers, "Id", "Name", product.SupplierId);
            return View(product);
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.Id == id);
        }
    }

}
