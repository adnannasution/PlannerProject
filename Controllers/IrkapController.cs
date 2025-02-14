using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Plan.Data;
using Plan.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using OfficeOpenXml;

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

             ViewData["Disiplin"] = new SelectList(_context.DisiplinMaster, "Disiplin", "Disiplin");

            return View();
        }

        // POST: Irkap/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Irkap irkap)
        {
            if (ModelState.IsValid)
            {
                _context.Add(irkap);
                await _context.SaveChangesAsync();

                 TempData["SuccessMessage"] = "successfully!";
                return RedirectToAction(nameof(Index));
            }
             ViewData["Disiplin"] = new SelectList(_context.DisiplinMaster, "Disiplin", "Disiplin");
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

               ViewData["Disiplin"] = new SelectList(_context.DisiplinMaster, "Disiplin", "Disiplin");

            return View(irkap);
        }

        // POST: Irkap/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Irkap irkap)
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

               ViewData["Disiplin"] = new SelectList(_context.DisiplinMaster, "Disiplin", "Disiplin");

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




        public IActionResult Import()
        {

          

            return View();
        }


    //  [HttpPost]
    //     public IActionResult Import(IFormFile file)
    //     {
    //         if (file == null || file.Length == 0)
    //         {
    //             TempData["Error"] = "File tidak ditemukan!";
    //             return RedirectToAction("Index");
    //         }

    //         try
    //         {
    //             using (var stream = new MemoryStream())
    //             {
    //                 file.CopyTo(stream);
    //                 using (var package = new ExcelPackage(stream))
    //                 {
    //                     var worksheet = package.Workbook.Worksheets[0]; // Ambil sheet pertama
    //                     int rowCount = worksheet.Dimension.Rows;

    //                     var dataList = new List<Irkap>();

    //                     for (int row = 2; row <= rowCount; row++) // Mulai dari baris ke-2 (skip header)
    //                     {
    //                         var data = new Irkap
    //                         {
    //                             NoProgram = worksheet.Cells[row, 1].Text, // Kolom A
    //                             Judul = worksheet.Cells[row, 2].Text,     // Kolom B
    //                             Disiplin = worksheet.Cells[row, 3].Text   // Kolom C
    //                         };

    //                         dataList.Add(data);
    //                     }

    //                     _context.Irkap.AddRange(dataList);
    //                     _context.SaveChanges();
    //                 }
    //             }

    //             TempData["Success"] = "Data berhasil diimpor!";
    //         }
    //         catch (Exception ex)
    //         {
    //             TempData["Error"] = $"Terjadi kesalahan: {ex.Message}";
    //         }

    //         return RedirectToAction("Index");
    //     }



[HttpPost]
public IActionResult Import(IFormFile file)
{
    if (file == null || file.Length == 0)
    {
        TempData["Error"] = "File tidak ditemukan!";
        return RedirectToAction("Index");
    }

    try
    {
        using (var stream = new MemoryStream())
        {
            file.CopyTo(stream);
            using (var package = new ExcelPackage(stream))
            {
                var worksheet = package.Workbook.Worksheets[0]; // Ambil sheet pertama
                int rowCount = worksheet.Dimension.Rows;

                for (int row = 2; row <= rowCount; row++) // Mulai dari baris ke-2 (skip header)
                {
                    string noProgram = worksheet.Cells[row, 1].Text; // Kolom A (NoProgram)
                    string judul = worksheet.Cells[row, 2].Text;     // Kolom B (Judul)
                    string disiplin = worksheet.Cells[row, 3].Text;  // Kolom C (Disiplin)

                    // Cek apakah NoProgram sudah ada
                    var existingData = _context.Irkap.FirstOrDefault(i => i.NoProgram == noProgram);

                    if (existingData != null)
                    {
                        // Jika sudah ada, lakukan UPDATE
                        existingData.Judul = judul;
                        existingData.Disiplin = disiplin;
                    }
                    else
                    {
                        // Jika belum ada, lakukan INSERT
                        var newData = new Irkap
                        {
                            NoProgram = noProgram,
                            Judul = judul,
                            Disiplin = disiplin
                        };

                        _context.Irkap.Add(newData);
                    }
                }

                _context.SaveChanges(); // Simpan semua perubahan sekaligus
            }
        }

          TempData["SuccessMessage"] = "successfully!";
    }
    catch (Exception ex)
    {
        TempData["Error"] = $"Terjadi kesalahan: {ex.Message}";
    }

    return RedirectToAction("Index");
}






        private bool IrkapExists(int id)
        {
            return _context.Irkap.Any(e => e.Id == id);
        }
    }
}
