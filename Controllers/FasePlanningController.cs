using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Plan.Data;
using Plan.Models;
using System.Linq;
using System.Threading.Tasks;
using OfficeOpenXml;
using System.IO;
using Microsoft.AspNetCore.Authorization;
using System.Dynamic;


namespace Plan.Controllers
{
  [Authorize]
  public class FasePlanningController : Controller
  {
    private readonly ApplicationDbContext _context;

    public FasePlanningController(ApplicationDbContext context)
    {
      _context = context;
    }

    public async Task<IActionResult> Index()
    {
      var fasePlanning = await _context.FasePlanning
                                       .OrderByDescending(fp => fp.Id) // Urutkan berdasarkan Id descending
                                       .ToListAsync();
      return View(fasePlanning);
    }


// public async Task<IActionResult> Index()
// {
//     var fasePlanning = await (from fp in _context.FasePlanning
//                               join m in _context.Memo on fp.No_Memo_Rekomendasi equals m.No_Memo_Rekomendasi
//                               orderby fp.Id descending
//                               select new 
//                               {
//                                   fp,
//                                   m
//                               }).ToListAsync();

//     return View(fasePlanning);
// }




    public IActionResult Create()
    {

      // Get the email of the currently logged-in user from claims
      // Get the email of the currently logged-in user from claims
      var userEmail = User.Claims.FirstOrDefault(c => c.Type == "Email")?.Value;


      // Filter Memo records by the user's email
      // var memos = _context.Memo
      //                     .Where(m => m.Email == userEmail && m.KebutuhanKontrak == "Ya")
      //                     .Select(m => new { m.Id, m.No_Memo_Rekomendasi })
      //                     .ToList();


      var excludedNoMemoRekomendasi = _context.FasePlanning
                                              .Select(fp => fp.No_Memo_Rekomendasi)
                                              .ToList();

      // var memos = _context.Memo
      //                     .Where(m => m.Email == userEmail && 
      //                                 m.KebutuhanKontrak == "Ya" &&
      //                                 !excludedNoMemoRekomendasi.Contains(m.No_Memo_Rekomendasi))
      //                     .Select(m => new { m.Id, m.No_Memo_Rekomendasi })
      //                     .ToList();



      var memos = _context.Memo
                          .Where(m => m.Email == userEmail &&
                                      m.KebutuhanKontrak == "Ya" &&
                                      !excludedNoMemoRekomendasi.Contains(m.No_Memo_Rekomendasi))
                          .Select(m => new
                          {
                            m.Id,
                            m.No_Memo_Rekomendasi,
                            m.Judul,
                            DisplayText = m.No_Memo_Rekomendasi + " - " + m.Judul
                          })
                          .ToList();

      // Pass this list to ViewBag for the dropdown
      ViewBag.NoMemoRekomendasi = new SelectList(memos, "No_Memo_Rekomendasi", "DisplayText");
      // Assign filtered No_Memo_Rekomendasi data to ViewBag
      //ViewBag.NoMemoRekomendasi = new SelectList(memos, "No_Memo_Rekomendasi", "No_Memo_Rekomendasi");

      ViewData["Disiplin"] = new SelectList(_context.DisiplinMaster, "Disiplin", "Disiplin");
      ViewData["Kategori_Equipment"] = new SelectList(_context.KategoriEquipmentMaster, "Kategori", "Kategori");
      ViewData["Kategori_Maintenance"] = new SelectList(_context.KategoriMaintenanceMaster, "Kategori", "Kategori");
      ViewData["Area"] = new SelectList(_context.AreaMaster, "Area", "Area");
      ViewData["Status_kontrak"] = new SelectList(_context.StatusKontrakMaster, "Status", "Status");
      ViewData["Direksi"] = new SelectList(_context.DireksiMaster, "Direksi", "Direksi");
      ViewData["Jenis_Kontrak"] = new SelectList(_context.JenisKontrakMaster, "Jenis", "Jenis");
      ViewData["Planner"] = new SelectList(_context.PlannerMaster, "Planner", "Planner");
      ViewData["Status_Next_Contract"] = new SelectList(_context.StatusNextMaster, "StatusNext", "StatusNext");

      ViewData["Kategori_Kontrak"] = new SelectList(_context.KategoriKontrakMaster, "Kategori", "Kategori");

      //  ViewData["NoProgram"] = new SelectList(_context.Irkap, "NoProgram", "NoProgram");

      // Retrieve the 'Disiplin' claim from the current user
      var userDisiplin = User.Claims.FirstOrDefault(c => c.Type == "Disiplin")?.Value;

      var filteredPrograms = _context.Irkap
          .Where(i => i.Disiplin == userDisiplin &&
                      !_context.FasePlanning.Any(fp => EF.Functions.Like(fp.NoProgram, "%" + i.NoProgram + "%")))
          .Select(i => new
          {
            i.NoProgram,
            i.Judul,
            DisplayText = i.NoProgram + " - " + i.Judul
          })
          .ToList();

      // Pass the filtered programs to ViewBag for use in the dropdown
      ViewBag.NoProgram = new SelectList(filteredPrograms, "NoProgram", "DisplayText");


      // Filter the Irkap table using the Disiplin claim value
      // var filteredPrograms = _context.Irkap
      //                                .Where(i => i.Disiplin == userDisiplin)
      //                                .Select(i => new 
      //                                { 
      //                                    i.NoProgram, 
      //                                    i.Judul, 
      //                                    DisplayText = i.NoProgram + " - " + i.Judul 
      //                                })
      //                                .ToList();

      // // Pass the filtered programs to ViewBag for use in the dropdown
      // ViewBag.NoProgram = new SelectList(filteredPrograms, "NoProgram", "DisplayText");




      // var lastNumber = _context.FasePlanning.OrderByDescending(b => b.No).Select(b => b.No).FirstOrDefault();


      // int nextNumber = lastNumber + 1;
      // ViewBag.NextNumber = nextNumber;

      // Generate angka acak 4 digit
Random random = new Random();
int randomNumber = random.Next(100, 999);

   ViewBag.NextNumber = randomNumber;




      return View();
    }








    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(FasePlanning fasePlanning, string[] Kategori_Equipment, string[] NoProgram, string[] Kategori_Maintenance)

