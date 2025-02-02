using Microsoft.AspNetCore.Mvc;
using Plan.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Plan.Data;
using Microsoft.AspNetCore.Mvc.Rendering;
using OfficeOpenXml;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json;




namespace Plan.Controllers
{
    [Authorize]
    public class FaseExecutionController : Controller
    {
        private readonly ApplicationDbContext _context;

        public FaseExecutionController(ApplicationDbContext context)
        {
            _context = context;
        }




public IActionResult Index()
{
    var groupedData = (from fase in _context.FaseExecution
                       join persen in _context.PersenTagihan
                       on fase.Kode_Project equals persen.Kode_Project into persenGroup
                       from persen in persenGroup.DefaultIfEmpty()
                       join planning in _context.FasePlanning
                       on fase.Kode_Project equals planning.Kode_Project into planningGroup
                       from planning in planningGroup.DefaultIfEmpty()  orderby fase.Id descending // Mengurutkan berdasarkan Id secara descending
                       group new { fase, persen, planning } by fase.Kode_Project into g
                       select new
                       {
                           Kode_Project = g.Key,
                           FaseExecution = g.OrderByDescending(x => x.fase.Id).FirstOrDefault().fase,
                           Persen_Nilai_Tagihan = g.Select(x => x.persen.Persen_Nilai_Tagihan).FirstOrDefault(),
                           Pekerjaan = g.Select(x => x.planning.Pekerjaan).FirstOrDefault() // Menyertakan Pekerjaan dari FasePlanning
                       }).ToList();

    // Membuat model hasil yang lebih terstruktur
    var result = groupedData.Select(g => new
    {
        g.Kode_Project,
        Start_Date = g.FaseExecution.Start_Date,  // Include Start_Date here
        End_Date_MPL = g.FaseExecution.End_Date_MPL,  // Include End_Date_MPL
        Amandemen_Waktu = g.FaseExecution.Amandemen_Waktu,  // Include Amandemen_Waktu
        End_Date = g.FaseExecution.End_Date,  // Include End_Date
        Durasi_Kontrak = g.FaseExecution.Durasi_Kontrak,  // Include Durasi_Kontrak
        Durasi_Aktual_HK = g.FaseExecution.Durasi_Aktual_HK,  // Include Durasi_Aktual_HK
        Date_LKP = g.FaseExecution.Date_LKP,  // Include Date_LKP
        Plan_Progress_Fisik = g.FaseExecution.Plan_Progress_Fisik,  // Include Plan_Progress_Fisik
        Progress_Fisik_0 = g.FaseExecution.Progress_Fisik_0,  // Include Progress_Fisik_0
        Progress_Fisik = g.FaseExecution.Progress_Fisik,  // Include Progress_Fisik
        Status_Performance = g.FaseExecution.Status_Performance,  // Include Status_Performance
        Progress_Finansial = g.Persen_Nilai_Tagihan,  // Include Progress_Finansial
        LKP_Time_Limit = g.FaseExecution.LKP_Time_Limit,  // Include LKP_Time_Limit
        Pekerjaan = g.Pekerjaan  // Include Pekerjaan from FasePlanning
    }).ToList();

    return View(result);
}




public IActionResult Detail(string kodeProject)
{
    if (string.IsNullOrEmpty(kodeProject))
    {
        return NotFound();
    }

    var projectDetails = _context.FaseExecution
        .Where(x => x.Kode_Project == kodeProject)
        .OrderBy(x => x.Start_Date) // Urutkan jika diperlukan
        .ToList();

    if (!projectDetails.Any())
    {
        return NotFound();
    }

    ViewBag.KodeProject = kodeProject; // Untuk menampilkan nama kode project di view
    return View(projectDetails);
}
 


        // GET: FaseExecution/Create
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
    // Use "Id" as the value and "Kode_Project" as the display text


