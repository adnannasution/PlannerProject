using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Plan.Models;
using Plan.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Plan.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ApplicationDbContext context, ILogger<HomeController> logger)
        {
            _context = context;
            _logger = logger;
        }

public IActionResult Index()
{
    // Ambil tahun unik dari tabel WbsData untuk dropdown

    return View();
}


        

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

public async Task<IActionResult> GetKategoriKontrakData(int tahun)
{
    var data = await _context.FasePlanning
        .Where(f => f.Tahun == tahun)  // Memfilter berdasarkan kolom Tahun
        .GroupBy(f => f.Kategori_Kontrak)
        .Select(g => new
        {
            KategoriKontrak = g.Key,
            Count = g.Count()
        })
        .ToListAsync();

    return Json(data);
}



        public IActionResult Chart()
{
    return View();
}




public async Task<IActionResult> GetJenisKontrakData(int? tahun)
{
    // Jika tahun tidak diberikan, ambil data untuk semua tahun
    var dataQuery = _context.FasePlanning.AsQueryable();

    if (tahun.HasValue)
    {
        dataQuery = dataQuery.Where(f => f.Tahun == tahun); // Filter berdasarkan tahun jika diberikan
    }

    var data = await dataQuery
        .GroupBy(f => f.Jenis_Kontrak)
        .Select(g => new
        {
            JenisKontrak = g.Key ?? "Tidak Diketahui", // Ganti nilai null dengan 'Tidak Diketahui'
            Count = g.Count()
        })
        .ToListAsync();

    return Json(data);
}



public IActionResult JenisKontrakChart()
{
    return View();
}


public async Task<IActionResult> GetAreaData(int year)
{
    var data = await _context.FasePlanning
        .Where(f => f.Tahun == year)  // Filter berdasarkan tahun
        .GroupBy(f => f.Area)
        .Select(g => new
        {
            Area = g.Key ?? "Tidak Diketahui",
            Count = g.Count()
        })
        .ToListAsync();

    return Json(data);
}

 

public IActionResult AreaChart()
{
    return View();
}


public async Task<JsonResult> GetDireksiData([FromQuery(Name = "year")] int? tahun)
{
    var dataQuery = _context.FasePlanning.AsQueryable();

    if (tahun.HasValue)
    {
        dataQuery = dataQuery.Where(f => f.Tahun == tahun);
    }

    var data = await dataQuery
        .GroupBy(d => d.Direksi)
        .Select(g => new
        {
            Direksi = g.Key ?? "Tidak Diketahui",
            Count = g.Count()
        })
        .ToListAsync();

    return Json(data);
}





public IActionResult DireksiChart()
{
    return View();
}




public JsonResult GetResumeStatusKontrakData(int tahun)
{
    var dataStatusKontrak = _context.Timelineok
        .Where(t => t.Tanggal.Year == tahun)  // Menambahkan filter berdasarkan tahun dari kolom Tanggal
        .GroupBy(t => t.ResumeStatusPekerjaan ?? "Tidak Diketahui")  // Mengganti nilai null dengan "Tidak Diketahui"
        .Select(g => new
        {
            statusKontrak = g.Key,  // Status Kontrak
            count = g.Count()  // Jumlah data untuk status kontrak tersebut
        })
        .ToList();

    return Json(dataStatusKontrak);
}


public IActionResult StatusKontrakChart()
{
    return View();
}


    public IActionResult Dashboard()
    {
        return View();
    }



    public JsonResult GetTotalKodeProject()
{
    var totalKodeProject = _context.FasePlanning
        .GroupBy(p => p.Kode_Project)
        .Count(); // Hitung jumlah Kode_Project yang unik

    return Json(totalKodeProject);
}



public JsonResult GetTotalStatusKosong()
{
    var totalStatusKosong = _context.Timelineok
        .Count(t => string.IsNullOrEmpty(t.Status));  // Menghitung baris dengan status kosong atau null

    return Json(totalStatusKosong);
}

public JsonResult GetTotalStatusAman()
{
    var totalStatusAman = _context.Timelineok
        .Where(t => t.Status == "Aman")
        .Count();

    return Json(totalStatusAman);
}


 





