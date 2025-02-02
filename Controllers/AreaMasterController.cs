using Microsoft.AspNetCore.Mvc;
using Plan.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Plan.Data;
using Microsoft.AspNetCore.Authorization;

namespace Plan.Controllers
{
     [Authorize]
    public class AreaMasterController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AreaMasterController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: AreaMaster
        public async Task<IActionResult> Index()
        {
            return View(await _context.AreaMaster.ToListAsync());
        }

        // GET: AreaMaster/Create
        public IActionResult Create()
        {
            return View();
        }
 
     
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Area")] AreaMaster areaMaster)
        {
            if (ModelState.IsValid)
            {
                _context.Add(areaMaster);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "successfully!";
                return RedirectToAction(nameof(Index));
            }
            return View(areaMaster);
        }
 
              [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> FormEdit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var areaMaster = await _context.AreaMaster.FindAsync(id);
            if (areaMaster == null)
            {
                return NotFound();
            }
            return View(areaMaster);
        }

        // POST: AreaMaster/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id, Area")] AreaMaster areaMaster)
        {
            if (id != areaMaster.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(areaMaster);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AreaMasterExists(areaMaster.Id))
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
            return View(areaMaster);
        }

        // GET: AreaMaster/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var areaMaster = await _context.AreaMaster
                .FirstOrDefaultAsync(m => m.Id == id);
            if (areaMaster == null)
            {
                return NotFound();
            }

            return View(areaMaster);
        }

        // POST: AreaMaster/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var areaMaster = await _context.AreaMaster.FindAsync(id);
            _context.AreaMaster.Remove(areaMaster);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "successfully!";
            return RedirectToAction(nameof(Index));
        }

        private bool AreaMasterExists(int id)
        {
            return _context.AreaMaster.Any(e => e.Id == id);
        }
    }
}
