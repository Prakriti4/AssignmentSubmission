using Microsoft.AspNetCore.Identity;
using SchoolManagementSystem.Models;
using System.ComponentModel.DataAnnotations;

namespace SchoolManagementSystem.ViewModel
{
    public class RegisterViewModel
    {
        public int Id { get; set; } 
        [Required]
        public string Name { get; set; }
        public string Email {  get; set; }
        public int FacultyId {  get; set; }
        public int SemesterId {  get; set; }
        public int SectionId {  get; set; }
        public int SubjectId {  get; set; }
        public IFormFile Image { get; set; }
        public string? ImageUrl { get; set; }
        public string? PhoneNumber {  get; set; }
        public string Address {  get; set; }
        public string Role {  get; set; }
        public DateTime? DateOfBirth { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }




    }
}
