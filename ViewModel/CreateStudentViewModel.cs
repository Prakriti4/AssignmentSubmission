using System.ComponentModel.DataAnnotations.Schema;

namespace SchoolManagementSystem.ViewModel
{
    public class CreateStudentViewModel
    {
        public string? StudentName {  get; set; }
        public string? StudentEmail { get; set; }
        public int? SemesterId { get; set; }
        public string? PhoneNumber { get; set; }
        public DateTime? DateofBirth { get; set; }
        public string? Address { get; set; }
        [NotMapped]
        public IFormFile? Image { get; set; }
        public string? ImageUrl { get; set; }
        public int? FacultyId { get; set; }
        public string? SectionName {  get; set; }
        public string Role { get; set; }

        public string Password {  get; set; }
        public string ConfirmPassword {  get; set; }
    }
}
