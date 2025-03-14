using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Plan.Data;
using Plan.Models;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;


namespace Plan.Controllers
{
  public class LoginController : Controller
  {
    private readonly ApplicationDbContext _context; // Ganti dengan context Anda

    public LoginController(ApplicationDbContext context)
    {
      _context = context;
    }

    [HttpGet]
    public IActionResult Index()
    {
      return View();
    }

    [HttpPost]
    public async Task<IActionResult> Index(string email, string pass)
    {
      if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(pass))
      {
        ModelState.AddModelError(string.Empty, "Email dan Password wajib diisi.");
        return View();
      }

      // Hash password menggunakan metode yang sudah didefinisikan
      var hashedPassword = HashPassword(pass);

      // Cek pengguna di database
      var user = _context.User.SingleOrDefault(u => u.Email == email && u.Pass == hashedPassword);


      // var semuaDisiplin = user.Disiplin?.Split(',').ToList() ?? new List<string>();
      // var disiplin = semuaDisiplin.FirstOrDefault();






      if (user != null)
      {


        var semuaDisiplin = user.Disiplin?
    .Replace(" ", "") // Hapus semua spasi
    .Split(',')
    .ToList() ?? new List<string>();

var disiplin = semuaDisiplin.FirstOrDefault();


        // Buat klaim untuk pengguna
        var claims = new List<Claim>
{
     new("Nama", user.Nama),
    new("Email", user.Email),
    new("Fungsi", user.Fungsi),
    new("Jabatan", user.Jabatan),
     new("Disiplin", disiplin),
      new ("SemuaDisiplin", string.Join(",", semuaDisiplin)), // Semua Disiplin disimpan di klaim

    new("Rule", user.Rule.ToString())
};


        // Membuat klaim identitas
        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

        // Atur properti autentikasi
        var authProperties = new AuthenticationProperties
        {
          IsPersistent = true // Cookie akan bertahan setelah browser ditutup
        };

        // Login dengan klaim
        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(claimsIdentity),
            authProperties);

        //   return RedirectToAction("Index", "Home");



        // Redirect based on the "Disiplin" claim
        if (user.Disiplin == "All")
        {
          return RedirectToAction("Index", "Home");  // Redirect to Home if Disiplin is "All"
        }
        else
        {
          return RedirectToAction("Index", "Home2"); // Redirect to Home2 if Disiplin is not "All"
        }


      }
      else
      {
        ModelState.AddModelError(string.Empty, "Email atau Password salah.");
        return View();
      }
    }

    [HttpPost]
    public async Task<IActionResult> Logout()
    {
      // Logout dan hapus cookie
      await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
      return RedirectToAction("Index");
    }

    public IActionResult AccessDenied()
    {
      return View("AccessDenied");
    }

    // Metode untuk menghash password menggunakan SHA-256
    private static string HashPassword(string password)
    {
      using (var sha256 = SHA256.Create())
      {
        var bytes = Encoding.UTF8.GetBytes(password);
        var hash = sha256.ComputeHash(bytes);
        return Convert.ToBase64String(hash);
      }
    }







    [HttpGet]
    public IActionResult ChangePassword()
    {
      return View();
    }






    [HttpPost]
    public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
    {
      if (!ModelState.IsValid)
      {
        return View(model);
      }

      var email = User.Claims.FirstOrDefault(c => c.Type == "Email")?.Value;
      if (email == null)
      {
        return RedirectToAction("Index");
      }

      var user = await _context.User.SingleOrDefaultAsync(u => u.Email == email);
      if (user == null)
      {
        return RedirectToAction("Index");
      }

      // Verify the old password
      var oldPasswordHash = HashPassword(model.OldPassword);
      if (user.Pass != oldPasswordHash)
      {
        ModelState.AddModelError(string.Empty, "Password lama salah.");
        return View(model);
      }

      // Check if the new password matches the confirm password
      if (model.NewPassword != model.ConfirmPassword)
      {
        ModelState.AddModelError(string.Empty, "Password baru dan konfirmasi password tidak cocok.");
        return View(model);
      }

      // Save the raw password to the Pass field (unencrypted version)
      user.RetypePass = model.NewPassword;

      // Save the hashed password in the Pass field for security
      user.Pass = HashPassword(model.NewPassword);

      _context.Update(user);
      await _context.SaveChangesAsync();

      ViewData["Message"] = "Password berhasil diubah.";
      TempData["SuccessMessage"] = "successfully!";
      return View();
    }



    // [HttpPost]
    // public async Task<IActionResult> SetDisiplinAktif(string disiplin)
    // {
    //   var userClaims = User.Claims.ToList();



    //   // Update klaim dengan DisiplinAktif baru
    //   var newClaims = userClaims.Where(c => c.Type != "Disiplin").ToList();
    //   newClaims.Add(new Claim("Disiplin", disiplin));

    //   var claimsIdentity = new ClaimsIdentity(newClaims, CookieAuthenticationDefaults.AuthenticationScheme);
    //   var authProperties = new AuthenticationProperties { IsPersistent = true };

    //   await HttpContext.SignInAsync(
    //       CookieAuthenticationDefaults.AuthenticationScheme,
    //       new ClaimsPrincipal(new ClaimsIdentity(claimsIdentity)),
    //       authProperties
    //   );

    //   return Ok();
    // }





[HttpPost]
public async Task<IActionResult> ReloginWithDisiplin(string disiplin)
{
    if (string.IsNullOrWhiteSpace(disiplin))
    {
        return BadRequest("Disiplin tidak boleh kosong.");
    }

    disiplin = disiplin.Trim(); // Pastikan tidak ada spasi tambahan

    var email = User.Claims.FirstOrDefault(c => c.Type == "Email")?.Value;
    if (email == null) return Unauthorized();

    var hashedPassword = _context.User
        .Where(u => u.Email == email)
        .Select(u => u.Pass)
        .FirstOrDefault();

    if (hashedPassword == null) return Unauthorized();

    var user = _context.User.SingleOrDefault(u => u.Email == email && u.Pass == hashedPassword);
    if (user == null) return Unauthorized();

    var semuaDisiplin = user.Disiplin?
        .Split(',')
        .Select(d => d.Trim()) // Trim setiap item dalam daftar
        .ToList() ?? new List<string>();

    Console.WriteLine($"Semua Disiplin User: {string.Join(", ", semuaDisiplin)}");
    Console.WriteLine($"Disiplin yang diterima: '{disiplin}'");

    if (!semuaDisiplin.Contains(disiplin))
    {
        return BadRequest("Disiplin tidak valid.");
    }

    // Buat ulang klaim
    var claims = new List<Claim>
    {
        new("Nama", user.Nama),
        new("Email", user.Email),
        new("Fungsi", user.Fungsi),
        new("Jabatan", user.Jabatan),
        new("Disiplin", disiplin), // Disiplin baru dipilih
        new("SemuaDisiplin", string.Join(",", semuaDisiplin)), // Semua Disiplin dalam klaim
        new("Rule", user.Rule.ToString())
    };

    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
    var authProperties = new AuthenticationProperties { IsPersistent = true };

    // Logout sebelum login ulang untuk memperbarui klaim
    await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);

    return RedirectToAction("Index", "Home");
}






  }
}
