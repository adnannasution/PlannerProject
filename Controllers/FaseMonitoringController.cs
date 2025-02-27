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
    public class FaseMonitoringController : Controller
    {
        private readonly ApplicationDbContext _context;

        public FaseMonitoringController(ApplicationDbContext context)
        {
            _context = context;
        }




public async Task<IActionResult> Index()
{
    // Query untuk join FaseMonitoring dengan FasePlanning berdasarkan Kode_Project
    var tenders = from monitoring in _context.FaseMonitoring
                  join planning in _context.FasePlanning
                  on monitoring.Kode_Project equals planning.Kode_Project into planningGroup
                  from planning in planningGroup.DefaultIfEmpty() orderby monitoring.Id descending // Mengurutkan berdasarkan Id secara descending
                  select new
                  {
                      Monitoring = monitoring,  // Menyertakan seluruh data dari FaseMonitoring
                      Planning = planning       // Menyertakan seluruh data dari FasePlanning
                  };

    // Ambil semua data tanpa filter atau pagination
    var allTenders = await tenders.ToListAsync();

    // Pass data ke view
    return View(allTenders);
}




        // GET: FaseMonitoring/Create
        public IActionResult Create()
        {

var claimDisiplin = User.Claims.FirstOrDefault(c => c.Type == "Disiplin")?.Value;

// Query LINQ yang memfilter berdasarkan klaim 'disiplin'
var kodeProjects = _context.FasePlanning
    .Where(p => claimDisiplin == "All" || p.Disiplin == claimDisiplin)
    .OrderByDescending(p => p.Id)  // Order by Id in descending order
    .Select(p => new { p.Kode_Project, p.Pekerjaan })
    .ToList();

// Serialize data to be used in JavaScript
ViewBag.KodeProjects = JsonConvert.SerializeObject(kodeProjects);

            // Kirim data ke View melalui ViewBag
          
            
            // Mengisi dropdown list untuk Create
            ViewData["Status_LKP"] = new SelectList(new[] { "WARNING", "GOOD" });
            ViewData["Status_Volume"] = new SelectList(new[] { "ACTIVE", "NONACTIVE" });
            ViewData["Status_Durasi"] = new SelectList(new[] { "IN PROGRESS", "WARNING" });

            return View();
        }





// POST: FaseMonitoring/Create
[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Create(FaseMonitoring faseMonitoring)
{
    if (ModelState.IsValid)
    {
        try
        {
            // Save FaseMonitoring
            _context.Add(faseMonitoring);
            await _context.SaveChangesAsync();

            // Create and save Tahapan
            // var tahapan = new Tahapan
            // {
            //     Kode_Project = faseMonitoring.Kode_Project,
            //     Tanggal = DateTime.Now.Date,
            //     Waktu = DateTime.Now.TimeOfDay,
            //     Email = User.Claims.FirstOrDefault(c => c.Type == "Email")?.Value, // Get the logged-in user's email
            //     Tahap = "Created Fase Monitoring"
            // };

            // _context.Add(tahapan);
            // await _context.SaveChangesAsync();




            
        var historyFaseMonitoring = new HistoryFaseMonitoring
    {
        Kode_Project = faseMonitoring.Kode_Project, 
        Tanggal = DateTime.Now.Date, 
        Waktu = DateTime.Now.TimeOfDay, 
        Email = User.Claims.FirstOrDefault(c => c.Type == "Email")?.Value, 
        Aksi = "Tambah data" 
    };


    // Simpan entitas ke database
    _context.Add(historyFaseMonitoring);
    await _context.SaveChangesAsync(); 

            TempData["SuccessMessage"] = "Successfully created FaseMonitoring and logged Tahapan!";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, $"An error occurred while saving: {ex.Message}");
        }
    }

    // Populate dropdowns in case validation fails or an exception occurs
    ViewData["Status_LKP"] = new SelectList(new[] { "WARNING", "GOOD" });
    ViewData["Status_Volume"] = new SelectList(new[] { "ACTIVE", "NONACTIVE" });
    ViewData["Status_Durasi"] = new SelectList(new[] { "IN PROGRESS", "WARNING" });

    return View(faseMonitoring);
}





            [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> FormEdit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var faseMonitoring = await _context.FaseMonitoring.FindAsync(id);
            if (faseMonitoring == null)
            {
                return NotFound();
            }

            // Mengisi dropdown untuk Edit
            ViewData["Status_LKP"] = new SelectList(new[] { "WARNING", "GOOD" });
            ViewData["Status_Volume"] = new SelectList(new[] { "ACTIVE", "NONACTIVE" });
            ViewData["Status_Durasi"] = new SelectList(new[] { "IN PROGRESS", "WARNING" });

            return View(faseMonitoring);
        }


         [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Detail(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var faseMonitoring = await _context.FaseMonitoring.FindAsync(id);
            if (faseMonitoring == null)
            {
                return NotFound();
            }

            // Mengisi dropdown untuk Edit
            ViewData["Status_LKP"] = new SelectList(new[] { "WARNING", "GOOD" });
            ViewData["Status_Volume"] = new SelectList(new[] { "ACTIVE", "NONACTIVE" });
            ViewData["Status_Durasi"] = new SelectList(new[] { "IN PROGRESS", "WARNING" });


    var historyFaseMonitoring = new HistoryFaseMonitoring
    {
        Kode_Project = faseMonitoring.Kode_Project, 
        Tanggal = DateTime.Now.Date, 
        Waktu = DateTime.Now.TimeOfDay, 
        Email = User.Claims.FirstOrDefault(c => c.Type == "Email")?.Value, 
        Aksi = "Lihat data" 
    };


    // Simpan entitas ke database
    _context.Add(historyFaseMonitoring);
    await _context.SaveChangesAsync(); 



            return View(faseMonitoring);
        }

        // POST: FaseMonitoring/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, FaseMonitoring faseMonitoring)
        {
            if (id != faseMonitoring.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(faseMonitoring);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FaseMonitoringExists(faseMonitoring.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }


    var historyFaseMonitoring = new HistoryFaseMonitoring
    {
        Kode_Project = faseMonitoring.Kode_Project, 
        Tanggal = DateTime.Now.Date, 
        Waktu = DateTime.Now.TimeOfDay, 
        Email = User.Claims.FirstOrDefault(c => c.Type == "Email")?.Value, 
        Aksi = "Edit data" 
    };


    // Simpan entitas ke database
    _context.Add(historyFaseMonitoring);
    await _context.SaveChangesAsync(); 


                 TempData["SuccessMessage"] = "successfully!";
                return RedirectToAction(nameof(Index));
            }

            // Mengisi dropdown jika validasi gagal
            ViewData["Status_LKP"] = new SelectList(new[] { "WARNING", "GOOD" });
            ViewData["Status_Volume"] = new SelectList(new[] { "ACTIVE", "NONACTIVE" });
            ViewData["Status_Durasi"] = new SelectList(new[] { "IN PROGRESS", "WARNING" });

            return View(faseMonitoring);
        }

        // GET: FaseMonitoring/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var faseMonitoring = await _context.FaseMonitoring
                .FirstOrDefaultAsync(m => m.Id == id);
            if (faseMonitoring == null)
            {
                return NotFound();
            }

            return View(faseMonitoring);
        }

        // POST: FaseMonitoring/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var faseMonitoring = await _context.FaseMonitoring.FindAsync(id);
            _context.FaseMonitoring.Remove(faseMonitoring);
            await _context.SaveChangesAsync();



    var historyFaseMonitoring = new HistoryFaseMonitoring
    {
        Kode_Project = faseMonitoring.Kode_Project, 
        Tanggal = DateTime.Now.Date, 
        Waktu = DateTime.Now.TimeOfDay, 
        Email = User.Claims.FirstOrDefault(c => c.Type == "Email")?.Value, 
        Aksi = "Hapus data" 
    };


    // Simpan entitas ke database
    _context.Add(historyFaseMonitoring);
    await _context.SaveChangesAsync(); 


             TempData["SuccessMessage"] = "successfully!";
            return RedirectToAction(nameof(Index));
        }

        private bool FaseMonitoringExists(int id)
        {
            return _context.FaseMonitoring.Any(e => e.Id == id);
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
                    for (int row = 2; row <= rowCount; row++) // Start from the second row to skip headers
                    {
                       var faseMonitoring = new FaseMonitoring
{
    Kode_Project = worksheet.Cells[row, 1]?.Value?.ToString(),
    Status_LKP = worksheet.Cells[row, 2]?.Value?.ToString(),
    Status_Volume = worksheet.Cells[row, 3]?.Value?.ToString(),
    Count_Days_Progress = int.TryParse(worksheet.Cells[row, 4]?.Value?.ToString(), out var daysProgress) ? daysProgress : 0,
    Count_Days_Over = int.TryParse(worksheet.Cells[row, 5]?.Value?.ToString(), out var daysOver) ? daysOver : 0,
    Status_Durasi = worksheet.Cells[row, 6]?.Value?.ToString(),
    Status_Kontrak = worksheet.Cells[row, 7]?.Value?.ToString(),
    Amandemen_Nilai = int.TryParse(worksheet.Cells[row, 8]?.Value?.ToString(), out var amandemen) ? amandemen : 0,
    Total_Nilai_Kontrak = decimal.TryParse(worksheet.Cells[row, 9]?.Value?.ToString().Replace(",", "").Replace(".", "").Trim(), out var totalKontrak) ? totalKontrak / 100 : 0,
    Nilai_Tagihan = decimal.TryParse(worksheet.Cells[row, 10]?.Value?.ToString().Replace(",", "").Replace(".", "").Trim(), out var nilaiTagihan) ? nilaiTagihan / 100 : 0,
};

                        _context.FaseMonitoring.Add(faseMonitoring);
                    }

                    await _context.SaveChangesAsync();
                }
            }

            return RedirectToAction(nameof(Index));
        }