    {
      if (ModelState.IsValid)
      {
        // Ambil nilai Tanggal_Kirim_Paket dari entitas FasePlanning yang sedang dibuat
        var tanggalKirimPaket = fasePlanning.Tanggal_Kirim_Paket;

        if (tanggalKirimPaket == null)
        {
          ModelState.AddModelError("Tanggal_Kirim_Paket", "Tanggal Kirim Paket belum diinput.");
          return View(fasePlanning);
        }

        // Jika Kategori_Equipment dipilih, gabungkan nilai yang dipilih menjadi satu string yang dipisahkan koma
        if (Kategori_Equipment != null && Kategori_Equipment.Length > 0)
        {
          fasePlanning.Kategori_Equipment = string.Join(", ", Kategori_Equipment);
        }

        if (Kategori_Maintenance != null && Kategori_Maintenance.Length > 0)
        {
          fasePlanning.Kategori_Maintenance = string.Join(", ", Kategori_Maintenance);
        }


        if (NoProgram != null && NoProgram.Length > 0)
        {
          fasePlanning.NoProgram = string.Join(", ", NoProgram);
        }


        // Ambil semua task dari TaskMaster, diurutkan berdasarkan ID
        var tasks = _context.TaskMaster
            .OrderBy(t => t.Id)
            .ToList();

        // Inisialisasi tanggal awal
       DateTime? previousTarget = DateTime.Now;

        // Buat daftar untuk menyimpan data TabelSla
        var tabelSlaList = new List<TabelSla>();

        // Update kolom Target secara berurutan dan persiapkan data untuk TabelSla
        foreach (var task in tasks)
        {
          if (previousTarget != null)
          {
            // Hitung Target berdasarkan SLA
            task.Target = previousTarget.Value.AddDays(task.Sla);
            previousTarget = task.Target;
          }

          // Tambahkan data ke tabelSlaList
          tabelSlaList.Add(new TabelSla
          {
            Task = task.Task,
            Sla = task.Sla,
            Target = task.Target,
            Kode_Project = fasePlanning.Kode_Project // Tambahkan Kode_Project dari FasePlanning
          });

        }

        // Simpan data ke tabel TabelSla
        _context.TabelSla.AddRange(tabelSlaList);

        // Simpan entitas FasePlanning
        _context.Add(fasePlanning);

        // Simpan semua perubahan ke database
        await _context.SaveChangesAsync();




        // Create a new Tahapan entity
        var tahapan = new Tahapan
        {
          Kode_Project = fasePlanning.Kode_Project,
          No_Memo_Rekomendasi = fasePlanning.No_Memo_Rekomendasi,
          Tanggal = DateTime.Now.Date, // Current date
          Waktu = DateTime.Now.TimeOfDay, // Current time
          Email = User.Claims.FirstOrDefault(c => c.Type == "Email")?.Value, // Get the logged-in user's email
          Tahap = "Created Planning Project", // A description of the current step
          Status = "Done" // Set status default
        };

        // Simpan entitas Tahapan
        _context.Add(tahapan);
        await _context.SaveChangesAsync();

 

        // Daftar tahapan yang akan diinput otomatis
var daftarTahapan = new List<string>
{
    "JOBPLAN - Menyusun GTC dan RAB",
    "REKOMENDASI - Comment Bestek ke Engineer/User",
    "REKOMENDASI - Menunggu Rekomendasi",
    "JOBPLAN - Sourcing",
    "JOBPLAN - Membuat OE",
    "JOBPLAN - Membuat analisa TKDN & Analisa RAM",
    "JOBPLAN - Membuat ijin persetujuan / prinsip",
    "JOBPLAN - Jobplan (Ext), mengajukan release dan print PR",
    "JOBPLAN - Menyiapkan memo pengantar DP3 Paket",
    "JOBPLAN - Kirim Paket Kontrak",
    "TENDER - Pre Bid Meeting",
    "JOBPLAN - Revisi GTC & RAB pasca prebid",
    "TENDER - Approval HPS OE",
    "TENDER - Open Bid",
    "JOBPLAN - Gagal Tender",
    "TENDER - Penunjukan Pemenang",
    "TENDER - SPB dan SP3MK",
    "TENDER - Terbit PO",
    "EKSEKUSI - KOM",
    "RUNNING KONTRAK - Day One",
    "SELESAI - Selesai"
};

// Menambahkan tahapan termin 1 - 36 secara otomatis
for (int i = 1; i <= 36; i++)
{
    daftarTahapan.Add($"Termin ke {i}");
}


// Loop untuk memasukkan semua tahapan
foreach (var tahap in daftarTahapan)
{
    var tahapan2 = new Tahapan
    {
        Kode_Project = fasePlanning.Kode_Project,
        No_Memo_Rekomendasi = null, // Kosong
        Tanggal = null, // Kosong
        Waktu = null, // Kosong
        Email = null, // Kosong
        Tahap = tahap, // Set nilai dari daftar tahapan
        Status = "Not yet" // Set status default
    };

    _context.Add(tahapan2);
}

// Simpan semua entri ke database sekaligus
await _context.SaveChangesAsync();




 

                    //----------------
                    var historyFasePlanning = new HistoryFasePlanning
                    {
                        Kode_Project = fasePlanning.Kode_Project,
                        Tanggal = DateTime.Now.Date,
                        Waktu = DateTime.Now.TimeOfDay,
                        Email = User.Claims.FirstOrDefault(c => c.Type == "Email")?.Value,
                        Aksi = "Tambah data"
                    };

                    // Simpan entitas ke database
                    _context.Add(historyFasePlanning);
                    await _context.SaveChangesAsync();




        TempData["SuccessMessage"] = "successfully!";
        return RedirectToAction(nameof(Index));
      }




      ViewData["Disiplin"] = new SelectList(_context.DisiplinMaster, "Disiplin", "Disiplin");
      ViewData["Kategori_Equipment"] = new SelectList(_context.KategoriEquipmentMaster, "Kategori", "Kategori");
      ViewData["Kategori_Maintenance"] = new SelectList(_context.KategoriMaintenanceMaster, "Kategori", "Kategori");
      ViewData["Area"] = new SelectList(_context.AreaMaster, "Area", "Area");
      ViewData["Status_kontrak"] = new SelectList(_context.StatusKontrakMaster, "Status", "Status");
      ViewData["Direksi"] = new SelectList(_context.DireksiMaster, "Direksi", "Direksi");
      ViewData["Jenis_Kontrak"] = new SelectList(_context.JenisKontrakMaster, "Jenis", "Jenis");
      ViewData["Planner"] = new SelectList(_context.PlannerMaster, "Planner", "Planner");
      ViewData["Status_Next_Contract"] = new SelectList(_context.StatusNextMaster, "StatusNext", "StatusNext");

      ViewData["Kategori_Kontrak"] = new SelectList(_context.KategoriKontrakMaster, "Kategori", "Kategori");

      ViewData["NoProgram"] = new SelectList(_context.Irkap, "NoProgram", "NoProgram");


      var lastNumber = _context.FasePlanning.OrderByDescending(b => b.No).Select(b => b.No).FirstOrDefault();


      int nextNumber = lastNumber + 1;
      ViewBag.NextNumber = nextNumber;

      return View(fasePlanning);
    }


