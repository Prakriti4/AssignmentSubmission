using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Data;
using SchoolManagementSystem.Models;

namespace SchoolManagementSystem.Controllers
{
    [Authorize(Roles = "Admin")]
    public class SectionController : Controller
    {
        private readonly SchoolIdentityDbcontext _context;
        public SectionController(SchoolIdentityDbcontext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            var sections = await _context.Sections.Include(s => s.Semester).ToListAsync();
            return View(sections);
        }
        public IActionResult Create()
        {
            ViewBag.Semesters = new SelectList(_context.Semesters, "Id", "Name");
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(Section section)
        {
            ViewBag.Semesters = new SelectList(_context.Semesters.ToList(), "Id", "Name");
            if (ModelState.IsValid)
            {
                _context.Add(section);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(section);

        }
		public async Task<IActionResult> Edit(int id)
		{
			ViewBag.Semesters=new SelectList(_context.Semesters.ToList(),"Id","Name");
            var model = await _context.Sections.FindAsync(id);
			if(model ==null)
            {
                return NotFound();
            }
            return View(model);
		}

        [HttpPost]
        public async Task<IActionResult> Edit(Section section)
        {
            ViewBag.Section = new SelectList(_context.Semesters.ToList(), "Id", "Name");
            if(ModelState.IsValid)
            {
                _context.Sections.Update(section);
                _context.SaveChanges();
                TempData["SuccessMessage"] = "Teacher record updated successfully";
                return RedirectToAction("Index");
            }
            TempData["ErrorMessage"] = "There was an error updating the record";
            return View(section);
        }


		[HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var model =await _context.Sections.FindAsync(id);
            if (model == null)
            {
                return NotFound();
            }
            return View(model);
        }
        [HttpPost]
        public IActionResult Delete(Section section)
        {
            var existingsection = _context.Sections.FirstOrDefault(s => s.Id == section.Id);
            if (section != null)
            {
                _context.Sections.Remove(existingsection);
                _context.SaveChanges();
                TempData["SuccessMessage"] = "Faculty record deleted successfully";
            }
            return RedirectToAction("Index");
        }

    }
}
