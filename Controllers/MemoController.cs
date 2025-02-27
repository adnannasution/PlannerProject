using Microsoft.AspNetCore.Mvc;
using Plan.Models;
using System;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Plan.Data;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Plan.Controllers
{
    public class MemoController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;

        public MemoController(ApplicationDbContext context, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            _hostEnvironment = hostEnvironment;
        }

        // Index action to display list of memo
// public IActionResult Index()
// {
//     var memo = _context.Memo
//                        .OrderByDescending(m => m.Id) // Mengurutkan berdasarkan Id secara descending
//                        .ToList();
//     return View(memo);
// }



public async Task<IActionResult> Index()
{
    var memos = await _context.Memo
        .Include(m => m.TabelDokumen) // Join dengan tabel Dokumen
        .ToListAsync();
    return View(memos);
}


public IActionResult Tampil()
{
    var memo = _context.Memo
    .Include(m => m.TabelDokumen) 
                       .OrderByDescending(m => m.Id) // Mengurutkan berdasarkan Id secara descending
                       .ToList();
    return View(memo);
}




public IActionResult TampilNew()
{
    var memo = _context.Memo
    .Include(m => m.TabelDokumen) 
                       .Where(m => m.KebutuhanKontrak == null) // Filter KebutuhanKontrak yang null
                       .OrderByDescending(m => m.Id) // Mengurutkan berdasarkan Id secara descending
                       .ToList();
    
    return View(memo);
}


// public IActionResult TampilYa()
// {
//     var memo = _context.Memo
//     .Include(m => m.TabelDokumen) 
//                        .Where(m => m.Email != null) // Filter KebutuhanKontrak yang null
//                        .OrderByDescending(m => m.Id) // Mengurutkan berdasarkan Id secara descending
//                        .ToList();
    
//     return View(memo);
// }

public IActionResult TampilYa()
{
   

    var memo = (from m in _context.Memo
                join f in _context.FasePlanning 
                on m.No_Memo_Rekomendasi equals f.No_Memo_Rekomendasi
               
                select new
                {
                    Id = m.Id,
                    IdFase = f.Id,
                    No_Memo_Rekomendasi = m.No_Memo_Rekomendasi,
                    Judul = m.Judul,
                    Disiplin = m.Disiplin,
                    Area = m.Area,
                    Direksi = m.Direksi,
                    Tanggal_Masuk_Memo = m.Tanggal_Masuk_Memo,
                    Kode_Project = f.Kode_Project,
                    KebutuhanKontrak = m.KebutuhanKontrak,
                    Komentar = m.Komentar,
                    Email = m.Email

                }).ToList();

    return View(memo);
}







public IActionResult TampilTidak()
{
    var memo = _context.Memo
    .Include(m => m.TabelDokumen) 
                       .Where(m => m.Email == null && m.KebutuhanKontrak != null) // Filter KebutuhanKontrak yang null
                       .OrderByDescending(m => m.Id) // Mengurutkan berdasarkan Id secara descending
                       .ToList();
    
    return View(memo);
}



public IActionResult TampilP()
{
    return View();
}


public IActionResult TampilPNew()
{
  string claimEmail = User.FindFirst("Email")?.Value;

    var memo = _context.Memo
    .Include(m => m.TabelDokumen) 
                       .Where(m => m.Email == claimEmail && 
                                   !_context.FasePlanning.Any(f => f.No_Memo_Rekomendasi == m.No_Memo_Rekomendasi))
                       .OrderByDescending(m => m.Id) // Urutkan berdasarkan Id
                       .ToList();

    return View(memo);
}



// public IActionResult TampilPAdd()
// {
//      string claimEmail = User.FindFirst("Email")?.Value;

//     var memo = _context.Memo
//     .Include(m => m.TabelDokumen) 
//                        .Where(m => m.Email == claimEmail && 
//                                    _context.FasePlanning.Any(f => f.No_Memo_Rekomendasi == m.No_Memo_Rekomendasi))
//                        .OrderByDescending(m => m.Id) // Urutkan berdasarkan Id
//                        .ToList();

//     return View(memo);
// }


public IActionResult TampilPAdd()
{
    string claimEmail = User.FindFirst("Email")?.Value;

    var memo = (from m in _context.Memo
                join f in _context.FasePlanning 
                on m.No_Memo_Rekomendasi equals f.No_Memo_Rekomendasi
                where m.Email == claimEmail
                select new
                {
                    Id = m.Id,
                    IdFase = f.Id,
                    No_Memo_Rekomendasi = m.No_Memo_Rekomendasi,
                    Judul = m.Judul,
                    Disiplin = m.Disiplin,
                    Area = m.Area,
                    Direksi = m.Direksi,
                    Tanggal_Masuk_Memo = m.Tanggal_Masuk_Memo,
                    Kode_Project = f.Kode_Project,
                    KebutuhanKontrak = m.KebutuhanKontrak,
                    Komentar = m.Komentar

                }).ToList();

    return View(memo);
}

 








        // Get action to create new memo
        public IActionResult Create()
        {
    ViewData["Disiplin"] = new SelectList(_context.DisiplinMaster, "Disiplin", "Disiplin");
    ViewData["Area"] = new SelectList(_context.AreaMaster, "Area", "Area");
    ViewData["Direksi"] = new SelectList(_context.DireksiMaster, "Direksi", "Direksi");

    ViewBag.Users = new SelectList(_context.User, "Email", "Email");
    return View();
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult FormEdit(int id)
        {
            var memo = _context.Memo.FirstOrDefault(m => m.Id == id);
            if (memo == null) return NotFound();

 ViewData["Disiplin"] = new SelectList(_context.DisiplinMaster, "Disiplin", "Disiplin");
     ViewData["Area"] = new SelectList(_context.AreaMaster, "Area", "Area");
    ViewData["Direksi"] = new SelectList(_context.DireksiMaster, "Direksi", "Direksi");
             ViewBag.Users = new SelectList(_context.User, "Email", "Email");




            return View(memo);
        }


        
        // [HttpPost]
        // [ValidateAntiForgeryToken]
        public IActionResult FormDisposisi(int id)
        {
var memo = _context.Memo.FirstOrDefault(m => m.Id == id);
if (memo == null) return NotFound();

var activeEmail = User.Claims.FirstOrDefault(c => c.Type == "Email")?.Value;

ViewBag.Users = new SelectList(
    _context.User.Where(u => u.Email != activeEmail && (u.Rule == "2" || u.Rule == "3" || u.Rule == "5" || u.Rule == "6")), 
    "Email",
    "Email"
); 

//return View(memo);

  return PartialView("_FormDisposisi", memo);

        }





 





        
// Post action to handle form submission for editing memo
[HttpPost]
[ValidateAntiForgeryToken]
public IActionResult Disposisi(int id, Memo model)
{
    if (ModelState.IsValid)
    {
        var memo = _context.Memo.FirstOrDefault(m => m.Id == id);
        if (memo == null) return NotFound();

        // Update other fields
  
        memo.Email = model.Email;
        memo.KebutuhanKontrak = model.KebutuhanKontrak;
        memo.Komentar = model.Komentar;

        ViewBag.Users = new SelectList(_context.User, "Email", "Email");

        _context.SaveChanges();




        // Create a new Tahapan entity
        var tahapan = new Tahapan
        {
            No_Memo_Rekomendasi = model.No_Memo_Rekomendasi,
            Tanggal = DateTime.Now.Date, // Current date
            Waktu = DateTime.Now.TimeOfDay, // Current time
            Email = User.Claims.FirstOrDefault(c => c.Type == "Email")?.Value, // Get the logged-in user's email
            Tahap = "Disposisi Memo", // A description of the current step
            Status = "Done"
        };

        // Simpan entitas Tahapan
        _context.Add(tahapan);
        _context.SaveChanges();

        TempData["SuccessMessage"] = "successfully!";
        return RedirectToAction(nameof(Tampil));
    }

    ViewBag.User = _context.User.ToList();
    return View(model);
}



                [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            var memo = _context.Memo.FirstOrDefault(m => m.Id == id);
            if (memo == null) return NotFound();

            _context.Memo.Remove(memo);
            _context.SaveChanges();
             TempData["SuccessMessage"] = "successfully!";
            return RedirectToAction(nameof(Index));
        }




public IActionResult GetFormDisposisi(int id)
{
    var memo = _context.Memo.FirstOrDefault(m => m.Id == id);
    if (memo == null)
    {
        return NotFound();
    }

      ViewBag.Users = new SelectList(_context.User, "Email", "Email");

    return PartialView("_FormDisposisiPartial", memo);
}

[HttpPost]
public IActionResult SubmitDisposisi(Memo model)
{
    if (ModelState.IsValid)
    {
        // Proses penyimpanan data
        var memo = _context.Memo.FirstOrDefault(m => m.Id == model.Id);
        if (memo != null)
        {
            memo.KebutuhanKontrak = model.KebutuhanKontrak;
            memo.Email = model.Email;
            _context.SaveChanges();
        }

        return Json(new { success = true });
    }

    return Json(new { success = false, message = "Form tidak valid." });
}



public IActionResult GetJumlahMemoBaru()
{
    int jumlahMemoBaru = _context.Memo.Count(m => m.Email == null);
    return Json(jumlahMemoBaru);
}




public IActionResult GetJumlahMemoTanpaFase()
{
    // Ambil nilai ClaimDisiplin dari user yang sedang login
    string claimEmail = User.FindFirst("Email")?.Value;



    // Hitung jumlah memo yang memiliki nilai Disiplin = ClaimDisiplin dari user login
    int jumlahMemoTanpaFase = _context.Memo
        .Where(m => m.Email == claimEmail && 
                    !_context.FasePlanning.Any(fp => fp.No_Memo_Rekomendasi == m.No_Memo_Rekomendasi))
        .Count();

    return Json(jumlahMemoTanpaFase);
}





[HttpGet]
public IActionResult GetMemoStatistics()
{
    var jumlahEmailNull = _context.Memo.Count(m => m.Email == null);
    var jumlahEmailNotNull = _context.Memo.Count(m => m.Email != null);
    var jumlahFasePlanning = _context.Memo
        .Where(m => _context.FasePlanning.Any(f => f.No_Memo_Rekomendasi == m.No_Memo_Rekomendasi))
        .Count();

    var data = new
    {
        EmailNull = jumlahEmailNull,
        EmailNotNull = jumlahEmailNotNull,
        FasePlanning = jumlahFasePlanning
    };

    return Json(data);
}




public IActionResult GetMemoByDisiplin()
{
    var memoByDisiplin = _context.Memo
        .GroupBy(m => m.Disiplin)
        .Select(g => new
        {
            Disiplin = g.Key, // Nama Disiplin
            Jumlah = g.Count() // Jumlah Memo per Disiplin
        })
        .ToList();

    return Json(memoByDisiplin);
}



public IActionResult Import()
{
    return View();
}

[HttpPost]
        public async Task<IActionResult> Import(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("File tidak ditemukan.");
            }

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial; // Wajib untuk EPPlus 5+

            using (var stream = new MemoryStream())
            {
                await file.CopyToAsync(stream);
                using (var package = new ExcelPackage(stream))
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                    if (worksheet == null)
                    {
                        return BadRequest("Sheet tidak ditemukan.");
                    }

                    int rowCount = worksheet.Dimension.Rows;
                    for (int row = 2; row <= rowCount; row++) // Mulai dari baris ke-2 (karena baris 1 adalah header)
                    {
                        string noMemoRekomendasi = worksheet.Cells[row, 1].Text; // Kolom 1 (No_Memo_Rekomendasi)
                        string tanggalMasukMemo = worksheet.Cells[row, 2].Text;
                        string noMemoKirimPaket = worksheet.Cells[row, 3].Text;
                        string dokumen = worksheet.Cells[row, 4].Text;
                        string email = worksheet.Cells[row, 5].Text;
                        string kebutuhanKontrak = worksheet.Cells[row, 6].Text;
                        string judul = worksheet.Cells[row, 7].Text;
                        string disiplin = worksheet.Cells[row, 8].Text;

                        // Cek apakah No_Memo_Rekomendasi sudah ada di database
                        var existingData = await _context.Memo
                            .FirstOrDefaultAsync(m => m.No_Memo_Rekomendasi == noMemoRekomendasi);

                        if (existingData != null)
                        {
                            // Jika sudah ada, lakukan UPDATE
                            existingData.Tanggal_Masuk_Memo = DateTime.Parse(tanggalMasukMemo);
                            existingData.No_Memo_Kirim_Paket = noMemoKirimPaket;
                            existingData.Dokumen = dokumen;
                            existingData.Email = email;
                            existingData.KebutuhanKontrak = kebutuhanKontrak;
                            existingData.Judul = judul;
                            existingData.Disiplin = disiplin;
                        }
                        else
                        {
                            // Jika belum ada, lakukan INSERT
                            var newMemo = new Memo
                            {
                                No_Memo_Rekomendasi = noMemoRekomendasi,
                                Tanggal_Masuk_Memo = DateTime.Parse(tanggalMasukMemo),
                                No_Memo_Kirim_Paket = noMemoKirimPaket,
                                Dokumen = dokumen,
                                Email = email,
                                KebutuhanKontrak = kebutuhanKontrak,
                                Judul = judul,
                                Disiplin = disiplin
                            };

                            _context.Memo.Add(newMemo);
                        }
                    }
                     TempData["SuccessMessage"] = "successfully!";

                    await _context.SaveChangesAsync();
                }
            }

             return RedirectToAction("Index");
        }





