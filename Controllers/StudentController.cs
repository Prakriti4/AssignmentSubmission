using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NuGet.DependencyResolver;
using SchoolManagementSystem.Data;
using SchoolManagementSystem.Models;

namespace SchoolManagementSystem.Controllers
{
    [Authorize(Roles = "Admin")]
    public class StudentController : Controller
    {
        private readonly SchoolIdentityDbcontext _context;
        public StudentController(SchoolIdentityDbcontext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            var students = await _context.Students
            .Include(s => s.Faculty)
            .Include(s => s.Semester)
            .Include(s=>s.Section)
            .ToListAsync();

            return View(students);
        }
        public async Task<IActionResult> Create()
        {
            ViewBag.Faculties = new SelectList(_context.Faculties, "Id", "Name");
            ViewBag.Semesters = new SelectList(_context.Semesters, "Id", "Name");
            ViewBag.Sections = new SelectList(_context.Sections, "Id", "SectionName");
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(Student student)
        {
            ViewBag.Faculties = new SelectList(_context.Faculties, "Id", "Name");
            ViewBag.Semesters = new SelectList(_context.Semesters, "Id", "Name");
            ViewBag.Sections = new SelectList(_context.Sections, "Id", "SectionName");
            if (ModelState.IsValid)
            {
                string uniqueFileName=UploadedFile(student);
                student.ImageUrl = uniqueFileName;
                _context.Add(student);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(student);

        }
        public string UploadedFile(Student model)
        {
            string uniqueFileName = null;
            if (model.Image != null)
            {
                string uploadFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");
                uniqueFileName = Guid.NewGuid().ToString() + "_" + model.Image.FileName;
                string filePath = Path.Combine(uploadFolder, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    model.Image.CopyTo(fileStream);
                }
            }
            return uniqueFileName;
        }
        public async Task<IActionResult> Edit(string id)
        {
            ViewBag.Faculties = new SelectList(_context.Faculties, "Id", "Name");
            ViewBag.Semesters = new SelectList(_context.Semesters, "Id", "Name");
            ViewBag.Sections = new SelectList(_context.Sections, "Id", "SectionName");
            var model = await _context.Students.FindAsync(id);
            if (model == null)
            {
                return NotFound();
            }
            return View(model);
        }

[HttpPost]
public async Task<IActionResult> Edit(string id, Student student)
{
    ViewBag.Faculties = new SelectList(_context.Faculties, "Id", "Name");
    ViewBag.Semesters = new SelectList(_context.Semesters, "Id", "Name");
    ViewBag.Sections = new SelectList(_context.Sections, "Id", "SectionName");

    var existingStudent = await _context.Students.FindAsync(id);
    if (existingStudent == null)
    {
        return NotFound();
    }

    // Update properties of the existing student
    existingStudent.Name = student.Name;
    existingStudent.Email = student.Email;
    existingStudent.DateofBirth = student.DateofBirth;
    existingStudent.Address = student.Address;

    // Handle file upload if there is a new image
    string uniqueFile = UploadedFile(student);
    if (!string.IsNullOrEmpty(uniqueFile))
    {
        existingStudent.ImageUrl = uniqueFile;
    }

    // Update the student in the context
    _context.Students.Update(existingStudent);
    await _context.SaveChangesAsync();

    TempData["SuccessMessage"] = "Student record updated successfully";
    return RedirectToAction("Index");
}
        public async Task<IActionResult> Detail(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var student = await _context.Students.FindAsync(id);

            if (student == null)
            {
                return NotFound();
            }

            return View(student);
        }



        // GET: Show the delete confirmation view
        [HttpGet]
        public async Task<IActionResult> Delete(string id)
        {
            ViewBag.Faculties = new SelectList(_context.Faculties, "Id", "Name");
            ViewBag.Semesters = new SelectList(_context.Semesters, "Id", "Name");
            ViewBag.Sections = new SelectList(_context.Sections, "Id", "SectionName");
            var student = await _context.Students.FindAsync(id);

            if (student == null)
            {
                return NotFound();
            }

            // Pass the student to the view for confirmation
            return View(student);
        }

        // POST: Perform the actual delete operation
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            ViewBag.Faculties = new SelectList(_context.Faculties, "Id", "Name");
            ViewBag.Semesters = new SelectList(_context.Semesters, "Id", "Name");
            ViewBag.Sections = new SelectList(_context.Sections, "Id", "SectionName");
            // Retrieve the student again by id
            var student = await _context.Students.FindAsync(id);

            if (student == null)
            {
                return NotFound();
            }

            // Remove the student from the database
            _context.Students.Remove(student);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Student record deleted successfully";
            return RedirectToAction("Index");
        }

    }
}
