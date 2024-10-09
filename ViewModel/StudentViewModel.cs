using System.ComponentModel.DataAnnotations.Schema;

namespace SchoolManagementSystem.ViewModel
{
	public class StudentViewModel
	{
		public string? StudentId {  get; set; }
		public string? StudentName { get; set; }
		public DateTime? DateOfBirth { get; set; }
        public string? Address { get; set; }
        [NotMapped]
        public IFormFile? Image { get; set; }
        public string? ImageUrl { get; set; }
        public int? FacultyId {  get; set; }	
		public int? SemesterId {  get; set; }
		public string SectionName {  get; set; }
	


	}
}
