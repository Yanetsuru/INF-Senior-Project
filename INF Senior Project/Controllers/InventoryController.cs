using INF_Senior_Project.Data;
using INF_Senior_Project.Models;
using INF_Senior_Project.ViewModels;
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
        public async Task<IActionResult> Dashboard(string search, string sortOrder)
        {
            var products = _context.Products
            .Include(p => p.Supplier)
            .AsQueryable();

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

            var vm = new InventoryDashboardViewModel
            {
                Products = await products.ToListAsync(),
                Suppliers = await _context.Suppliers.ToListAsync()
            };

            return View(vm);
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
                Log("Create", "Product", product.Id);
                return RedirectToAction(nameof(Dashboard));
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
                    Log("Edit", "Product", product.Id);
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
                return RedirectToAction(nameof(Dashboard));
            }
            ViewData["Suppliers"] = new SelectList(_context.Suppliers, "Id", "Name", product.SupplierId);
            return View(product);
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.Id == id);
        }
        

        // Delete - Show the confirmation page to delete a product
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Supplier)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }
        public async Task<IActionResult> DeleteSupplier(int? id)
        {
            if (id == null) return NotFound();

            var supplier = await _context.Suppliers
                .FirstOrDefaultAsync(s => s.Id == id);

            if (supplier == null) return NotFound();

            return View(supplier);
        }

        // DELETE - Confirm and delete the product
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Products.FindAsync(id);
            Log("Delete", "Product", id);
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Dashboard));
        }

        
        [HttpPost, ActionName("DeleteSupplier")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteSupplierConfirmed(int id)
        {
            var supplier = await _context.Suppliers.FindAsync(id);
            Log("Delete", "Supplier", id);
            _context.Suppliers.Remove(supplier);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Dashboard));
        }

        public async Task<IActionResult> Suppliers()
        {
            var suppliers = await _context.Suppliers.ToListAsync();
            return View();
        }

        public IActionResult CreateSupplier()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateSupplier(Supplier supplier)
        {
            if (ModelState.IsValid)
            {
                _context.Suppliers.Add(supplier);
                await _context.SaveChangesAsync();
                Log("Create", "Supplier", supplier.Id);
                return RedirectToAction(nameof(Dashboard));
            }

            return View();
        }

        public async Task<IActionResult> EditSupplier(int? id)
        {
            if (id == null) return NotFound();

            var supplier = await _context.Suppliers.FindAsync(id);
            if (supplier == null) return NotFound();

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditSupplier(int id, Supplier supplier)
        {
            if (id != supplier.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(supplier);
                    await _context.SaveChangesAsync();
                    Log("Edit", "Supplier", supplier.Id);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Suppliers.Any(e => e.Id == supplier.Id))
                        return NotFound();
                    else
                        throw;
                }

                return RedirectToAction(nameof(Dashboard));
            }

            return View();
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

}
