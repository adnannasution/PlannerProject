using Microsoft.AspNetCore.Mvc;
using Plan.Models;  // Gantilah dengan namespace model Anda
using Plan.Data;    // Gantilah dengan namespace DbContext Anda
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json;
using System.Threading.Tasks;




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



var userEmail = User.Claims.FirstOrDefault(c => c.Type == "Email")?.Value;

var memos = _context.Memo
    .Where(m => m.Email == userEmail)
    .Select(m => new 
    { 
        m.No_Memo_Rekomendasi,
        m.Judul,
        DisplayText = m.No_Memo_Rekomendasi + " - " + m.Judul
    })
    .ToList();

ViewBag.NoMemoRekomendasi = new SelectList(memos, "No_Memo_Rekomendasi", "DisplayText");


var users = _context.User

    .Select(u => new 
    { 
        u.Id, 
        u.Nama,
        u.Email
    })
    .ToList();

ViewBag.Evaluasi_Planner = new SelectList(users, "Email", "Email");


        return View();
    }




[HttpGet]
public IActionResult GetTanggalMasukMemo(string noMemo)
{
    var tanggalMasuk = _context.Memo
        .Where(m => m.No_Memo_Rekomendasi == noMemo)
        .Select(m => m.Tanggal_Masuk_Memo)
        .FirstOrDefault();

        return Json(new { tanggalMasuk = tanggalMasuk.ToString("yyyy-MM-dd") });
    
}




[HttpGet]
public IActionResult GetPlannerByDisiplin(string noMemo)
{
    var disiplin = _context.Memo
        .Where(m => m.No_Memo_Rekomendasi == noMemo)
        .Select(m => m.Disiplin)
        .FirstOrDefault();

    if (disiplin == null)
    {
        return Json(new { planners = new List<object>() });
    }

    var planners = _context.User
        .Where(u => u.Jabatan == "Planner" && u.Disiplin == disiplin)
        .Select(u => u.Email)  // Ambil Nama Planner saja
        .ToList();

    return Json(new { planners });
}




     // Create: POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Termin model)
        {
          
               

                _context.Termin.Add(model);
                    await _context.SaveChangesAsync();



        //             // Create a new Tahapan entity
        // var tahapan = new Tahapan
        // {
        //     Kode_Project = model.Kode_Project,
        //     Tanggal = DateTime.Now.Date, // Current date
        //     Waktu = DateTime.Now.TimeOfDay, // Current time
        //     Email = User.Claims.FirstOrDefault(c => c.Type == "Email")?.Value, // Get the logged-in user's email
        //     Tahap = $"Termin ke {model.JenisTermin}", // A description of the current step
        //     Status= "Done" // Set status default
        
        // };

        // // Simpan entitas Tahapan
        // _context.Add(tahapan);
        // await _context.SaveChangesAsync();




        
var tahapan = await _context.Tahapan
.FirstOrDefaultAsync(t => t.Kode_Project == model.Kode_Project && t.Tahap == $"Termin ke {model.JenisTermin}");

if (tahapan != null) // Jika sudah ada, lakukan update
{
  
    tahapan.Tanggal = DateTime.Now.Date; // Perbarui tanggal
    tahapan.Waktu = DateTime.Now.TimeOfDay; // Perbarui waktu
    tahapan.Email = User.Claims.FirstOrDefault(c => c.Type == "Email")?.Value; // Perbarui email
    tahapan.Status = "Done"; // Set status default
    
    _context.Update(tahapan); // Tandai untuk update
    await _context.SaveChangesAsync();
}


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




            

 

            var historyTermin  = new HistoryTermin
            {
                Kode_Project = model.Kode_Project,
                Tanggal = DateTime.Now.Date,
                Waktu = DateTime.Now.TimeOfDay,
                Email = User.Claims.FirstOrDefault(c => c.Type == "Email")?.Value,
                Aksi = "Add data"
            };

            // Simpan entitas ke database
            _context.Add(historyTermin);
           await _context.SaveChangesAsync();


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


var claimDisiplin = User.Claims.FirstOrDefault(c => c.Type == "Disiplin")?.Value;

// Query LINQ yang memfilter berdasarkan klaim 'disiplin'
var kodeProjects = _context.FasePlanning
    .Where(p => claimDisiplin == "All" || p.Disiplin == claimDisiplin)
    .OrderByDescending(p => p.Id)  // Order by Id in descending order
    .Select(p => new { p.Kode_Project, p.Pekerjaan })
    .ToList();

// Serialize data to be used in JavaScript
ViewBag.KodeProjects = JsonConvert.SerializeObject(kodeProjects);



var userEmail = User.Claims.FirstOrDefault(c => c.Type == "Email")?.Value;

var memos = _context.Memo
    .Where(m => m.Email == userEmail)
    .Select(m => new 
    { 
        m.No_Memo_Rekomendasi
    })
    .ToList();

ViewBag.NoMemoRekomendasi = new SelectList(memos, "No_Memo_Rekomendasi", "No_Memo_Rekomendasi");


        var users = _context.User

    .Select(u => new 
    { 
        u.Id, 
        u.Nama,
        u.Email
    })
    .ToList();

ViewBag.Evaluasi_Planner = new SelectList(users, "Email", "Email");


        return View(termin);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Termin termin)
   
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



            var historyTermin  = new HistoryTermin
            {
                Kode_Project = termin.Kode_Project,
                Tanggal = DateTime.Now.Date,
                Waktu = DateTime.Now.TimeOfDay,
                Email = User.Claims.FirstOrDefault(c => c.Type == "Email")?.Value,
                Aksi = "Edit data"
            };

            // Simpan entitas ke database
            _context.Add(historyTermin);
           await _context.SaveChangesAsync();

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




