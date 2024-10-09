using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Data;
using SchoolManagementSystem.Models;
using SchoolManagementSystem.ViewModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace SchoolManagementSystem.Controllers
{
    [Authorize(Roles = "Admin")]
    public class SectionController : Controller
    {
        private readonly SchoolIdentityDbcontext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private const int MaxStudentsPerSection = 5;

        public SectionController(SchoolIdentityDbcontext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<IActionResult> Index()
        {
            var sections = await _context.Sections
                .Include(s => s.Semester)
                .Include(s => s.Faculty)
                .ToListAsync();
            return View(sections);
        }

        public async Task<IActionResult> Section(int semesterId)
        {
            var semester = await _context.Semesters.FindAsync(semesterId);
            if (semester == null)
            {
                return NotFound($"Semester with ID {semesterId} not found.");
            }

            // Retrieve sections for the specific semester
            var sections = await _context.Sections
                .Include(s => s.Students)
                .Where(s => s.SemesterId == semesterId)
                .ToListAsync();

            // Prepare SectionViewModel
            var sectionViewModel = new List<SectionViewModel>();
            foreach (var section in sections)
            {
                sectionViewModel.Add(new SectionViewModel
                {
                    SectionId = section.Id,
                    SectionName = section.SectionName,
                    SemesterId = section.SemesterId,
                    FacultyId = section.FacultyId,
                    Students = section.Students.Select(student => new StudentViewModel
                    {
                        StudentId = student.Id,
                        StudentName = student.Name,
                        Address = student.Address,
                        DateOfBirth = student.DateOfBirth,
                        ImageUrl = student.ImageUrl // Display image URL here
                    }).ToList()
                });
            }

            return View(sectionViewModel);
        }

        public async Task<IActionResult> CreateStudent()
        {
            ViewBag.Semesters = new SelectList(_context.Semesters, "Id", "Name");
            ViewBag.Faculties = new SelectList(_context.Faculties, "Id", "Name");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateStudent(CreateStudentViewModel model)
        {
            if (ModelState.IsValid)
            {
                var semester = await _context.Semesters.FindAsync(model.SemesterId);
                var faculty = await _context.Faculties.FindAsync(model.FacultyId);

                if (semester == null || faculty == null)
                {
                    return NotFound("Invalid Semester or Faculty.");
                }

                var section = await _context.Sections
                    .Include(s => s.Students)
                    .Where(s => s.SemesterId == model.SemesterId && s.FacultyId == model.FacultyId)
                    .OrderBy(s => s.SectionName)
                    .FirstOrDefaultAsync(s => s.Students.Count < MaxStudentsPerSection);

                // Create a new section if none exists or if the current one is full
                if (section == null || section.Students.Count >= MaxStudentsPerSection)
                {
                    var lastSection = await _context.Sections
                        .Where(s => s.SemesterId == model.SemesterId && s.FacultyId == model.FacultyId)
                        .OrderByDescending(s => s.SectionName)
                        .FirstOrDefaultAsync();

                    string newSectionName = lastSection == null ? "A" : GenerateNewSectionName(lastSection.SectionName);

                    section = new Section
                    {
                        SectionName = newSectionName,
                        SemesterId = model.SemesterId,
                        FacultyId = model.FacultyId,
                        Students = new List<Student>()
                    };

                    _context.Sections.Add(section);
                    await _context.SaveChangesAsync();
                }

                var student = new Student
                {
                    Name = model.StudentName,
                    SectionId = section.Id,
                    Address = model.Address,
                    Email = model.StudentEmail,
                    SemesterId = model.SemesterId,
                    FacultyId = model.FacultyId,
                    PhoneNumber = model.PhoneNumber,
                    ImageUrl = UploadedFile(model)
                };

                _context.Students.Add(student);
                await _context.SaveChangesAsync();

                TempData["success"] = "Student created successfully!";
                return RedirectToAction(nameof(Section), new { semesterId = model.SemesterId });
            }

            ViewBag.Semesters = new SelectList(_context.Semesters, "Id", "Name", model.SemesterId);
            ViewBag.Faculties = new SelectList(_context.Faculties, "Id", "Name", model.FacultyId);
            return View(model);
        }

        private string UploadedFile(CreateStudentViewModel model)
        {
            if (model.Image == null) return null;

            string uniqueFileName = Guid.NewGuid().ToString() + "_" + model.Image.FileName;
            string uploadFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images");
            string filePath = Path.Combine(uploadFolder, uniqueFileName);
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                model.Image.CopyTo(fileStream);
            }

            return $"/images/{uniqueFileName}";
        }

        private string GenerateNewSectionName(string lastSectionName)
        {
            char lastChar = lastSectionName[0];
            if (lastChar < 'Z')
            {
                return ((char)(lastChar + 1)).ToString();
            }
            throw new InvalidOperationException("Cannot create more than 26 sections (A-Z).");
        }

        public async Task<IActionResult> Delete(int id)
        {
            var section = await _context.Sections.FindAsync(id);
            if (section == null)
            {
                return NotFound();
            }

            return View(section);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var section = await _context.Sections.Include(s => s.Students).FirstOrDefaultAsync(s => s.Id == id);
            if (section != null)
            {
                _context.Students.RemoveRange(section.Students);
                _context.Sections.Remove(section);
                await _context.SaveChangesAsync();
                TempData["success"] = "Section deleted successfully!";
            }
            else
            {
                TempData["error"] = "Section not found.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
