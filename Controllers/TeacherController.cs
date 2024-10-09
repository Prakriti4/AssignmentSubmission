using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Data;
using SchoolManagementSystem.Models;
using SchoolManagementSystem.ViewModel;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SchoolManagementSystem.Controllers
{
    [Authorize(Roles = "Admin")]
    public class TeacherController : Controller
    {
        private readonly SchoolIdentityDbcontext _context;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public TeacherController(SchoolIdentityDbcontext context, RoleManager<IdentityRole> roleManager,UserManager<User> userManager)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _roleManager = roleManager;
            _userManager = userManager;
        }

        //public async Task<IActionResult> Index()
        //{

        //    var teacherUsers = await _userManager.GetUsersInRoleAsync("Teacher");



        //    if (teacherUsers == null || !teacherUsers.Any())
        //    {
        //        return Content("No users found with the 'Teacher' role.");
        //    }

        //    var teacherUserIds= teacherUsers.Select(u=>u.Id).ToList();

        //    if (!teacherUserIds.Any())
        //    {
        //        return Content("No teacher users have been found.");
        //    }
        //    var teachers = await _context.Teachers
        //        .Include(t => t.Faculty)
        //        .Include(t => t.Semester)
        //        .Include(t => t.TeacherSections)
        //            .ThenInclude(ts => ts.Section)
        //        .Include(t => t.TeacherSubjects)
        //            .ThenInclude(ts => ts.Subject)
        //         .Where(t=>teacherUserIds.Contains(t.Id))
        //        .ToListAsync();

        //    if (teachers == null)
        //    {
        //        return View("Error");
        //    }

        //    return View(teachers);
        //}

        public async Task<IActionResult> Index()
        {
            // Fetch all users with "Teacher" role
            var teachers = await _userManager.GetUsersInRoleAsync("Teacher");

            // Pass the list of teachers to the view
            return View(teachers);
        }
        public IActionResult Create()
        {
            PopulateViewBags(); // Populate ViewBag with faculties, sections, subjects, semesters
            return View();
        }

       
    

        public async Task<IActionResult> Edit(string id)
        {
            PopulateViewBags();

            var teacher = await _context.Teachers
                .Include(t => t.TeacherSections)
                .Include(t => t.TeacherSubjects)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (teacher == null)
            {
                return NotFound();
            }

            // Pass the currently selected sections and subjects to the view for editing
            ViewBag.SelectedSections = teacher.TeacherSections.Select(ts => ts.SectionId).ToList();
            ViewBag.SelectedSubjects = teacher.TeacherSubjects.Select(ts => ts.SubjectId).ToList();

            return View(teacher);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(string id, Teacher teacher)
        {
            PopulateViewBags();

            var existingTeacher = await _context.Teachers
                .Include(t => t.TeacherSections)
                .Include(t => t.TeacherSubjects)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (existingTeacher == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                // Update basic teacher properties
                existingTeacher.Name = teacher.Name;
                existingTeacher.Address = teacher.Address;
                existingTeacher.Email = teacher.Email;
                existingTeacher.FacultyId = teacher.FacultyId;
                existingTeacher.SemesterId = teacher.SemesterId;

                // Handle file upload
                string uniqueFile = UploadedFile(teacher.Image);
                if (!string.IsNullOrEmpty(uniqueFile))
                {
                    existingTeacher.ImageUrl = uniqueFile;
                }

                //// Update sections (many-to-many relationship)
                //existingTeacher.TeacherSections = selectedSections.Select(sectionId => new TeacherSection
                //{
                //    TeacherId = existingTeacher.Id,
                //    SectionId = sectionId
                //}).ToList();

                //// Update subjects (many-to-many relationship)
                //existingTeacher.TeacherSubjects = selectedSubjects.Select(subjectId => new TeacherSubject
                //{
                //    TeacherId = existingTeacher.Id,
                //    SubjectId = subjectId
                //}).ToList();

                _context.Teachers.Update(existingTeacher);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Teacher record updated successfully";
                return RedirectToAction("Index");
            }

            return View(teacher);
        }

        public async Task<IActionResult> Details(string id)
        {
            var teacher = await _context.Teachers
                .Include(t => t.Faculty)
                .Include(t => t.Semester)
                .Include(t => t.TeacherSections)
                    .ThenInclude(ts => ts.Section)
                .Include(t => t.TeacherSubjects)
                    .ThenInclude(ts => ts.Subject)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (teacher == null)
            {
                return NotFound();
            }

            return View(teacher);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(string id)
        {
            var teacher = await _context.Teachers.FindAsync(id);
            if (teacher == null)
            {
                return NotFound();
            }

            return View(teacher);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var teacher = await _context.Teachers.FindAsync(id);
            if (teacher == null)
            {
                return NotFound();
            }

            _context.Teachers.Remove(teacher);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Teacher record deleted successfully";
            return RedirectToAction("Index");
        }

        private void PopulateViewBags()
        {
            // Load faculties, semesters, sections, and subjects to populate dropdowns in views
            ViewBag.Faculties = new SelectList(_context.Faculties, "Id", "Name");
            ViewBag.Semesters = new SelectList(_context.Semesters, "Id", "Name");
            ViewBag.Sections = new MultiSelectList(_context.Sections, "Id", "SectionName");
            ViewBag.Subjects = new MultiSelectList(_context.Subjects, "Id", "SubjectName");
        }

        public string UploadedFile(IFormFile file)
        {
            string uniqueFileName = null;
            if (file != null)
            {
                string uploadFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");
                uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
                string filePath = Path.Combine(uploadFolder, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                  file.CopyTo(fileStream);
                }
            }
            return uniqueFileName;
        }
    }
}