var tahapan = await _context.Tahapan
.FirstOrDefaultAsync(t => t.Kode_Project == termin.Kode_Project && t.Tahap == $"Termin ke {termin.JenisTermin}");

if (tahapan != null) // Jika sudah ada, lakukan update
{
  
    tahapan.Tanggal = null; // Perbarui tanggal
    tahapan.Waktu = null; // Perbarui waktu
    tahapan.Email = null; // Perbarui email
    tahapan.Status = "Not yet"; // Set status default
    
    _context.Update(tahapan); // Tandai untuk update
    await _context.SaveChangesAsync();
}



        _context.Termin.Remove(termin);
        await _context.SaveChangesAsync();



                    var historyTermin  = new HistoryTermin
            {
                Kode_Project = termin.Kode_Project,
                Tanggal = DateTime.Now.Date,
                Waktu = DateTime.Now.TimeOfDay,
                Email = User.Claims.FirstOrDefault(c => c.Type == "Email")?.Value,
                Aksi = "Hapus data"
            };

            // Simpan entitas ke database
            _context.Add(historyTermin);
           await _context.SaveChangesAsync();


         TempData["SuccessMessage"] = "successfully!";
        return RedirectToAction(nameof(Index));
    }

    private bool TerminExists(int id)
    {
        return _context.Termin.Any(e => e.Id == id);
    }




public async Task<IActionResult> Detail(string kode_project)
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



            var historyTermin  = new HistoryTermin
            {
                Kode_Project = kode_project,
                Tanggal = DateTime.Now.Date,
                Waktu = DateTime.Now.TimeOfDay,
                Email = User.Claims.FirstOrDefault(c => c.Type == "Email")?.Value,
                Aksi = "Lihat Detail"
            };

            // Simpan entitas ke database
            _context.Add(historyTermin);
            await _context.SaveChangesAsync();


    return View(await data.ToListAsync());
}



 


    // [HttpPost]
    // public IActionResult UploadDokumen(IFormFile File, string NamaDokumen, string Kode_Project, int JenisTermin)
    // {
    //     if (File == null || string.IsNullOrEmpty(NamaDokumen) || string.IsNullOrEmpty(Kode_Project))
    //     {
    //         return BadRequest("Semua field harus diisi!");
    //     }

    //     var filePath = Path.Combine("wwwroot/uploads", File.FileName);
    //     using (var stream = new FileStream(filePath, FileMode.Create))
    //     {
    //         File.CopyTo(stream);
    //     }

    //     var dokumen = new TabelDokumenTermin
    //     {
    //         Kode_Project = Kode_Project,
    //         JenisTermin = JenisTermin,
    //         NamaDokumen = NamaDokumen,
    //         NamaFile = File.FileName,
    //         Path = "/uploads/" + File.FileName
    //     };

    //     _context.TabelDokumenTermin.Add(dokumen);
    //     _context.SaveChanges();

    //     return Ok();
    // }




    [HttpPost]