public JsonResult GetResumeStatusPekerjaanData(string tahunresume)
{
    var data = _context.Timelineok
        .Join(_context.FasePlanning,
              t => t.Kode_Project,   
              f => f.Kode_Project,   
              (t, f) => new { t, f })  
        .Where(tf => tf.t.Tanggal.Year.ToString() == tahunresume)  
        .AsEnumerable()  // Evaluasi di memori setelah filter
        .GroupBy(tf => tf.t.Kode_Project)  // Kelompokkan berdasarkan Kode_Project
        .Select(g => g.OrderByDescending(tf => tf.t.Tanggal).FirstOrDefault()) // Ambil data terbaru berdasarkan Tanggal
        .Where(tf => tf != null)  // Pastikan tidak ada nilai null
        .GroupBy(tf => tf.t.ResumeStatusPekerjaan ?? "Tidak Diketahui")  
        .Select(g => new
        {
            ResumeStatusPekerjaan = g.Key,
            Count = g.Count()  // Hitung jumlah Kode_Project unik yang memiliki ResumeStatusPekerjaan terbaru
        })
        .ToList();

    return Json(data);
}






public IActionResult DetailFasePlanning(string status, int tahun)
{
    var detailData = _context.FasePlanning
        .Where(f => _context.Timelineok
            .Where(t => t.ResumeStatusPekerjaan == status && t.Tanggal.Year == tahun)
            .Select(t => t.Kode_Project) // Ambil hanya Kode_Project dari Timelineok
            .Contains(f.Kode_Project) // Cocokkan dengan Kode_Project di FasePlanning
        )
        .ToList();

    return View(detailData);
}







public IActionResult DetailFaseTender(string status, int tahun)
{
    var detailData = _context.FasePlanning
        .Where(f => _context.Timelineok
            .Where(t => t.ResumeStatusPekerjaan == status && t.Tanggal.Year == tahun)
            .Select(t => t.Kode_Project) // Ambil hanya Kode_Project dari Timelineok
            .Contains(f.Kode_Project) // Cocokkan dengan Kode_Project di FasePlanning
        )
        .ToList();

    return View(detailData);
}





public IActionResult DetailFaseExecution(string status, int tahun)
{
    var detailData = _context.FasePlanning
        .Where(f => _context.Timelineok
            .Where(t => t.ResumeStatusPekerjaan == status && t.Tanggal.Year == tahun)
            .Select(t => t.Kode_Project) // Ambil hanya Kode_Project dari Timelineok
            .Contains(f.Kode_Project) // Cocokkan dengan Kode_Project di FasePlanning
        )
        .ToList();

    return View(detailData);
}








public IActionResult DetailResumeStatusPekerjaan(string status, int tahun)
{
    var detailData = _context.FasePlanning
        .Where(f => _context.Timelineok
            .Where(t => t.ResumeStatusPekerjaan == status && t.Tanggal.Year == tahun)
            .Select(t => t.Kode_Project) // Ambil hanya Kode_Project dari Timelineok
            .Contains(f.Kode_Project) // Cocokkan dengan Kode_Project di FasePlanning
        )
        .ToList();

    return View(detailData);
}






public IActionResult DetailDireksi(string direksi, int tahun)
{
    var detailData = _context.FasePlanning
        .Where(t => t.Direksi == direksi && t.Tahun == tahun)  // Filter berdasarkan Direksi dan Tahun
        .ToList();
    return View(detailData);
}


        public IActionResult DetailKategoriKontrak(string kategori, int tahun)
    {
        var detailData = _context.FasePlanning
            .Where(t => t.Kategori_Kontrak == kategori && t.Tahun == tahun)
            .ToList();
        return View(detailData);
    }


        public IActionResult DetailArea(string area, int tahun)
    {
        var detailData = _context.FasePlanning
            .Where(t => t.Area == area && t.Tahun == tahun)
            .ToList();
        return View(detailData);
    }



            public IActionResult DetailJenisKontrak(string jenis, int tahun)
    {
        var detailData = _context.FasePlanning
            .Where(t => t.Jenis_Kontrak == jenis && t.Tahun == tahun)
            .ToList();
        return View(detailData);
    }



// public JsonResult GetChartData(int year, string filterType)
// {
//     var data = _context.WbsData
//         .Where(w => w.JenisBiaya == filterType && w.Tahun == year)  // Filter berdasarkan JenisBiaya = RUTIN dan tahun
//         .Select(w => new
//         {
//             w.Bulan,
//             w.PercentActual,
//             w.PercentForecasting
//         })
//         .OrderBy(w => w.Bulan)  // Mengurutkan berdasarkan bulan
//         .ToList();

