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



 
//  public JsonResult GetResumeStatusPekerjaanData(string tahunresume)
//     {
//         var data = _context.Timelineok
//             .Where(t => t.Tanggal.Year.ToString() == tahunresume)  // Filter berdasarkan tahun dari kolom Tanggal
//             .GroupBy(t => t.ResumeStatusPekerjaan ?? "Tidak Diketahui")
//             .Select(g => new
//             {
//                 ResumeStatusPekerjaan = g.Key,
//                 Count = g.Count()
//             })
//             .ToList();

//         return Json(data);
//     }

public JsonResult GetResumeStatusPekerjaanData(string tahunresume)
{
    // Mendapatkan klaim Disiplin dari User
    //var disiplin = User.Claims.FirstOrDefault(c => c.Type == "Disiplin")?.Value;

    var data = _context.Timelineok
        .Join(_context.FasePlanning,
              t => t.Kode_Project,   // Kolom untuk join dari tabel Timelineok
              f => f.Kode_Project,   // Kolom untuk join dari tabel FasePlanning
              (t, f) => new { t, f })  // Menggabungkan hasil join
        .Where(tf => tf.t.Tanggal.Year.ToString() == tahunresume)  // Filter berdasarkan tahun dan Disiplin
        .Select(tf => new 
        { 
            tf.t.Kode_Project, 
            tf.t.ResumeStatusPekerjaan 
        }) // Ambil hanya kolom yang relevan
        .Distinct() // Hilangkan duplikasi berdasarkan Kode_Project
        .GroupBy(tf => tf.ResumeStatusPekerjaan ?? "Tidak Diketahui")  // Ganti nilai null dengan "Tidak Diketahui"
        .Select(g => new
        {
            ResumeStatusPekerjaan = g.Key,
            Count = g.Count()  // Hitung jumlah Kode_Project unik
        })
        .ToList();

    return Json(data);
}

// public IActionResult DetailFasePlanning(string status, int tahun) 
// {
//     var detailData = (from fp in _context.FasePlanning
//                       join t in _context.Timelineok on fp.Kode_Project equals t.Kode_Project
//                       where t.ResumeStatusPekerjaan == status && t.Tanggal.Year == tahun
//                       select new{
//                           fp.Kode_Project,
//                           fp.Disiplin,
//                           fp.Pekerjaan,
//                           fp.Kategori_Equipment,
//                           fp.Kategori_Maintenance,
//                           fp.Area,
//                           fp.Status_kontrak,
//                           fp.Kategori_Kontrak,
//                           fp.Direksi,
//                           fp.Jenis_Kontrak,
//                           fp.Planner,
//                           fp.No_Memo_Rekomendasi,
//                           fp.Tanggal_Masuk_Memo,
//                           fp.No_Memo_Kirim_Paket,
//                           fp.Tanggal_Kirim_Paket,
//                           fp.Status_Next_Contract,
//                           fp.Informasi_Detail,
//                           fp.Nomor_PR_Kontrak_Baru,
//                           fp.Tanggal_Update,
//                           fp.Tahun,
//                           fp.Tipe_Project,
//                           fp.Jenis_Project,
//                           t.Task,
//                           t.Pic,
//                           t.Tanggal,
//                           t.Target,
//                           t.Dokumen,
//                           t.ResumeStatusPekerjaan,
//                           t.Status
//                       }).ToList();

//     return View(detailData);
// }


public IActionResult DetailFasePlanning(string status, int tahun) 
{
    //var disiplin = User.Claims.FirstOrDefault(c => c.Type == "Disiplin")?.Value;

    var detailData = _context.FasePlanning
        .Join(_context.Timelineok, 
            fp => fp.Kode_Project, 
            t => t.Kode_Project, 
            (fp, t) => new { fp, t })
        .Where(joined => joined.t.ResumeStatusPekerjaan == status 
                         && joined.t.Tanggal.Year == tahun 
                         )
        .OrderByDescending(joined => joined.t.Id) // Urutkan sebelum grup
        .AsEnumerable() // Konversi ke memory untuk pengolahan LINQ yang tidak bisa di-translate ke SQL
        .GroupBy(joined => joined.fp.Kode_Project) // Grup berdasarkan Kode_Project
        .Select(g => g.FirstOrDefault()) // Ambil entri pertama dari setiap grup
        .Select(e => new {
            e.fp.Kode_Project,
            e.fp.Disiplin,
            e.fp.Pekerjaan,
            e.fp.Kategori_Equipment,
            e.fp.Kategori_Maintenance,
            e.fp.Area,
            e.fp.Status_kontrak,
            e.fp.Kategori_Kontrak,
            e.fp.Direksi,
            e.fp.Jenis_Kontrak,
            e.fp.Planner,
            e.fp.No_Memo_Rekomendasi,
            e.fp.Tanggal_Masuk_Memo,
            e.fp.No_Memo_Kirim_Paket,
            e.fp.Tanggal_Kirim_Paket,
            e.fp.Status_Next_Contract,
            e.fp.Informasi_Detail,
            e.fp.Nomor_PR_Kontrak_Baru,
            e.fp.Tanggal_Update,
            e.fp.Tahun,
            e.fp.Tipe_Project,
            e.fp.Jenis_Project,
            e.t.Task,
            e.t.Pic,
            e.t.Tanggal,
            e.t.Target,
            e.t.Dokumen,
            e.t.ResumeStatusPekerjaan,
            e.t.Status
        })
        .ToList(); // Eksekusi query

    return View(detailData);
}



