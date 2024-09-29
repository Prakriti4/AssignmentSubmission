using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Models;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using SchoolManagementSystem.Data;
using SchoolManagementSystem.Data;
using SchoolManagementSystem.Models;

namespace SchoolManagementSystem.Controllers
{
    [Authorize(Roles ="Admin")]
    public class FacultyController : Controller
    {
        private readonly SchoolIdentityDbcontext _context;

        public FacultyController(SchoolIdentityDbcontext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<IActionResult> Index()
        {
            var detail = _context.Faculties.ToList();
            return View(detail);
        }

        public IActionResult Create()
        {
            //// Load available courses for selection in the form
            //ViewBag.AllCourses = _context.Courses.ToList();
            return View();
        }

        [HttpPost]
        public IActionResult Create(Faculty faculty)
        {

            //if (selectedCourseIds != null)
            //{
            //    faculty.Courses = new List<Course>();


            _context.Faculties.Add(faculty);
            _context.SaveChanges();
            return RedirectToAction("Index");

            //ViewBag.AllCourses = _context.Courses.ToList();

        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var faculty = _context.Faculties.FirstOrDefault(f => f.Id == id);
            if (faculty == null)
            {
                return NotFound();
            }

            //ViewBag.AllCourses = _context.Courses.ToList();
            //ViewBag.SelectedCourseIds = faculty.Courses.Select(c => c.Id).ToArray();

            return View(faculty);
        }

        [HttpPost]
        public IActionResult Edit(Faculty faculty)
        {
            if (ModelState.IsValid)
            {
                var existingFaculty = _context.Faculties.FirstOrDefault(f => f.Id == faculty.Id);
                if (existingFaculty != null)
                {
                    existingFaculty.Name = faculty.Name;
                    existingFaculty.Dean = faculty.Dean;
                    existingFaculty.Description = faculty.Description;

                    _context.Faculties.Update(existingFaculty);
                    _context.SaveChanges();

                    TempData["SuccessMessage"] = "Faculty record updated successfully";
                    return RedirectToAction("Index");
                }
            }
            TempData["ErrorMessage"] = "There was an error updating the record";
            return View(faculty);
        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
            var faculty = _context.Faculties.FirstOrDefault(f => f.Id == id);
            return View(faculty);
        }

        [HttpPost]
        public IActionResult Delete(Faculty faculty)
        {
            var existingFaculty = _context.Faculties.FirstOrDefault(f => f.Id == faculty.Id);
            if (existingFaculty != null)
            {
                _context.Faculties.Remove(existingFaculty);
                _context.SaveChanges();
                TempData["SuccessMessage"] = "Faculty record deleted successfully";
            }
            return RedirectToAction("Index");
        }
    }
}
