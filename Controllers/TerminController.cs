using Microsoft.AspNetCore.Mvc;
using Plan.Models;  // Gantilah dengan namespace model Anda
using Plan.Data;    // Gantilah dengan namespace DbContext Anda
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json;




namespace Plan.Controllers
{
     [Authorize]
public class TerminController : Controller
{
    private readonly ApplicationDbContext _context;

    public TerminController(ApplicationDbContext context)
    {
        _context = context;
    }





public IActionResult Index()
{
    var data = from t in _context.Termin
               join ft in _context.FaseTender
               on t.Kode_Project equals ft.Kode_Project
               join fp in _context.FasePlanning
               on t.Kode_Project equals fp.Kode_Project into fpGroup
               from fp in fpGroup.DefaultIfEmpty() orderby t.Id descending // Left join to FasePlanning
               
               select new TerminDetailViewModel
               {
                   Id = t.Id,
                   Kode_Project = t.Kode_Project,
                   JenisTermin = t.JenisTermin,
                   Evaluasi_Planner = t.Evaluasi_Planner,
                   Nilai_Plan = t.Nilai_Plan,
                   Nilai_Tagihan = t.Nilai_Tagihan,
                   No_WO_Tagihan = t.No_WO_Tagihan,
                   Prosentase_Tagihan = t.Prosentase_Tagihan,
                   SA = t.SA,
                   Periode = t.Periode,
                   Dokumen = t.Dokumen,
                   Nilai_Purchasing_Order = (decimal?)ft.Nilai_Purchasing_Order, // Cast to nullable decimal
                   Pekerjaan = fp != null ? fp.Pekerjaan : null // Include Pekerjaan from FasePlanning
               };

    return View(data.ToList());
}



    // CREATE
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

        return View();
    }

     // Create: POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Termin model, IFormFile dokumenFile)
        {
            if (ModelState.IsValid)
            {
                if (dokumenFile != null && dokumenFile.Length > 0)
                {
                    // Generate unique file name
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(dokumenFile.FileName);

                    // Define upload path
                  string uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");




                    // Ensure the directory exists
                    if (!Directory.Exists(uploadPath))
                    {
                        Directory.CreateDirectory(uploadPath);
                    }

                    // Save the file
                    string filePath = Path.Combine(uploadPath, fileName);
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await dokumenFile.CopyToAsync(fileStream);
                    }

                    // Save the file path to the model
                    model.Dokumen = $"/uploads/{fileName}";

                    // Save to database
    
                }
                else
                {
                    
                    ModelState.AddModelError("Dokumen", "Please upload a valid PDF document.");
                }


                _context.Termin.Add(model);
                    await _context.SaveChangesAsync();



                    // Create a new Tahapan entity
        var tahapan = new Tahapan
        {
            Kode_Project = model.Kode_Project,
            Tanggal = DateTime.Now.Date, // Current date
            Waktu = DateTime.Now.TimeOfDay, // Current time
            Email = User.Claims.FirstOrDefault(c => c.Type == "Email")?.Value, // Get the logged-in user's email
            Tahap = $"Termin ke {model.JenisTermin}" // A description of the current step
        };

        // Simpan entitas Tahapan
        _context.Add(tahapan);
        await _context.SaveChangesAsync();


 TempData["SuccessMessage"] = "successfully!";
                   


 // Hitung nilai persentase tagihan berdasarkan input
        var totalNilaiTagihan = _context.Termin
            .Where(t => t.Kode_Project == model.Kode_Project)
            .Sum(t => t.Nilai_Tagihan);

        var nilaiPurchasingOrderTerakhir = _context.FaseTender?
            .Where(fp => fp.Kode_Project == model.Kode_Project)
            .OrderByDescending(fp => fp.Id)
            .Select(fp => fp.Nilai_Purchasing_Order)
            .FirstOrDefault() ?? 0;

        // Hindari pembagian oleh nol
        var persentaseNilaiTagihan = nilaiPurchasingOrderTerakhir > 0
            ? (totalNilaiTagihan / nilaiPurchasingOrderTerakhir) * 100
            : 0;

        // Cek apakah Kode_Project sudah ada di tabel PersenTagihan
        var existingPersenTagihan = _context.PersenTagihan
            .FirstOrDefault(pt => pt.Kode_Project == model.Kode_Project);

        if (existingPersenTagihan != null)
        {
            // Update data jika Kode_Project sudah ada
            existingPersenTagihan.Persen_Nilai_Tagihan = persentaseNilaiTagihan;
            _context.PersenTagihan.Update(existingPersenTagihan);
        }
        else
        {
            // Tambahkan data baru jika Kode_Project belum ada
            var newPersenTagihan = new PersenTagihan
            {
                Kode_Project = model.Kode_Project,
                Persen_Nilai_Tagihan = persentaseNilaiTagihan
            };
            _context.PersenTagihan.Add(newPersenTagihan);
        }




            }

            //return View(model);
             return RedirectToAction("Index");
        }


    // READ (Index)
    // public async Task<IActionResult> Index()
    // {
    //     return View(await _context.Termin.ToListAsync());
    // }

                  [HttpPost]
        [ValidateAntiForgeryToken]
    public async Task<IActionResult> FormEdit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var termin = await _context.Termin.FindAsync(id);
        if (termin == null)
        {
            return NotFound();
        }
        return View(termin);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,JenisTermin,Evaluasi_Planner,No_WO_Tagihan,Prosentase_Tagihan,SA,Periode,Tagihan,Evaluasi_Planner_2,No_WO_Tagihan_2,Prosentase_Tagihan_2")] Termin termin)
   
    {
        if (id != termin.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(termin);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TerminExists(termin.Id))
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
        return View(termin);
    }

    // DELETE
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var termin = await _context.Termin
            .FirstOrDefaultAsync(m => m.Id == id);
        if (termin == null)
        {
            return NotFound();
        }

        return View(termin);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var termin = await _context.Termin.FindAsync(id);
        _context.Termin.Remove(termin);
        await _context.SaveChangesAsync();
         TempData["SuccessMessage"] = "successfully!";
        return RedirectToAction(nameof(Index));
    }

    private bool TerminExists(int id)
    {
        return _context.Termin.Any(e => e.Id == id);
    }




public IActionResult Detail(string kode_project)
{
    if (string.IsNullOrEmpty(kode_project))
    {
        return RedirectToAction("Index");
    }

    var data = from t in _context.Termin
               join ft in _context.FaseTender
               on t.Kode_Project equals ft.Kode_Project
               where t.Kode_Project == kode_project
               select new TerminDetailViewModel
               {
                   Id = t.Id,
                   Kode_Project = t.Kode_Project,
                   JenisTermin = t.JenisTermin,
                   Evaluasi_Planner = t.Evaluasi_Planner,
                   Nilai_Plan = t.Nilai_Plan,
                   Nilai_Tagihan = t.Nilai_Tagihan,
                
                   No_WO_Tagihan = t.No_WO_Tagihan,
                   Prosentase_Tagihan = t.Prosentase_Tagihan,
                   SA = t.SA,
                   Periode = t.Periode,
                   Dokumen = t.Dokumen,
                   // Ensure Nilai_Purchasing_Order is cast to decimal
                   Nilai_Purchasing_Order = (decimal?)ft.Nilai_Purchasing_Order // Cast to nullable decimal
               };

    return View(data.ToList());
}

 
}

}
