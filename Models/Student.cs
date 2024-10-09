using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using System.Reflection;

namespace SchoolManagementSystem.Models
{
    public class Student : User
    {
        public int? FacultyId { get; set; }
        public Faculty? Faculty { get; set; }
        public int? SemesterId { get; set; }
        public Semester? Semester { get; set; }
        public int? SectionId { get; set; }
        public Section? Section { get; set; }

        public ICollection<Submission>? Submissions { get; set; }

    }
}
