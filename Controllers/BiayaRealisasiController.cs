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
    public class BiayaRealisasiController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BiayaRealisasiController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: BiayaRealisasi
        public async Task<IActionResult> Index()
        {
            return View(await _context.BiayaRealisasi.ToListAsync());
        }

        // GET: BiayaRealisasi/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var biayaRealisasi = await _context.BiayaRealisasi.FirstOrDefaultAsync(m => m.Id == id);
            if (biayaRealisasi == null)
                return NotFound();

            return View(biayaRealisasi);
        }

        // GET: BiayaRealisasi/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: BiayaRealisasi/Create
       [HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Create(BiayaRealisasi biayaRealisasi)
{
    if (ModelState.IsValid)
    {
        _context.Add(biayaRealisasi);
        await _context.SaveChangesAsync();
         TempData["SuccessMessage"] = "successfully!";
        return RedirectToAction(nameof(Index));
    }
    return View(biayaRealisasi);
}


        // GET: BiayaRealisasi/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var biayaRealisasi = await _context.BiayaRealisasi.FindAsync(id);
            if (biayaRealisasi == null)
                return NotFound();

            return View(biayaRealisasi);
        }

        // POST: BiayaRealisasi/Edit/5
[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Edit(int id, BiayaRealisasi biayaRealisasi)
{
    if (id != biayaRealisasi.Id)
        return NotFound();

    if (ModelState.IsValid)
    {
        try
        {
            _context.Update(biayaRealisasi);
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!BiayaRealisasiExists(biayaRealisasi.Id))
                return NotFound();
            else
                throw;
        }
         TempData["SuccessMessage"] = "successfully!";
        return RedirectToAction(nameof(Index));
    }
    return View(biayaRealisasi);
}


        // GET: BiayaRealisasi/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var biayaRealisasi = await _context.BiayaRealisasi.FirstOrDefaultAsync(m => m.Id == id);
            if (biayaRealisasi == null)
                return NotFound();

            return View(biayaRealisasi);
        }

        // POST: BiayaRealisasi/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var biayaRealisasi = await _context.BiayaRealisasi.FindAsync(id);
            _context.BiayaRealisasi.Remove(biayaRealisasi);
            await _context.SaveChangesAsync();
             TempData["SuccessMessage"] = "successfully!";
            return RedirectToAction(nameof(Index));
        }

        private bool BiayaRealisasiExists(int id)
        {
            return _context.BiayaRealisasi.Any(e => e.Id == id);
        }
    }
}
