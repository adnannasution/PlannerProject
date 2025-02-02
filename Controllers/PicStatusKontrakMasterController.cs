using Microsoft.AspNetCore.Mvc;
using Plan.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Plan.Data;
using Microsoft.AspNetCore.Authorization;

namespace Plan.Controllers
{
    [Authorize]
    public class PicStatusKontrakMasterController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PicStatusKontrakMasterController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: PicStatusKontrak
        public async Task<IActionResult> Index()
        {
            return View(await _context.PicStatusKontrakMaster.ToListAsync());
        }

        // GET: PicStatusKontrak/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: PicStatusKontrak/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Pic")] PicStatusKontrakMaster picStatusKontrak)
        {
            if (ModelState.IsValid)
            {
                _context.Add(picStatusKontrak);
                await _context.SaveChangesAsync();
                 TempData["SuccessMessage"] = "successfully!";
                return RedirectToAction(nameof(Index));
            }
            return View(picStatusKontrak);
        }

              [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> FormEdit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var picStatusKontrak = await _context.PicStatusKontrakMaster.FindAsync(id);
            if (picStatusKontrak == null)
            {
                return NotFound();
            }
            return View(picStatusKontrak);
        }

        // POST: PicStatusKontrak/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id, Pic")] PicStatusKontrakMaster picStatusKontrak)
        {
            if (id != picStatusKontrak.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(picStatusKontrak);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PicStatusKontrakExists(picStatusKontrak.Id))
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
            return View(picStatusKontrak);
        }

        // GET: PicStatusKontrak/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var picStatusKontrak = await _context.PicStatusKontrakMaster
                .FirstOrDefaultAsync(m => m.Id == id);
            if (picStatusKontrak == null)
            {
                return NotFound();
            }

            return View(picStatusKontrak);
        }

        // POST: PicStatusKontrak/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var picStatusKontrak = await _context.PicStatusKontrakMaster.FindAsync(id);
            _context.PicStatusKontrakMaster.Remove(picStatusKontrak);
            await _context.SaveChangesAsync();
             TempData["SuccessMessage"] = "successfully!";
            return RedirectToAction(nameof(Index));
        }

        private bool PicStatusKontrakExists(int id)
        {
            return _context.PicStatusKontrakMaster.Any(e => e.Id == id);
        }
    }
}