//     return Json(data);
// }


public JsonResult GetChartData(int year, string filterType)
{
    // List urutan bulan dalam bahasa Inggris
    var monthOrder = new List<string>
    {
        "January", "February", "March", "April", "May", "June",
        "July", "August", "September", "October", "November", "December"
    };

    var data = _context.WbsData
        .Where(w => w.JenisBiaya == filterType && w.Tahun == year)
        .Select(w => new
        {
            w.Bulan,
            w.PercentActual,
            w.PercentForecasting,
            w.ActualWbsRt
        })
        .AsEnumerable() // Ambil data dulu, baru urutkan di memori
        .OrderBy(w => monthOrder.IndexOf(w.Bulan)) // Urutkan berdasarkan indeks bulan
        .ToList();

    return Json(data);
}




// public JsonResult GetSumData(int year, string filterType)
// {
//     // Mengambil data dari tabel WbsData berdasarkan tahun dan filter jenis biaya
//     var wbsData = _context.WbsData
//         .Where(w => w.JenisBiaya == filterType && w.Tahun == year)
//         .ToList();

//     // Menghitung SUM untuk CommitmentWbsRt, RopWbsRt, dan ActualWbsRt
//     var wbsSumData = new
//     {
//         commitmentWbsRt = wbsData.Sum(w => w.CommitmentWbsRt),
//         ropWbsRt = wbsData.Sum(w => w.RopWbsRt),
//         actualWbsRt = wbsData.Sum(w => w.ActualWbsRt)
//     };

//     // Mengambil data dari tabel BudgetWbsData berdasarkan tahun
//     var budgetWbsData = _context.BudgetWbsData
//         .Where(b => b.Tahun == year)
//         .ToList();

//     // Menghitung SUM untuk BudgetWbs
//     var budgetSumData = new
//     {
//         budgetWbs = budgetWbsData.Sum(b => b.BudgetWbs)
//     };

//     // Menggabungkan hasil dari WbsData dan BudgetWbsData
//     var result = new
//     {
//         wbs = wbsSumData,
//         budget = budgetSumData
//     };

//     return Json(result);
// }


public JsonResult GetSumData(int year, string filterType)
{
    // Mengambil data terakhir dari WbsData berdasarkan Tahun dan JenisBiaya
    var latestWbsData = _context.WbsData
        .Where(w => w.JenisBiaya == filterType && w.Tahun == year)
        .OrderByDescending(w => w.Id) // Pastikan 'Id' adalah primary key atau gunakan Tanggal jika ada
        .FirstOrDefault();

   
 // Mengambil data terakhir dari BudgetWbsData berdasarkan Tahun dan FilterType (JenisBiaya)
    var latestBudgetData = _context.BudgetWbsData
        .Where(b => b.Tahun == year && b.JenisBiaya == filterType) // Sesuaikan dengan jenis biaya
        .OrderByDescending(b => b.Id) // Sesuaikan dengan kolom yang bisa menentukan urutan
        .FirstOrDefault();

    // Jika tidak ada data, beri nilai default
    var result = new
    {
        wbs = latestWbsData != null ? new
        {
            commitmentWbsRt = latestWbsData.CommitmentWbsRt,
            ropWbsRt = latestWbsData.RopWbsRt,
            actualWbsRt = latestWbsData.ActualWbsRt
        } : null, // Jika data kosong, set null

        budget = latestBudgetData != null ? new
        {
            budgetWbs = latestBudgetData.BudgetWbs
        } : null // Jika data kosong, set null
    };

    return Json(result);
}
 


public JsonResult GetDashboardData(int tahun)
{
    var sixMonthsAgo = DateTime.Now.AddMonths(-6);



var amanCount = _context.FasePlanning
    .Where(f => f.Tanggal_Kirim_Paket >= DateTime.Now &&
                f.Kode_Project.EndsWith($"/{tahun}"))
    .Count();


var terlambatCount = _context.FasePlanning
    .Where(f => f.Tanggal_Kirim_Paket < DateTime.Now &&
                f.Kode_Project.EndsWith($"/{tahun}"))
    .Count();




    var selesaiCount = _context.Timelineok
        .Where(t => t.ResumeStatusPekerjaan == "Selesai" && t.Kode_Project.EndsWith($"/{tahun}"))
        .Count();

    // Menghitung total proyek berdasarkan tahun
    var totalProjectCount = _context.FasePlanning
        .Where(fp => fp.Kode_Project.EndsWith($"/{tahun}"))  // Filter berdasarkan tahun di Kode_Project
        .Count();


    return Json(new
    {
        jumlahAman = amanCount,
        jumlahTerlambat = terlambatCount,
        jumlahSelesai = selesaiCount,
        totalProject = totalProjectCount,
      
    });
}