public IActionResult UploadDokumen(IFormFile File, string NamaDokumen, string Kode_Project, int JenisTermin)
{
    if (File == null || string.IsNullOrEmpty(NamaDokumen) || string.IsNullOrEmpty(Kode_Project))
    {
        return BadRequest("Semua field harus diisi!");
    }

    // Ambil ekstensi file (misalnya .pdf, .jpg, .docx)
    var fileExtension = Path.GetExtension(File.FileName);

    // Buat nama file unik dengan GUID
    var uniqueFileName = $"{Guid.NewGuid()}{fileExtension}";

    // Tentukan path penyimpanan
    var filePath = Path.Combine("wwwroot/uploads", uniqueFileName);

    // Simpan file ke folder tujuan
    using (var stream = new FileStream(filePath, FileMode.Create))
    {
        File.CopyTo(stream);
    }

    // Simpan informasi file ke database
    var dokumen = new TabelDokumenTermin
    {
        Kode_Project = Kode_Project,
        JenisTermin = JenisTermin,
        NamaDokumen = NamaDokumen,
        NamaFile = uniqueFileName,  // Simpan nama file yang sudah diubah
        Path = "/uploads/" + uniqueFileName
    };

    _context.TabelDokumenTermin.Add(dokumen);
    _context.SaveChanges();

    return Ok(new { message = "File berhasil diunggah!", fileName = uniqueFileName });
}


[HttpGet]
public IActionResult GetDokumenList(string kodeProject, int JenisTermin)
{
    var dokumenList = _context.TabelDokumenTermin
        .Where(d => d.Kode_Project == kodeProject && d.JenisTermin == JenisTermin)
        .Select(d => new 
        {
            d.Id,
            d.NamaDokumen,
            d.Path
        })
        .ToList();

    return Json(dokumenList);
}


    [HttpDelete]
    public IActionResult DeleteDokumen(int id)
    {
        var dokumen = _context.TabelDokumenTermin.Find(id);
        if (dokumen == null) return NotFound();

        var filePath = Path.Combine("wwwroot/uploads", dokumen.NamaFile);
        if (System.IO.File.Exists(filePath))
        {
            System.IO.File.Delete(filePath);
        }

        _context.TabelDokumenTermin.Remove(dokumen);
        _context.SaveChanges();

        return Ok();
    }


[HttpGet]
public IActionResult GetDokumen(string kodeProject)
{
    var dokumenList = _context.TabelDokumenTermin
        .Where(d => d.Kode_Project == kodeProject)
        .Select(d => new 
        {
            d.NamaDokumen,
            d.Path
        })
        .ToList();

    return Json(dokumenList);
}




        [HttpPost]
        public async Task<IActionResult> RekamAktivitas(string kodeProject, string namaDokumen)
        {
            var history = new HistoryTermin
            {
                Kode_Project = kodeProject,
                NamaDokumen = namaDokumen,
                Aksi = "Lihat Dokumen",
                Tanggal = DateTime.Now.Date,
                Waktu = DateTime.Now.TimeOfDay,
                Email = User.Claims.FirstOrDefault(c => c.Type == "Email")?.Value
            };

            _context.Add(history);
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Aktivitas berhasil direkam." });
        }


        



        public async Task<IActionResult> TampilHistoryTermin(string kodeProject)
        {
            var histori = await _context.HistoryTermin
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

 

  public ActionResult TampilDokumenList(string kode_project, int JenisTermin)
    {
      

        var dokumenList = _context.TabelDokumenTermin
            .Where(d => d.Kode_Project == kode_project && d.JenisTermin == JenisTermin)
            .ToList();

        ViewBag.KodeProject = kode_project;
        ViewBag.JenisTermin = JenisTermin;
        return PartialView("TampilDokumenList", dokumenList);
    }

 
}

}
