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
    public class Home2Controller : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<HomeController> _logger;

        public Home2Controller(ApplicationDbContext context, ILogger<HomeController> logger)
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
    // Mendapatkan klaim Disiplin dari User
    var disiplin = User.Claims.FirstOrDefault(c => c.Type == "Disiplin")?.Value;

    var data = await _context.FasePlanning
        .Where(f => f.Tahun == tahun && f.Disiplin == disiplin)  // Memfilter berdasarkan Tahun dan Disiplin
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
   
      var disiplin = User.Claims.FirstOrDefault(c => c.Type == "Disiplin")?.Value;
       // Jika tahun tidak diberikan, ambil data untuk semua tahun
    var dataQuery = _context.FasePlanning.AsQueryable();

    if (tahun.HasValue)
    {
        dataQuery = dataQuery.Where(f => f.Tahun == tahun && f.Disiplin == disiplin); // Filter berdasarkan tahun jika diberikan
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
       var disiplin = User.Claims.FirstOrDefault(c => c.Type == "Disiplin")?.Value;

    var data = await _context.FasePlanning
        .Where(f => f.Tahun == year && f.Disiplin == disiplin)  // Filter berdasarkan tahun
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
     var disiplin = User.Claims.FirstOrDefault(c => c.Type == "Disiplin")?.Value;
    var dataQuery = _context.FasePlanning.AsQueryable();

    if (tahun.HasValue)
    {
        dataQuery = dataQuery.Where(f => f.Tahun == tahun && f.Disiplin == disiplin);
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




// public JsonResult GetResumeStatusKontrakData(int tahun)
// {
//     // Mendapatkan klaim Disiplin dari User
//     var disiplin = User.Claims.FirstOrDefault(c => c.Type == "Disiplin")?.Value;

//     var dataStatusKontrak = _context.Timelineok
//         .Join(_context.FasePlanning,
//               t => t.Kode_Project,   // Kolom untuk join dari tabel Timelineok
//               f => f.Kode_Project,   // Kolom untuk join dari tabel FasePlanning
//               (t, f) => new { t, f })  // Menggabungkan hasil join
//         .Where(tf => tf.t.Tanggal.Year == tahun && tf.f.Disiplin == disiplin)  // Menambahkan filter berdasarkan tahun dan Disiplin
//         .GroupBy(tf => tf.t.ResumeStatusPekerjaan ?? "Tidak Diketahui")  // Mengganti nilai null dengan "Tidak Diketahui"
//         .Select(g => new
//         {
//             statusKontrak = g.Key,  // Status Kontrak
//             count = g.Count()  // Jumlah data untuk status kontrak tersebut
//         })
//         .ToList();

//     return Json(dataStatusKontrak);
// }


public JsonResult GetResumeStatusKontrakData(int tahun)
{
    // Mendapatkan klaim Disiplin dari User
    var disiplin = User.Claims.FirstOrDefault(c => c.Type == "Disiplin")?.Value;

    var dataStatusKontrak = _context.Timelineok
        .Join(_context.FasePlanning,
              t => t.Kode_Project,   // Kolom untuk join dari tabel Timelineok
              f => f.Kode_Project,   // Kolom untuk join dari tabel FasePlanning
              (t, f) => new { t, f })  // Menggabungkan hasil join
        .Where(tf => tf.t.Tanggal.Year == tahun && tf.f.Disiplin == disiplin)  // Filter berdasarkan tahun dan Disiplin
        .Select(tf => new 
        { 
            tf.t.Kode_Project, 
            tf.t.ResumeStatusPekerjaan 
        }) // Ambil hanya kolom yang dibutuhkan
        .Distinct() // Hilangkan duplikasi berdasarkan Kode_Project
        .GroupBy(tf => tf.ResumeStatusPekerjaan ?? "Tidak Diketahui")  // Mengganti nilai null dengan "Tidak Diketahui"
        .Select(g => new
        {
            statusKontrak = g.Key,  // Status Kontrak
            count = g.Count()  // Hitung jumlah Kode_Project unik
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
    // Mendapatkan klaim Disiplin dari User
    var disiplin = User.Claims.FirstOrDefault(c => c.Type == "Disiplin")?.Value;

    var totalKodeProject = _context.FasePlanning
        .Where(p => p.Disiplin == disiplin)  // Menambahkan filter berdasarkan klaim Disiplin
        .GroupBy(p => p.Kode_Project)
        .Count(); // Hitung jumlah Kode_Project yang unik

    return Json(totalKodeProject);
}




public JsonResult GetTotalStatusKosong()
{
    // Mendapatkan klaim Disiplin dari User
    var disiplin = User.Claims.FirstOrDefault(c => c.Type == "Disiplin")?.Value;

    var totalStatusKosong = _context.Timelineok
        .Join(_context.FasePlanning,
              t => t.Kode_Project,   // Kolom untuk join dari tabel Timelineok
              f => f.Kode_Project,   // Kolom untuk join dari tabel FasePlanning
              (t, f) => new { t, f })  // Menggabungkan hasil join
        .Where(tf => string.IsNullOrEmpty(tf.t.Status) && tf.f.Disiplin == disiplin)  // Menambahkan filter untuk status kosong atau null dan klaim Disiplin
        .Count();  // Menghitung jumlah baris dengan status kosong atau null

    return Json(totalStatusKosong);
}



public JsonResult GetTotalStatusAman()
{
    // Mendapatkan klaim Disiplin dari User
    var disiplin = User.Claims.FirstOrDefault(c => c.Type == "Disiplin")?.Value;

    var totalStatusAman = _context.Timelineok
        .Join(_context.FasePlanning,
              t => t.Kode_Project,   // Kolom untuk join dari tabel Timelineok
              f => f.Kode_Project,   // Kolom untuk join dari tabel FasePlanning
              (t, f) => new { t, f })  // Menggabungkan hasil join
        .Where(tf => tf.t.Status == "Aman" && tf.f.Disiplin == disiplin)  // Menambahkan filter untuk status "Aman" dan klaim Disiplin
        .Count();  // Menghitung jumlah baris yang sesuai

    return Json(totalStatusAman);
}







public JsonResult GetResumeStatusPekerjaanData(string tahunresume)
{

     var disiplin = User.Claims.FirstOrDefault(c => c.Type == "Disiplin")?.Value;

    var data = _context.Timelineok
        .Join(_context.FasePlanning,
              t => t.Kode_Project,   
              f => f.Kode_Project,   
              (t, f) => new { t, f })  
        .Where(tf => tf.t.Tanggal.Year.ToString() == tahunresume && tf.f.Disiplin == disiplin)  
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
     var disiplin = User.Claims.FirstOrDefault(c => c.Type == "Disiplin")?.Value;
    var detailData = _context.FasePlanning
        .Where(f => _context.Timelineok
            .Where(t => t.ResumeStatusPekerjaan == status && t.Tanggal.Year == tahun && f.Disiplin == disiplin)
            .Select(t => t.Kode_Project) // Ambil hanya Kode_Project dari Timelineok
            .Contains(f.Kode_Project) // Cocokkan dengan Kode_Project di FasePlanning
        )
        .ToList();

    return View(detailData);
}







public IActionResult DetailFaseTender(string status, int tahun)
{
     var disiplin = User.Claims.FirstOrDefault(c => c.Type == "Disiplin")?.Value;
    var detailData = _context.FasePlanning
        .Where(f => _context.Timelineok
            .Where(t => t.ResumeStatusPekerjaan == status && t.Tanggal.Year == tahun && f.Disiplin == disiplin)
            .Select(t => t.Kode_Project) // Ambil hanya Kode_Project dari Timelineok
            .Contains(f.Kode_Project) // Cocokkan dengan Kode_Project di FasePlanning
        )
        .ToList();

    return View(detailData);
}





public IActionResult DetailFaseExecution(string status, int tahun)
{
     var disiplin = User.Claims.FirstOrDefault(c => c.Type == "Disiplin")?.Value;
    var detailData = _context.FasePlanning
        .Where(f => _context.Timelineok
            .Where(t => t.ResumeStatusPekerjaan == status && t.Tanggal.Year == tahun && f.Disiplin == disiplin)
            .Select(t => t.Kode_Project) // Ambil hanya Kode_Project dari Timelineok
            .Contains(f.Kode_Project) // Cocokkan dengan Kode_Project di FasePlanning
        )
        .ToList();

    return View(detailData);
}








public IActionResult DetailResumeStatusPekerjaan(string status, int tahun)
{
     var disiplin = User.Claims.FirstOrDefault(c => c.Type == "Disiplin")?.Value;
    var detailData = _context.FasePlanning
        .Where(f => _context.Timelineok
            .Where(t => t.ResumeStatusPekerjaan == status && t.Tanggal.Year == tahun && f.Disiplin == disiplin)
            .Select(t => t.Kode_Project) // Ambil hanya Kode_Project dari Timelineok
            .Contains(f.Kode_Project) // Cocokkan dengan Kode_Project di FasePlanning
        )
        .ToList();

    return View(detailData);
}






public IActionResult DetailDireksi(string direksi, int tahun)
{
     var disiplin = User.Claims.FirstOrDefault(c => c.Type == "Disiplin")?.Value;
    var detailData = _context.FasePlanning
        .Where(t => t.Direksi == direksi && t.Tahun == tahun && t.Disiplin == disiplin)  // Filter berdasarkan Direksi dan Tahun
        .ToList();
    return View(detailData);
}


        public IActionResult DetailKategoriKontrak(string kategori, int tahun)
    {
         var disiplin = User.Claims.FirstOrDefault(c => c.Type == "Disiplin")?.Value;
        var detailData = _context.FasePlanning
            .Where(t => t.Kategori_Kontrak == kategori && t.Tahun == tahun && t.Disiplin == disiplin)
            .ToList();
        return View(detailData);
    }


        public IActionResult DetailArea(string area, int tahun)
    {
         var disiplin = User.Claims.FirstOrDefault(c => c.Type == "Disiplin")?.Value;
        var detailData = _context.FasePlanning
            .Where(t => t.Area == area && t.Tahun == tahun && t.Disiplin == disiplin)
            .ToList();
        return View(detailData);
    }



            public IActionResult DetailJenisKontrak(string jenis, int tahun)
    {
          var disiplin = User.Claims.FirstOrDefault(c => c.Type == "Disiplin")?.Value;
          
        var detailData = _context.FasePlanning
            .Where(t => t.Jenis_Kontrak == jenis && t.Tahun == tahun && t.Disiplin == disiplin)
            .ToList();
        return View(detailData);
    }











public JsonResult GetChartData(int year, string filterType)
{
    var data = _context.WbsData
        .Where(w => w.JenisBiaya == filterType && w.Tahun == year)  // Filter berdasarkan JenisBiaya = RUTIN dan tahun
        .Select(w => new
        {
            w.Bulan,
            w.PercentActual,
            w.PercentForecasting
        })
        .OrderBy(w => w.Bulan)  // Mengurutkan berdasarkan bulan
        .ToList();

    return Json(data);
}





public JsonResult GetSumData(int year, string filterType)
{
    // Mengambil data dari tabel WbsData berdasarkan tahun dan filter jenis biaya
    var wbsData = _context.WbsData
        .Where(w => w.JenisBiaya == filterType && w.Tahun == year)
        .ToList();

    // Menghitung SUM untuk CommitmentWbsRt, RopWbsRt, dan ActualWbsRt
    var wbsSumData = new
    {
        commitmentWbsRt = wbsData.Sum(w => w.CommitmentWbsRt),
        ropWbsRt = wbsData.Sum(w => w.RopWbsRt),
        actualWbsRt = wbsData.Sum(w => w.ActualWbsRt)
    };

    // Mengambil data dari tabel BudgetWbsData berdasarkan tahun
    var budgetWbsData = _context.BudgetWbsData
        .Where(b => b.Tahun == year)
        .ToList();

    // Menghitung SUM untuk BudgetWbs
    var budgetSumData = new
    {
        budgetWbs = budgetWbsData.Sum(b => b.BudgetWbs)
    };

    // Menggabungkan hasil dari WbsData dan BudgetWbsData
    var result = new
    {
        wbs = wbsSumData,
        budget = budgetSumData
    };

    return Json(result);
}





public JsonResult GetDashboardData(int tahun)
{


var userClaimDisiplin = User.Claims.FirstOrDefault(c => c.Type == "Disiplin")?.Value;

var amanCount = _context.FasePlanning
    .Where(f => f.Tanggal_Kirim_Paket >= DateTime.Now &&
                f.Kode_Project.EndsWith($"/{tahun}") && f.Disiplin == userClaimDisiplin)
    .Count();


var terlambatCount = _context.FasePlanning
    .Where(f => f.Tanggal_Kirim_Paket < DateTime.Now &&
                f.Kode_Project.EndsWith($"/{tahun}") && f.Disiplin == userClaimDisiplin)
    .Count();



var selesaiCount = (from t in _context.Timelineok
                    join fase in _context.FasePlanning 
                    on t.Kode_Project equals fase.Kode_Project
                    where t.ResumeStatusPekerjaan == "Selesai"
                          && t.Kode_Project.EndsWith($"/{tahun}")
                          && fase.Disiplin == userClaimDisiplin
                    select t).Count();


var totalProjectCount = _context.FasePlanning
    .Where(fp => fp.Kode_Project.EndsWith($"/{tahun}")  // Filter berdasarkan tahun di Kode_Project
                && fp.Disiplin == userClaimDisiplin) // Filter berdasarkan Claim Disiplin
    .Count();





    return Json(new
    {
        jumlahAman = amanCount,
        jumlahTerlambat = terlambatCount,
        jumlahSelesai = selesaiCount,
        totalProject = totalProjectCount,
  
    });
}



public async Task<IActionResult> Tahapan()
{
    int currentYear = DateTime.Now.Year;
    var fasePlanning = await _context.FasePlanning
        .Where(f => f.Tahun == currentYear)
        .ToListAsync();

    return Json(fasePlanning);
}



public IActionResult Timeline(string kodeProject)
{
    var model = _context.Tahapan.Where(t => t.Kode_Project == kodeProject).ToList();
    return PartialView("_TimelinePartial", model);
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




    }
}