    [HttpGet]

    public async Task<IActionResult> FormEdit(int? id)
    {
      if (id == null)
      {
        return NotFound();
      }

      var fasePlanning = await _context.FasePlanning.FindAsync(id);
      if (fasePlanning == null)
      {
        return NotFound();
      }

      var userEmail = User.Claims.FirstOrDefault(c => c.Type == "Email")?.Value;
      // Filter Memo records by the user's email
      var memos = _context.Memo
                          .Where(m => m.Email == userEmail && m.KebutuhanKontrak == "Ya")
                          .Select(m => new { m.Id, m.No_Memo_Rekomendasi })
                          .ToList();


      // Assign filtered No_Memo_Rekomendasi data to ViewBag
      ViewBag.NoMemoRekomendasi = new SelectList(memos, "No_Memo_Rekomendasi", "No_Memo_Rekomendasi");







      // Populate dropdowns
      ViewData["Disiplin"] = new SelectList(_context.DisiplinMaster, "Disiplin", "Disiplin", fasePlanning.Disiplin);
      ViewData["Kategori_Equipment"] = new SelectList(_context.KategoriEquipmentMaster, "Kategori", "Kategori", fasePlanning.Kategori_Equipment);
      ViewData["Kategori_Maintenance"] = new SelectList(_context.KategoriMaintenanceMaster, "Kategori", "Kategori", fasePlanning.Kategori_Maintenance);
      ViewData["Area"] = new SelectList(_context.AreaMaster, "Area", "Area", fasePlanning.Area);
      ViewData["Status_kontrak"] = new SelectList(_context.StatusKontrakMaster, "Status", "Status", fasePlanning.Status_kontrak);
      ViewData["Direksi"] = new SelectList(_context.DireksiMaster, "Direksi", "Direksi", fasePlanning.Direksi);
      ViewData["Jenis_Kontrak"] = new SelectList(_context.JenisKontrakMaster, "Jenis", "Jenis", fasePlanning.Jenis_Kontrak);
      ViewData["Planner"] = new SelectList(_context.PlannerMaster, "Planner", "Planner", fasePlanning.Planner);
      ViewData["Status_Next_Contract"] = new SelectList(_context.StatusNextMaster, "StatusNext", "StatusNext", fasePlanning.Status_Next_Contract);

      ViewData["Kategori_Kontrak"] = new SelectList(_context.KategoriKontrakMaster, "Kategori", "Kategori", fasePlanning.Kategori_Kontrak);

      ViewData["NoProgram"] = new SelectList(_context.Irkap, "NoProgram", "NoProgram");


      return View(fasePlanning);
    }



    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(FasePlanning fasePlanning, string[] Kategori_Equipment, string[] NoProgram, string[] Kategori_Maintenance)
    {



      // Jika Kategori_Equipment dipilih, gabungkan nilai yang dipilih menjadi satu string yang dipisahkan koma
      if (Kategori_Equipment != null && Kategori_Equipment.Length > 0)
      {
        fasePlanning.Kategori_Equipment = string.Join(", ", Kategori_Equipment);
      }

      if (Kategori_Maintenance != null && Kategori_Maintenance.Length > 0)
      {
        fasePlanning.Kategori_Maintenance = string.Join(", ", Kategori_Maintenance);
      }


      if (NoProgram != null && NoProgram.Length > 0)
      {
        fasePlanning.NoProgram = string.Join(", ", NoProgram);
      }


      _context.Update(fasePlanning);
      await _context.SaveChangesAsync();




                    //----------------
                    var historyFasePlanning = new HistoryFasePlanning
                    {
                        Kode_Project = fasePlanning.Kode_Project,
                        Tanggal = DateTime.Now.Date,
                        Waktu = DateTime.Now.TimeOfDay,
                        Email = User.Claims.FirstOrDefault(c => c.Type == "Email")?.Value,
                        Aksi = "Edit data"
                    };

                    // Simpan entitas ke database
                    _context.Add(historyFasePlanning);
                    await _context.SaveChangesAsync();


      TempData["SuccessMessage"] = "successfully!";
      return RedirectToAction(nameof(Index)); // Redirect to Index if successful

    }




