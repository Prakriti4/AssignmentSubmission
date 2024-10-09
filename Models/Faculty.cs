using System.ComponentModel.DataAnnotations;

namespace SchoolManagementSystem.Models
{
    public class Faculty
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }    
        public string Dean { get; set; }
        public string Description { get; set; }
        public int NumberOfSemester { get; set; }
        public ICollection<Subject>? Subjects { get; set; }
       public ICollection<Section>? Sections { get; set; }
        public ICollection<Student>? Students { get; set; }
        public ICollection<Semester>? Semesters { get; set; }
        public ICollection<Teacher>? Teachers { get; set; }
    }
}
