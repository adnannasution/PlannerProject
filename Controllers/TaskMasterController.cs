using Microsoft.AspNetCore.Mvc;
using Plan.Data;
using Plan.Models;
using System.Linq;
using Microsoft.AspNetCore.Authorization;

namespace Plan.Controllers
{
     [Authorize]
    public class TaskMasterController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TaskMasterController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: TaskMaster
        public IActionResult Index()
        {
            var tasks = _context.TaskMaster.ToList();
            return View(tasks);
        }

        // GET: TaskMaster/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: TaskMaster/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(TaskMaster taskMaster)
        {
            if (ModelState.IsValid)
            {
                _context.TaskMaster.Add(taskMaster);
                _context.SaveChanges();
                 TempData["SuccessMessage"] = "successfully!";
                return RedirectToAction(nameof(Index));
            }
            return View(taskMaster);
        }

                     [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult FormEdit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var taskMaster = _context.TaskMaster.Find(id);
            if (taskMaster == null)
            {
                return NotFound();
            }

            return View(taskMaster);
        }

        // POST: TaskMaster/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, TaskMaster taskMaster)
        {
            if (id != taskMaster.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                _context.TaskMaster.Update(taskMaster);
                _context.SaveChanges();
                 TempData["SuccessMessage"] = "successfully!";
                return RedirectToAction(nameof(Index));
            }
            return View(taskMaster);
        }

        // GET: TaskMaster/Delete/5
        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var taskMaster = _context.TaskMaster.Find(id);
            if (taskMaster == null)
            {
                return NotFound();
            }

            return View(taskMaster);
        }

        // POST: TaskMaster/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var taskMaster = _context.TaskMaster.Find(id);
            if (taskMaster != null)
            {
                _context.TaskMaster.Remove(taskMaster);
                _context.SaveChanges();
            }
 TempData["SuccessMessage"] = "successfully!";
            return RedirectToAction(nameof(Index));
        }
    }
}
