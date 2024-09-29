using System.ComponentModel.DataAnnotations;

namespace SchoolManagementSystem.Models
{
    public class Grade
    {
        [Key]
        public int Id { get; set; }
        public string LetterGrade { get; set; }
        public ICollection<Submission>? Submissions { get; set; }
 
    }
}