public IActionResult DetailOnProgress(int? tahun)
{
    var currentYear = DateTime.Now.Year;
    var selectedYear = tahun ?? currentYear;
    var currentDate = DateTime.Now;

    // Ambil data dari TabelSla yang sudah difilter berdasarkan kondisi
    var allSlaData = _context.TabelSla
        .Where(s => s.Task == "Tanggal Kirim Paket" && s.Target >= currentDate)
        .ToList();

    // Join dengan FasePlanning menggunakan Kode_Project dan filter berdasarkan Tahun di FasePlanning menggunakan LIKE
    var data = (from sla in allSlaData
                join fase in _context.FasePlanning
                on sla.Kode_Project equals fase.Kode_Project
                where EF.Functions.Like(fase.Tahun.ToString(), "%" + selectedYear.ToString() + "%") // Menggunakan LIKE untuk pencocokan tahun
                select new SlaFaseViewModel
                {
                    Kode_Project = sla.Kode_Project.Trim(), // Trim untuk menghindari spasi tambahan
                    Task = sla.Task,
                    Target = sla.Target,
                    Pekerjaan = fase.Pekerjaan, // Ambil Pekerjaan dari FasePlanning
                    Disiplin = fase.Disiplin,  // Ambil Disiplin dari FasePlanning
                    Tahun = fase.Tahun // Ambil Tahun dari FasePlanning
                }).ToList();

    // Debugging
    Console.WriteLine($"Total Data Sebelum Filter: {allSlaData.Count}");
    Console.WriteLine($"Total Data Setelah Filter: {data.Count}");
    Console.WriteLine("Jumlah Data yang Dikirim ke View: " + data.Count);

    return View(data);
}






  public new IActionResult NotFound()
    {
        return View();
    }






[HttpGet]
public IActionResult AmanData(int tahun)
{
    var data = _context.FasePlanning
                .Where(f => f.Tanggal_Kirim_Paket >= DateTime.Now &&
                            f.Kode_Project.EndsWith($"/" + tahun))
                .ToList(); // Mengambil data langsung tanpa Select baru

    return View(data); // Menampilkan di view Index seperti halaman utama
}

 
[HttpGet]
public IActionResult TerlambatData(int tahun)
{
    var data = _context.FasePlanning
                .Where(f => f.Tanggal_Kirim_Paket < DateTime.Now &&
                            f.Kode_Project.EndsWith($"/" + tahun))
                .ToList(); // Mengambil data langsung tanpa Select baru

    return View(data); // Menampilkan di view Index seperti halaman utama
}






[HttpGet]
public IActionResult SelesaiData(int tahun)
{
    var data = _context.FasePlanning
                .Where(f => _context.Timelineok
                    .Where(t => t.ResumeStatusPekerjaan == "Selesai" 
                                && t.Kode_Project.EndsWith($"/" + tahun))
                    .GroupBy(t => t.Kode_Project)
                    .Select(g => g.OrderByDescending(t => t.Tanggal).FirstOrDefault().Kode_Project) // Ambil hanya Kode_Project
                    .Contains(f.Kode_Project) // Cocokkan dengan FasePlanning
                )
                .ToList();

    ViewBag.SelectedYear = tahun;
    return View(data);
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




public IActionResult TampilPAdd()
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
                    Komentar = m.Komentar

                }).ToList();

    return View(memo);
}


public IActionResult TampilMemoDisiplin(string disiplin)
{
    var memo = _context.Memo
    .Include(m => m.TabelDokumen) 
                       .Where(m => m.Disiplin == disiplin) // Filter KebutuhanKontrak yang null
                       .OrderByDescending(m => m.Id) // Mengurutkan berdasarkan Id secara descending
                       .ToList();
    
    return View(memo);
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

    return View(model);
}


    }
}
