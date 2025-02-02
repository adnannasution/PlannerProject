using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Plan.Models; // Update with your actual namespace
using System.Threading.Tasks;
using Plan.Data;
using Microsoft.AspNetCore.Authorization;


namespace Plan.Controllers
{
    [Authorize]
    public class LongTermServiceAgreementController : Controller
    {
        private readonly ApplicationDbContext _context;

        public LongTermServiceAgreementController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: LongTermServiceAgreement
public async Task<IActionResult> Index()
{
    var data = await _context.LongTermServiceAgreement
                             .OrderByDescending(l => l.Id) // Mengurutkan berdasarkan Id secara descending
                             .ToListAsync();
    return View(data);
}


        // GET: LongTermServiceAgreement/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var agreement = await _context.LongTermServiceAgreement
                .FirstOrDefaultAsync(m => m.Id == id);

            if (agreement == null)
                return NotFound();

            return View(agreement);
        }

        // GET: LongTermServiceAgreement/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: LongTermServiceAgreement/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(LongTermServiceAgreement agreement)
        {
            if (ModelState.IsValid)
            {
                _context.Add(agreement);
                await _context.SaveChangesAsync();
                 TempData["SuccessMessage"] = "successfully!";
                return RedirectToAction(nameof(Index));
            }
            return View(agreement);
        }

        // GET: LongTermServiceAgreement/Edit/5
        public async Task<IActionResult> FormEdit(int? id)
        {
            if (id == null)
                return NotFound();

            var agreement = await _context.LongTermServiceAgreement.FindAsync(id);
            if (agreement == null)
                return NotFound();

            return View(agreement);
        }

        // POST: LongTermServiceAgreement/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, LongTermServiceAgreement agreement)
        {
            if (id != agreement.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(agreement);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AgreementExists(agreement.Id))
                        return NotFound();
                    else
                        throw;
                }
                 TempData["SuccessMessage"] = "successfully!";
                return RedirectToAction(nameof(Index));
            }
            return View(agreement);
        }

        // GET: LongTermServiceAgreement/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var agreement = await _context.LongTermServiceAgreement
                .FirstOrDefaultAsync(m => m.Id == id);

            if (agreement == null)
                return NotFound();

            return View(agreement);
        }

        // POST: LongTermServiceAgreement/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var agreement = await _context.LongTermServiceAgreement.FindAsync(id);
            if (agreement != null)
            {
                _context.LongTermServiceAgreement.Remove(agreement);
                await _context.SaveChangesAsync();
            }
             TempData["SuccessMessage"] = "successfully!";
            return RedirectToAction(nameof(Index));
        }

        private bool AgreementExists(int id)
        {
            return _context.LongTermServiceAgreement.Any(e => e.Id == id);
        }
    }
}
