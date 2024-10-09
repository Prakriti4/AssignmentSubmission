using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Data;
using SchoolManagementSystem.Models;

namespace SchoolManagementSystem.Controllers
{
    [Authorize]
    public class AssignmentController : Controller
    {
        private readonly SchoolIdentityDbcontext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly UserManager<User> _userManager;

        public AssignmentController(SchoolIdentityDbcontext context, IWebHostEnvironment webHostEnvironment, UserManager<User> userManager)
        {
            _webHostEnvironment=webHostEnvironment;
            _context = context;
            _userManager=userManager;
        }

        public string UploadedFile(Assignment model, out string originalFileName)
        {
            string uniqueFileName = null;
            originalFileName = null;
            if (model.FormFile != null)
            {
                string uploadFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/files");
                originalFileName = model.FormFile.FileName;
                uniqueFileName = Guid.NewGuid().ToString() + "_" + model.FormFile.FileName;
                string filePath = Path.Combine(uploadFolder, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    model.FormFile.CopyTo(fileStream);
                }
            }
            return uniqueFileName;
        }

   
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if(user == null)
            {
                return NotFound();
            }

            var assignment = await _context.Assignments
                                           .Include(s => s.Subject)
                                           .Where(a => a.Subject.TeacherSubjects.Any(a => a.TeacherId == user.Id))
                                           .ToListAsync();

            Console.WriteLine(assignment);

            return View(assignment);
        }

        public IActionResult Download(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                return NotFound("File path is missing.");
            }

            string fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/files", filePath);

            if (System.IO.File.Exists(fullPath))
            {
                byte[] fileBytes = System.IO.File.ReadAllBytes(fullPath);
                string fileName = Path.GetFileName(fullPath);
                return File(fileBytes, "application/octet-stream", fileName);
            }

            return NotFound("File not found.");
        }

        [Authorize(Roles = "Teacher")]
        public IActionResult Create()
        {
            ViewBag.Subjects = new SelectList(_context.Subjects, "Id", "SubjectName");
            return View();
        }

        [Authorize(Roles = "Teacher")]
        [HttpPost]
        public async Task<IActionResult> Create(Assignment assignment)
        {
            ViewBag.Subjects = new SelectList(_context.Subjects, "Id", "SubjectName");
            if (ModelState.IsValid)
            {
                string originalFileName;
                string uniqueFileName=UploadedFile(assignment,out originalFileName);
                assignment.FilePath=uniqueFileName;
                assignment.FileName=originalFileName;
                _context.Assignments.Add(assignment);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(assignment);

        }

       
        public async Task<IActionResult> Details(int id)
        {
            if (id <= 0)
            {
                return NotFound();
            }

            var assignment = await _context.Assignments
                                           .Include(a => a.Subject) 
                                           .FirstOrDefaultAsync(a => a.Id == id);

            if (assignment == null)
            {
                return NotFound();
            }

            return View(assignment);
        }
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> Edit(int id)
        {
            ViewBag.Subjects = new SelectList(_context.Subjects, "Id", "SubjectName");
      
            var model = await _context.Assignments.FindAsync(id);
            if (model == null)
            {
                return NotFound();
            }
            return View(model);
        
    }

    [Authorize(Roles = "Teacher")]
    [HttpPost]
        public async Task<IActionResult> Edit(Assignment assignment)
        {
            ViewBag.Subjects = new SelectList(_context.Subjects, "Id", "SubjectName");
          
            if (ModelState.IsValid)
            {
                string originalFileName;
                string uniqueFileName = UploadedFile(assignment, out originalFileName);
                assignment.FileName = originalFileName;
                assignment.FilePath = uniqueFileName;
                _context.Assignments.Update(assignment);
                _context.SaveChanges();
                TempData["SuccessMessage"] = "Teacher record updated successfully";
                return RedirectToAction("Index");
            }
            TempData["ErrorMessage"] = "There was an error updating the record";
            return View(assignment);
        }




        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var model = await _context.Assignments.FindAsync(id);
            if (model == null)
            {
                return NotFound();
            }
            return View(model);
        }


        [Authorize(Roles = "Teacher")]
        [HttpPost]
        public IActionResult Delete(Assignment assignment)
        {
            var model = _context.Assignments.Remove(assignment);
            _context.SaveChanges();
            TempData["SuccessMessage"] = "Subject record deleted successfully";
            return RedirectToAction("Index");

        }

    }
}
