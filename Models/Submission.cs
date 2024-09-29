using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SchoolManagementSystem.Models
{
    public class Submission
    {
        [Key]
        public int Id { get; set; }
        public string Description { get; set; }
        public DateTime SubmissionDate { get; set; }
        [NotMapped]
        [Required]
        public IFormFile? FormFile { get; set; }
        public string? FileName { get; set; }
        public string? FilePath { get; set; }
        public int? AssignmentId {  get; set; }
        public Assignment? Assignment { get; set; }
        public string? StudentId {  get; set; }    
        public Student? Student { get; set; }
        public int? GradeId { get; set; }
        public Grade? Grade { get; set; }
    }
}
