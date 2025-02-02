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

      if (user != null)
      {
        // Buat klaim untuk pengguna
        var claims = new List<Claim>
{
     new("Nama", user.Nama),
    new("Email", user.Email),
    new("Fungsi", user.Fungsi),
    new("Jabatan", user.Jabatan),
     new("Disiplin", user.Disiplin),
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

// [HttpPost]
// public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
// {
//     if (!ModelState.IsValid)
//     {
//         return View(model);
//     }

//     var email = User.Claims.FirstOrDefault(c => c.Type == "Email")?.Value;
//     if (email == null)
//     {
//         return RedirectToAction("Index");
//     }

//     var user = await _context.User.SingleOrDefaultAsync(u => u.Email == email);
//     if (user == null)
//     {
//         return RedirectToAction("Index");
//     }

//     var oldPasswordHash = HashPassword(model.OldPassword);
//     if (user.Pass != oldPasswordHash)
//     {
//         ModelState.AddModelError(string.Empty, "Password lama salah.");
//         return View(model);
//     }

//     if (model.NewPassword != model.ConfirmPassword)
//     {
//         ModelState.AddModelError(string.Empty, "Password baru dan konfirmasi password tidak cocok.");
//         return View(model);
//     }

//     user.Pass = HashPassword(model.NewPassword);
//     _context.Update(user);
//     await _context.SaveChangesAsync();

//     ViewData["Message"] = "Password berhasil diubah.";
//      TempData["SuccessMessage"] = "successfully!";
//     return View();
// }




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





  }
}
