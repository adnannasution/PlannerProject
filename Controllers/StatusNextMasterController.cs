using Microsoft.AspNetCore.Mvc;
using Plan.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Plan.Data;
using Microsoft.AspNetCore.Authorization;

namespace Plan.Controllers
{
     [Authorize]
    public class StatusNextMasterController : Controller
    {
        private readonly ApplicationDbContext _context;

        public StatusNextMasterController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: StatusNextMaster
        public async Task<IActionResult> Index()
        {
            return View(await _context.StatusNextMaster.ToListAsync());
        }

        // GET: StatusNextMaster/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: StatusNextMaster/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("StatusNext")] StatusNextMaster statusNextMaster)
        {
            if (ModelState.IsValid)
            {
                _context.Add(statusNextMaster);
                await _context.SaveChangesAsync();
                 TempData["SuccessMessage"] = "successfully!";
                return RedirectToAction(nameof(Index));
            }
            return View(statusNextMaster);
        }

                      [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> FormEdit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var statusNextMaster = await _context.StatusNextMaster.FindAsync(id);
            if (statusNextMaster == null)
            {
                return NotFound();
            }
            return View(statusNextMaster);
        }

        // POST: StatusNextMaster/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id, StatusNext")] StatusNextMaster statusNextMaster)
        {
            if (id != statusNextMaster.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(statusNextMaster);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StatusNextMasterExists(statusNextMaster.Id))
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
            return View(statusNextMaster);
        }

        // GET: StatusNextMaster/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var statusNextMaster = await _context.StatusNextMaster
                .FirstOrDefaultAsync(m => m.Id == id);
            if (statusNextMaster == null)
            {
                return NotFound();
            }

            return View(statusNextMaster);
        }

        // POST: StatusNextMaster/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var statusNextMaster = await _context.StatusNextMaster.FindAsync(id);
            _context.StatusNextMaster.Remove(statusNextMaster);
            await _context.SaveChangesAsync();
             TempData["SuccessMessage"] = "successfully!";
            return RedirectToAction(nameof(Index));
        }

        private bool StatusNextMasterExists(int id)
        {
            return _context.StatusNextMaster.Any(e => e.Id == id);
        }
    }
}
