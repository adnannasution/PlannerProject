using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Plan.Data;  // Ganti dengan namespace aplikasi Anda
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpContextAccessor();


// Konfigurasi DbContext dengan string koneksi dari appsettings.json
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


    // Tambahkan layanan untuk Cookie Authentication
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Login"; // Halaman login jika belum login
        options.LogoutPath = "/Login/Logout"; // Halaman logout
        options.AccessDeniedPath = "/Login/AccessDenied"; // Halaman akses ditolak
        options.ExpireTimeSpan = TimeSpan.FromMinutes(30); // Waktu kedaluwarsa cookie
    });

// Tambahkan layanan untuk MVC
builder.Services.AddControllersWithViews();

var app = builder.Build();


// Middleware untuk menangani rute yang tidak ditemukan
app.Use(async (context, next) =>
{
    await next();

    if (context.Response.StatusCode == 404)
    {
        context.Request.Path = "/Home/NotFound"; // Redirect ke halaman NotFound
        await next();
    }
});


// Konfigurasi HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();  // Menambahkan HSTS (HTTP Strict Transport Security)
}

app.UseHttpsRedirection();  // Mengarahkan HTTP ke HTTPS
app.UseStaticFiles();  // Menyajikan file statis seperti CSS dan JS

app.UseRouting();  // Menyusun rute

app.UseAuthentication();  // Menyertakan autentikasi jika diperlukan
app.UseAuthorization();  // Menyertakan otorisasi



// Konfigurasi rute aplikasi
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();  // Menjalankan aplikasi
