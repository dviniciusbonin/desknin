using System.ComponentModel.DataAnnotations;

namespace DeskNin.ViewModels;

public class UserForm
{
    public string? Id { get; set; }

    [Required(ErrorMessage = "Name is required")]
    [StringLength(256)]
    [Display(Name = "Username")]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email is required")]
    [EmailAddress]
    [Display(Name = "Email")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Role is required")]
    [Display(Name = "Role")]
    public string Role { get; set; } = string.Empty;

    [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters")]
    [DataType(DataType.Password)]
    [Display(Name = "Password")]
    public string? Password { get; set; }

    [DataType(DataType.Password)]
    [Display(Name = "Confirm password")]
    [Compare("Password", ErrorMessage = "Password and confirmation do not match")]
    public string? ConfirmPassword { get; set; }
}