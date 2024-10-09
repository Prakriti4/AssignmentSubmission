using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NuGet.DependencyResolver;
using SchoolManagementSystem.Data;
using SchoolManagementSystem.Models;
using SchoolManagementSystem.ViewModel;

namespace SchoolManagementSystem.Controllers
{
    [Authorize(Roles = "Admin")]
    public class StudentController : Controller
    {
        private readonly SchoolIdentityDbcontext _context;
        private readonly UserManager<User> _userManager; 
        private readonly RoleManager<IdentityRole> _roleManager;
        public StudentController(SchoolIdentityDbcontext context, RoleManager<IdentityRole> roleManager, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }
        public async Task<IActionResult> Index()
        {
            var students = await _userManager.GetUsersInRoleAsync("Student");

            return View(students);
        }
        public async Task<IActionResult> Create()
        {
            ViewBag.Faculties = new SelectList(_context.Faculties, "Id", "Name");
            ViewBag.Semesters = new SelectList(_context.Semesters, "Id", "Name");
     
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Check for existing sections based on Semester and Faculty IDs
                var existingSection = await _context.Sections
                    .Include(s => s.Students)
                    .Where(s => s.SemesterId == model.SemesterId && s.FacultyId == model.FacultyId)
                    .OrderBy(s => s.SectionName)
                    .FirstOrDefaultAsync(s => s.Students.Count < 5);

                // If no existing section is found or if all sections are full, create a new one
                if (existingSection == null || existingSection.Students.Count >= 5)
                {
                    var lastSection = await _context.Sections
                        .Where(s => s.SemesterId == model.SemesterId && s.FacultyId == model.FacultyId)
                        .OrderByDescending(s => s.SectionName)
                        .FirstOrDefaultAsync();

                    string newSectionName = lastSection == null ? "A" : GenerateNewSectionName(lastSection.SectionName);

                    // Create a new section and add it to the context
                    existingSection = new Section
                    {
                        SectionName = newSectionName,
                        SemesterId = model.SemesterId,
                        FacultyId = model.FacultyId,
                        Students = new List<Student>() // Initialize the students list
                    };

                    _context.Sections.Add(existingSection);
                    await _context.SaveChangesAsync(); // Save changes to the database to generate the Section ID
                }

                // Create the new student
                var student = new Student
                {
                    Name = model.Name,
                    Address = model.Address,
                    DateOfBirth = model.DateOfBirth,
                    SectionId = existingSection.Id, // Assign the section ID to the student
                    SemesterId = model.SemesterId,
                    FacultyId = model.FacultyId,
                    // Add other properties from the model as necessary
                };

                // Add the student to the context
                _context.Students.Add(student);
                await _context.SaveChangesAsync(); // Save changes to the database to persist the student

                TempData["success"] = "Student registered successfully!";
                return RedirectToAction("Index"); // Redirect to the appropriate action
            }

            // Return the form view with validation messages if model state is invalid
            ViewBag.Faculties = new SelectList(_context.Faculties, "Id", "Name", model.FacultyId);
            ViewBag.Semesters = new SelectList(_context.Semesters, "Id", "Name", model.SemesterId);
            return View(model);
        }

        private string UploadedFile(Student model)
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
        
            var model = await _context.Students.FindAsync(id);
            if (model==null)
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


            var existingStudent = await _context.Students
                .Include(s=>s.Faculty)
                .Include(s=>s.Semester)
                .FirstOrDefaultAsync(s => s.Id == id);
            if (existingStudent == null)
            {
                return NotFound();
            }

            existingStudent.Name = student.Name;
            existingStudent.Email = student.Email;
            existingStudent.DateOfBirth = student.DateOfBirth;
            existingStudent.Address = student.Address;
            existingStudent.FacultyId = student.FacultyId; 
            existingStudent.SemesterId = student.SemesterId;

         
            string uniqueFile = UploadedFile(student);
            if (!string.IsNullOrEmpty(uniqueFile))
            {
                existingStudent.ImageUrl = uniqueFile;
            }

      
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

