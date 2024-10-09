using SchoolManagementSystem.Models;

namespace SchoolManagementSystem.ViewModel
{
	public class SectionViewModel
	{
		public int? SectionId {  get; set; }	
		public string? SectionName { get; set; }
		public int? SemesterId { get; set; }

		public int? FacultyId { get; set; }

		public Semester Semester { get; set; }
		public Faculty Faculty { get; set; }
		public List<StudentViewModel>? Students { get; set; }
	}
}
