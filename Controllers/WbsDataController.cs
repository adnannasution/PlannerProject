using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Plan.Data;
using Plan.Models;
using Microsoft.AspNetCore.Authorization;
 
namespace Plan.Controllers
{
     [Authorize]
    public class WbsDataController : Controller
    {
        private readonly ApplicationDbContext _context;

        public WbsDataController(ApplicationDbContext context)
        {
            _context = context;
        }

        // // GET: WbsData
        // public async Task<IActionResult> Index()
        // {
        //     return View(await _context.WbsData.ToListAsync());
        // }



public IActionResult Index()
{
    var joinedData = from wbs in _context.WbsData
                     join budget in _context.BudgetWbsData
                     on new { wbs.JenisBiaya, wbs.Tahun } equals new { budget.JenisBiaya, budget.Tahun }
                     into budgetJoin
                     from budget in budgetJoin.DefaultIfEmpty() // Left join
                     select new
                     {
                         wbs.JenisBiaya,
                         wbs.Tahun,
                         wbs.ActualWbsRt,
                         wbs.CommitmentWbsRt,
                         wbs.RopWbsRt,
                         BudgetWbs = budget != null ? budget.BudgetWbs : 0
                     };

    var viewModel = joinedData.ToList();
    return View(viewModel);
}



public IActionResult Details(string jenisBiaya, int tahun)
{
    // Gunakan `jenisBiaya` dan `tahun` untuk mendapatkan data spesifik.
    var data = _context.WbsData
        .Where(x => x.JenisBiaya == jenisBiaya && x.Tahun == tahun)
        .ToList();

    if (data == null)
    {
        return NotFound();
    }

    return View(data);
}




        // GET: WbsData/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: WbsData/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(WbsData wbsData)
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

            var wbsData = await _context.WbsData.FindAsync(id);
            if (wbsData == null)
            {
                return NotFound();
            }
            return View(wbsData);
        }

        // POST: WbsData/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, WbsData wbsData)
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

            var wbsData = await _context.WbsData
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
            var wbsData = await _context.WbsData.FindAsync(id);
            _context.WbsData.Remove(wbsData);
            await _context.SaveChangesAsync();
             TempData["SuccessMessage"] = "successfully!";
            return RedirectToAction(nameof(Index));
        }

        private bool WbsDataExists(int id)
        {
            return _context.WbsData.Any(e => e.Id == id);
        }


        [HttpGet]
public IActionResult GetBudgetWbs(string jenisBiaya, int tahun)
{
    var budgetWbs = _context.BudgetWbsData
                    .Where(b => b.JenisBiaya == jenisBiaya && b.Tahun == tahun)
                    .Select(b => b.BudgetWbs)
                    .FirstOrDefault();

    return Json(new { budgetWbs });
}



    }
}
