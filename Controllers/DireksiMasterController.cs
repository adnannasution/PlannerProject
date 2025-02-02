using Microsoft.AspNetCore.Mvc;
using Plan.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Plan.Data;
using Microsoft.AspNetCore.Authorization;

namespace Plan.Controllers
{
    [Authorize]
    public class DireksiMasterController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DireksiMasterController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: DireksiMaster
        public async Task<IActionResult> Index()
        {
            return View(await _context.DireksiMaster.ToListAsync());
        }

        // GET: DireksiMaster/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: DireksiMaster/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Direksi")] DireksiMaster direksiMaster)
        {
            if (ModelState.IsValid)
            {
                _context.Add(direksiMaster);
                await _context.SaveChangesAsync();
                 TempData["SuccessMessage"] = "successfully!";
                return RedirectToAction(nameof(Index));
            }
            return View(direksiMaster);
        }

        
              [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> FormEdit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var direksiMaster = await _context.DireksiMaster.FindAsync(id);
            if (direksiMaster == null)
            {
                return NotFound();
            }
            return View(direksiMaster);
        }

        // POST: DireksiMaster/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id, Direksi")] DireksiMaster direksiMaster)
        {
            if (id != direksiMaster.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(direksiMaster);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DireksiMasterExists(direksiMaster.Id))
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
            return View(direksiMaster);
        }

        // GET: DireksiMaster/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var direksiMaster = await _context.DireksiMaster
                .FirstOrDefaultAsync(m => m.Id == id);
            if (direksiMaster == null)
            {
                return NotFound();
            }

            return View(direksiMaster);
        }

        // POST: DireksiMaster/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var direksiMaster = await _context.DireksiMaster.FindAsync(id);
            _context.DireksiMaster.Remove(direksiMaster);
            await _context.SaveChangesAsync();
             TempData["SuccessMessage"] = "successfully!";
            return RedirectToAction(nameof(Index));
        }

        private bool DireksiMasterExists(int id)
        {
            return _context.DireksiMaster.Any(e => e.Id == id);
        }
    }
}
