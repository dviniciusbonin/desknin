using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;

namespace DeskNin.Areas.Identity.Pages.Account;

public class ResetPasswordModel(UserManager<IdentityUser> userManager) : PageModel
{
    private readonly UserManager<IdentityUser> _userManager = userManager;

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public bool IsSuccess { get; private set; }

    public sealed class InputModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; } = string.Empty;

        [Required]
        public string Code { get; set; } = string.Empty;
    }

    public IActionResult OnGet(string? code = null, string? email = null)
    {
        if (string.IsNullOrWhiteSpace(code) || string.IsNullOrWhiteSpace(email))
            return RedirectToPage("./Login");

        Input = new InputModel
        {
            Email = email,
            Code = code
        };

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
            return Page();

        var user = await _userManager.FindByEmailAsync(Input.Email);
        if (user == null)
        {
            IsSuccess = true;
            return Page();
        }

        var decodedCode = System.Text.Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(Input.Code));
        var result = await _userManager.ResetPasswordAsync(user, decodedCode, Input.Password);
        if (result.Succeeded)
        {
            IsSuccess = true;
            return Page();
        }

        foreach (var error in result.Errors)
            ModelState.AddModelError(string.Empty, error.Description);

        return Page();
    }
}
