using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Plan.Models;

using Plan.Data;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json;





namespace Plan.Controllers
{
    [Authorize]
    public class TimelineokController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _hostingEnvironment;

        public TimelineokController(ApplicationDbContext context, IWebHostEnvironment hostingEnvironment)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
        }

        // Index: Read all data
        // public IActionResult Index()
        // {
        //     var timelineokData = _context.Timelineok.ToList();
        //     return View(timelineokData);
        // }

        public IActionResult Index()
        {
            var timelineokData = (from timeline in _context.Timelineok
                                  join fasePlanning in _context.FasePlanning
                                  on timeline.Kode_Project equals fasePlanning.Kode_Project into fasePlanningGroup
                                  from fasePlanning in fasePlanningGroup.DefaultIfEmpty()
                                  orderby timeline.Id descending // Left Join
                                  select new
                                  {
                                      timeline.Kode_Project,

                                      timeline.Status,
                                      // Mengambil data dari FasePlanning yang terkait dengan Kode_Project
                                      Pekerjaan = fasePlanning != null ? fasePlanning.Pekerjaan : null,
                                      // Anda bisa menambahkan kolom lainnya sesuai dengan kebutuhan
                                  }).ToList();

            // Mengirimkan model yang telah digabungkan ke View
            return View(timelineokData);
        }


        // Create: GET
        [HttpGet]
        public IActionResult Create()
        {




            var claimDisiplin = User.Claims.FirstOrDefault(c => c.Type == "Disiplin")?.Value;


            var kodeProjects = _context.FasePlanning
                .Where(p => claimDisiplin == "All" || p.Disiplin == claimDisiplin)
                .OrderByDescending(p => p.Id)  // Order by Id in descending order
                .Select(p => new { p.Kode_Project, p.Pekerjaan })
                .ToList();


            // Serialize data to be used in JavaScript
            ViewBag.KodeProjects = JsonConvert.SerializeObject(kodeProjects);

            // Kirim data ke View melalui ViewBag

            ViewData["PicStatusKontrak"] = new SelectList(_context.PicStatusKontrakMaster, "Pic", "Pic");
          //  ViewData["Task"] = new SelectList(_context.TaskMaster, "Task", "Task");
            ViewData["Resume"] = new SelectList(_context.ResumeStatusKontrakMaster, "Resume", "Resume");


// Ambil nilai Rule dari Claims
    var claimRule = User.Claims.FirstOrDefault(c => c.Type == "Rule")?.Value;

    // Konversi ke integer (karena biasanya string di claims)
    int rule = claimRule != null ? int.Parse(claimRule) : 0;

    // var taskList = _context.TaskMaster
    //     .Select(t => new { t.Task }) // Ambil hanya Task
    //     .ToList();

    // // Jika ClaimRule = 2 atau 7, hanya tampilkan opsi tertentu
    // if (rule == 2 || rule == 7)
    // {
    //     taskList = taskList.Where(t => t.Task == "Menyusun GTC dan RAB" || t.Task == "Membuat OE").ToList();
    // }

    // ViewData["Task"] = new SelectList(taskList, "Task", "Task");



    var taskList = _context.TaskMaster
    .Select(t => new { t.Task }) // Ambil hanya Task
    .ToList();

// Buat daftar khusus untuk rule 2 dan 7
var additionalTasks = new List<string> { "Menyusun GTC dan RAB", "Membuat OE" };

// Jika rule 2 atau 7, tambahkan opsi tambahan ke taskList
if (rule == 2 || rule == 7)
{
    // Gabungkan taskList dengan opsi tambahan, hindari duplikasi
    taskList = taskList
        .Select(t => t.Task)
        .Union(additionalTasks)
        .Distinct()
        .Select(t => new { Task = t }) // Format ulang ke bentuk list yang sesuai
        .ToList();
}
else
{
    // Hapus task tambahan dari taskList jika bukan rule 2 atau 7
    taskList = taskList
        .Where(t => !additionalTasks.Contains(t.Task)) // Hilangkan pilihan tambahan
        .ToList();
}

ViewData["Task"] = new SelectList(taskList, "Task", "Task");



 

            return View();
        }

        // Create: POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Timelineok model)
        {


            var claimDisiplin = User.Claims.FirstOrDefault(c => c.Type == "Disiplin")?.Value;

            // Query LINQ yang memfilter berdasarkan klaim 'disiplin'
            var kodeProjects = _context.FasePlanning
                .Where(p => claimDisiplin == "All" || p.Disiplin == claimDisiplin)
                .Select(p => new { p.Kode_Project, p.Pekerjaan })
                .ToList();

            // Serialize data to be used in JavaScript
            ViewBag.KodeProjects = JsonConvert.SerializeObject(kodeProjects);

            // Kirim data ke View melalui ViewBag

            ViewData["PicStatusKontrak"] = new SelectList(_context.PicStatusKontrakMaster, "Pic", "Pic");
            ViewData["Task"] = new SelectList(_context.TaskMaster, "Task", "Task");
            ViewData["Resume"] = new SelectList(_context.ResumeStatusKontrakMaster, "Resume", "Resume");


            if (model.Task == "Kirim Paket Kontrak")
            {
                var fasePlanning = _context.FasePlanning.FirstOrDefault(f => f.Id == model.Id);
                if (fasePlanning != null)
                {
                    fasePlanning.Kirim_Paket_Kontrak = model.Tanggal;
                    _context.SaveChanges();
                }
            }

            if (ModelState.IsValid)
            {

                // // Cari entri di TabelSla berdasarkan Kode_Project dan Task
                // var tabelSla = await _context.TabelSla
                //     .FirstOrDefaultAsync(t => t.Kode_Project == model.Kode_Project && t.Task == model.Task);

                // if (tabelSla != null)
                // {
                //     model.Target = tabelSla.Target; // Set kolom Target berdasarkan TabelSla

                //     // Bandingkan Tanggal dengan Target untuk menentukan Status
                //     if (model.Tanggal != default(DateTime) && tabelSla.Target != default(DateTime))
                //     {
                //         var daysDifference = (tabelSla.Target - model.Tanggal).Days;

                //         if (daysDifference >= 3)
                //         {
                //             model.Status = "Ontime";
                //         }

                //         else
                //         {
                //             model.Status = "Overdue";
                //         }
                //     }
                //     else
                //     {
                //         ModelState.AddModelError("Tanggal", "Tanggal or Target date is not valid.");
                //         return View(model);
                //     }
                // }
                // else
                // {
                //     ModelState.AddModelError("Task", "Task or Kode_Project not found in TabelSla.");
                //     return View(model);
                // }



                // Get the Target from TaskMaster based on the selected Task
                var taskMaster = await _context.TaskMaster
                    .FirstOrDefaultAsync(t => t.Task == model.Task);

                if (taskMaster != null)
                {
                    model.Target = taskMaster.Target;  // Set the Target field from TaskMaster
                }

                
                   



                    // Simpan data ke tabel timelineok
                    _context.Timelineok.Add(model);
                    await _context.SaveChangesAsync();

                    // Cari data di FasePlanning berdasarkan Kode_Project
                    var faseplanning = await _context.FasePlanning
                        .FirstOrDefaultAsync(f => f.Kode_Project == model.Kode_Project);

                    if (faseplanning != null)
                    {
                        // Perbarui Tanggal_Kirim_Paket jika Task = "Tanggal Memo"
                        if (model.Task == "Kirim Paket Kontrak")
                        {
                            faseplanning.Kirim_Paket_Kontrak = model.Tanggal; // Perbarui Tanggal_Kirim_Paket
                        }

                        _context.FasePlanning.Update(faseplanning);
                    }

                    // Simpan perubahan ke database
                    await _context.SaveChangesAsync();

                    // var tahapan = new Tahapan
                    // {
                    //     Kode_Project = model.Kode_Project,
                    //     Tanggal = DateTime.Now.Date, // Current date
                    //     Waktu = DateTime.Now.TimeOfDay, // Current time
                    //     Email = User.Claims.FirstOrDefault(c => c.Type == "Email")?.Value, // Get the logged-in user's email
                    //     Tahap = $"{model.ResumeStatusPekerjaan} - {model.Task}" // Combines them into a string


                    // };



                    // // Simpan entitas Tahapan
                    // _context.Add(tahapan);
                    // await _context.SaveChangesAsync();




var tahapan = await _context.Tahapan
.FirstOrDefaultAsync(t => t.Kode_Project == model.Kode_Project && t.Tahap == $"{model.ResumeStatusPekerjaan} - {model.Task}");

if (tahapan != null) // Jika sudah ada, lakukan update
{
    tahapan.Kode_Project = model.Kode_Project;
    tahapan.Tanggal = DateTime.Now.Date; // Perbarui tanggal
    tahapan.Waktu = DateTime.Now.TimeOfDay; // Perbarui waktu
    tahapan.Email = User.Claims.FirstOrDefault(c => c.Type == "Email")?.Value; // Perbarui email
    tahapan.Status = "Done"; // Set status default
    tahapan.Tahap =  $"{model.ResumeStatusPekerjaan} - {model.Task}"; // Set status default
    tahapan.Keterangan = model.Keterangan;
    
    _context.Update(tahapan); // Tandai untuk update
    await _context.SaveChangesAsync();
}

 

                    //----------------


                    var historyTimelineok = new HistoryTimelineok
                    {
                        Kode_Project = model.Kode_Project,
                        NamaDokumen = model.Dokumen,
                        Tanggal = DateTime.Now.Date,
                        Waktu = DateTime.Now.TimeOfDay,
                        Email = User.Claims.FirstOrDefault(c => c.Type == "Email")?.Value,
                        Aksi = "Tambah data"
                    };

                    // Simpan entitas ke database
                    _context.Add(historyTimelineok);
                    await _context.SaveChangesAsync();


                    TempData["SuccessMessage"] = "successfully!";


                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("Dokumen", "Please upload a valid PDF document.");
                }
            





            // Kirim data ke View melalui ViewBag



            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult FormEdit(int id)
        {
            var timelineok = _context.Timelineok.FirstOrDefault(t => t.Id == id);
            if (timelineok == null)
            {
                return NotFound();
            }

            // Ambil daftar Kode_Project dari FasePlanning
            var kodeProjects = _context.FasePlanning
                .Select(p => new { p.Id, p.Kode_Project })
                .ToList();

            // Kirim daftar Kode_Project ke View melalui ViewBag
            ViewBag.KodeProjects = new SelectList(kodeProjects, "Kode_Project", "Kode_Project", timelineok.Kode_Project);
            ViewData["PicStatusKontrak"] = new SelectList(_context.PicStatusKontrakMaster, "Pic", "Pic");
            ViewData["Task"] = new SelectList(_context.TaskMaster, "Task", "Task");
            ViewData["Resume"] = new SelectList(_context.ResumeStatusKontrakMaster, "Resume", "Resume");

            return View(timelineok);
        }

        // Edit: POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Timelineok model, IFormFile dokumenFile)
        {
            if (ModelState.IsValid)
            {
                var existingTimelineok = _context.Timelineok.FirstOrDefault(t => t.Id == id);
                if (existingTimelineok == null)
                {
                    return NotFound();
                }

                existingTimelineok.Kode_Project = model.Kode_Project;
                existingTimelineok.Task = model.Task;
                existingTimelineok.Tanggal = model.Tanggal;
                existingTimelineok.Pic = model.Pic;
                existingTimelineok.ResumeStatusPekerjaan = model.ResumeStatusPekerjaan;
                existingTimelineok.Keterangan = model.Keterangan;

                // Handle file upload
                if (dokumenFile != null && dokumenFile.Length > 0)
                {
                    // Delete existing file
                    if (!string.IsNullOrEmpty(existingTimelineok.Dokumen))
                    {
                        string existingFilePath = Path.Combine(_hostingEnvironment.WebRootPath, existingTimelineok.Dokumen.TrimStart('/'));
                        if (System.IO.File.Exists(existingFilePath))
                        {
                            System.IO.File.Delete(existingFilePath);
                        }
                    }

                    // Save new file
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(dokumenFile.FileName);
                    string uploadPath = Path.Combine(_hostingEnvironment.WebRootPath, "uploads");
                    string filePath = Path.Combine(uploadPath, fileName);
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await dokumenFile.CopyToAsync(fileStream);
                    }

                    existingTimelineok.Dokumen = $"/uploads/{fileName}";
                }

                _context.Timelineok.Update(existingTimelineok);
                await _context.SaveChangesAsync();







                var historyTimelineok = new HistoryTimelineok
                {
                    Kode_Project = model.Kode_Project,
                    Tanggal = DateTime.Now.Date,
                    Waktu = DateTime.Now.TimeOfDay,
                    Email = User.Claims.FirstOrDefault(c => c.Type == "Email")?.Value,
                    Aksi = "Edit Data"
                };

                // Simpan entitas ke database
                _context.Add(historyTimelineok);
                await _context.SaveChangesAsync();


                TempData["SuccessMessage"] = "successfully!";
                return RedirectToAction("Index");
            }

            return View(model);
        }





        // Delete: GET
        [HttpGet]
        public IActionResult Delete(int id)
        {
            var timelineok = _context.Timelineok.FirstOrDefault(t => t.Id == id);
            if (timelineok == null)
            {
                return NotFound();
            }

            return View(timelineok);
        }

        // Delete: POST
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var timelineok = _context.Timelineok.FirstOrDefault(t => t.Id == id);
            if (timelineok == null)
            {
                return NotFound();
            }

            // Delete file if exists
            if (!string.IsNullOrEmpty(timelineok.Dokumen))
            {
                string filePath = Path.Combine(_hostingEnvironment.WebRootPath, timelineok.Dokumen.TrimStart('/'));
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
            }




            var tahapan = await _context.Tahapan
