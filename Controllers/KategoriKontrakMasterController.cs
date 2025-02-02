using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Plan.Data;
using Plan.Models;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace Plan.Controllers
{
    [Authorize]
    public class KategoriKontrakMasterController : Controller
    {
        private readonly ApplicationDbContext _context;

        public KategoriKontrakMasterController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: KategoriKontrak
        public async Task<IActionResult> Index()
        {
            return View(await _context.KategoriKontrakMaster.ToListAsync());
        }

        // GET: KategoriKontrak/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: KategoriKontrak/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Kategori")] KategoriKontrakMaster kategoriKontrak)
        {
            if (ModelState.IsValid)
            {
                _context.Add(kategoriKontrak);
                await _context.SaveChangesAsync();
                 TempData["SuccessMessage"] = "successfully!";
                return RedirectToAction(nameof(Index));
            }
            return View(kategoriKontrak);
        }

                      [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> FormEdit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var kategoriKontrak = await _context.KategoriKontrakMaster.FindAsync(id);
            if (kategoriKontrak == null)
            {
                return NotFound();
            }
            return View(kategoriKontrak);
        }

        // POST: KategoriKontrak/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Kategori")] KategoriKontrakMaster kategoriKontrak)
        {
            if (id != kategoriKontrak.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(kategoriKontrak);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!KategoriKontrakExists(kategoriKontrak.Id))
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
            return View(kategoriKontrak);
        }

        // GET: KategoriKontrak/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var kategoriKontrak = await _context.KategoriKontrakMaster
                .FirstOrDefaultAsync(m => m.Id == id);
            if (kategoriKontrak == null)
            {
                return NotFound();
            }

            return View(kategoriKontrak);
        }

        // POST: KategoriKontrak/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var kategoriKontrak = await _context.KategoriKontrakMaster.FindAsync(id);
            _context.KategoriKontrakMaster.Remove(kategoriKontrak);
            await _context.SaveChangesAsync();
             TempData["SuccessMessage"] = "successfully!";
            return RedirectToAction(nameof(Index));
        }

        private bool KategoriKontrakExists(int id)
        {
            return _context.KategoriKontrakMaster.Any(e => e.Id == id);
        }
    }
}
