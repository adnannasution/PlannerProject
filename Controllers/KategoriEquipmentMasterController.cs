using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Plan.Data;
using Plan.Models;
using Microsoft.AspNetCore.Authorization;

namespace Plan.Controllers
{
  [Authorize]
  public class KategoriEquipmentMasterController : Controller
  {
    private readonly ApplicationDbContext _context;

    public KategoriEquipmentMasterController(ApplicationDbContext context)
    {
      _context = context;
    }

    // GET: DisiplinMasters
    public async Task<IActionResult> Index()
    {
      return View(await _context.KategoriEquipmentMaster.ToListAsync());
    }

    // GET: DisiplinMasters/Create
    public IActionResult Create()
    {
      return View();
    }

    // POST: KategoriEquipmentMasters/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Id,Kategori")] KategoriEquipmentMaster kategoriEquipmentMaster)
    {
      if (ModelState.IsValid)
      {
        _context.Add(kategoriEquipmentMaster);
        await _context.SaveChangesAsync();
         TempData["SuccessMessage"] = "successfully!";
        return RedirectToAction(nameof(Index));
      }
      return View(kategoriEquipmentMaster);
    }

                  [HttpPost]
        [ValidateAntiForgeryToken]
    public async Task<IActionResult> FormEdit(int? id)
    {
      if (id == null) return NotFound();

      var kategoriEquipmentMaster = await _context.KategoriEquipmentMaster.FindAsync(id);
      if (kategoriEquipmentMaster == null) return NotFound();

      return View(kategoriEquipmentMaster);
    }

    // POST: DisiplinMasters/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,Kategori")] KategoriEquipmentMaster kategoriEquipmentMaster)
    {
      if (id != kategoriEquipmentMaster.Id) return NotFound();

      if (ModelState.IsValid)
      {
        try
        {
          _context.Update(kategoriEquipmentMaster);
          await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
          if (!DisiplinMasterExists(kategoriEquipmentMaster.Id)) return NotFound();
          else throw;
        }
         TempData["SuccessMessage"] = "successfully!";
        return RedirectToAction(nameof(Index));
      }
      return View(kategoriEquipmentMaster);
    }

    // GET: DisiplinMasters/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
      if (id == null) return NotFound();

      var kategoriEquipmentMaster = await _context.KategoriEquipmentMaster
          .FirstOrDefaultAsync(m => m.Id == id);
      if (kategoriEquipmentMaster == null) return NotFound();

      return View(kategoriEquipmentMaster);
    }

    // POST: DisiplinMasters/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
      var kategoriEquipmentMaster = await _context.KategoriEquipmentMaster.FindAsync(id);

      if (kategoriEquipmentMaster == null)
      {
        return NotFound();
      }

      _context.KategoriEquipmentMaster.Remove(kategoriEquipmentMaster);
      await _context.SaveChangesAsync();
       TempData["SuccessMessage"] = "successfully!";
      return RedirectToAction(nameof(Index));
    }

    private bool DisiplinMasterExists(int id)
    {
      return _context.KategoriEquipmentMaster.Any(e => e.Id == id);
    }
  }
}