    [HttpGet]
    public async Task<IActionResult> Detail(int? id)
    {
      if (id == null)
      {
        return NotFound();
      }

      var fasePlanning = await _context.FasePlanning.FindAsync(id);
      if (fasePlanning == null)
      {
        return NotFound();
      }

      // Populate dropdowns
      ViewData["Disiplin"] = new SelectList(_context.DisiplinMaster, "Disiplin", "Disiplin", fasePlanning.Disiplin);
      ViewData["Kategori_Equipment"] = new SelectList(_context.KategoriEquipmentMaster, "Kategori", "Kategori", fasePlanning.Kategori_Equipment);
      ViewData["Kategori_Maintenance"] = new SelectList(_context.KategoriMaintenanceMaster, "Kategori", "Kategori", fasePlanning.Kategori_Maintenance);
      ViewData["Area"] = new SelectList(_context.AreaMaster, "Area", "Area", fasePlanning.Area);
      ViewData["Status_kontrak"] = new SelectList(_context.StatusKontrakMaster, "Status", "Status", fasePlanning.Status_kontrak);
      ViewData["Direksi"] = new SelectList(_context.DireksiMaster, "Direksi", "Direksi", fasePlanning.Direksi);
      ViewData["Jenis_Kontrak"] = new SelectList(_context.JenisKontrakMaster, "Jenis", "Jenis", fasePlanning.Jenis_Kontrak);
      ViewData["Planner"] = new SelectList(_context.PlannerMaster, "Planner", "Planner", fasePlanning.Planner);
      ViewData["Status_Next_Contract"] = new SelectList(_context.StatusNextMaster, "StatusNext", "StatusNext", fasePlanning.Status_Next_Contract);

      ViewData["Kategori_Kontrak"] = new SelectList(_context.KategoriKontrakMaster, "Kategori", "Kategori", fasePlanning.Kategori_Kontrak);

      ViewData["NoProgram"] = new SelectList(_context.Irkap, "NoProgram", "NoProgram");



                    //----------------
                    var historyFasePlanning = new HistoryFasePlanning
                    {
                        Kode_Project = fasePlanning.Kode_Project,
                        Tanggal = DateTime.Now.Date,
                        Waktu = DateTime.Now.TimeOfDay,
                        Email = User.Claims.FirstOrDefault(c => c.Type == "Email")?.Value,
                        Aksi = "Lihat data"
                    };

                    // Simpan entitas ke database
                    _context.Add(historyFasePlanning);
                    await _context.SaveChangesAsync();



      return View(fasePlanning);
    }