[HttpGet]
public IActionResult GetStartDate(string kodeProject)
{
    var startDate = _context.FaseExecution
                    .Where(f => f.Kode_Project == kodeProject)
                    .Select(f => f.Start_Date)
                    .FirstOrDefault();

    if (startDate.HasValue) // Cek apakah startDate tidak null
    {
        return Json(new { startDate = startDate.Value.ToString("yyyy-MM-dd") });
    }

    return Json(new { startDate = "" }); // Return string kosong jika tidak ditemukan
}

 


[HttpGet]
public IActionResult GetPurchaseOrderValue(string kodeProject)
{
    var purchaseOrderValue = _context.FaseTender
                    .Where(f => f.Kode_Project == kodeProject)
                    .Select(f => f.Nilai_Purchasing_Order)
                    .FirstOrDefault();

    return Json(new { nilaiPurchaseOrder = purchaseOrderValue });
}


[HttpGet]
public IActionResult GetTotalTagihan(string kodeProject)
{
    var totalTagihan = _context.Termin
        .Where(t => t.Kode_Project == kodeProject)
        .Sum(t => (decimal?)t.Nilai_Tagihan) ?? 0; // Jika null, set default 0

    return Json(new { totalTagihan = totalTagihan });
}



       public async Task<IActionResult> TampilHistoryFaseMonitoring(string kodeProject)
        {
            var histori = await _context.HistoryFaseMonitoring
                .Where(t => t.Kode_Project == kodeProject)
                .ToListAsync();

            if (histori == null || !histori.Any())
            {
                return NotFound("No records found for this project.");
            }

            // Set Kode_Project in ViewData for display in the view
            ViewData["KodeProject"] = kodeProject;

            return View(histori);

            //return PartialView("ViewSLA", histori);
        }


    }
}
