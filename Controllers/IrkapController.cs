using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Plan.Data;
using Plan.Models;

namespace Plan.Controllers
{
    public class IrkapController : Controller
    {
        private readonly ApplicationDbContext _context;

        public IrkapController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Irkap
        public async Task<IActionResult> Index()
        {
            return View(await _context.Irkap.ToListAsync());
        }

        // GET: Irkap/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var irkap = await _context.Irkap
                .FirstOrDefaultAsync(m => m.Id == id);
            if (irkap == null)
            {
                return NotFound();
            }

            return View(irkap);
        }

        // GET: Irkap/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Irkap/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,NoProgram")] Irkap irkap)
        {
            if (ModelState.IsValid)
            {
                _context.Add(irkap);
                await _context.SaveChangesAsync();

                 TempData["SuccessMessage"] = "successfully!";
                return RedirectToAction(nameof(Index));
            }
            return View(irkap);
        }

        // GET: Irkap/Edit/5
        [HttpPost]
        public async Task<IActionResult> FormEdit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var irkap = await _context.Irkap.FindAsync(id);
            if (irkap == null)
            {
                return NotFound();
            }
            return View(irkap);
        }

        // POST: Irkap/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,NoProgram")] Irkap irkap)
        {
            if (id != irkap.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(irkap);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!IrkapExists(irkap.Id))
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
            return View(irkap);
        }



        // POST: Irkap/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var irkap = await _context.Irkap.FindAsync(id);
            _context.Irkap.Remove(irkap);
            await _context.SaveChangesAsync();
             TempData["SuccessMessage"] = "successfully!";
            return RedirectToAction(nameof(Index));
        }

        private bool IrkapExists(int id)
        {
            return _context.Irkap.Any(e => e.Id == id);
        }
    }
}
