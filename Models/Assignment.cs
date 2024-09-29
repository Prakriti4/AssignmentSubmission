using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SchoolManagementSystem.Models
{
    public class Assignment
    {
        [Key]
        public int Id {  get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime SubmissionDate { get; set; }
        public string Feedback {  get; set; }
        [NotMapped]
        public IFormFile? FormFile { get; set; }
        public string? FileName { get; set; }
        public string? FilePath {  get; set; }
        public int SubjectId { get; set; }
        public Subject? Subject { get; set; }
        public ICollection<Submission>? Submissions { get; set; }

    }
}
