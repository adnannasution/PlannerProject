using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Plan.Data;
using Plan.Models;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace Plan.Controllers
{
    [Authorize]
    public class JenisKontrakMasterController : Controller
    {
        private readonly ApplicationDbContext _context;

        public JenisKontrakMasterController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: JenisKontrakMaster
        public async Task<IActionResult> Index()
        {
            return View(await _context.JenisKontrakMaster.ToListAsync());
        }

        // GET: JenisKontrakMaster/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: JenisKontrakMaster/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Jenis")] JenisKontrakMaster jenisKontrakMaster)
        {
            if (ModelState.IsValid)
            {
                _context.Add(jenisKontrakMaster);
                await _context.SaveChangesAsync();
                 TempData["SuccessMessage"] = "successfully!";
                return RedirectToAction(nameof(Index));
            }
            return View(jenisKontrakMaster);
        }

                          [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> FormEdit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var jenisKontrakMaster = await _context.JenisKontrakMaster.FindAsync(id);
            if (jenisKontrakMaster == null)
            {
                return NotFound();
            }
            return View(jenisKontrakMaster);
        }

        // POST: JenisKontrakMaster/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Jenis")] JenisKontrakMaster jenisKontrakMaster)
        {
            if (id != jenisKontrakMaster.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(jenisKontrakMaster);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!JenisKontrakMasterExists(jenisKontrakMaster.Id))
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
            return View(jenisKontrakMaster);
        }

        // GET: JenisKontrakMaster/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var jenisKontrakMaster = await _context.JenisKontrakMaster
                .FirstOrDefaultAsync(m => m.Id == id);
            if (jenisKontrakMaster == null)
            {
                return NotFound();
            }

            return View(jenisKontrakMaster);
        }

        // POST: JenisKontrakMaster/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var jenisKontrakMaster = await _context.JenisKontrakMaster.FindAsync(id);
            _context.JenisKontrakMaster.Remove(jenisKontrakMaster);
            await _context.SaveChangesAsync();
             TempData["SuccessMessage"] = "successfully!";
            return RedirectToAction(nameof(Index));
        }

        private bool JenisKontrakMasterExists(int id)
        {
            return _context.JenisKontrakMaster.Any(e => e.Id == id);
        }
    }
}
