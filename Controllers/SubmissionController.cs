using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NuGet.DependencyResolver;
using SchoolManagementSystem.Data;
using SchoolManagementSystem.Models;

namespace SchoolManagementSystem.Controllers
{
    public class SubmissionController : Controller
    {
        private readonly SchoolIdentityDbcontext _context;
        public SubmissionController(SchoolIdentityDbcontext context)
        {
            _context = context;
        }
        public string UploadedFile(Submission model, out string originalFileName)
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
            var subjects = await _context.Submissions
            .Include(s => s.Assignment)
            .Include(a=>a.Student)
            .Include(s=>s.Grade)
            .ToListAsync();

            var grades = _context.Grades.ToList();
            ViewBag.Grades = grades;
            return View(subjects);
        }
        public IActionResult Create()
        {
            ViewBag.Assignments = new SelectList(_context.Assignments, "Id", "Title");
            ViewBag.Students = new SelectList(_context.Students, "Id", "Name");
            ViewBag.Grades = new SelectList(_context.Grades, "Id", "LetterGrade");
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(Submission submission)
        {

            ViewBag.Assignments = new SelectList(_context.Assignments, "Id", "Title");
            ViewBag.Students = new SelectList(_context.Students, "Id", "Name");
            ViewBag.Grades = new SelectList(_context.Grades, "Id", "LetterGrade");

            var username = User.Identity.Name;

            var student= await _context.Students.FirstOrDefaultAsync(s=>s.Email == username);
            if (student == null)
            {
                ModelState.AddModelError("", "Student not found.");
                return View(submission);
            }
            if (ModelState.IsValid)
            {
                submission.StudentId = student?.Id;
                string originalFileName;
                string uniqueFileName = UploadedFile(submission, out originalFileName);
                submission.FileName = originalFileName;
                submission.FilePath = uniqueFileName;
                submission.SubmissionDate = DateTime.Now;
                _context.Submissions.Add(submission);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }


            return View(submission);

        }
        public async Task<IActionResult> Edit(int id)
        {
          
            ViewBag.Assignments = new SelectList(_context.Assignments, "Id", "Title");
            ViewBag.Students = new SelectList(_context.Students, "Id", "Name");
            ViewBag.Grades = new SelectList(_context.Grades, "Id", "LetterGrade");
            var model = await _context.Submissions.FindAsync(id);
            if (model == null)
            {
                return NotFound();
            }
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(int id, Submission submission)
        {
            if (id != submission.Id)
            {
                return NotFound();
            }

            // Repopulate ViewBag to retain dropdown data on error
            ViewBag.Assignments = new SelectList(_context.Assignments, "Id", "Title", submission.AssignmentId);
            ViewBag.Students = new SelectList(_context.Students, "Id", "Name", submission.StudentId);
            ViewBag.Grades = new SelectList(_context.Grades, "Id", "LetterGrade", submission.GradeId);

            if (ModelState.IsValid)
            {
                try
                {
                    var existingSubmission = await _context.Submissions.FindAsync(id);

                    if (existingSubmission == null)
                    {
                        return NotFound();
                    }

                    // Update properties except file if no new file is uploaded
                    existingSubmission.Description = submission.Description;
                    existingSubmission.AssignmentId = submission.AssignmentId;
                    existingSubmission.StudentId = submission.StudentId;
                    existingSubmission.GradeId = submission.GradeId;
                    existingSubmission.SubmissionDate = submission.SubmissionDate;

                    // Check if a new file was uploaded
                    if (submission.FormFile != null)
                    {
                        // Handle the new file upload and update file properties
                        string originalFileName;
                        string uniqueFileName = UploadedFile(submission, out originalFileName);
                        existingSubmission.FileName = originalFileName;
                        existingSubmission.FilePath = uniqueFileName;
                    }

                    _context.Update(existingSubmission);
                    await _context.SaveChangesAsync();

                    return RedirectToAction("Index");
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SubmissionExists(submission.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            // Return the form with the entered data if the model state is invalid
            return View(submission);
        }
        // Helper method to check if a submission exists
        private bool SubmissionExists(int id)
        {
            return _context.Submissions.Any(e => e.Id == id);
        }
        //[HttpPost]
        //public async Task<IActionResult> Edit(int id, Submission submission)
        //{
        //    if(id!= submission.Id)
        //    {
        //        return NotFound();
        //    }
        //    ViewBag.Assignments = new SelectList(_context.Assignments, "Id", "Title");
        //    ViewBag.Students = new SelectList(_context.Students, "Id", "Name");
        //    ViewBag.Grades = new SelectList(_context.Grades, "Id", "LetterGrade");
        //    if (ModelState.IsValid)
        //    {
        //        string originalFileName;
        //        string uniqueFileName = UploadedFile(submission, out originalFileName);
        //        submission.FileName = originalFileName;
        //        submission.FilePath = uniqueFileName;
        //        _context.Submissions.Update(submission);
        //        _context.SaveChanges();
        //        TempData["SuccessMessage"] = "Teacher record updated successfully";
        //        return RedirectToAction("Index");
        //    }
        //    TempData["ErrorMessage"] = "There was an error updating the record";
        //    return View(submission);
        //}


        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var model = await _context.Submissions.FindAsync(id);
            if (model == null)
            {
                return NotFound();
            }
            return View(model);
        }
        [HttpPost]
        public IActionResult Delete(Submission sumbission)
        {
            var model = _context.Submissions.Remove(sumbission);
            _context.SaveChanges();
            TempData["SuccessMessage"] = "Subject record deleted successfully";
            return RedirectToAction("Index");

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
        //[HttpPost]
        //public async Task<IActionResult> AssignGrade(int SubmissionId, string Grade)
        //{
        //    // Find the submission by ID
        //    var submission = await _context.Submissions.FindAsync(SubmissionId);
        //    if (submission == null)
        //    {
        //        return NotFound();
        //    }

        //    // Assign the grade
        //    submission.Grade = new Grade { LetterGrade = Grade }; // Assuming Grade is an entity. Modify based on your model.

        //    // Save changes to the database
        //    await _context.SaveChangesAsync();

        //    // Redirect back to the index page
        //    return RedirectToAction("Index");
        //}
        [HttpPost]
        public async Task<IActionResult> AssignGrade(int SubmissionId, int GradeId)
        {
            // Find the submission by ID
            var grade = await _context.Grades.FindAsync(GradeId);
            if (grade == null)
            {
                ModelState.AddModelError("", "Invalid grade selected.");
                return RedirectToAction("Index");
            }
            var submission = await _context.Submissions.FindAsync(SubmissionId);
            if (submission == null)
            {
                return NotFound();
            }

            // Assign the grade using the selected GradeId
            submission.GradeId = GradeId;

            // Save changes to the database
            await _context.SaveChangesAsync();

            // Redirect back to the index page
            return RedirectToAction("Index");
        }



    }
}
