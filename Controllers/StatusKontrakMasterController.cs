using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Plan.Data;
using Plan.Models;
using Microsoft.AspNetCore.Authorization;

namespace Plan.Controllers
{
     [Authorize]
    public class StatusKontrakMasterController : Controller
    {
        private readonly ApplicationDbContext _context;

        public StatusKontrakMasterController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: StatusKontrakMaster
        public async Task<IActionResult> Index()
        {
            return View(await _context.StatusKontrakMaster.ToListAsync());
        }

        // GET: StatusKontrakMaster/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var statusKontrakMaster = await _context.StatusKontrakMaster
                .FirstOrDefaultAsync(m => m.Id == id);
            if (statusKontrakMaster == null)
            {
                return NotFound();
            }

            return View(statusKontrakMaster);
        }

        // GET: StatusKontrakMaster/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: StatusKontrakMaster/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Status")] StatusKontrakMaster statusKontrakMaster)
        {
            if (ModelState.IsValid)
            {
                _context.Add(statusKontrakMaster);
                await _context.SaveChangesAsync();
                 TempData["SuccessMessage"] = "successfully!";
                return RedirectToAction(nameof(Index));
            }
            return View(statusKontrakMaster);
        }

                     [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> FormEdit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var statusKontrakMaster = await _context.StatusKontrakMaster.FindAsync(id);
            if (statusKontrakMaster == null)
            {
                return NotFound();
            }
            return View(statusKontrakMaster);
        }

        // POST: StatusKontrakMaster/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Status")] StatusKontrakMaster statusKontrakMaster)
        {
            if (id != statusKontrakMaster.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(statusKontrakMaster);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StatusKontrakMasterExists(statusKontrakMaster.Id))
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
            return View(statusKontrakMaster);
        }

        // GET: StatusKontrakMaster/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var statusKontrakMaster = await _context.StatusKontrakMaster
                .FirstOrDefaultAsync(m => m.Id == id);
            if (statusKontrakMaster == null)
            {
                return NotFound();
            }

            return View(statusKontrakMaster);
        }

        // POST: StatusKontrakMaster/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var statusKontrakMaster = await _context.StatusKontrakMaster.FindAsync(id);

            if (statusKontrakMaster == null)
            {
                return NotFound();
            }

            _context.StatusKontrakMaster.Remove(statusKontrakMaster);
            await _context.SaveChangesAsync();
             TempData["SuccessMessage"] = "successfully!";
            return RedirectToAction(nameof(Index));
        }

        private bool StatusKontrakMasterExists(int id)
        {
            return _context.StatusKontrakMaster.Any(e => e.Id == id);
        }
    }
}
