using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace SchoolManagementSystem.Models
{
    public class User:IdentityUser
    {

        public string? Name { get; set; }

        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
        [NotMapped]
        public IFormFile? Image { get; set; }
        public string? ImageUrl { get; set; }

        public DateTime? DateOfBirth {  get; set; }
   

    }
}
