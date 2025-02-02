using Microsoft.AspNetCore.Mvc;
using Plan.Data;
using Plan.Models;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using System.Security.Cryptography;
using System.Text;
using System;
using Microsoft.EntityFrameworkCore;

using Microsoft.AspNetCore.Mvc.Rendering;



namespace Plan.Controllers
{
    [Authorize]
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _context;

        public UserController(ApplicationDbContext context)
        {
            _context = context;
        }

       
        public IActionResult Index()
        {
            var user = _context.User.ToList();
            return View(user);
        }

      
        public IActionResult Detail(int id)
        {
            var user = _context.User.Find(id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

     
        public IActionResult Create()
        {
                   ViewData["Disiplin"] = new SelectList(_context.DisiplinMaster, "Disiplin", "Disiplin");
            return View();
        }





        
       
        [ValidateAntiForgeryToken]
         [HttpPost]
        public IActionResult Create(User user)
        {

       

            if (ModelState.IsValid)
            {
                // Check if username already exists
                bool usernameExists = _context.User.Any(u => u.Pass == user.Email);
                if (usernameExists)
                {
                    TempData["ErrorMessage"] = "Username already exists.";
                    return View(user);
                }

                // Hash the password and save the user
                user.Pass = PasswordHasher.HashPassword(user.Pass);
                _context.User.Add(user);
                _context.SaveChanges();
                TempData["SuccessMessage"] = "User created successfully.";
                
                return RedirectToAction(nameof(Index));
            }
            return View(user);
        }

      
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult FormEdit(int id)
        {

             ViewData["Disiplin"] = new SelectList(_context.DisiplinMaster, "Disiplin", "Disiplin");

            var user = _context.User.Find(id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }
 
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, User user)
        {
            if (id != user.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                 user.Pass = PasswordHasher.HashPassword(user.Pass);
                _context.User.Update(user);
                _context.SaveChanges();
                TempData["SuccessMessage"] = "User updated successfully.";
                return RedirectToAction(nameof(Index));
            }
            return View(user);
        }


        

        [Authorize]
        [HttpGet]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            var user = _context.User.Find(id);
            if (user != null)
            {
                _context.User.Remove(user);
                _context.SaveChanges();
                TempData["SuccessMessage"] = "User deleted successfully.";
            }
            return RedirectToAction(nameof(Index));
        }
    }

    public static class PasswordHasher
    {
        public static string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var bytes = Encoding.UTF8.GetBytes(password);
                var hash = sha256.ComputeHash(bytes);
                return Convert.ToBase64String(hash);
            }
        }
    }
}