// public IActionResult DetailFaseTender(string status, int tahun)
// {
//     var data = from ft in _context.FaseTender
//                join tl in _context.Timelineok
//                on ft.Kode_Project equals tl.Kode_Project
//                join f in _context.FasePlanning
//                on ft.Kode_Project equals f.Kode_Project  // Join with FasePlanning
//                where tl.ResumeStatusPekerjaan == status && tl.Tanggal.Year == tahun
//                select new 
//                {
//                    ft.Id,
//                    ft.Kode_Project,
//                    f.Pekerjaan,
//                    ft.No_Vendor,
//                    ft.Pelaksana,
//                    ft.PO_OA,
//                    ft.PO_SR,
//                    ft.PO_RO,
//                    ft.PR_OA,
//                    ft.PR_MT,
//                    ft.PR_SR,
//                    ft.Nomor_SP3MK,
//                    ft.Nilai_Purchasing_Order,
//                    ft.Nilai_Purchasing_Request,
//                    ft.Durasi_Masa_Penyelesaian_MPL,
//                    ft.Bulan,
//                    ft.Otorisasi,
//                    ft.Notif,
//                    ft.Buyer,
//                    tl.ResumeStatusPekerjaan,
//                    tl.Task,
//                    tl.Pic,
//                    tl.Tanggal,
//                    tl.Target
//                };

//     var result = data.ToList();
//     return View("DetailFaseTender", result);

// }





public IActionResult DetailFaseTender(string status, int tahun)
{
   // var disiplin = User.Claims.FirstOrDefault(c => c.Type == "Disiplin")?.Value;

    var detailData = _context.FaseTender
        .Join(_context.Timelineok, 
            ft => ft.Kode_Project, 
            t => t.Kode_Project, 
            (ft, t) => new { ft, t })
        .Join(_context.FasePlanning, 
            joined => joined.ft.Kode_Project, 
            fp => fp.Kode_Project, 
            (joined, fp) => new { joined.ft, joined.t, fp })
        .Where(joined => joined.t.ResumeStatusPekerjaan == status 
                         && joined.t.Tanggal.Year == tahun 
                        )
        .OrderByDescending(joined => joined.ft.Id) // Urutkan berdasarkan Id DESC sebelum grup
        .AsEnumerable() // Konversi ke memory untuk pengolahan LINQ yang tidak bisa di-translate ke SQL
        .GroupBy(joined => joined.ft.Kode_Project) // Grup berdasarkan Kode_Project
        .Select(g => g.FirstOrDefault()) // Ambil entri pertama dari setiap grup (Id terbesar)
        .Select(e => new {
            e.ft.Id,
            e.ft.Kode_Project,
            e.fp.Pekerjaan,
            e.ft.No_Vendor,
            e.ft.Pelaksana,
            e.ft.PO_OA,
            e.ft.PO_SR,
            e.ft.PO_RO,
            e.ft.PR_OA,
            e.ft.PR_MT,
            e.ft.PR_SR,
            e.ft.Nomor_SP3MK,
            e.ft.Nilai_Purchasing_Order,
            e.ft.Nilai_Purchasing_Request,
            e.ft.Durasi_Masa_Penyelesaian_MPL,
            e.ft.Bulan,
            e.ft.Otorisasi,
            e.ft.Notif,
            e.ft.Buyer,
            e.t.ResumeStatusPekerjaan,
            e.t.Task,
            e.t.Pic,
            e.t.Tanggal,
            e.t.Target
        })
        .ToList(); // Eksekusi query

    return View(detailData);
}



// public IActionResult DetailFaseExecution(string status, int tahun)
// {
//     var data = from fe in _context.FaseExecution
//                join tl in _context.Timelineok
//                on fe.Kode_Project equals tl.Kode_Project
//                join fp in _context.FasePlanning
//                on fe.Kode_Project equals fp.Kode_Project  // Join with FasePlanning
//                where tl.ResumeStatusPekerjaan == status && tl.Tanggal.Year == tahun
//                select new 
//                {
//                    fe.Id,
//                    fe.Kode_Project,
//                    fp.Pekerjaan,
//                    fe.Start_Date,
//                    fe.End_Date_MPL,
//                    fe.Amandemen_Waktu,
//                    fe.End_Date,
//                    fe.Durasi_Kontrak,
//                    fe.Durasi_Aktual_HK,
//                    fe.Date_LKP,
//                    fe.Plan_Progress_Fisik,
//                    fe.Progress_Fisik_0,
//                    fe.Progress_Fisik,
//                    fe.Status_Performance,
//                    fe.Progress_Finansial,
//                    fe.LKP_Time_Limit,
//                    tl.ResumeStatusPekerjaan,
//                    tl.Task,
//                    tl.Pic,
//                    tl.Tanggal,
//                    tl.Target
//                };

