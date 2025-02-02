using Microsoft.AspNetCore.Mvc;
using Plan.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Plan.Data;
using Microsoft.AspNetCore.Authorization;

namespace Plan.Controllers
{
     [Authorize]
    public class ResumeStatusKontrakMasterController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ResumeStatusKontrakMasterController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: ResumeStatusKontrak
        public async Task<IActionResult> Index()
        {
            return View(await _context.ResumeStatusKontrakMaster.ToListAsync());
        }

        // GET: ResumeStatusKontrak/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: ResumeStatusKontrak/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Resume")] ResumeStatusKontrakMaster resumeStatusKontrak)
        {
            if (ModelState.IsValid)
            {
                _context.Add(resumeStatusKontrak);
                await _context.SaveChangesAsync();
                 TempData["SuccessMessage"] = "successfully!";
                return RedirectToAction(nameof(Index));
            }
            return View(resumeStatusKontrak);
        }

                    [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> FormEdit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var resumeStatusKontrak = await _context.ResumeStatusKontrakMaster.FindAsync(id);
            if (resumeStatusKontrak == null)
            {
                return NotFound();
            }
            return View(resumeStatusKontrak);
        }

        // POST: ResumeStatusKontrak/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Resume")] ResumeStatusKontrakMaster resumeStatusKontrak)
        {
            if (id != resumeStatusKontrak.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(resumeStatusKontrak);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ResumeStatusKontrakExists(resumeStatusKontrak.Id))
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
            return View(resumeStatusKontrak);
        }

        // GET: ResumeStatusKontrak/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var resumeStatusKontrak = await _context.ResumeStatusKontrakMaster
                .FirstOrDefaultAsync(m => m.Id == id);
            if (resumeStatusKontrak == null)
            {
                return NotFound();
            }

            return View(resumeStatusKontrak);
        }

        // POST: ResumeStatusKontrak/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var resumeStatusKontrak = await _context.ResumeStatusKontrakMaster.FindAsync(id);
            _context.ResumeStatusKontrakMaster.Remove(resumeStatusKontrak);
            await _context.SaveChangesAsync();
             TempData["SuccessMessage"] = "successfully!";
            return RedirectToAction(nameof(Index));
            
        }

        private bool ResumeStatusKontrakExists(int id)
        {
            return _context.ResumeStatusKontrakMaster.Any(e => e.Id == id);
        }
    }
}
