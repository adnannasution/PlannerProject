using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Plan.Data;
using Plan.Models;
using Microsoft.AspNetCore.Authorization;

namespace Plan.Controllers
{
  [Authorize]
  public class DisiplinMasterController : Controller
  {
    private readonly ApplicationDbContext _context;

    public DisiplinMasterController(ApplicationDbContext context)
    {
      _context = context;
    }

    // GET: DisiplinMasters
    public async Task<IActionResult> Index()
    {
      return View(await _context.DisiplinMaster.ToListAsync());
    }

    // GET: DisiplinMasters/Details/5
    public async Task<IActionResult> Details(int? id)
    {
      if (id == null) return NotFound();

      var disiplinMaster = await _context.DisiplinMaster
          .FirstOrDefaultAsync(m => m.Id == id);
      if (disiplinMaster == null) return NotFound();

      return View(disiplinMaster);
    }

    // GET: DisiplinMasters/Create
    public IActionResult Create()
    {
      return View();
    }

    // POST: DisiplinMasters/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Id,Disiplin")] DisiplinMaster disiplinMaster)
    {
      if (ModelState.IsValid)
      {
        _context.Add(disiplinMaster);
        await _context.SaveChangesAsync();
         TempData["SuccessMessage"] = "successfully!";
        return RedirectToAction(nameof(Index));
      }
      return View(disiplinMaster);
    }

                  [HttpPost]
        [ValidateAntiForgeryToken]
    public async Task<IActionResult> FormEdit(int? id)
    {
      if (id == null) return NotFound();

      var disiplinMaster = await _context.DisiplinMaster.FindAsync(id);
      if (disiplinMaster == null) return NotFound();

      return View(disiplinMaster);
    }

    // POST: DisiplinMasters/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,Disiplin")] DisiplinMaster disiplinMaster)
    {
      if (id != disiplinMaster.Id) return NotFound();

      if (ModelState.IsValid)
      {
        try
        {
          _context.Update(disiplinMaster);
          await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
          if (!DisiplinMasterExists(disiplinMaster.Id)) return NotFound();
          else throw;
        }
         TempData["SuccessMessage"] = "successfully!";
        return RedirectToAction(nameof(Index));
      }
      return View(disiplinMaster);
    }

    // GET: DisiplinMasters/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
      if (id == null) return NotFound();

      var disiplinMaster = await _context.DisiplinMaster
          .FirstOrDefaultAsync(m => m.Id == id);
      if (disiplinMaster == null) return NotFound();

      return View(disiplinMaster);
    }

    // POST: DisiplinMasters/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
      var disiplinMaster = await _context.DisiplinMaster.FindAsync(id);

      if (disiplinMaster == null)
      {
        return NotFound();
      }

      _context.DisiplinMaster.Remove(disiplinMaster);
      await _context.SaveChangesAsync();
       TempData["SuccessMessage"] = "successfully!";
      return RedirectToAction(nameof(Index));
    }

    private bool DisiplinMasterExists(int id)
    {
      return _context.DisiplinMaster.Any(e => e.Id == id);
    }
  }
}