.FirstOrDefaultAsync(t => t.Kode_Project == timelineok.Kode_Project && t.Tahap == $"{timelineok.ResumeStatusPekerjaan} - {timelineok.Task}");

if (tahapan != null) // Jika sudah ada, lakukan update
{
    tahapan.Kode_Project = timelineok.Kode_Project;
    tahapan.Tanggal = null;
    tahapan.Waktu = null;
    tahapan.Email = null;
    tahapan.Status = "Not yet"; // Set status default
    tahapan.Tahap = $"{timelineok.ResumeStatusPekerjaan} - {timelineok.Task}"; // Set status default
    tahapan.Keterangan = null;
    
    _context.Update(tahapan); // Tandai untuk update
    await _context.SaveChangesAsync();
}



            _context.Timelineok.Remove(timelineok);
            await _context.SaveChangesAsync();









            var historyTimelineok = new HistoryTimelineok
            {
                Kode_Project = timelineok.Kode_Project,
                Tanggal = DateTime.Now.Date,
                Waktu = DateTime.Now.TimeOfDay,
                Email = User.Claims.FirstOrDefault(c => c.Type == "Email")?.Value,
                Aksi = "Hapus Data"
            };

            // Simpan entitas ke database
            _context.Add(historyTimelineok);
            await _context.SaveChangesAsync();


            TempData["SuccessMessage"] = "successfully!";
            return RedirectToAction("Index");
        }

 
        public IActionResult Detail(string kode_project)
        {
            if (string.IsNullOrEmpty(kode_project))
            {
                return RedirectToAction("Index");
            }

            var data = _context.Timelineok
                               .Where(t => t.Kode_Project == kode_project)
                               .OrderBy(t => t.Id)
                               .ToList();

            var historyTimelineok = new HistoryTimelineok
            {
                Kode_Project = kode_project,
                Tanggal = DateTime.Now.Date,
                Waktu = DateTime.Now.TimeOfDay,
                Email = User.Claims.FirstOrDefault(c => c.Type == "Email")?.Value,
                Aksi = "Lihat Detail"
            };

            // Simpan entitas ke database
            _context.Add(historyTimelineok);
            _context.SaveChangesAsync();


            return View(data);
        }


        public async Task<IActionResult> ViewSla(string kodeProject)
        {
            var slaRecords = await _context.TabelSla
                .Where(t => t.Kode_Project == kodeProject)
                .ToListAsync();

            if (slaRecords == null || !slaRecords.Any())
            {
                return NotFound("No SLA records found for this project.");
            }

            // Set Kode_Project in ViewData for display in the view
            ViewData["KodeProject"] = kodeProject;

            return View(slaRecords);

            //return PartialView("ViewSLA", slaRecords);
        }




        public async Task<IActionResult> EditSla(int id)
        {
            // Retrieve the SLA record by its ID
            var slaRecord = await _context.TabelSla.FindAsync(id);
            if (slaRecord == null)
            {
                return NotFound("SLA record not found.");
            }

            // Display the edit form with the SLA record details
            return View(slaRecord);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditSla(int id, TabelSla updatedSla)
        {
            if (id != updatedSla.Id)
            {
                return BadRequest("SLA record ID mismatch.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(updatedSla);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(ViewSla), new { kodeProject = updatedSla.Kode_Project });
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.TabelSla.Any(e => e.Id == id))
                    {
                        return NotFound("SLA record not found.");
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            return View(updatedSla);
        }







        [HttpPost]
        public async Task<IActionResult> RekamAktivitas(string kodeProject, string namaDokumen)
        {
            var history = new HistoryTimelineok
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




        public async Task<IActionResult> TampilHistoryTimelineok(string kodeProject)
        {
            var histori = await _context.HistoryTimelineok
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





    //      [HttpPost]
    // public IActionResult UploadDokumen(IFormFile File, string NamaDokumen, string Kode_Project, string Task)
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

    //     var dokumen = new TabelDokumenTimelineok
    //     {
    //         Kode_Project = Kode_Project,
    //         Task = Task,
    //         NamaDokumen = NamaDokumen,
    //         NamaFile = File.FileName,
    //         Path = "/uploads/" + File.FileName
    //     };

    //     _context.TabelDokumenTimelineok.Add(dokumen);
    //     _context.SaveChanges();

    //     return Ok();
    // }




[HttpPost]
public IActionResult UploadDokumen(IFormFile File, string NamaDokumen, string Kode_Project, string Task)
{
    if (File == null || string.IsNullOrEmpty(NamaDokumen) || string.IsNullOrEmpty(Kode_Project))
    {
        return BadRequest("Semua field harus diisi!");
    }

    // Ekstrak ekstensi file (misalnya .pdf, .jpg, .docx)
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

    // Simpan ke database
    var dokumen = new TabelDokumenTimelineok
    {
        Kode_Project = Kode_Project,
        Task = Task,
        NamaDokumen = NamaDokumen,
        NamaFile = uniqueFileName,  // Simpan nama file baru
        Path = "/uploads/" + uniqueFileName
    };

    _context.TabelDokumenTimelineok.Add(dokumen);
    _context.SaveChanges();

    return Ok(new { message = "File berhasil diunggah!", fileName = uniqueFileName });
}


 
[HttpGet]
public IActionResult GetDokumenList(string kodeProject, string Task)
{
    var dokumenList = _context.TabelDokumenTimelineok
        .Where(d => d.Kode_Project == kodeProject && d.Task == Task)
        .Select(d => new 
        {
            d.Id,
            d.NamaDokumen,
            d.Path
        })
        .ToList();

    return Json(dokumenList);
}

 public ActionResult TampilDokumenList(string kode_project, string task)
    {
        if (string.IsNullOrEmpty(kode_project) || string.IsNullOrEmpty(task))
        {
            return Content("<p class='text-danger'>Parameter tidak valid.</p>");
        }

        var dokumenList = _context.TabelDokumenTimelineok
            .Where(d => d.Kode_Project == kode_project && d.Task == task)
            .ToList();

        ViewBag.KodeProject = kode_project;
        ViewBag.Task = task;
        return PartialView("TampilDokumenList", dokumenList);
    }


    [HttpDelete]
    public IActionResult DeleteDokumen(int id)
    {
        var dokumen = _context.TabelDokumenTimelineok.Find(id);
        if (dokumen == null) return NotFound();

        var filePath = Path.Combine("wwwroot/uploads", dokumen.NamaFile);
        if (System.IO.File.Exists(filePath))
        {
            System.IO.File.Delete(filePath);
        }

        _context.TabelDokumenTimelineok.Remove(dokumen);
        _context.SaveChanges();

        return Ok();
    }


[HttpGet]
public IActionResult GetDokumen(string kodeProject)
{
    var dokumenList = _context.TabelDokumenTimelineok
        .Where(d => d.Kode_Project == kodeProject)
        .Select(d => new 
        {
            d.NamaDokumen,
            d.Path
        })
        .ToList();

    return Json(dokumenList);
}


    }
}
