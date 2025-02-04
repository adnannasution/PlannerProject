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
public IActionResult Index()
{
    var memo = _context.Memo
                       .OrderByDescending(m => m.Id) // Mengurutkan berdasarkan Id secara descending
                       .ToList();
    return View(memo);
}


public IActionResult Tampil()
{
    var memo = _context.Memo
                       .OrderByDescending(m => m.Id) // Mengurutkan berdasarkan Id secara descending
                       .ToList();
    return View(memo);
}




public IActionResult TampilNew()
{
    var memo = _context.Memo
                       .Where(m => m.KebutuhanKontrak == null) // Filter KebutuhanKontrak yang null
                       .OrderByDescending(m => m.Id) // Mengurutkan berdasarkan Id secara descending
                       .ToList();
    
    return View(memo);
}


public IActionResult TampilYa()
{
    var memo = _context.Memo
                       .Where(m => m.KebutuhanKontrak == "Ya") // Filter KebutuhanKontrak yang null
                       .OrderByDescending(m => m.Id) // Mengurutkan berdasarkan Id secara descending
                       .ToList();
    
    return View(memo);
}


public IActionResult TampilTidak()
{
    var memo = _context.Memo
                       .Where(m => m.KebutuhanKontrak == "Tidak") // Filter KebutuhanKontrak yang null
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
                       .Where(m => m.Email == claimEmail && 
                                   !_context.FasePlanning.Any(f => f.No_Memo_Rekomendasi == m.No_Memo_Rekomendasi))
                       .OrderByDescending(m => m.Id) // Urutkan berdasarkan Id
                       .ToList();

    return View(memo);
}



public IActionResult TampilPAdd()
{
     string claimEmail = User.FindFirst("Email")?.Value;

    var memo = _context.Memo
                       .Where(m => m.Email == claimEmail && 
                                   _context.FasePlanning.Any(f => f.No_Memo_Rekomendasi == m.No_Memo_Rekomendasi))
                       .OrderByDescending(m => m.Id) // Urutkan berdasarkan Id
                       .ToList();

    return View(memo);
}









        // Get action to create new memo
        public IActionResult Create()
        {
    ViewData["Disiplin"] = new SelectList(_context.DisiplinMaster, "Disiplin", "Disiplin");
    ViewBag.Users = new SelectList(_context.User, "Email", "Email");
    return View();
        }

        // Post action to handle the form submission for creating or updating memo
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Memo model, IFormFile dokumen)
        {
            if (ModelState.IsValid)
            {
                // Handle file upload
                if (dokumen != null && dokumen.ContentType == "application/pdf")
                {
                    string fileName = Guid.NewGuid().ToString() + ".pdf";
                    string filePath = Path.Combine(_hostEnvironment.WebRootPath, "uploads", fileName);
                    
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        dokumen.CopyTo(fileStream);
                    }

                    model.Dokumen = fileName;
                }
                else
                {
                    ModelState.AddModelError("Dokumen", "Please upload a PDF file.");
                    ViewBag.User = _context.User.ToList();
                    return View(model);
                }

                // Save the memo to the database
                _context.Memo.Add(model);
                _context.SaveChanges();




                // Create a new Tahapan entity
        var tahapan = new Tahapan
        {
            No_Memo_Rekomendasi = model.No_Memo_Rekomendasi,
            Tanggal = DateTime.Now.Date, // Current date
            Waktu = DateTime.Now.TimeOfDay, // Current time
            Email = User.Claims.FirstOrDefault(c => c.Type == "Email")?.Value, // Get the logged-in user's email
            Tahap = "Created Memo" // A description of the current step
        };

        // Simpan entitas Tahapan
        _context.Add(tahapan);
        _context.SaveChanges();



    TempData["SuccessMessage"] = "successfully!";
                  ViewBag.Users = new SelectList(_context.User, "Email", "Email");
                      ViewData["Disiplin"] = new SelectList(_context.DisiplinMaster, "Disiplin", "Disiplin");
 
                return RedirectToAction(nameof(Index));
            }
            
            ViewBag.User = _context.User.ToList();
         
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult FormEdit(int id)
        {
            var memo = _context.Memo.FirstOrDefault(m => m.Id == id);
            if (memo == null) return NotFound();

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



//         public IActionResult Timeline(string kodeProject)
// {
//     var model = _context.Tahapan.Where(t => t.Kode_Project == kodeProject).ToList();
//     return PartialView("_TimelinePartial", model);
// }

        // Post action to handle form submission for editing memo
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Memo model, IFormFile dokumen)
        {
            if (ModelState.IsValid)
            {
                var memo = _context.Memo.FirstOrDefault(m => m.Id == id);
                if (memo == null) return NotFound();

                // Handle file upload for updating document
                if (dokumen != null && dokumen.ContentType == "application/pdf")
                {
                    string fileName = Guid.NewGuid().ToString() + ".pdf";
                    string filePath = Path.Combine(_hostEnvironment.WebRootPath, "uploads", fileName);
                    
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        dokumen.CopyTo(fileStream);
                    }

                    memo.Dokumen = fileName;
                }

                // Update other fields
                memo.No_Memo_Rekomendasi = model.No_Memo_Rekomendasi;
                memo.Judul = model.Judul;
                memo.Tanggal_Masuk_Memo = model.Tanggal_Masuk_Memo;
                memo.No_Memo_Kirim_Paket = model.No_Memo_Kirim_Paket;
                memo.Email = model.Email;
                memo.KebutuhanKontrak = model.KebutuhanKontrak;

                  ViewBag.Users = new SelectList(_context.User, "Email", "Email");

                _context.SaveChanges();
                 TempData["SuccessMessage"] = "successfully!";
                return RedirectToAction(nameof(Index));
            }

            ViewBag.User = _context.User.ToList();
            return View(model);
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

        ViewBag.Users = new SelectList(_context.User, "Email", "Email");

        _context.SaveChanges();




        // Create a new Tahapan entity
        var tahapan = new Tahapan
        {
            No_Memo_Rekomendasi = model.No_Memo_Rekomendasi,
            Tanggal = DateTime.Now.Date, // Current date
            Waktu = DateTime.Now.TimeOfDay, // Current time
            Email = User.Claims.FirstOrDefault(c => c.Type == "Email")?.Value, // Get the logged-in user's email
            Tahap = "Disposisi Memo" // A description of the current step
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



    }
}
 