    return View();
}


    
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(FaseExecution faseExecution)
        {
            if (ModelState.IsValid)
            {
                _context.Add(faseExecution);
                await _context.SaveChangesAsync();

        var tahapan = new Tahapan
        {
             Kode_Project = faseExecution.Kode_Project,
            Tanggal = DateTime.Now.Date, // Current date
            Waktu = DateTime.Now.TimeOfDay, // Current time
            Email = User.Claims.FirstOrDefault(c => c.Type == "Email")?.Value, // Get the logged-in user's email
            Tahap = "Created Fase Execution" // A description of the current step
        };

        // Simpan entitas Tahapan
        _context.Add(tahapan);
        await _context.SaveChangesAsync();

                 TempData["SuccessMessage"] = "successfully!";
                return RedirectToAction(nameof(Index));
            }


            var claimDisiplin = User.Claims.FirstOrDefault(c => c.Type == "Disiplin")?.Value;

// Query LINQ yang memfilter berdasarkan klaim 'disiplin'
var kodeProjects = _context.FasePlanning
    .Where(p => claimDisiplin == "All" || p.Disiplin == claimDisiplin)
    .Select(p => new { p.Kode_Project, p.Pekerjaan })
    .ToList();

// Serialize data to be used in JavaScript
ViewBag.KodeProjects = JsonConvert.SerializeObject(kodeProjects);

            
            return View(faseExecution);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> FormEdit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var faseExecution = await _context.FaseExecution.FindAsync(id);
            if (faseExecution == null)
            {
                return NotFound();
            }
            return View(faseExecution);
        }

        // POST: FaseExecution/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, FaseExecution faseExecution)
        {
            if (id != faseExecution.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(faseExecution);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FaseExecutionExists(faseExecution.Id))
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
            return View(faseExecution);
        }






        // GET: FaseExecution/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var faseExecution = await _context.FaseExecution
                .FirstOrDefaultAsync(m => m.Id == id);
            if (faseExecution == null)
            {
                return NotFound();
            }

            return View(faseExecution);
        }

        // POST: FaseExecution/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var faseExecution = await _context.FaseExecution.FindAsync(id);
            _context.FaseExecution.Remove(faseExecution);
            await _context.SaveChangesAsync();
             TempData["SuccessMessage"] = "successfully!";
            return RedirectToAction(nameof(Index));
        }

        private bool FaseExecutionExists(int id)
        {
            return _context.FaseExecution.Any(e => e.Id == id);
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
                       

                       var faseExecution = new FaseExecution
{
    Kode_Project = worksheet.Cells[row, 1]?.Value?.ToString(),
    Start_Date = DateTime.TryParse(worksheet.Cells[row, 2]?.Value?.ToString(), out var startDate) ? startDate : default,
    End_Date_MPL = DateTime.TryParse(worksheet.Cells[row, 3]?.Value?.ToString(), out var endDateMPL) ? endDateMPL : default,
    Amandemen_Waktu = int.TryParse(worksheet.Cells[row, 4]?.Value?.ToString(), out var amandemenWaktu) ? amandemenWaktu : 0,
    End_Date = DateTime.TryParse(worksheet.Cells[row, 5]?.Value?.ToString(), out var endDate) ? endDate : default,
    Durasi_Kontrak = int.TryParse(worksheet.Cells[row, 6]?.Value?.ToString(), out var durasiKontrak) ? durasiKontrak : 0,
    Durasi_Aktual_HK = int.TryParse(worksheet.Cells[row, 7]?.Value?.ToString(), out var durasiAktualHK) ? durasiAktualHK : 0,
Date_LKP = int.TryParse(worksheet.Cells[row, 8]?.Value?.ToString(), out var dateLKP) ? dateLKP : 0,
    Plan_Progress_Fisik = decimal.TryParse(worksheet.Cells[row, 9]?.Value?.ToString(), out var planProgressFisik) ? planProgressFisik : 0,
    Progress_Fisik_0 = decimal.TryParse(worksheet.Cells[row, 10]?.Value?.ToString(), out var progressFisik0) ? progressFisik0 : 0,
    Progress_Fisik = decimal.TryParse(worksheet.Cells[row, 11]?.Value?.ToString(), out var progressFisik) ? progressFisik : 0,
    Status_Performance = worksheet.Cells[row, 12]?.Value?.ToString(),
    Progress_Finansial = decimal.TryParse(worksheet.Cells[row, 13]?.Value?.ToString(), out var progressFinansial) ? progressFinansial : 0,
    LKP_Time_Limit = int.TryParse(worksheet.Cells[row, 14]?.Value?.ToString(), out var lkpTimeLimit) ? lkpTimeLimit : 0,
};


                        _context.FaseExecution.Add(faseExecution);
                    }

                    await _context.SaveChangesAsync();
                }
            }

            return RedirectToAction(nameof(Index));
        }





    }
}
