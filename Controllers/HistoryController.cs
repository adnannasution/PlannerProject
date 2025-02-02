using Microsoft.AspNetCore.Mvc;
using Plan.Data; // Ensure this namespace matches your DbContext
using Plan.Models;
using System.IO;
using System.Linq;
using OfficeOpenXml;
using Microsoft.AspNetCore.Authorization;

namespace Plan.Controllers
{
    [Authorize]
    public class HistoryController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HistoryController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: History/Import
        public IActionResult Import()
        {
            return View();
        }

        // POST: Import Excel Data
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Import(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                ModelState.AddModelError(string.Empty, "Please upload a valid Excel file.");
                return View();
            }

            try
            {
                using (var stream = new MemoryStream())
                {
                    file.CopyTo(stream);
                    stream.Position = 0;

                    using (var package = new ExcelPackage(stream))
                    {
                        var worksheet = package.Workbook.Worksheets[0];
                        var rowCount = worksheet.Dimension.Rows;

                        for (int row = 2; row <= rowCount; row++) // Skip header
                        {
                            int tahun = int.Parse(worksheet.Cells[row, 1].Value?.ToString() ?? "0");
                            string bulan = worksheet.Cells[row, 2].Value?.ToString();
                            string minggu = worksheet.Cells[row, 3].Value?.ToString();
                            float nominal = float.Parse(worksheet.Cells[row, 4].Value?.ToString() ?? "0");

                            // Check if the record exists
                            var existingRecord = _context.History
                                .FirstOrDefault(h => h.Tahun == tahun && h.Bulan == bulan && h.Minggu == minggu);

                            if (existingRecord != null)
                            {
                                // Update existing record
                                existingRecord.Nominal = nominal;
                            }
                            else
                            {
                                // Add new record
                                var history = new History
                                {
                                    Tahun = tahun,
                                    Bulan = bulan,
                                    Minggu = minggu,
                                    Nominal = nominal
                                };

                                _context.History.Add(history);
                            }
                        }

                        _context.SaveChanges();
                    }
                }

                TempData["SuccessMessage"] = "Data imported successfully.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Error: {ex.Message}");
                return View();
            }
        }

        // POST: Clear Table
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ClearTable()
        {
            try
            {
                _context.History.RemoveRange(_context.History);
                _context.SaveChanges();

                TempData["SuccessMessage"] = "All records have been cleared.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error: {ex.Message}";
            }

            return RedirectToAction("Index");
        }

        // GET: History/Index
        public IActionResult Index()
        {
            var data = _context.History.ToList();
            return View(data);
        }
    }
}
