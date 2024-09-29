using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SchoolManagementSystem.Models
{
    public class Teacher:User
    { 
         public ICollection<TeacherSemester>? TeacherSemesters { get; set; }

    }
}
