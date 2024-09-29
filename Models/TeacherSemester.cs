namespace SchoolManagementSystem.Models
{
    public class TeacherSemester
    {
        public string TeacherId { get; set; }
        public int SemesterId { get; set; }
        public int SectionId { get; set; }
        public Semester? Semester { get; set; }
        public Teacher? Teacher { get; set; }

        public Section? Section { get; set; }
    }
}
