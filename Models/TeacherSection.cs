namespace SchoolManagementSystem.Models
{
    public class TeacherSection
    {
        public int SectionId { get; set; }
        public Section? Section { get; set; }    
        public string TeacherId {  get; set; }
        public Teacher? Teacher { get; set; }
    }
}
