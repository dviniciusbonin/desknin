using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace DeskNin.ViewComponents;

public class AdminNavViewComponent : ViewComponent
{
    private readonly UserManager<IdentityUser> _userManager;

    public AdminNavViewComponent(UserManager<IdentityUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var user = await _userManager.GetUserAsync(HttpContext.User);
        if (user == null) return View(false);

        var isAdmin = await _userManager.IsInRoleAsync(user, "Admin");
        return View(isAdmin);
    }
}
