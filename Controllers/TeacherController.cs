using System.Globalization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Data;
using SchoolManagementSystem.Models;

namespace SchoolManagementSystem.Controllers
{
    [Authorize(Roles = "Admin")]
    public class TeacherController : Controller
    {
        private readonly SchoolIdentityDbcontext _context;
        public TeacherController(SchoolIdentityDbcontext context)

        {
            _context = context ?? throw new ArgumentNullException (nameof(context));
            
        }
        public async Task <IActionResult> Index()
        {
             
            var models = await _context.Teachers.ToListAsync();
            if(models == null)
            {
                return View("Error");
            }
            return View(models);
        }
        public IActionResult Create()
        {
         
            return View();
        }
        [HttpPost]
        public IActionResult Create(Teacher teacher)
        {
            if (ModelState.IsValid)
            {
                string uniqueFileName= UploadedFile(teacher);
                teacher.ImageUrl=uniqueFileName;
                _context.Teachers.Add(teacher);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(teacher);
        }
        public string UploadedFile(Teacher model)
        {
            string uniqueFileName = null;
            if (model.Image != null)
            {
                string uploadFolder=Path.Combine(Directory.GetCurrentDirectory(),"wwwroot/images");
                uniqueFileName = Guid.NewGuid().ToString() + "_" + model.Image.FileName;
                string filePath=Path.Combine(uploadFolder,uniqueFileName);
                using(var fileStream = new FileStream(filePath,FileMode.Create))
                {
                    model.Image.CopyTo(fileStream);
                }
            }
            return uniqueFileName;
        } 
        public async Task<IActionResult> Edit(string id)
        {
         
            var model=_context.Teachers.Find(id);
            if (model == null)
            {
                return NotFound();
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(string id, Teacher teacher)
        {
            var existingTeacher=await _context.Teachers.FindAsync(id);
            if (existingTeacher == null)
            {
                return NotFound();
            }
            existingTeacher.Name = teacher.Name;
            existingTeacher.Address = teacher.Address;
            existingTeacher.Email = teacher.Email;
            string uniqueFile = UploadedFile(teacher);
            if (!string.IsNullOrEmpty(uniqueFile))
            {
                existingTeacher.ImageUrl = uniqueFile;
            }
            _context.Teachers.Update(existingTeacher);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Teacher record updated successfully";
            return RedirectToAction("Index");
        }


        public async Task<IActionResult> Detail(string id)
        {
            if (string.IsNullOrEmpty(id)) 
            {
                return NotFound();
            }

            var teacher = await _context.Teachers.FindAsync(id);

            if (teacher == null) 
            {
                return NotFound();
            }

            return View(teacher); 
        }


        [HttpGet]
        public IActionResult Delete(string id)
        {
            var model = _context.Teachers.Find(id);
            return View(model);
        }
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            // Retrieve the student again by id
            var teacher = await _context.Teachers.FindAsync(id);

            if (teacher == null)
            {
                return NotFound();
            }

            // Remove the teacher from the database
            _context.Teachers.Remove(teacher);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Student record deleted successfully";
            return RedirectToAction("Index");
        }
    }
}
