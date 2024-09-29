using System.ComponentModel.DataAnnotations;

namespace SchoolManagementSystem.Models
{
    public class Semester
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }    
  
        public ICollection<Section>? Sections { get; set; }
        public ICollection<Subject>? Subjects { get; set; } 
        public ICollection<Student>? Students { get; set; }
        public ICollection<TeacherSemester>? TeacherSemesters { get; set; }
    }
    
}