    [HttpPost]
    [ValidateAntiForgeryToken]

    public async Task<IActionResult> DetailByMemo(string idmemo)
    {


      var fasePlanning = await _context.FasePlanning
          .Where(f => f.No_Memo_Rekomendasi == idmemo) // Ganti 'KolomStringId' dengan nama kolom yang sesuai
          .FirstOrDefaultAsync();

      if (fasePlanning == null)
      {
        ViewBag.Message = "Data tidak ditemukan.";
      }

      return View(fasePlanning);
    }




    public async Task<IActionResult> Delete(int? id)
    {
      if (id == null)
      {
        return NotFound();
      }

      var fasePlanning = await _context.FasePlanning
          .FirstOrDefaultAsync(m => m.Id == id);
      if (fasePlanning == null)
      {
        return NotFound();
      }

      return View(fasePlanning);
    }

    [HttpGet, ActionName("Delete")]

    public async Task<IActionResult> DeleteConfirmed(int id)
    {
      var fasePlanning = await _context.FasePlanning.FindAsync(id);
      _context.FasePlanning.Remove(fasePlanning);
      await _context.SaveChangesAsync();




                    //----------------
                    var historyFasePlanning = new HistoryFasePlanning
                    {
                        Kode_Project = fasePlanning.Kode_Project,
                        Tanggal = DateTime.Now.Date,
                        Waktu = DateTime.Now.TimeOfDay,
                        Email = User.Claims.FirstOrDefault(c => c.Type == "Email")?.Value,
                        Aksi = "Hapus data"
                    };

                    // Simpan entitas ke database
                    _context.Add(historyFasePlanning);
                    await _context.SaveChangesAsync();



      TempData["SuccessMessage"] = "successfully!";
      return RedirectToAction(nameof(Index));
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

      try
      {
        // Membaca file Excel
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

            // Memproses data di Excel
            int rowCount = worksheet.Dimension.Rows;
            var fasePlannings = new List<FasePlanning>();

            for (int row = 2; row <= rowCount; row++) // Mulai dari baris ke-2 untuk melewati header
            {
              if (worksheet.Cells[row, 1]?.Value == null) // Validasi jika kolom utama kosong
              {
                ModelState.AddModelError("File", $"Row {row} is missing required data.");
                continue;
              }


              var fasePlanning = new FasePlanning
              {
                No = int.TryParse(worksheet.Cells[row, 1]?.Value?.ToString(), out var no) ? no : 0,
                Kode_Project = worksheet.Cells[row, 2]?.Value?.ToString(),
                Jenis_Project = worksheet.Cells[row, 3]?.Value?.ToString(),
                Tipe_Project = worksheet.Cells[row, 4]?.Value?.ToString(),
                Tahun = int.TryParse(worksheet.Cells[row, 5]?.Value?.ToString(), out var tahun) ? tahun : 0,
                Disiplin = worksheet.Cells[row, 6]?.Value?.ToString(),
                Pekerjaan = worksheet.Cells[row, 7]?.Value?.ToString(),
                Kategori_Equipment = worksheet.Cells[row, 8]?.Value?.ToString(),
                Kategori_Maintenance = worksheet.Cells[row, 9]?.Value?.ToString(),
                Area = worksheet.Cells[row, 10]?.Value?.ToString(),
                Direksi = worksheet.Cells[row, 11]?.Value?.ToString(),
                Status_kontrak = worksheet.Cells[row, 12]?.Value?.ToString(),
                Kategori_Kontrak = worksheet.Cells[row, 13]?.Value?.ToString(),
                Jenis_Kontrak = worksheet.Cells[row, 14]?.Value?.ToString(),
                Planner = worksheet.Cells[row, 15]?.Value?.ToString(),
                No_Memo_Rekomendasi = worksheet.Cells[row, 16]?.Value?.ToString(),
                Tanggal_Masuk_Memo = DateTime.TryParse(worksheet.Cells[row, 17]?.Value?.ToString(), out var masukMemo) ? masukMemo : null,
                No_Memo_Kirim_Paket = worksheet.Cells[row, 18]?.Value?.ToString(),
                Tanggal_Kirim_Paket = DateTime.TryParse(worksheet.Cells[row, 19]?.Value?.ToString(), out var kirimPaket) ? kirimPaket : null,
                Status_Next_Contract = worksheet.Cells[row, 20]?.Value?.ToString(),
                Informasi_Detail = worksheet.Cells[row, 21]?.Value?.ToString(),
                Nomor_PR_Kontrak_Baru = worksheet.Cells[row, 22]?.Value?.ToString(),
                Tanggal_Update = DateTime.TryParse(worksheet.Cells[row, 23]?.Value?.ToString(), out var updateTanggal) ? updateTanggal : null,
              };


              fasePlannings.Add(fasePlanning);
            }

            if (fasePlannings.Any())
            {
              _context.FasePlanning.AddRange(fasePlannings);
              await _context.SaveChangesAsync();
            }
          }
        }