//     var result = data.ToList();
//      return View("DetailFaseExecution", result);
// }



public IActionResult DetailFaseExecution(string status, int tahun)
{
    var disiplin = User.Claims.FirstOrDefault(c => c.Type == "Disiplin")?.Value;

    var detailData = _context.FaseExecution
        .Join(_context.Timelineok,
            fe => fe.Kode_Project,
            t => t.Kode_Project,
            (fe, t) => new { fe, t })
        .Join(_context.FasePlanning,
            joined => joined.fe.Kode_Project,
            fp => fp.Kode_Project,
            (joined, fp) => new { joined.fe, joined.t, fp })
        .Where(joined => joined.t.ResumeStatusPekerjaan == status
                         && joined.t.Tanggal.Year == tahun
                         )
        .OrderByDescending(joined => joined.fe.Id) // Urutkan berdasarkan Id DESC sebelum grup
        .AsEnumerable() // Konversi ke memory untuk pengolahan LINQ yang tidak bisa di-translate ke SQL
        .GroupBy(joined => joined.fe.Kode_Project) // Grup berdasarkan Kode_Project
        .Select(g => g.FirstOrDefault()) // Ambil entri pertama dari setiap grup (Id terbesar)
        .Select(e => new {
            e.fe.Id,
            e.fe.Kode_Project,
            e.fp.Pekerjaan,
            e.fe.Start_Date,
            e.fe.End_Date_MPL,
            e.fe.Amandemen_Waktu,
            e.fe.End_Date,
            e.fe.Durasi_Kontrak,
            e.fe.Durasi_Aktual_HK,
            e.fe.Date_LKP,
            e.fe.Plan_Progress_Fisik,
            e.fe.Progress_Fisik_0,
            e.fe.Progress_Fisik,
            e.fe.Status_Performance,
            e.fe.Progress_Finansial,
            e.fe.LKP_Time_Limit,
            e.t.ResumeStatusPekerjaan,
            e.t.Task,
            e.t.Pic,
            e.t.Tanggal,
            e.t.Target
        })
        .ToList(); // Eksekusi query

    return View("DetailFaseExecution", detailData);
}




public IActionResult DetailResumeStatusPekerjaan(string status, int tahun)
{
    var detailData = from t in _context.Timelineok
                     join f in _context.FasePlanning
                     on t.Kode_Project equals f.Kode_Project  // Join with FasePlanning
                     where t.ResumeStatusPekerjaan == status && t.Tanggal.Year == tahun
                     select new
                     {
                         t.Kode_Project,
                         f.Pekerjaan, // Example field from Timelineok
                         t.ResumeStatusPekerjaan,
                         t.Task,
                         t.Tanggal

                     };

    return View(detailData.ToList());
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
    var sixMonthsAgo = DateTime.Now.AddMonths(-6);

    var amanCount = _context.TabelSla
        .Where(s => s.Task == "Tanggal Kirim Paket" && s.Target >= DateTime.Now && s.Kode_Project.EndsWith($"/{tahun}"))
        .Count();

    var terlambatCount = _context.TabelSla
        .Where(s => s.Task == "Tanggal Kirim Paket" && s.Target < DateTime.Now && s.Kode_Project.EndsWith($"/{tahun}"))
        .Count();

    var selesaiCount = _context.Timelineok
        .Where(t => t.ResumeStatusPekerjaan == "Selesai" && t.Kode_Project.EndsWith($"/{tahun}"))
        .Count();

    // Menghitung total proyek berdasarkan tahun
    var totalProjectCount = _context.FasePlanning
        .Where(fp => fp.Kode_Project.EndsWith($"/{tahun}"))  // Filter berdasarkan tahun di Kode_Project
        .Count();

    var totalProjectLastSixMonths = _context.FasePlanning
        .Where(fp => fp.Tanggal_Kirim_Paket >= sixMonthsAgo && fp.Kode_Project.EndsWith($"/{tahun}"))  // Filter tahun
        .Count();

    return Json(new
    {
        jumlahAman = amanCount,
        jumlahTerlambat = terlambatCount,
        jumlahSelesai = selesaiCount,
        totalProject = totalProjectCount,
        totalProjectLastSixMonths = totalProjectLastSixMonths
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


    }
}
