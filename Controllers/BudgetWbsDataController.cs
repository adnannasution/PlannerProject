using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Plan.Data;
using Plan.Models;
using Microsoft.AspNetCore.Authorization;

namespace Plan.Controllers
{
    [Authorize]
    public class BudgetWbsDataController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BudgetWbsDataController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: WbsData
public async Task<IActionResult> Index()
{
    var budgetWbsData = await _context.BudgetWbsData
                                      .OrderByDescending(b => b.Id) // Mengurutkan berdasarkan Id secara descending
                                      .ToListAsync();
    return View(budgetWbsData);
}


        // GET: WbsData/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: WbsData/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BudgetWbsData wbsData)
        {
            if (ModelState.IsValid)
            {
                _context.Add(wbsData);
                await _context.SaveChangesAsync();
                 TempData["SuccessMessage"] = "successfully!";
                return RedirectToAction(nameof(Index));
            }
            return View(wbsData);
        }

                      [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> FormEdit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var wbsData = await _context.BudgetWbsData.FindAsync(id);
            if (wbsData == null)
            {
                return NotFound();
            }
            return View(wbsData);
        }

        // POST: WbsData/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, BudgetWbsData wbsData)
        {
            if (id != wbsData.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(wbsData);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!WbsDataExists(wbsData.Id))
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
            return View(wbsData);
        }

        // GET: WbsData/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var wbsData = await _context.BudgetWbsData
                .FirstOrDefaultAsync(m => m.Id == id);
            if (wbsData == null)
            {
                return NotFound();
            }

            return View(wbsData);
        }

        // POST: WbsData/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var BudgetWbsData = await _context.BudgetWbsData.FindAsync(id);
            _context.BudgetWbsData.Remove(BudgetWbsData);
            await _context.SaveChangesAsync();
             TempData["SuccessMessage"] = "successfully!";
            return RedirectToAction(nameof(Index));
        }

        private bool WbsDataExists(int id)
        {
            return _context.BudgetWbsData.Any(e => e.Id == id);
        }
    }
}
