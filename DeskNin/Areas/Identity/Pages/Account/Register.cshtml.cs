using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DeskNin.Areas.Identity.Pages.Account;

public class RegisterModel : PageModel
{
    public IActionResult OnGet(string? returnUrl = null) =>
        RedirectToPage("/Account/Login", new { area = "Identity", returnUrl });

    public IActionResult OnPost(string? returnUrl = null) =>
        RedirectToPage("/Account/Login", new { area = "Identity", returnUrl });
}
