using System.ComponentModel.DataAnnotations;

namespace SchoolManagementSystem.Models
{
    public class Section
    {
        [Key]
        public int Id { get; set; }
        public string? SectionName {  get; set; }
       
        public int? SemesterId { get; set; } 
        public Semester? Semester { get; set; }

        public int? FacultyId {  get; set; }
        public Faculty? Faculty { get; set; }
        public ICollection<Student>? Students { get; set; }
        public ICollection<TeacherSection>? TeacherSections { get; set; }
        public ICollection<Teacher>? Teachers { get; set; }
    }
}
