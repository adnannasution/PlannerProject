using Microsoft.AspNetCore.Mvc;
using System.Linq;
using Plan.Models;
using Plan.Data;
using Microsoft.AspNetCore.Authorization;

 [Authorize]
public class ResumeController : Controller
{
   
    private readonly ApplicationDbContext _context;

    public ResumeController(ApplicationDbContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        // Query untuk menghitung total Nilai_Tagihan per Disiplin
        var resumeData = (from fase in _context.FasePlanning
                          join termin in _context.Termin on fase.Kode_Project equals termin.Kode_Project
                          group termin by fase.Disiplin into g
                          select new
                          {
                              Disiplin = g.Key,
                              TotalTagihan = g.Sum(x => x.Nilai_Tagihan)
                          }).ToList();

        ViewBag.ResumeData = resumeData;
        return View();
    }

    public IActionResult Details(string disiplin)
    {
        // Query untuk mengambil detail data berdasarkan Disiplin tertentu
        var detailData = (from fase in _context.FasePlanning
                          join termin in _context.Termin on fase.Kode_Project equals termin.Kode_Project
                          where fase.Disiplin == disiplin
                          select new
                          {
                              termin.Kode_Project,
                              termin.JenisTermin,
                              termin.Nilai_Tagihan,
                              termin.No_WO_Tagihan,
                              termin.Periode
                          }).ToList();

        ViewBag.Disiplin = disiplin;
        ViewBag.DetailData = detailData;
        return View();
    }
}
