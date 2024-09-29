using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Models;

namespace SchoolManagementSystem.Data
{
    public class SchoolIdentityDbcontext : IdentityDbContext<User>
    {
        public SchoolIdentityDbcontext(DbContextOptions<SchoolIdentityDbcontext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Semester>().HasData(
          new Semester { Id = 1, Name = "Semester 1" },
          new Semester { Id = 2, Name = "Semester 2" },
          new Semester { Id = 3, Name = "Semester 3" },
          new Semester { Id = 4, Name = "Semester 4" },
          new Semester { Id = 5, Name = "Semester 5" },
          new Semester { Id = 6, Name = "Semester 6" },
          new Semester { Id = 7, Name = "Semester 7" },
          new Semester { Id = 8, Name = "Semester 8" }
      );

            modelBuilder.Entity<Grade>().HasData(
 new Grade { Id = 1, LetterGrade = "A+" },
 new Grade { Id = 2, LetterGrade = "A" },
 new Grade { Id = 3, LetterGrade = "B+" },
 new Grade { Id = 4, LetterGrade = "B" },
 new Grade { Id = 5, LetterGrade = "C+" },
 new Grade { Id = 6, LetterGrade = "C" },
 new Grade { Id = 7, LetterGrade = "D" },
 new Grade { Id = 8, LetterGrade = "NG" }

 );

            //FacultyandSemesterSeeding Data
            modelBuilder.Entity<Faculty>()
               .Property(f => f.NumberOfSemester)
               .HasDefaultValue(8);

            // Faculty and Students relationship
            modelBuilder.Entity<Faculty>()
                .HasMany(f => f.Students)
                .WithOne(s => s.Faculty)
                .HasForeignKey(s => s.FacultyId)
                .OnDelete(DeleteBehavior.Restrict);

            // Faculty and Subjects relationship
            modelBuilder.Entity<Faculty>()
                .HasMany(f => f.Subjects)
                .WithOne(s => s.Faculty)
                .HasForeignKey(s => s.FacultyId)
                .OnDelete(DeleteBehavior.Restrict);

            // Section and Students relationship
            modelBuilder.Entity<Section>()
                .HasMany(sec => sec.Students)
                .WithOne(stu => stu.Section)
                .HasForeignKey(stu => stu.SectionId)
                 .OnDelete(DeleteBehavior.Restrict);

            // Assignment and Subject relationship
            modelBuilder.Entity<Assignment>()
                .HasOne(a => a.Subject)
                .WithMany(s => s.Assignments)
                .HasForeignKey(a => a.SubjectId)
                .OnDelete(DeleteBehavior.Restrict);

            // Section and Semester relationship
            modelBuilder.Entity<Section>()
                .HasOne(sec => sec.Semester)
                .WithMany(sem => sem.Sections)
                .HasForeignKey(sec => sec.SemesterId)
                .OnDelete(DeleteBehavior.Restrict);

            // Subject and Semester relationship
            modelBuilder.Entity<Subject>()
                .HasOne(sub => sub.Semester)
                .WithMany(sem => sem.Subjects)
                .HasForeignKey(sub => sub.SemesterId)
                .OnDelete(DeleteBehavior.Restrict);

            // Student and Semester relationship
            modelBuilder.Entity<Student>()
                .HasOne(stu => stu.Semester)
                .WithMany(sem => sem.Students)
                .HasForeignKey(stu => stu.SemesterId)
                .OnDelete(DeleteBehavior.Restrict);

            // Student and Section relationship
            modelBuilder.Entity<Student>()
                .HasOne(stu => stu.Section)
                .WithMany(sec => sec.Students)
                .HasForeignKey(stu => stu.SectionId)
                .OnDelete(DeleteBehavior.Restrict);

            // Subject and Faculty relationship
            modelBuilder.Entity<Subject>()
                .HasOne(sub => sub.Faculty)
                .WithMany(fac => fac.Subjects)
                .HasForeignKey(sub => sub.FacultyId)
                .OnDelete(DeleteBehavior.Restrict);

            // TeacherSemester configuration
            modelBuilder.Entity<TeacherSemester>()
                .HasKey(ts => new { ts.TeacherId, ts.SemesterId, ts.SectionId });

            modelBuilder.Entity<TeacherSemester>()
                .HasOne(ts => ts.Teacher)
                .WithMany(t => t.TeacherSemesters)
                .HasForeignKey(ts => ts.TeacherId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TeacherSemester>()
                .HasOne(ts => ts.Semester)
                .WithMany(s => s.TeacherSemesters)
                .HasForeignKey(ts => ts.SemesterId)
                .OnDelete(DeleteBehavior.Restrict);
          
            modelBuilder.Entity<Submission>()
                .HasOne(s => s.Assignment)           
                .WithMany(a => a.Submissions)       
                .HasForeignKey(s => s.AssignmentId)  
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Submission>()
               .HasOne(a => a.Student)              
               .WithMany(s => s.Submissions)          
               .HasForeignKey(a => a.StudentId)      
               .OnDelete(DeleteBehavior.Restrict);   
  

            modelBuilder.Entity<Submission>()
              .HasOne(s => s.Grade)
              .WithMany(g => g.Submissions)
              .HasForeignKey(s => s.GradeId)
              .OnDelete(DeleteBehavior.Cascade);


        }

        public DbSet<Teacher> Teachers { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Subject> Subjects { get; set; }
        public DbSet<Faculty> Faculties { get; set; }
        public DbSet<Assignment> Assignments { get; set; }
        public DbSet<Semester> Semesters { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<Section> Sections { get; set; }
        public DbSet<Submission> Submissions { get; set; }
        public DbSet<TeacherSemester> TeacherSemesters { get; set; }
        public DbSet<Grade> Grades { get; set; }
     
    }
}
