using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Plan.Models;
using Plan.Data;
using Microsoft.AspNetCore.Authorization;

namespace Plan.Controllers
{
    [Authorize]
    public class OtorisasiKontrakMasterController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OtorisasiKontrakMasterController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: OtorisasiKontrakMaster
        public async Task<IActionResult> Index()
        {
            return View(await _context.Set<OtorisasiKontrakMaster>().ToListAsync());
        }

        // GET: OtorisasiKontrakMaster/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: OtorisasiKontrakMaster/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Otorisasi")] OtorisasiKontrakMaster otorisasiKontrak)
        {
            if (ModelState.IsValid)
            {
                _context.Add(otorisasiKontrak);
                await _context.SaveChangesAsync();
                 TempData["SuccessMessage"] = "successfully!";
                return RedirectToAction(nameof(Index));
            }
            return View(otorisasiKontrak);
        }

                    [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> FormEdit(int? id)
        {
            if (id == null)
                return NotFound();

            var otorisasiKontrak = await _context.Set<OtorisasiKontrakMaster>().FindAsync(id);
            if (otorisasiKontrak == null)
                return NotFound();

            return View(otorisasiKontrak);
        }

        // POST: OtorisasiKontrakMaster/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Otorisasi")] OtorisasiKontrakMaster otorisasiKontrak)
        {
            if (id != otorisasiKontrak.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(otorisasiKontrak);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OtorisasiKontrakMasterExists(otorisasiKontrak.Id))
                        return NotFound();
                    else
                        throw;
                }
                 TempData["SuccessMessage"] = "successfully!";
                return RedirectToAction(nameof(Index));
            }
            return View(otorisasiKontrak);
        }

        // GET: OtorisasiKontrakMaster/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var otorisasiKontrak = await _context.Set<OtorisasiKontrakMaster>()
                .FirstOrDefaultAsync(m => m.Id == id);

            if (otorisasiKontrak == null)
                return NotFound();

            return View(otorisasiKontrak);
        }

        // POST: OtorisasiKontrakMaster/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var otorisasiKontrak = await _context.Set<OtorisasiKontrakMaster>().FindAsync(id);
            if (otorisasiKontrak != null)
            {
                _context.Set<OtorisasiKontrakMaster>().Remove(otorisasiKontrak);
                await _context.SaveChangesAsync();
            }
             TempData["SuccessMessage"] = "successfully!";
            return RedirectToAction(nameof(Index));
        }

        private bool OtorisasiKontrakMasterExists(int id)
        {
            return _context.Set<OtorisasiKontrakMaster>().Any(e => e.Id == id);
        }
    }
}
