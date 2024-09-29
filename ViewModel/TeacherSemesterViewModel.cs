using Microsoft.AspNetCore.Mvc.Rendering;
using SchoolManagementSystem.Models;

namespace SchoolManagementSystem.ViewModel
{
    public class TeacherSemesterViewModel
    {
        public string TeacherId { get; set; }
        public int SemesterId { get; set; }
        public int SectionId { get; set; }

        public ICollection<Semester> Semesters { get; set; }
        public ICollection<Section> Sections { get; set; }
    }
}
