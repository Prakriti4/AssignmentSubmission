using System.ComponentModel.DataAnnotations;

namespace SchoolManagementSystem.Models
{
    public class Subject
    {
        [Key]
        public int Id { get;set; }
        public string SubjectName {  get; set; }
        public string SubjectCode {  get; set; }
        public string Credits { get; set; }
        public string Description { get; set; }

        public int FacultyId { get; set; }
        public Faculty? Faculty { get; set; }

        public int SemesterId {  get; set; }
        public Semester? Semester { get; set; }
        public ICollection<Assignment>? Assignments { get; set; }
        public ICollection<Submission>? Submissions { get; set; }


    }
}
