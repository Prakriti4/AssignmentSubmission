using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SchoolManagementSystem.Models
{
    public class Teacher:User
    {

        public int? FacultyId { get; set; }
        public Faculty? Faculty { get; set; }
        public int? SemesterId { get; set; }
        public Semester? Semester { get; set; }
        public ICollection<TeacherSection>? TeacherSections { get; set; }    
        public ICollection<TeacherSubject>? TeacherSubjects { get; set; }

    }
}
