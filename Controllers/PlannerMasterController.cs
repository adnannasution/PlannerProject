using Microsoft.AspNetCore.Mvc;
using Plan.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Plan.Data;
using Microsoft.AspNetCore.Authorization;

namespace Plan.Controllers
{
    [Authorize]
    public class PlannerMasterController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PlannerMasterController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: PlannerMaster
        public async Task<IActionResult> Index()
        {
            return View(await _context.PlannerMaster.ToListAsync());
        }

        // GET: PlannerMaster/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: PlannerMaster/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Planner")] PlannerMaster plannerMaster)
        {
            if (ModelState.IsValid)
            {
                _context.Add(plannerMaster);
                await _context.SaveChangesAsync();
                 TempData["SuccessMessage"] = "successfully!";
                return RedirectToAction(nameof(Index));
            }
            return View(plannerMaster);
        }

              [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> FormEdit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var plannerMaster = await _context.PlannerMaster.FindAsync(id);
            if (plannerMaster == null)
            {
                return NotFound();
            }
            return View(plannerMaster);
        }

        // POST: PlannerMaster/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id, Planner")] PlannerMaster plannerMaster)
        {
            if (id != plannerMaster.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(plannerMaster);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PlannerMasterExists(plannerMaster.Id))
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
            return View(plannerMaster);
        }

        // GET: PlannerMaster/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var plannerMaster = await _context.PlannerMaster
                .FirstOrDefaultAsync(m => m.Id == id);
            if (plannerMaster == null)
            {
                return NotFound();
            }

            return View(plannerMaster);
        }

        // POST: PlannerMaster/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var plannerMaster = await _context.PlannerMaster.FindAsync(id);
            _context.PlannerMaster.Remove(plannerMaster);
            await _context.SaveChangesAsync();
             TempData["SuccessMessage"] = "successfully!";
            return RedirectToAction(nameof(Index));
        }

        private bool PlannerMasterExists(int id)
        {
            return _context.PlannerMaster.Any(e => e.Id == id);
        }
    }
}
