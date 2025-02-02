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
                          from fasePlanning in fasePlanningGroup.DefaultIfEmpty() orderby timeline.Id descending // Left Join
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

// Query LINQ yang memfilter berdasarkan klaim 'disiplin'
// var kodeProjects = _context.FasePlanning
//     .Where(p => claimDisiplin == "All" || p.Disiplin == claimDisiplin)
//     .Select(p => new { p.Kode_Project, p.Pekerjaan })
//     .ToList();


var kodeProjects = _context.FasePlanning
    .Where(p => claimDisiplin == "All" || p.Disiplin == claimDisiplin)
    .OrderByDescending(p => p.Id)  // Order by Id in descending order
    .Select(p => new { p.Kode_Project, p.Pekerjaan })
    .ToList();


// Serialize data to be used in JavaScript
ViewBag.KodeProjects = JsonConvert.SerializeObject(kodeProjects);

            // Kirim data ke View melalui ViewBag
    
            ViewData["PicStatusKontrak"] = new SelectList(_context.PicStatusKontrakMaster, "Pic", "Pic");
            ViewData["Task"] = new SelectList(_context.TaskMaster, "Task", "Task");
            ViewData["Resume"] = new SelectList(_context.ResumeStatusKontrakMaster, "Resume", "Resume");

            return View();
        }

        // Create: POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Timelineok model, IFormFile dokumenFile)
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


            if (ModelState.IsValid)
            {

                  // Cari entri di TabelSla berdasarkan Kode_Project dan Task
        var tabelSla = await _context.TabelSla
            .FirstOrDefaultAsync(t => t.Kode_Project == model.Kode_Project && t.Task == model.Task);

        if (tabelSla != null)
        {
            model.Target = tabelSla.Target; // Set kolom Target berdasarkan TabelSla

            // Bandingkan Tanggal dengan Target untuk menentukan Status
            if (model.Tanggal != default(DateTime) && tabelSla.Target != default(DateTime))
            {
                var daysDifference = (tabelSla.Target - model.Tanggal).Days;

                if (daysDifference >= 3)
                {
                    model.Status = "On Progress";
                }
                else if (daysDifference >= 0 && daysDifference <= 2)
                {
                    model.Status = "Warning";
                }
                else
                {
                    model.Status = "Overdue";
                }
            }
            else
            {
                ModelState.AddModelError("Tanggal", "Tanggal or Target date is not valid.");
                return View(model);
            }
        }
        else
        {
            ModelState.AddModelError("Task", "Task or Kode_Project not found in TabelSla.");
            return View(model);
        }



                 // Get the Target from TaskMaster based on the selected Task
        var taskMaster = await _context.TaskMaster
            .FirstOrDefaultAsync(t => t.Task == model.Task);

        if (taskMaster != null)
        {
            model.Target = taskMaster.Target;  // Set the Target field from TaskMaster
        }
        
                if (dokumenFile != null && dokumenFile.Length > 0)
                {
                    // Generate unique file name
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(dokumenFile.FileName);

                    // Define upload path
                    string uploadPath = Path.Combine(_hostingEnvironment.WebRootPath, "uploads");

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



                    // Simpan data ke tabel timelineok
                    _context.Timelineok.Add(model);
                    await _context.SaveChangesAsync();

                    // Cari data di FasePlanning berdasarkan Kode_Project
                    var faseplanning = await _context.FasePlanning
                        .FirstOrDefaultAsync(f => f.Kode_Project == model.Kode_Project);

                    if (faseplanning != null)
                    {
                        // Perbarui Tanggal_Kirim_Paket jika Task = "Tanggal Memo"
                        if (model.Task == "Mengirim dokumen DP3 Paket via P Office")
                        {
                            faseplanning.Tanggal_Kirim_Paket = model.Tanggal; // Perbarui Tanggal_Kirim_Paket
                        }

                        if (model.Task == "Memo turun")
                        {
                            faseplanning.Tanggal_Masuk_Memo = model.Tanggal; // Perbarui Tanggal_Kirim_Paket
                        }

                        // Selalu perbarui Tanggal_Update
                        faseplanning.Tanggal_Update = model.Tanggal;

                        // Tandai perubahan pada faseplanning
                        _context.FasePlanning.Update(faseplanning);
                    }

                    // Simpan perubahan ke database
                    await _context.SaveChangesAsync();

        var tahapan = new Tahapan
        {
            Kode_Project = model.Kode_Project,
            Tanggal = DateTime.Now.Date, // Current date
            Waktu = DateTime.Now.TimeOfDay, // Current time
            Email = User.Claims.FirstOrDefault(c => c.Type == "Email")?.Value, // Get the logged-in user's email
            Tahap = $"{model.ResumeStatusPekerjaan} - {model.Task}" // Combines them into a string


        };

         

        // Simpan entitas Tahapan
        _context.Add(tahapan);
        await _context.SaveChangesAsync();


                   TempData["SuccessMessage"] = "successfully!";

            
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("Dokumen", "Please upload a valid PDF document.");
                }
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

            _context.Timelineok.Remove(timelineok);
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


    }
}
