using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

public class RegisterViewModel
{
    [Required]
    public string Name { get; set; }    

    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; }

    [Required]
    [DataType(DataType.Password)]
    [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
    public string ConfirmPassword { get; set; }

    [Required]
    public string Role { get; set; } // Property to hold the selected role

    public IEnumerable<SelectListItem> Roles { get; set; } // List of roles for the dropdown
}
