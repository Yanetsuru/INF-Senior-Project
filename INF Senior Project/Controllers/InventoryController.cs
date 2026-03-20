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

        // CREATE GET
        public IActionResult Create()
        {

            ViewBag.Suppliers = new SelectList(_context.Suppliers, "Id", "Name");
            return View();
        }

        // CREATE POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Models.Product product)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Suppliers = new SelectList(_context.Suppliers, "Id", "Name", product.SupplierId);
                return View(product);
            }

            _context.Add(product); // EF tracks SupplierId automatically
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(InventoryDashboard));
        }

        // EDIT GET
        public async Task<IActionResult> Edit(int id)
        {
            var product = await _context.Products
                                        .Include(p => p.Supplier)
                                        .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null) return NotFound();

            ViewBag.Suppliers = new SelectList(_context.Suppliers, "Id", "Name", product?.SupplierId);
            return View(product);
        }

        // EDIT POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Models.Product product)
        {
            if (id != product.Id) return NotFound();

            if (!ModelState.IsValid)
            {
                ViewBag.Suppliers = new SelectList(_context.Suppliers, "Id", "Name", product.SupplierId);
                return View(product);
            }

            // fetch the existing entity
            var existingProduct = await _context.Products.FindAsync(id);
            if (existingProduct == null) return NotFound();

            existingProduct.Name = product.Name;
            existingProduct.Category = product.Category;
            existingProduct.Price = product.Price;
            existingProduct.Quantity = product.Quantity;
            existingProduct.ExpirationDate = product.ExpirationDate;
            existingProduct.SupplierId = product.SupplierId;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(InventoryDashboard));
        }

        // DELETE
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(InventoryDashboard));
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.Id == id);
        }
    }
}