        return RedirectToAction(nameof(Index));
      }
      catch (Exception ex)
      {
        ModelState.AddModelError("File", $"An error occurred while processing the file: {ex.Message}");
        return View(model);
      }
    }

 


    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult SubmitKehandalan(List<int> kehandalanIds)
    {
      if (kehandalanIds != null && kehandalanIds.Any())
      {
        var fasePlannings = _context.FasePlanning.Where(f => kehandalanIds.Contains(f.Id)).ToList();
        foreach (var fase in fasePlannings)
        {
          fase.Kehandalan = 1; // Menandai sebagai diceklis
        }
        _context.SaveChanges();
      }

      return RedirectToAction("Index"); // Redirect kembali ke halaman utama
    }




    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult SubmitBukanKehandalan(List<int> kehandalanIds)
    {
      if (kehandalanIds != null && kehandalanIds.Any())
      {
        var fasePlannings = _context.FasePlanning.Where(f => kehandalanIds.Contains(f.Id)).ToList();
        foreach (var fase in fasePlannings)
        {
          fase.Kehandalan = 0; // Menandai sebagai diceklis
        }
        _context.SaveChanges();
      }

      return RedirectToAction("TampilKehandalan"); // Redirect kembali ke halaman utama
    }




    public IActionResult TampilKehandalan()
    {
      var kehandalanData = _context.FasePlanning.Where(p => p.Kehandalan == 1).ToList();
      return View(kehandalanData);
    }




public IActionResult Timeline(string kodeProject, string noMemo)
{
    var query = _context.Tahapan.AsQueryable();

    if (!string.IsNullOrEmpty(noMemo))
    {
        query = query.Where(t => t.No_Memo_Rekomendasi == noMemo || t.Kode_Project == kodeProject);
    }
    else if (!string.IsNullOrEmpty(kodeProject))
    {
        query = query.Where(t => t.Kode_Project == kodeProject);
    }

    var model = query
        .OrderBy(t => t.Id) // Urutkan berdasarkan ID agar timeline tetap rapi
        .ToList();

    return PartialView("_TimelinePartial", model);
}





        public async Task<IActionResult> TampilHistoryFasePlanning(string kodeProject)
        {
            var histori = await _context.HistoryFasePlanning
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