[HttpPost]
public async Task<IActionResult> Create(Memo memo, List<IFormFile> files, List<string> namaDokumen)
{
    if (ModelState.IsValid)
    {
        _context.Memo.Add(memo);
        await _context.SaveChangesAsync();

        if (files != null && files.Count > 0)
        {
            for (int i = 0; i < files.Count; i++)
            {
                var file = files[i];
                var filePath = Path.Combine("wwwroot/uploads", file.FileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                var dokumen = new TabelDokumen
                {
                    MemoId = memo.Id,
                    NamaDokumen = namaDokumen[i], // Simpan nama dokumen yang diinput user
                    NamaFile = file.FileName,
                    Path = "/uploads/" + file.FileName
                };

                _context.TabelDokumen.Add(dokumen);
            }
            await _context.SaveChangesAsync();
        }


        // Create a new Tahapan entity
        var tahapan = new Tahapan
        {
            No_Memo_Rekomendasi = memo.No_Memo_Rekomendasi,
            Tanggal = DateTime.Now.Date, // Current date
            Waktu = DateTime.Now.TimeOfDay, // Current time
            Email = User.Claims.FirstOrDefault(c => c.Type == "Email")?.Value, // Get the logged-in user's email
            Tahap = "Created Memo", // A description of the current step
            Status = "Done"
        };

        // Simpan entitas Tahapan
        _context.Add(tahapan);
        _context.SaveChanges();
        
        return RedirectToAction("Index");
    }
    return View(memo);
}











[HttpPost]
public async Task<IActionResult> Edit(Memo memo, List<IFormFile> files, List<string> namaDokumen)
{
    if (ModelState.IsValid)
    {
        _context.Memo.Update(memo);
        await _context.SaveChangesAsync();

        if (files != null && files.Count > 0)
        {
            for (int i = 0; i < files.Count; i++)
            {
                var file = files[i];
                var filePath = Path.Combine("wwwroot/uploads", file.FileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                var dokumen = new TabelDokumen
                {
                    MemoId = memo.Id,
                    NamaDokumen = namaDokumen[i], // Simpan nama dokumen yang diinput user
                    NamaFile = file.FileName,
                    Path = "/uploads/" + file.FileName
                };

                _context.TabelDokumen.Add(dokumen);
            }
            await _context.SaveChangesAsync();
        }
        return RedirectToAction("Index");
    }
    return View(memo);
}






[HttpGet]
public async Task<IActionResult> DetailByMemo(string idmemo)
{
    Console.WriteLine("ID Memo yang diterima: " + idmemo);

    ViewBag.IdMemo = idmemo;

    var fasePlanning = await _context.FasePlanning
        .Where(f => f.No_Memo_Rekomendasi == idmemo)
        .FirstOrDefaultAsync();

    if (fasePlanning == null)
    {
        ViewBag.Message = "Data tidak ditemukan.";
        return View(); // Tampilkan halaman dengan pesan
    }

    return View(fasePlanning);
}




[HttpGet]
public IActionResult GetDokumen([FromQuery] int memoId)
{
    var dokumenList = _context.TabelDokumen
        .Where(d => d.MemoId == memoId)
        .Select(d => new
        {
            NamaDokumen = d.NamaDokumen,
            Path = d.Path
        })
        .ToList();

    Console.WriteLine($"Dokumen ditemukan: {dokumenList.Count} items");
    return Json(dokumenList);
}



[HttpGet]
public IActionResult GetDokumenList(int id)
{
    var dokumenList = _context.TabelDokumen
        .Where(d => d.MemoId == id)
        .ToList();

    return PartialView("DokumenList", dokumenList);
}



    }
}
 