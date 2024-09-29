using System.ComponentModel.DataAnnotations;

namespace SchoolManagementSystem.Models
{
    public class Section
    {
        [Key]
        public int Id { get; set; }
        public string SectionName {  get; set; }
       
        public int SemesterId { get; set; } 
        public Semester? Semester { get; set; }
        public ICollection<TeacherSemester>? TeacherSemesters { get; set; }
        public ICollection<Student>? Students { get; set; }
    }
}
