using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Plan.Data;
using Plan.Models;
using System.Linq;
using System.Threading.Tasks;
using OfficeOpenXml;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json;



 
namespace Plan.Controllers
{
[Authorize]
    public class FaseTenderController : Controller
    {
        private readonly ApplicationDbContext _context;

        public FaseTenderController(ApplicationDbContext context)
        {
            _context = context;
        }




public async Task<IActionResult> Index()
{
    // Join FaseTender and FasePlanning
    var tenders = from tender in _context.FaseTender
                  join planning in _context.FasePlanning
                  on tender.Kode_Project equals planning.Kode_Project into planningGroup
                  from planning in planningGroup.DefaultIfEmpty()
                  orderby tender.Id descending // Mengurutkan berdasarkan Id secara descending
                  select new
                  {
                      Tender = tender,
                      Planning = planning
                  };

    var tendersList = await tenders.ToListAsync();

    return View(tendersList);
}






// GET: FaseTender/Create
public IActionResult Create()
{
// Ambil klaim 'disiplin' dari User.Claims
var claimDisiplin = User.Claims.FirstOrDefault(c => c.Type == "Disiplin")?.Value;

// Query LINQ yang memfilter berdasarkan klaim 'disiplin'
var kodeProjects = _context.FasePlanning
    .Where(p => claimDisiplin == "All" || p.Disiplin == claimDisiplin)
    .OrderByDescending(p => p.Id)  // Order by Id in descending order
    .Select(p => new { p.Kode_Project, p.Pekerjaan })
    .ToList();

// Serialize data to be used in JavaScript
ViewBag.KodeProjects = JsonConvert.SerializeObject(kodeProjects);

    ViewData["Otorisasi"] = new SelectList(_context.OtorisasiKontrakMaster, "Otorisasi", "Otorisasi");

    return View();
}

 
        // POST: FasePlanning/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(FaseTender faseTender)
        {
            if (ModelState.IsValid)
            {
                _context.Add(faseTender);
                await _context.SaveChangesAsync();


                 TempData["SuccessMessage"] = "successfully!";
                return RedirectToAction(nameof(Index));
            }
            return View(faseTender);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> FormEdit(int? id)
        {



            if (id == null)
            {
                return NotFound();
            }

            var faseTender = await _context.FaseTender.FindAsync(id);
            if (faseTender == null)
            {
                return NotFound();
            }

                              ViewData["Otorisasi"] = new SelectList(_context.OtorisasiKontrakMaster, "Otorisasi", "Otorisasi", faseTender.Otorisasi);

            return View(faseTender);
        }




        // [HttpPost]
        // [ValidateAntiForgeryToken]
        // public async Task<IActionResult> Detail(int? id)
        // {



        //     if (id == null)
        //     {
        //         return NotFound();
        //     }

        //     var faseTender = await _context.FaseTender.FindAsync(id);
        //     if (faseTender == null)
        //     {
        //         return NotFound();
        //     }

        //                       ViewData["Otorisasi"] = new SelectList(_context.OtorisasiKontrakMaster, "Otorisasi", "Otorisasi", faseTender.Otorisasi);

        //     return View(faseTender);
        // }


[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Detail(int? id)
{
    if (id == null)
    {
        return NotFound();
    }

    // Ambil data FaseTender berdasarkan ID
    var faseTender = await _context.FaseTender.FindAsync(id);
    if (faseTender == null)
    {
        return NotFound();
    }

    // Ambil daftar komponen berdasarkan Kode_Project yang sama
    var components = await _context.TabelComponent
        .Where(c => c.Kode_Project == faseTender.Kode_Project)
        .ToListAsync();

    // Kirim data ke View
    ViewData["Otorisasi"] = new SelectList(_context.OtorisasiKontrakMaster, "Otorisasi", "Otorisasi", faseTender.Otorisasi);
    ViewData["Components"] = components;

    return View(faseTender);
}





        // POST: FaseTender/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, FaseTender faseTender)
        {
            if (id != faseTender.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(faseTender);
                    await _context.SaveChangesAsync();
                     TempData["SuccessMessage"] = "successfully!";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FaseTenderExists(faseTender.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            return View(faseTender);
        }


        // GET: FaseTender/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var faseTender = await _context.FaseTender
                .FirstOrDefaultAsync(m => m.Id == id);
            if (faseTender == null)
            {
                return NotFound();
            }

            return View(faseTender);
        }

        // // POST: FaseTender/Delete/5
        // [HttpPost, ActionName("Delete")]
        // [ValidateAntiForgeryToken]
        // public async Task<IActionResult> DeleteConfirmed(int id)
        // {
        //     var faseTender = await _context.FaseTender.FindAsync(id);
        //     _context.FaseTender.Remove(faseTender);
        //     await _context.SaveChangesAsync();
        //      TempData["SuccessMessage"] = "successfully!";
        //     return RedirectToAction(nameof(Index));
        // }


// POST: FaseTender/Delete/5
[HttpPost, ActionName("Delete")]
[ValidateAntiForgeryToken]
public async Task<IActionResult> DeleteConfirmed(int id)
{
    var faseTender = await _context.FaseTender.FindAsync(id);
    if (faseTender == null)
    {
        return NotFound();
    }

    // Ambil Kode_Project dari faseTender
    string kodeProject = faseTender.Kode_Project;

    // Hapus semua data di TabelComponent yang memiliki Kode_Project yang sama
    var components = _context.TabelComponent.Where(c => c.Kode_Project == kodeProject);
    _context.TabelComponent.RemoveRange(components);

    // Hapus faseTender setelah menghapus komponennya
    _context.FaseTender.Remove(faseTender);

    // Simpan perubahan ke database
    await _context.SaveChangesAsync();

    // Notifikasi sukses
    TempData["SuccessMessage"] = "Data berhasil dihapus!";

    return RedirectToAction(nameof(Index));
}


        private bool FaseTenderExists(int id)
        {
            return _context.FaseTender.Any(e => e.Id == id);
        }





[HttpGet]
public IActionResult Upload()
{
    return View();
}

[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Upload(FileUploadViewModel model)
{
    if (model.File == null || model.File.Length == 0)
    {
        ModelState.AddModelError("File", "Please upload a valid Excel file.");
        return View(model);
    }

    using (var stream = new MemoryStream())
    {
        await model.File.CopyToAsync(stream);

        using (var package = new ExcelPackage(stream))
        {
            var worksheet = package.Workbook.Worksheets.FirstOrDefault();
            if (worksheet == null)
            {
                ModelState.AddModelError("File", "The Excel file is empty.");
                return View(model);
            }

            int rowCount = worksheet.Dimension.Rows;
            for (int row = 2; row <= rowCount; row++) // Mulai dari baris kedua untuk melewati header
            {
               


               var faseTender = new FaseTender
{
    Kode_Project = worksheet.Cells[row, 1]?.Value?.ToString(),
    No_Vendor = worksheet.Cells[row, 2]?.Value?.ToString(),
    Pelaksana = worksheet.Cells[row, 3]?.Value?.ToString(),
    PO_OA = worksheet.Cells[row, 4]?.Value?.ToString(),
    PO_SR = worksheet.Cells[row, 5]?.Value?.ToString(),
    PO_RO = worksheet.Cells[row, 6]?.Value?.ToString(),
    PR_OA = worksheet.Cells[row, 7]?.Value?.ToString(),
    PR_MT = worksheet.Cells[row, 8]?.Value?.ToString(),
    PR_SR = worksheet.Cells[row, 9]?.Value?.ToString(),
    Nomor_SP3MK = worksheet.Cells[row, 10]?.Value?.ToString(),
    Nilai_Purchasing_Order = decimal.TryParse(worksheet.Cells[row, 11]?.Value?.ToString(), out var npo) ? npo : 0,
    Nilai_Purchasing_Request = decimal.TryParse(worksheet.Cells[row, 12]?.Value?.ToString(), out var npr) ? npr : 0,
    Durasi_Masa_Penyelesaian_MPL = int.TryParse(worksheet.Cells[row, 13]?.Value?.ToString(), out var durasi) ? durasi : 0,
    Bulan = int.TryParse(worksheet.Cells[row, 14]?.Value?.ToString(), out var bulan) ? bulan : 0,
    Buyer = worksheet.Cells[row, 15]?.Value?.ToString(),
    Otorisasi = worksheet.Cells[row, 16]?.Value?.ToString(),
};

                _context.FaseTender.Add(faseTender);
            }

            await _context.SaveChangesAsync();
        }
    }

    return RedirectToAction(nameof(Index));
}



[HttpGet]
public IActionResult GetProjectData(string kodeProject)
{

        if (string.IsNullOrEmpty(kodeProject))
    {
        Console.WriteLine("Kode Project tidak diterima!");
        return Json(new { error = "Kode Project tidak boleh kosong." });
    }

    Console.WriteLine($"Kode Project yang diterima: {kodeProject}");

    var project = _context.FasePlanning
                          .Where(fp => fp.Kode_Project == kodeProject)
                          .Select(fp => new 
                          { 
                              JenisKontrak = fp.Jenis_Kontrak, 
                              JenisProject = fp.Jenis_Project 
                          })
                          .FirstOrDefault();

    if (project != null)
    {
        return Json(project);
    }
    return Json(null);
}


[HttpPost]
public IActionResult CreateComponent([FromBody] TabelComponent model)
{
    if (model == null) return BadRequest("Data tidak valid.");

    _context.TabelComponent.Add(model);
    _context.SaveChanges();

    return Json(model); // Kembalikan data yang baru dimasukkan
}





[HttpDelete]
public IActionResult DeleteComponent(int id)
{
    var component = _context.TabelComponent.Find(id);
    if (component == null) return NotFound();

    _context.TabelComponent.Remove(component);
    _context.SaveChanges();

    return Ok();
}





[HttpGet]
public IActionResult GetComponents(string kodeProject)
{
    var components = _context.TabelComponent
        .Where(c => c.Kode_Project == kodeProject)
        .Select(c => new
        {
            c.Id,
            c.Component,
            c.Description,
            c.Quantity,
            c.Unit,
            c.ValuationPrice,
            c.DeliveryDate
        })
        .ToList();

    Console.WriteLine(JsonConvert.SerializeObject(components)); // Debug di server
    return Json(components);
}


[HttpGet]
public IActionResult GetComponentsHtml(string kodeProject)
{
    var components = _context.TabelComponent
        .Where(c => c.Kode_Project == kodeProject)
        .ToList();

    return PartialView("TampilComponent", components);
}



    }

}