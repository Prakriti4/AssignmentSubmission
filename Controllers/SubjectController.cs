using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NuGet.DependencyResolver;
using SchoolManagementSystem.Data;
using SchoolManagementSystem.Models;

namespace SchoolManagementSystem.Controllers
{
    public class SubjectController : Controller
    {
        private readonly SchoolIdentityDbcontext _context;
        public SubjectController(SchoolIdentityDbcontext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            var subjects = await _context.Subjects
            .Include(s => s.Faculty)
            .Include(s => s.Semester)
            .ToListAsync();

            return View(subjects);
        }
        public IActionResult Create()
        {
            ViewBag.Faculties = new SelectList(_context.Faculties, "Id", "Name");
            ViewBag.Semesters = new SelectList(_context.Semesters, "Id", "Name");
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(Subject subject)
        {
            ViewBag.Faculty = new SelectList(_context.Faculties.ToList(), "Id", "Name");
            ViewBag.Semesters = new SelectList(_context.Semesters, "Id", "Name");
            if (ModelState.IsValid)
            {
                _context.Add(subject);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(subject);

        }
        public async Task<IActionResult> Edit(int id)
        {
            ViewBag.Faculties = new SelectList(_context.Faculties, "Id", "Name");
            ViewBag.Semesters = new SelectList(_context.Semesters, "Id", "Name");
            var model = await _context.Subjects.FindAsync(id);
            if (model == null)
            {
                return NotFound();
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Subject subject)
        {
            ViewBag.Faculty = new SelectList(_context.Faculties, "Id", "Name");
            ViewBag.Semesters = new SelectList(_context.Semesters, "Id", "Name");
            if (ModelState.IsValid)
            {
                _context.Subjects.Update(subject);
                _context.SaveChanges();
                TempData["SuccessMessage"] = "Teacher record updated successfully";
                return RedirectToAction("Index");
            }
            TempData["ErrorMessage"] = "There was an error updating the record";
            return View(subject);
        }


        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var model = await _context.Subjects.FindAsync(id);
            if (model == null)
            {
                return NotFound();
            }
            return View(model);
        }
        [HttpPost]
        public IActionResult Delete(Subject subject)
        {
            var model = _context.Subjects.Remove(subject);
            _context.SaveChanges();
            TempData["SuccessMessage"] = "Subject record deleted successfully";
            return RedirectToAction("Index");
        
        }

    }
}
