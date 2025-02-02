using Microsoft.AspNetCore.Mvc;
using Plan.Data; // Namespace untuk DbContext Anda
using Plan.Models; // Namespace untuk model Budget
using Microsoft.AspNetCore.Authorization;

namespace Plan.Controllers
{
      [Authorize]
    public class BudgetController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BudgetController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Budget
public IActionResult Index()
{
    var budgets = _context.Budget
                          .OrderByDescending(b => b.Id) // Mengurutkan berdasarkan Id secara descending
                          .ToList();
    return View(budgets);
}


        // GET: Budget/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Budget/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Budget budget)
        {
            if (ModelState.IsValid)
            {
                _context.Add(budget);
                _context.SaveChanges();
                 TempData["SuccessMessage"] = "successfully!";
                return RedirectToAction(nameof(Index));
            }
            return View(budget);
        }

        // GET: Budget/Edit/5
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var budget = _context.Budget.Find(id);
            if (budget == null)
            {
                return NotFound();
            }
            return View(budget);
        }

        // POST: Budget/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Budget budget)
        {
            if (id != budget.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                _context.Update(budget);
                _context.SaveChanges();
                 TempData["SuccessMessage"] = "successfully!";
                return RedirectToAction(nameof(Index));
            }
            return View(budget);
        }

        // GET: Budget/Delete/5
        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var budget = _context.Budget.Find(id);
            if (budget == null)
            {
                return NotFound();
            }

            return View(budget);
        }

        // POST: Budget/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var budget = _context.Budget.Find(id);
            if (budget != null)
            {
                _context.Budget.Remove(budget);
                _context.SaveChanges();
            }
             TempData["SuccessMessage"] = "successfully!";
            return RedirectToAction(nameof(Index));
        }
    }
}