            bool doesExists = await _context.Students.AnyAsync(x => x.Id == id);

            if (!doesExists)
            {
                return NotFound();
            }

            var student = await _context.Students.FindAsync(id);

            return View(student);
        }




        [HttpGet]
        public async Task<IActionResult> Delete(string id)
        {
            ViewBag.Faculties = new SelectList(_context.Faculties, "Id", "Name");
            ViewBag.Semesters = new SelectList(_context.Semesters, "Id", "Name");

            var student = await _context.Students.FindAsync(id);

            if (student == null)
            {
                return NotFound();
            }

            return View(student);
        }


        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            ViewBag.Faculties = new SelectList(_context.Faculties, "Id", "Name");
            ViewBag.Semesters = new SelectList(_context.Semesters, "Id", "Name");

            var student = await _context.Students.FindAsync(id);

            if (student == null)
            {
                return NotFound();
            }

            _context.Students.Remove(student);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Student record deleted successfully";
            return RedirectToAction("Index");
        }

    

        public async Task<IActionResult> Section(int semesterId)
        {

            var semester = await _context.Semesters.FindAsync(semesterId);
            if (semester == null)
            {
                return NotFound($"Semester with ID {semesterId} not found.");
            }

            var currentSection = await _context.Sections
                .Include(s => s.Students) 
                .Where(s => s.SemesterId == semesterId)
                .OrderBy(s => s.SectionName)
                .FirstOrDefaultAsync(s => s.Students.Count < 5);


            if (currentSection == null)
            {
    
                var lastSection = await _context.Sections
                    .Where(s => s.SemesterId == semesterId)
                    .OrderByDescending(s => s.SectionName)
                    .FirstOrDefaultAsync();

                string newSectionName;
                if (lastSection == null)
                {
                    newSectionName = "A"; 
                }
                else
                {
                    newSectionName = GenerateNewSectionName(lastSection.SectionName); 
                }

       
                currentSection = new Section
                {
                    SectionName = newSectionName,
                    SemesterId = semesterId,
                    Students = new List<Student>() 
                };

   
                _context.Sections.Add(currentSection);
                await _context.SaveChangesAsync();
            }
    
            else if (currentSection.Students.Count >= 5)
            {
                currentSection = new Section
                {
                    SectionName = GenerateNewSectionName(currentSection.SectionName),  
                    SemesterId = semesterId,
                    Students = new List<Student>() 
                };

         
                _context.Sections.Add(currentSection);
                await _context.SaveChangesAsync();
            }

            var sectionViewModel = new SectionViewModel
            {
                SectionId = currentSection.Id,
                SectionName = currentSection.SectionName,
                SemesterId = currentSection.SemesterId,
                Students = currentSection.Students.Select(student => new StudentViewModel
                {
                    StudentId = student.Id,
                    StudentName = student.Name
                }).ToList()
            };

            return View(sectionViewModel);
        }
        public async Task<IActionResult> CreateStudent(CreateStudentViewModel model)
        {
            if (ModelState.IsValid)
            {
  
                var section = await _context.Sections
                    .Include(s => s.Students)
                    .Where(s => s.SemesterId == model.SemesterId)
                    .OrderBy(s => s.SectionName)
                    .FirstOrDefaultAsync(s => s.Students.Count < 5);

                if (section == null || section.Students.Count >= 5) 
                {
          
                    section = new Section
                    {
                        SectionName = GenerateNewSectionName(section?.SectionName),
                        SemesterId = model.SemesterId,
                        Students = new List<Student>() 
                    };
                    _context.Sections.Add(section);
                    await _context.SaveChangesAsync();
                }

                var student = new Student
                {
                    Name = model.StudentName,
                    SectionId = section.Id 
                };

                _context.Students.Add(student);
                await _context.SaveChangesAsync();

              
                return RedirectToAction("Section", "Student", new { semesterId = section.SemesterId });
            }

       
            ViewBag.Semesters = new SelectList(_context.Semesters, "Id", "Name", model.SemesterId);
            return View(model);
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



    }
}
