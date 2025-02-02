using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Plan.Data;
using Plan.Models;
using System.Linq;
using System.Threading.Tasks;
using OfficeOpenXml;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json;

namespace Plan.Controllers
{

public class TahapanController : Controller
{
    private readonly ApplicationDbContext _context;

    public TahapanController(ApplicationDbContext context)
    {
        _context = context;
    }



// public async Task<IActionResult> Index()
// {
//     // Ambil tahun saat ini
//     int currentYear = DateTime.Now.Year;

//     // Ambil data FasePlanning yang tahunnya sesuai dengan tahun berjalan
//     var fasePlanning = await _context.FasePlanning
//         .Where(f => f.Tahun == currentYear)  // Filter berdasarkan tahun
//         .ToListAsync();

//     return View(fasePlanning);
// }


public async Task<IActionResult> Index()
{
    // Ambil claim disiplin pengguna yang sedang login
    var claimDisiplin = User.Claims.FirstOrDefault(c => c.Type == "Disiplin")?.Value;

    if (string.IsNullOrEmpty(claimDisiplin))
    {
        // Jika claim disiplin tidak ada, bisa memberi respon yang sesuai
        return Unauthorized();
    }

    IQueryable<FasePlanning> query = _context.FasePlanning
        .OrderByDescending(f => f.Id); // Urutkan berdasarkan Id secara descending

    if (claimDisiplin != "All")
    {
        // Filter berdasarkan claim disiplin jika tidak "All"
        query = query.Where(f => f.Disiplin == claimDisiplin);
    }

    var fasePlanning = await query
        //.Take(10) // Ambil hanya 10 baris data
        .ToListAsync();

    return View(fasePlanning);
}



public async Task<IActionResult> TahapanAll()
{
    // Ambil claim disiplin pengguna yang sedang login
    var claimDisiplin = User.Claims.FirstOrDefault(c => c.Type == "Disiplin")?.Value;

    if (string.IsNullOrEmpty(claimDisiplin))
    {
        // Jika claim disiplin tidak ada, bisa memberi respon yang sesuai
        return Unauthorized();
    }

    IQueryable<FasePlanning> query = _context.FasePlanning
        .OrderByDescending(f => f.Id); // Urutkan berdasarkan Id secara descending

    if (claimDisiplin != "All")
    {
        // Filter berdasarkan claim disiplin jika tidak "All"
        query = query.Where(f => f.Disiplin == claimDisiplin);
    }

    var fasePlanning = await query
        .ToListAsync();

    return View(fasePlanning);
}





    // public async Task<IActionResult> Timeline(string kodeProject)
    // {
    //     var tahapanDetails = await _context.Tahapan
    //         .Where(t => t.Kode_Project == kodeProject)
    //         .OrderBy(t => t.Tanggal)
    //         .ToListAsync();

    //     return View(tahapanDetails);
    // }


public IActionResult Timeline(string kodeProject)
{
    var model = _context.Tahapan.Where(t => t.Kode_Project == kodeProject).ToList();
    return PartialView("_TimelinePartial", model);
}




    
}


}