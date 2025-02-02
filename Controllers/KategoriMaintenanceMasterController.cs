using Microsoft.AspNetCore.Mvc;
using Plan.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Plan.Data;
using Microsoft.AspNetCore.Authorization;

namespace Plan.Controllers
{
    [Authorize]
    public class KategoriMaintenanceMasterController : Controller
    {
        private readonly ApplicationDbContext _context;

        public KategoriMaintenanceMasterController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: KategoriMaintenanceMaster
        public async Task<IActionResult> Index()
        {
            return View(await _context.KategoriMaintenanceMaster.ToListAsync());
        }

        // GET: KategoriMaintenanceMaster/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: KategoriMaintenanceMaster/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Kategori")] KategoriMaintenanceMaster kategoriMaintenanceMaster)
        {
            if (ModelState.IsValid)
            {
                _context.Add(kategoriMaintenanceMaster);
                await _context.SaveChangesAsync();
                 TempData["SuccessMessage"] = "successfully!";
                return RedirectToAction(nameof(Index));
            }
            return View(kategoriMaintenanceMaster);
        }

                      [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> FormEdit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var kategoriMaintenanceMaster = await _context.KategoriMaintenanceMaster.FindAsync(id);
            if (kategoriMaintenanceMaster == null)
            {
                return NotFound();
            }
            return View(kategoriMaintenanceMaster);
        }

        // POST: KategoriMaintenanceMaster/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id, Kategori")] KategoriMaintenanceMaster kategoriMaintenanceMaster)
        {
            if (id != kategoriMaintenanceMaster.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(kategoriMaintenanceMaster);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!KategoriMaintenanceMasterExists(kategoriMaintenanceMaster.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                 TempData["SuccessMessage"] = "successfully!";
                return RedirectToAction(nameof(Index));
            }
            return View(kategoriMaintenanceMaster);
        }

        // GET: KategoriMaintenanceMaster/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var kategoriMaintenanceMaster = await _context.KategoriMaintenanceMaster
                .FirstOrDefaultAsync(m => m.Id == id);
            if (kategoriMaintenanceMaster == null)
            {
                return NotFound();
            }

            return View(kategoriMaintenanceMaster);
        }

        // POST: KategoriMaintenanceMaster/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var kategoriMaintenanceMaster = await _context.KategoriMaintenanceMaster.FindAsync(id);
            _context.KategoriMaintenanceMaster.Remove(kategoriMaintenanceMaster);
            await _context.SaveChangesAsync();
             TempData["SuccessMessage"] = "successfully!";
            return RedirectToAction(nameof(Index));
        }

        private bool KategoriMaintenanceMasterExists(int id)
        {
            return _context.KategoriMaintenanceMaster.Any(e => e.Id == id);
        }
    }
}
