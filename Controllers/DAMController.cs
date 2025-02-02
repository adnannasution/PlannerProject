using Microsoft.AspNetCore.Mvc;
using Plan.Data; // Namespace untuk DbContext Anda
using Plan.Models; // Namespace untuk model DAM
using Microsoft.AspNetCore.Authorization;

namespace Plan.Controllers
{
    [Authorize]
    public class DAMController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DAMController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: DAM
public IActionResult Index()
{
    var dams = _context.DAM
                       .OrderByDescending(d => d.Id) // Mengurutkan berdasarkan Id secara descending
                       .ToList();
    return View(dams);
}


        // GET: DAM/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: DAM/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(DAM dam)
        {
            if (ModelState.IsValid)
            {
                _context.Add(dam);
                _context.SaveChanges();
                 TempData["SuccessMessage"] = "successfully!";
                return RedirectToAction(nameof(Index));
            }
            return View(dam);
        }

                   [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult FormEdit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dam = _context.DAM.Find(id);
            if (dam == null)
            {
                return NotFound();
            }
            return View(dam);
        }

        // POST: DAM/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, DAM dam)
        {
            if (id != dam.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                _context.Update(dam);
                _context.SaveChanges();
                 TempData["SuccessMessage"] = "successfully!";
                return RedirectToAction(nameof(Index));
            }
            return View(dam);
        }

        // GET: DAM/Delete/5
        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dam = _context.DAM.Find(id);
            if (dam == null)
            {
                return NotFound();
            }

            return View(dam);
        }

        // POST: DAM/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var dam = _context.DAM.Find(id);
            if (dam != null)
            {
                _context.DAM.Remove(dam);
                _context.SaveChanges();
            }
             TempData["SuccessMessage"] = "successfully!";
            return RedirectToAction(nameof(Index));
        }
    }
}
